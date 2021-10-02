using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private float gravity = 1f;

    [SerializeField]
    private float terminalVelocity = 1f;

    [SerializeField]
    private float jumpVelocity = 2f;

    [SerializeField]
    private float moveSpeed = 1f;

    [SerializeField]
    private Rigidbody2D rigidBody;

    [SerializeField]
    private BoxCollider2D GroundCollider;

    [SerializeField]
    private SpriteRenderer renderer;

    [SerializeField]
    private Animator animator;

    private CharacterState currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = new FlyState(this);
        currentState.Start();
    }

    // Update is called once per frame
    void Update()
    {
        CharacterState nextState = currentState.Update();
        if(nextState != currentState)
        {
            currentState.End();
            currentState = nextState;
            currentState.Start();
        }
    }

    public void AccelerateByGravity()
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.y - (gravity * Time.deltaTime));
    }

    public void Jump()
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpVelocity);
    }

    public void SetHorizontalVelocity(float input)
    {
        rigidBody.velocity = new Vector2(moveSpeed * input, rigidBody.velocity.y);
        if (renderer.flipX && input > 0f)
            renderer.flipX = false;
        if (!renderer.flipX && input < 0f)
            renderer.flipX = true;
    }

    public void TriggerAnimation(string animation)
    {
        animator.SetTrigger(animation);
    }

    public bool IsMovingHorizontally()
    {
        Debug.Log(rigidBody.velocity.x);
        return Mathf.Abs(rigidBody.velocity.x) > .1f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GlobalSignalManager.Inst.FireSignal(new CharacterHitGroundSignal());
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        GlobalSignalManager.Inst.FireSignal(new CharacterLeftGroundSignal());
    }
}

public abstract class CharacterState
{
    protected Character character;

    public CharacterState (Character character)
    {
        this.character = character;
    }

    public virtual void Start() { }
    public virtual CharacterState Update() { return this; }
    public virtual void End() { }
}

public class StandState : CharacterState
{
    private CharacterState nextState;

    public StandState (Character character) : base (character)
    {
        nextState = this;
    }

    public override void Start()
    {
        character.TriggerAnimation("Idle");
        GlobalSignalManager.Inst.AddListener<CharacterLeftGroundSignal>(onCharacterLeftGround);
    }

    public override CharacterState Update()
    {
        character.SetHorizontalVelocity(Input.GetAxis("Horizontal"));
        if (Input.GetButtonDown("Jump"))
            character.Jump();
        if (nextState == this && character.IsMovingHorizontally())
            nextState = new RunState(character);

        return nextState;
    }

    public override void End()
    {
        GlobalSignalManager.Inst.RemoveListener<CharacterLeftGroundSignal>(onCharacterLeftGround);
    }

    private void onCharacterLeftGround(GlobalSignal signal)
    {
        nextState = new FlyState(character);
    }
}

public class FlyState : CharacterState
{
    private CharacterState nextState;

    public FlyState (Character character) : base (character)
    { 
        nextState = this;
    }

    public override void Start()
    {
        character.TriggerAnimation("Jump");
        GlobalSignalManager.Inst.AddListener<CharacterHitGroundSignal>(onCharacterHitGround);
    }

    public override CharacterState Update()
    {
        character.SetHorizontalVelocity(Input.GetAxis("Horizontal"));
        character.AccelerateByGravity();
        return nextState;
    }

    public override void End()
    {
        GlobalSignalManager.Inst.RemoveListener<CharacterHitGroundSignal>(onCharacterHitGround);
    }

    private void onCharacterHitGround(GlobalSignal signal)
    {
        if (character.IsMovingHorizontally())
            nextState = new RunState(character);
        else
            nextState = new StandState(character);
    }
}

public class RunState : CharacterState
{
    private CharacterState nextState;

    public RunState (Character character) : base (character)
    {
        nextState = this;
    }

    public override void Start()
    {
        character.TriggerAnimation("Run");
        GlobalSignalManager.Inst.AddListener<CharacterLeftGroundSignal>(onCharacterLeftGround);

    }

    public override CharacterState Update()
    {
        if (Input.GetButtonDown("Jump"))
            character.Jump();
        character.SetHorizontalVelocity(Input.GetAxis("Horizontal"));
        if (nextState == this && !character.IsMovingHorizontally())
            nextState = new StandState(character);
        return nextState;
    }

    public override void End()
    {
        GlobalSignalManager.Inst.RemoveListener<CharacterLeftGroundSignal>(onCharacterLeftGround);
    }

    private void onCharacterLeftGround(GlobalSignal signal)
    {
        nextState = new FlyState(character);
    }

}