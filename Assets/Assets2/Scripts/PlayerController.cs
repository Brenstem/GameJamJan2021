using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public StateMachine<PlayerController> playerControllerStateMachine;

    public PlayerIdleState playerIdleState;
    public PlayerRunningState playerRunningState;
    public PlayerJumpingState playerJumpingState;
    public PlayerFallingState playerFallingState;

    public float runningMaxSpeed;
    public float runningAccelerationSpeed;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Vector3 targetVelocity;
    [HideInInspector] public Vector3 zeroVector = Vector3.zero;

    [Range(0, .3f)] public float playerMovementSmoothing = .05f;

    private void Awake()
    {
        playerControllerStateMachine = new StateMachine<PlayerController>(this);

        playerIdleState = new PlayerIdleState();
        playerRunningState = new PlayerRunningState();
        playerJumpingState = new PlayerJumpingState();
        playerFallingState = new PlayerFallingState();

        playerControllerStateMachine.ChangeState(playerIdleState);

        rb = GetComponent<Rigidbody>();

    }

    void Start()
    {

    }


    void FixedUpdate()
    {
        playerControllerStateMachine.Update();
    }
}

public class PlayerIdleState : State<PlayerController>
{
    public override void EnterState(PlayerController owner)
    {
        Debug.Log("idle");
    }

    public override void ExitState(PlayerController owner)
    {

    }

    public override void UpdateState(PlayerController owner)
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            owner.playerControllerStateMachine.ChangeState(owner.playerRunningState);
        }

        //myRigidbody.velocity = Vector3.SmoothDamp(myRigidbody.velocity, Vector2.zero, ref velocity, movementSmoothing);
        owner.rb.velocity = Vector3.SmoothDamp(owner.rb.velocity, Vector2.zero, ref owner.zeroVector, owner.playerMovementSmoothing);
    }
}

public class PlayerRunningState : State<PlayerController>
{
    float moveX;
    public override void EnterState(PlayerController owner)
    {
        Debug.Log("shoovin");
    }

    public override void ExitState(PlayerController owner)
    {

    }

    public override void UpdateState(PlayerController owner)
    {
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            owner.playerControllerStateMachine.ChangeState(owner.playerIdleState);
        }

        moveX = Input.GetAxis("Horizontal");

        owner.targetVelocity = new Vector2(moveX * owner.runningMaxSpeed, owner.rb.velocity.y);
        owner.rb.velocity = Vector3.SmoothDamp(owner.rb.velocity, owner.targetVelocity, ref owner.zeroVector, owner.playerMovementSmoothing);


        //Vector3 newVelocity = new Vector3(Mathf.Clamp(owner.rb.velocity.x + moveX * owner.runningAccelerationSpeed * Time.fixedDeltaTime, -owner.runningMaxSpeed, owner.runningMaxSpeed), 0, 0);

        //newVelocity = Vector3.ClampMagnitude(newVelocity, owner.runningMaxSpeed);

        //owner.rb.velocity = newVelocity;


        Debug.Log(owner.rb.velocity);
    }
}

public class PlayerJumpingState : State<PlayerController>
{
    public override void EnterState(PlayerController owner)
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState(PlayerController owner)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(PlayerController owner)
    {
        throw new System.NotImplementedException();
    }
}

public class PlayerFallingState : State<PlayerController>
{
    public override void EnterState(PlayerController owner)
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState(PlayerController owner)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(PlayerController owner)
    {
        throw new System.NotImplementedException();
    }
}