using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanesUnityLibrary;

public class PlayerController : MonoBehaviour
{
    #region State machine stuff
    public StateMachine<PlayerController> stateMachine;

    public PlayerIdleState playerIdleState;
    public PlayerRunningState playerRunningState;
    public PlayerJumpingState playerJumpingState;
    public PlayerFallingState playerFallingState;

    public PlayerAttackingState playerAttackingState;
    #endregion

    #region Attack Vars
    public GameObject attackHitboxObject;
    [HideInInspector] public HitBoxController attackHitboxScript;

    public float attackStartupTime;
    public float attackTotalTime;
    #endregion

    #region Movement Vars
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Vector3 targetVelocity;
    [HideInInspector] public Vector3 zeroVector = Vector3.zero;
    [HideInInspector] public float moveX;
    [HideInInspector] public bool lookingLeft;

    public float runningMaxSpeed;
    public float runningAccelerationSpeed;
    [Range(0, .3f)] public float playerMovementGroundedAccelerationSmoothing = .05f;
    [Range(0, .3f)] public float playerMovementGroundedSlowDownSmoothing = .05f;
    [Range(0, .3f)] public float playerMovementAirborneAccelerationSmoothing = .05f;

    public GameObject groundCheckObject;
    public float groundCheckRadius;
    public LayerMask groundLayerMask;

    public float airborneWallDetectionDistance;

    public float jumpSpeed;
    public float jumpTime;

    public float gravityScale;
    public float terminalVelocity;
    #endregion

    public Animator animator;

    private void Awake()
    {
        stateMachine = new StateMachine<PlayerController>(this);

        playerIdleState = new PlayerIdleState();
        playerRunningState = new PlayerRunningState();
        playerJumpingState = new PlayerJumpingState();
        playerFallingState = new PlayerFallingState();
        playerAttackingState = new PlayerAttackingState();

        stateMachine.ChangeState(playerIdleState);

        rb = GetComponent<Rigidbody>();

        attackHitboxScript = attackHitboxObject.GetComponent<HitBoxController>();
    }

    private void Update()
    {
        stateMachine.Update();
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void SlowDown()
    {
        rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector2.zero, ref zeroVector, playerMovementGroundedSlowDownSmoothing);
    }

    public void AirStrafe()
    {
        moveX = Input.GetAxis("Horizontal");

        if (!Physics.Raycast(transform.position, Vector3.right * Mathf.Sign(moveX), airborneWallDetectionDistance, groundLayerMask))
        {
            targetVelocity = new Vector3(moveX * runningMaxSpeed, rb.velocity.y, 0f);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref zeroVector, playerMovementAirborneAccelerationSmoothing);
        }

        //targetVelocity = new Vector3(moveX * runningMaxSpeed, rb.velocity.y, 0f);
        //rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref zeroVector, playerMovementAirborneAccelerationSmoothing);

        HandleYRotation();

        //Debug.DrawRay(transform.position, targetVelocity, Color.green);
    }

    public void HandleYRotation()
    {
        //spelarens rotation runt Y axeln
        if (moveX < 0 && !lookingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            lookingLeft = true;
        }
        else if (moveX  > 0 && lookingLeft)
        {
            print("it flip");
            transform.rotation = Quaternion.Euler(0, 0, 0);
            lookingLeft = false;
        }
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
        Debug.Log("idle");
        //Debug.Break();
    }

    public override void ExitState(PlayerController owner)
    { }

    public override void FixedUpdateState(PlayerController owner)
    {
        owner.SlowDown();
    }

    public override void UpdateState(PlayerController owner)
    {
        #region State transition checks
        Collider[] groundCol = Physics.OverlapSphere(owner.groundCheckObject.transform.position, owner.groundCheckRadius, owner.groundLayerMask);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            owner.stateMachine.ChangeState(owner.playerJumpingState);
        }
        else if (groundCol.Length == 0)
        {
            owner.stateMachine.ChangeState(owner.playerFallingState);
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            owner.stateMachine.ChangeState(owner.playerRunningState);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            owner.stateMachine.ChangeState(owner.playerAttackingState);
        }
        #endregion
    }
}

public class PlayerRunningState : State<PlayerController>
{
    public override void EnterState(PlayerController owner)
    {
        Debug.Log("shmoovin");
        owner.animator.SetBool("Running", true);
    }

    public override void ExitState(PlayerController owner)
    {
        owner.animator.SetBool("Running", false);
    }

    public override void FixedUpdateState(PlayerController owner)
    {
        #region Movement
        owner.moveX = Input.GetAxis("Horizontal");
        owner.targetVelocity = new Vector3(owner.moveX * owner.runningMaxSpeed, 0f, 0f);

        RaycastHit hit;
        Physics.Raycast(owner.groundCheckObject.transform.position, Vector3.down, out hit, Mathf.Infinity, owner.groundLayerMask);

        if (hit.normal.z > 0.05f || hit.normal.z < -0.05f)
            Debug.LogError(hit.collider.gameObject.name + " has a Y and/or X rotation thats not 0, make it 0 :)");

        float angualDifferenc = Vector3.SignedAngle(hit.normal, Vector3.up, Vector3.forward);
        owner.targetVelocity = Quaternion.Euler(0f, 0f, -angualDifferenc) * owner.targetVelocity;
        //Debug.DrawRay(owner.transform.position, owner.targetVelocity, Color.green);

        owner.rb.velocity = Vector3.SmoothDamp(owner.rb.velocity, owner.targetVelocity, ref owner.zeroVector, owner.playerMovementGroundedAccelerationSmoothing);
        #endregion

        owner.HandleYRotation();
    }

    public override void UpdateState(PlayerController owner)
    {
        #region State transition checks
        Collider[] groundCol = Physics.OverlapSphere(owner.groundCheckObject.transform.position, owner.groundCheckRadius, owner.groundLayerMask);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            owner.stateMachine.ChangeState(owner.playerJumpingState);
        }
        else if (groundCol.Length == 0)
        {
            owner.stateMachine.ChangeState(owner.playerFallingState);
        }
        else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            owner.stateMachine.ChangeState(owner.playerIdleState);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            owner.stateMachine.ChangeState(owner.playerAttackingState);
        }
        #endregion
    }
}

public class PlayerJumpingState : State<PlayerController>
{
    Timer jumpTimer;

    public override void EnterState(PlayerController owner)
    {
        Debug.Log("jumbi");
        jumpTimer = new Timer(owner.jumpTime);
    }

    public override void ExitState(PlayerController owner)
    { }

    public override void FixedUpdateState(PlayerController owner)
    {
        owner.AirStrafe();

        //jump
        owner.rb.velocity = new Vector3(owner.rb.velocity.x, owner.jumpSpeed, 0);
    }

    public override void UpdateState(PlayerController owner)
    {
        #region State transition checks
        jumpTimer.UpdateTimer(Time.deltaTime);
        //Collider[] groundCol = Physics.OverlapSphere(owner.groundCheckObject.transform.position, owner.groundCheckRadius, owner.groundLayerMask);
        //if (groundCol.Length >= 1)
        //{
        //    owner.playerControllerStateMachine.ChangeState(owner.playerIdleState);
        //}
        /*else*/
        if (jumpTimer.Expired || Input.GetKeyUp(KeyCode.Space))
        {
            owner.stateMachine.ChangeState(owner.playerFallingState);
        }
        #endregion
    }
}

public class PlayerFallingState : State<PlayerController>
{

    public override void EnterState(PlayerController owner)
    {
        //Debug.Log("falling");
    }

    public override void ExitState(PlayerController owner)
    { }

    public override void FixedUpdateState(PlayerController owner)
    {
        owner.AirStrafe();

        #region Gravity
        owner.rb.velocity += new Vector3(0, -owner.gravityScale, 0);
        owner.rb.velocity = new Vector3(owner.rb.velocity.x, Mathf.Clamp(owner.rb.velocity.y, -owner.terminalVelocity, Mathf.Infinity), 0);
        #endregion
    }

    public override void UpdateState(PlayerController owner)
    {
        #region State transition checks
        Collider[] groundCol = Physics.OverlapSphere(owner.groundCheckObject.transform.position, owner.groundCheckRadius, owner.groundLayerMask);
        if (groundCol.Length >= 1)
        {
            owner.stateMachine.ChangeState(owner.playerIdleState);
        }
        #endregion
    }
}

public class PlayerAttackingState : State<PlayerController>
{
    private Timer timer;

    public override void EnterState(PlayerController owner)
    {
        //Debug.Log("Attack");

        owner.animator.SetTrigger("Punch");

        timer = new Timer(owner.attackTotalTime);
        owner.StartCoroutine(HitBoxActivationDelay(owner));
    }

    public override void ExitState(PlayerController owner)
    { }

    public override void FixedUpdateState(PlayerController owner)
    { 
        owner.SlowDown();
    }

    public override void UpdateState(PlayerController owner)
    {
        #region State transition checks
        timer.UpdateTimer(Time.deltaTime);
        if (timer.Expired)
        {
            owner.stateMachine.ChangeState(owner.playerIdleState);
        }
        #endregion
    }

    IEnumerator HitBoxActivationDelay(PlayerController owner)
    {
        yield return new WaitForSeconds(owner.attackTotalTime);
        owner.attackHitboxScript.ExposeHitBox();
    }
}