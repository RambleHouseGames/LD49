using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    [SerializeField]
    private CircleCollider2D KillCollider;

    [SerializeField]
    private BoxCollider2D PlatformCollider;

    [SerializeField]
    private Rigidbody2D rigidBody;

    [SerializeField]
    private Animator myAnimator;

    [SerializeField]
    private float velocity = 5f;

    public float stickDuration = 5f;

    [NonSerialized]
    public bool GoLeft = false;

    public Action<Collider2D> stickCallback;

    private ShurikenState currentState;

    private void Start()
    {
        currentState = new ShurikenFlyState(this);
        currentState.Start();
    }

    private void Update()
    {
        ShurikenState nextState = currentState.Update();
        if (nextState != currentState)
        {
            currentState.End();
            currentState = nextState;
            currentState.Start();
        }
    }

    public void Fly()
    {
        if(GoLeft)
            rigidBody.velocity = new Vector2(-velocity, 0f);
        else
            rigidBody.velocity = new Vector2(velocity, 0f);
    }

    public void StickTo(GameObject newParent)
    {
        rigidBody.velocity = Vector2.zero;
        transform.SetParent(newParent.transform);
        rigidBody.bodyType = RigidbodyType2D.Kinematic;
        gameObject.layer = 6;
        myAnimator.SetTrigger("Stick");
        KillCollider.enabled = false;
        PlatformCollider.enabled = true;
    }

    public void Break()
    {
        myAnimator.SetTrigger("Break");
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        rigidBody.gravityScale = 15f;
        rigidBody.constraints = RigidbodyConstraints2D.None;
    }

    public void SelfDestruct()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(stickCallback != null)
            stickCallback(collision.collider);
    }
}

public abstract class ShurikenState
{
    protected Shuriken shuriken;

    public ShurikenState (Shuriken shuriken)
    {
        this.shuriken = shuriken;
    }

    public virtual void Start() { }
    public virtual ShurikenState Update() { return this; }
    public virtual void End() { }
}

public class ShurikenFlyState : ShurikenState
{
    private ShurikenState nextState;

    public ShurikenFlyState(Shuriken shuriken) : base (shuriken)
    {
        nextState = this;
    }

    public override void Start()
    {
        shuriken.stickCallback += onStick;
        shuriken.Fly();
    }

    public override ShurikenState Update()
    {
        return nextState;
    }

    public override void End()
    {
        shuriken.stickCallback -= onStick;
    }

    private void onStick(Collider2D collider)
    {
        shuriken.StickTo(collider.gameObject);
        nextState = new ShurikenStuckState(shuriken);
    }
}

public class ShurikenStuckState : ShurikenState
{
    private float timer = 0f;

    public ShurikenStuckState(Shuriken shuriken) : base (shuriken) {}

    public override ShurikenState Update()
    {
        timer += Time.deltaTime;
        if (timer > shuriken.stickDuration)
            return new ShurikenBreakState(shuriken);
        else
            return this;
    }
}

public class ShurikenBreakState : ShurikenState
{
    private float timer = 0f;

    public ShurikenBreakState (Shuriken shuriken) : base (shuriken)
    {

    }

    public override void Start()
    {
        shuriken.Break();
    }

    public override ShurikenState Update()
    {
        timer += Time.deltaTime;
        if (timer > 2f)
            shuriken.SelfDestruct();
        return this;
    }
}