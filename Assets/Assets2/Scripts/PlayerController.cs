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

    public float runningForce;

    public Rigidbody rb;

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
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            owner.playerControllerStateMachine.ChangeState(owner.playerRunningState);
        }
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

        owner.rb.AddForce(new Vector3(moveX * owner.runningForce * Time.fixedDeltaTime, 0, 0));
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