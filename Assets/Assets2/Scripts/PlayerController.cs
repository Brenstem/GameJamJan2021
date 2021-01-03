using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanesUnityLibrary;

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

    public GameObject groundCheckObject;
    public float groundCheckRadius;
    public LayerMask groundLayerMask;

    public float jumpSpeed;
    public float jumpTime;

    public float gravityScale;
    public float terminalVelocity;
    [HideInInspector] public float moveX;

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


    public void AirStrafe()
    {
        float moveX = Input.GetAxis("Horizontal");
        targetVelocity = new Vector3(moveX * runningMaxSpeed, rb.velocity.y, 0f);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref zeroVector, playerMovementSmoothingAcceleration);

        Debug.DrawRay(transform.position, targetVelocity, Color.green);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheckObject.transform.position, groundCheckRadius);
    }

}

public class PlayerIdleState : State<PlayerController>
{
    public override void EnterState(PlayerController owner)
    {
        //Debug.Log("idle");
    }

    public override void ExitState(PlayerController owner)
    {

    }

    public override void UpdateState(PlayerController owner)
    {
        //Collider[] groundCol = Physics.OverlapSphere(owner.groundCheckObject.transform.position, owner.groundCheckDistance, owner.groundLayerMask);

        //RaycastHit hit = owner.GroundCheck();

        //Physics.Raycast(owner.groundCheckObject.transform.position, Vector3.down, out hit, owner.groundCheckDistance, owner.groundLayerMask);

        Collider[] groundCol = Physics.OverlapSphere(owner.groundCheckObject.transform.position, owner.groundCheckRadius, owner.groundLayerMask);

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
    public override void EnterState(PlayerController owner)
    {
        //Debug.Log("shmoovin");
    }

    public override void ExitState(PlayerController owner)
    {

    }

    public override void UpdateState(PlayerController owner)
    {
        //RaycastHit hit = owner.GroundCheck();
        //Physics.Raycast(owner.groundCheckObject.transform.position, Vector3.down, out hit, owner.groundCheckDistance, owner.groundLayerMask);

        Collider[] groundCol = Physics.OverlapSphere(owner.groundCheckObject.transform.position, owner.groundCheckRadius, owner.groundLayerMask);

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


        owner.moveX = Input.GetAxis("Horizontal");
        
        owner.targetVelocity = new Vector3(owner.moveX * owner.runningMaxSpeed, 0f, 0f);

        RaycastHit hit;
        Physics.Raycast(owner.groundCheckObject.transform.position, Vector3.down, out hit, Mathf.Infinity, owner.groundLayerMask);

        if (hit.normal.z > 0.05f || hit.normal.z < -0.05f)
            Debug.LogError(hit.collider.gameObject.name + " has a Y and/or X rotation thats not 0, make it 0 :)");


        float angualDifferenc = Vector3.SignedAngle(hit.normal, Vector3.up, Vector3.forward);
        
        Debug.Log(angualDifferenc);

        //owner.targetVelocity = Quaternion.AngleAxis(angualDifferenc /** Mathf.Sign(owner.moveX)*/, Vector3.forward) * owner.targetVelocity;
        owner.targetVelocity = Quaternion.Euler(0f, 0f, -angualDifferenc /** Mathf.Sign(owner.moveX)*/) * owner.targetVelocity;

        Debug.Log(owner.targetVelocity);

        Debug.DrawRay(owner.transform.position, owner.targetVelocity, Color.green);

        //owner.rb.velocity = Vector3.SmoothDamp(owner.rb.velocity, owner.targetVelocity, ref owner.zeroVector, owner.playerMovementSmoothingAcceleration);
        
        owner.rb.velocity = owner.targetVelocity;

        //Vector3 newVelocity = new Vector3(Mathf.Clamp(owner.rb.velocity.x + moveX * owner.runningAccelerationSpeed * Time.fixedDeltaTime, -owner.runningMaxSpeed, owner.runningMaxSpeed), 0, 0);
        //newVelocity = Vector3.ClampMagnitude(newVelocity, owner.runningMaxSpeed);
        //owner.rb.velocity = newVelocity;
        //Debug.Log(owner.rb.velocity);

        //Debug.Break();
    }
}

public class PlayerJumpingState : State<PlayerController>
{
    Timer jumpTimer;

    public override void EnterState(PlayerController owner)
    {
        //Debug.Log("jumbi");
        jumpTimer = new Timer(owner.jumpTime);
    }

    public override void ExitState(PlayerController owner)
    {

    }

    public override void UpdateState(PlayerController owner)
    {
        //Collider[] groundCol = Physics.OverlapSphere(owner.groundCheckObject.transform.position, owner.groundCheckDistance, owner.groundLayerMask);
        jumpTimer.UpdateTimer(Time.fixedDeltaTime);

        //Collider[] groundCol = Physics.OverlapSphere(owner.groundCheckObject.transform.position, owner.groundCheckRadius, owner.groundLayerMask);
        //if (groundCol.Length >= 1)
        //{
        //    owner.playerControllerStateMachine.ChangeState(owner.playerIdleState);
        //}
        /*else*/ if (jumpTimer.Expired || !Input.GetKey(KeyCode.Space))
        {
            owner.playerControllerStateMachine.ChangeState(owner.playerFallingState);
        }

        owner.AirStrafe();

        owner.rb.velocity = new Vector3(owner.rb.velocity.x, owner.jumpSpeed, 0);
    }
}

public class PlayerFallingState : State<PlayerController>
{

    public override void EnterState(PlayerController owner)
    {
        Debug.LogWarning("-----------------falling-----------------");
        //Debug.Log("falling");
    }

    public override void ExitState(PlayerController owner)
    {

    }

    public override void UpdateState(PlayerController owner)
    {
        //RaycastHit hit = owner.GroundCheck();
        //if (hit.collider != null)
        //{
        //    owner.playerControllerStateMachine.ChangeState(owner.playerIdleState);
        //}

        Collider[] groundCol = Physics.OverlapSphere(owner.groundCheckObject.transform.position, owner.groundCheckRadius, owner.groundLayerMask);
        if (groundCol.Length >= 1)
        {
            owner.playerControllerStateMachine.ChangeState(owner.playerIdleState);
        }


        owner.AirStrafe();

        //gravity 
        owner.rb.velocity += new Vector3(0, -owner.gravityScale, 0);
        owner.rb.velocity = new Vector3(owner.rb.velocity.x, Mathf.Clamp(owner.rb.velocity.y, -owner.terminalVelocity, Mathf.Infinity), 0);
    }
}