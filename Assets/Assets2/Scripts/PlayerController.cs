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

    [Range(0, .3f)] public float playerMovementSmoothingAcceleration = .05f;
    [Range(0, .3f)] public float playerMovementSmoothingSlowDown = .05f;

    public GameObject groundCheckPosition;
    public float groundCheckRadius;
    public LayerMask groundLayerMask;


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
        Collider[] groundCol = Physics.OverlapSphere(owner.groundCheckPosition.transform.position, owner.groundCheckRadius, owner.groundLayerMask);

        if (Input.GetKey(KeyCode.Space)/* || Input.GetKey(KeyCode.W)*/)
        {
            owner.playerControllerStateMachine.ChangeState(owner.playerJumpingState);
        }
        else if (groundCol.Length == 0)
        {
            owner.playerControllerStateMachine.ChangeState(owner.playerFallingState);
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            owner.playerControllerStateMachine.ChangeState(owner.playerRunningState);
        }

        //slowdown
        owner.rb.velocity = Vector3.SmoothDamp(owner.rb.velocity, Vector2.zero, ref owner.zeroVector, owner.playerMovementSmoothingSlowDown);



    }
}

public class PlayerRunningState : State<PlayerController>
{
    float moveX;
    public override void EnterState(PlayerController owner)
    {
        Debug.Log("shmoovin");
    }

    public override void ExitState(PlayerController owner)
    {

    }

    public override void UpdateState(PlayerController owner)
    {
        Collider[] groundCol = Physics.OverlapSphere(owner.groundCheckPosition.transform.position, owner.groundCheckRadius, owner.groundLayerMask);

        if (Input.GetKey(KeyCode.Space)/* || Input.GetKey(KeyCode.W)*/)
        {
            owner.playerControllerStateMachine.ChangeState(owner.playerJumpingState);
        }
        else if (groundCol.Length == 0)
        {
            owner.playerControllerStateMachine.ChangeState(owner.playerFallingState);
        }
        else if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            owner.playerControllerStateMachine.ChangeState(owner.playerIdleState);
        }

        moveX = Input.GetAxis("Horizontal");
        owner.targetVelocity = new Vector2(moveX * owner.runningMaxSpeed, owner.rb.velocity.y);
        owner.rb.velocity = Vector3.SmoothDamp(owner.rb.velocity, owner.targetVelocity, ref owner.zeroVector, owner.playerMovementSmoothingAcceleration);

        //Vector3 newVelocity = new Vector3(Mathf.Clamp(owner.rb.velocity.x + moveX * owner.runningAccelerationSpeed * Time.fixedDeltaTime, -owner.runningMaxSpeed, owner.runningMaxSpeed), 0, 0);
        //newVelocity = Vector3.ClampMagnitude(newVelocity, owner.runningMaxSpeed);
        //owner.rb.velocity = newVelocity;

        Debug.Log(owner.rb.velocity);
    }
}

public class PlayerJumpingState : State<PlayerController>
{
    float moveX;

    public override void EnterState(PlayerController owner)
    {
        Debug.Log("jumbi");
    }

    public override void ExitState(PlayerController owner)
    {

    }

    public override void UpdateState(PlayerController owner)
    {
        Collider[] groundCol = Physics.OverlapSphere(owner.groundCheckPosition.transform.position, owner.groundCheckRadius, owner.groundLayerMask);
        if (groundCol.Length == 0)
        {
            owner.playerControllerStateMachine.ChangeState(owner.playerIdleState);
        }

        moveX = Input.GetAxis("Horizontal");
        owner.targetVelocity = new Vector2(moveX * owner.runningMaxSpeed, owner.rb.velocity.y);
        owner.rb.velocity = Vector3.SmoothDamp(owner.rb.velocity, owner.targetVelocity, ref owner.zeroVector, owner.playerMovementSmoothingAcceleration);

    }
}

public class PlayerFallingState : State<PlayerController>
{
    float moveX;

    public override void EnterState(PlayerController owner)
    {
        Debug.Log("falling");
    }

    public override void ExitState(PlayerController owner)
    {

    }

    public override void UpdateState(PlayerController owner)
    {
        Collider[] groundCol = Physics.OverlapSphere(owner.groundCheckPosition.transform.position, owner.groundCheckRadius, owner.groundLayerMask);
        if (groundCol.Length == 0)
        {
            owner.playerControllerStateMachine.ChangeState(owner.playerIdleState);
        }

        moveX = Input.GetAxis("Horizontal");
        owner.targetVelocity = new Vector2(moveX * owner.runningMaxSpeed, owner.rb.velocity.y);
        owner.rb.velocity = Vector3.SmoothDamp(owner.rb.velocity, owner.targetVelocity, ref owner.zeroVector, owner.playerMovementSmoothingAcceleration);

    }
}