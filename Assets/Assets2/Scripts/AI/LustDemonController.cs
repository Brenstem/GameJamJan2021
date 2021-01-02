using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LustDemonController : MonoBehaviour
{
    [SerializeField] public Transform player;
    [SerializeField] public float aggroRange;
    [SerializeField] public float attackRange;
    [SerializeField] private bool debug;

    [HideInInspector] public NavMeshAgent navigation { get; private set; }

    [HideInInspector] public StateMachine<LustDemonController> stateMachine { get; private set; }
    [HideInInspector] public LustDemonIdle idleState { get; private set; }
    [HideInInspector] public LustDemonMove movementState { get; private set; }
    [HideInInspector] public LustDemonAttack attackState { get; private set; }

    private void Awake()
    {
        stateMachine = new StateMachine<LustDemonController>(this);
        idleState = new LustDemonIdle();
        movementState = new LustDemonMove();
        attackState = new LustDemonAttack();
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

public class LustDemonIdle : State<LustDemonController>
{
    public override void EnterState(LustDemonController owner)
    {
    }

    public override void ExitState(LustDemonController owner)
    {
    }

    public override void UpdateState(LustDemonController owner)
    {
        if (Vector3.Distance(owner.transform.position, owner.player.position) < owner.aggroRange)
        {
            owner.stateMachine.ChangeState(owner.movementState);
        }
    }
}

public class LustDemonMove : State<LustDemonController>
{
    public override void EnterState(LustDemonController owner)
    {
        owner.navigation.SetDestination(owner.player.position);
    }

    public override void ExitState(LustDemonController owner)
    {
    }

    public override void UpdateState(LustDemonController owner)
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

public class LustDemonAttack : State<LustDemonController>
{
    public override void EnterState(LustDemonController owner)
    {
        Debug.Log("Attack!");
    }

    public override void ExitState(LustDemonController owner)
    {

    }

    public override void UpdateState(LustDemonController owner)
    {
        if (Vector3.Distance(owner.transform.position, owner.player.position) > owner.attackRange)
        {
            owner.stateMachine.ChangeState(owner.movementState);
        }
    }
}
