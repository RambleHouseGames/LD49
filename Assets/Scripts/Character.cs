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
    private GroundCheck groundCheck;

    [SerializeField]
    private SpriteRenderer renderer;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Shuriken shurikenPrefab;

    [SerializeField]
    private float shurikenSpawnOffset;

    private CharacterState currentState;

    // Start is called before the first frame update
    void Start()
    {
        GlobalSignalManager.Inst.AddListener<CharacterHitGroundSignal>(onCharacterHitGround);

        currentState = new FlyState(this);
        currentState.Start();
    }

    private void onCharacterHitGround(GlobalSignal signal)
    {
        Debug.Log("Hit Ground on state: " + currentState.GetType());
    }


    // Update is called once per frame
    void Update()
    {
        CharacterState nextState = currentState.Update();
        if (nextState != currentState)
        {
            currentState.End();
            currentState = nextState;
            currentState.Start();
        }
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
        return Mathf.Abs(rigidBody.velocity.x) > .1f;
    }

    public bool IsGrounded()
    {
        return groundCheck.IsGrounded();
    }

    public void OnThrowAnimationReleasePoint()
    {
        GlobalSignalManager.Inst.FireSignal(new ThrowAnimationReleasePointSignal());
    }

    public void OnThrowAnimationEnded()
    {
        GlobalSignalManager.Inst.FireSignal(new ThrowAnimationEndedSignal());
    }

    public void SpawnShuriken()
    {
        if (renderer.flipX)
        {
            Vector3 spawnPosition = new Vector3(transform.position.x - shurikenSpawnOffset, transform.position.y, transform.position.z);
            Shuriken newShuriken = Instantiate(shurikenPrefab, spawnPosition, Quaternion.identity);
            newShuriken.GoLeft = true;
        }
        else
        {
            Vector3 spawnPosition = new Vector3(transform.position.x + shurikenSpawnOffset, transform.position.y, transform.position.z);
            Shuriken newShuriken = Instantiate(shurikenPrefab, spawnPosition, Quaternion.identity);
            newShuriken.GoLeft = false;
        }
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
    }

    public override CharacterState Update()
    {
        character.SetHorizontalVelocity(Input.GetAxis("Horizontal"));
        if (Input.GetButtonDown("Jump"))
            character.Jump();
        if (Input.GetButtonDown("Fire1"))
            nextState = new GroundedShurikenState(character);
        if (!character.IsGrounded())
            nextState = new FlyState(character);
        if (nextState == this && character.IsMovingHorizontally())
            nextState = new RunState(character);

        return nextState;
    }
}

public class FlyState : CharacterState
{
    private CharacterState nextState;

    public FlyState (Character character) : base (character)
    { 
        nextState = this;
    }

    public override CharacterState Update()
    {
        character.SetHorizontalVelocity(Input.GetAxis("Horizontal"));
        if (Input.GetButtonDown("Fire1"))
            nextState = new FlyingShurikenState(character);
        else if(character.IsGrounded())
        {
            if (character.IsMovingHorizontally())
                nextState = new RunState(character);
            else
                nextState = new StandState(character);
        }
        return nextState;
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
        if (Input.GetButtonDown("Fire1"))
            nextState = new GroundedShurikenState(character);
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

public class GroundedShurikenState : CharacterState
{
    private CharacterState nextState;

    public GroundedShurikenState(Character character) : base (character)
    {
        nextState = this;
    }

    public override void Start()
    {
        character.TriggerAnimation("Shuriken");
        GlobalSignalManager.Inst.AddListener<CharacterLeftGroundSignal>(onCharacterLeftGround);
        GlobalSignalManager.Inst.AddListener<ThrowAnimationEndedSignal>(onThrowAnimationEnded);
        GlobalSignalManager.Inst.AddListener<ThrowAnimationReleasePointSignal>(onThrowAnimationReleasePoint);
    }

    public override CharacterState Update()
    {
        character.SetHorizontalVelocity(Input.GetAxis("Horizontal"));
        if (Input.GetButtonDown("Jump"))
            character.Jump();
        return nextState;
    }

    public override void End()
    {
        GlobalSignalManager.Inst.RemoveListener<CharacterLeftGroundSignal>(onCharacterLeftGround);
        GlobalSignalManager.Inst.RemoveListener<ThrowAnimationEndedSignal>(onThrowAnimationEnded);
        GlobalSignalManager.Inst.RemoveListener<ThrowAnimationReleasePointSignal>(onThrowAnimationReleasePoint);
    }

    private void onCharacterLeftGround(GlobalSignal signal)
    {
        nextState = new FlyState(character);
    }

    private void onThrowAnimationReleasePoint(GlobalSignal signal)
    {
        character.SpawnShuriken();
    }

    private void onThrowAnimationEnded(GlobalSignal signal)
    {
        if (character.IsMovingHorizontally())
            nextState = new RunState(character);
        else
            nextState = new StandState(character);
    }
}

public class FlyingShurikenState : CharacterState
{
    private CharacterState nextState;

    public FlyingShurikenState(Character character) : base (character)
    {
        nextState = this;
    }

    public override void Start()
    {
        character.TriggerAnimation("Shuriken");
        GlobalSignalManager.Inst.AddListener<CharacterHitGroundSignal>(onCharacterHitGround);
        GlobalSignalManager.Inst.AddListener<ThrowAnimationEndedSignal>(onThrowAnimationEnded);
        GlobalSignalManager.Inst.AddListener<ThrowAnimationReleasePointSignal>(onThrowAnimationReleasePoint);
    }

    public override CharacterState Update()
    {
        character.SetHorizontalVelocity(Input.GetAxis("Horizontal"));
        return nextState;
    }

    public override void End()
    {
        GlobalSignalManager.Inst.RemoveListener<CharacterHitGroundSignal>(onCharacterHitGround);
        GlobalSignalManager.Inst.RemoveListener<ThrowAnimationEndedSignal>(onThrowAnimationEnded);
        GlobalSignalManager.Inst.RemoveListener<ThrowAnimationReleasePointSignal>(onThrowAnimationReleasePoint);
    }

    private void onCharacterHitGround(GlobalSignal signal)
    {
        if (character.IsMovingHorizontally())
            nextState = new RunState(character);
        else
            nextState = new StandState(character);
    }

    private void onThrowAnimationReleasePoint(GlobalSignal signal)
    {
        character.SpawnShuriken();
    }

    private void onThrowAnimationEnded(GlobalSignal signal)
    {
        nextState = new FlyState(character);
    }
}