using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IceDemonController : MonoBehaviour
{
    [SerializeField] public Transform player;
    [SerializeField] public float aggroRange;
    [SerializeField] public float attackRange;
    [SerializeField] private bool debug;

    [HideInInspector] public NavMeshAgent navigation { get; private set; }

    [HideInInspector] public StateMachine<IceDemonController> stateMachine { get; private set; }
    [HideInInspector] public IceDemonIdle idleState { get; private set; }
    [HideInInspector] public IceDemonMove movementState { get; private set; }
    [HideInInspector] public IceDemonAttack attackState { get; private set; }
    [HideInInspector] public IceDemonShield shieldState { get; private set; }

    private void Awake()
    {
        stateMachine = new StateMachine<IceDemonController>(this);
        idleState = new IceDemonIdle();
        movementState = new IceDemonMove();
        attackState = new IceDemonAttack();
        shieldState = new IceDemonShield();
    }

    private void Start()
    {
        navigation = GetComponent<NavMeshAgent>();
        stateMachine.ChangeState(idleState);

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        stateMachine.Update();

        if (debug)
            print(stateMachine.currentState);
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, aggroRange);
        }
    }
}

public class IceDemonIdle : State<IceDemonController>
{
    public override void EnterState(IceDemonController owner)
    {
    }

    public override void ExitState(IceDemonController owner)
    {
    }

    public override void UpdateState(IceDemonController owner)
    {
        if (Vector3.Distance(owner.transform.position, owner.player.position) < owner.aggroRange)
        {
            owner.stateMachine.ChangeState(owner.movementState);
        }
    }
}

public class IceDemonMove : State<IceDemonController>
{
    public override void EnterState(IceDemonController owner)
    {
        owner.navigation.SetDestination(owner.player.position);
    }

    public override void ExitState(IceDemonController owner)
    {
    }

    public override void UpdateState(IceDemonController owner)
    {
        if (Vector3.Distance(owner.transform.position, owner.player.position) < owner.attackRange)
        {
            owner.stateMachine.ChangeState(owner.attackState);
            owner.navigation.SetDestination(owner.transform.position);
        }
        else if (Vector3.Distance(owner.transform.position, owner.player.position) > owner.aggroRange)
        {
            owner.stateMachine.ChangeState(owner.idleState);
        }
    }
}

public class IceDemonShield : State<IceDemonController>
{
    public override void EnterState(IceDemonController owner)
    {
        Debug.Log("Shield!");
    }

    public override void ExitState(IceDemonController owner)
    {
    }

    public override void UpdateState(IceDemonController owner)
    {
    }
}

public class IceDemonAttack : State<IceDemonController>
{
    public override void EnterState(IceDemonController owner)
    {
        Debug.Log("Attack!");
    }

    public override void ExitState(IceDemonController owner)
    {

    }

    public override void UpdateState(IceDemonController owner)
    {
        if (Vector3.Distance(owner.transform.position, owner.player.position) > owner.attackRange)
        {
            owner.stateMachine.ChangeState(owner.movementState);
        }
    }
}
