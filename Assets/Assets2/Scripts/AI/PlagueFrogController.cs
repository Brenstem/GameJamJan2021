using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlagueFrogController : MonoBehaviour
{
    [SerializeField] public Transform player;
    [SerializeField] public float aggroRange;
    [SerializeField] public float attackRange;
    [SerializeField] private bool debug;

    [HideInInspector] public NavMeshAgent navigation { get; private set; }

    [HideInInspector] public StateMachine<PlagueFrogController> stateMachine { get; private set; }
    [HideInInspector] public PlagueFrogIdle idleState { get; private set; }
    [HideInInspector] public PlagueFrogMove movementState { get; private set; }
    [HideInInspector] public PlagueFrogAttack attackState { get; private set; }

    private void Awake()
    {
        stateMachine = new StateMachine<PlagueFrogController>(this);
        idleState = new PlagueFrogIdle();
        movementState = new PlagueFrogMove();
        attackState = new PlagueFrogAttack();
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

public class PlagueFrogIdle : State<PlagueFrogController>
{
    public override void EnterState(PlagueFrogController owner)
    {
    }

    public override void ExitState(PlagueFrogController owner)
    {
    }

    public override void UpdateState(PlagueFrogController owner)
    {
        if (Vector3.Distance(owner.transform.position, owner.player.position) < owner.aggroRange)
        {
            owner.stateMachine.ChangeState(owner.movementState);
        }
    }
}

public class PlagueFrogMove : State<PlagueFrogController>
{
    public override void EnterState(PlagueFrogController owner)
    {
        owner.navigation.SetDestination(owner.player.position);
    }

    public override void ExitState(PlagueFrogController owner)
    {
    }

    public override void UpdateState(PlagueFrogController owner)
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

public class PlagueFrogAttack : State<PlagueFrogController>
{
    public override void EnterState(PlagueFrogController owner)
    {
        Debug.Log("Attack!");
    }

    public override void ExitState(PlagueFrogController owner)
    {
        
    }

    public override void UpdateState(PlagueFrogController owner)
    {
        if (Vector3.Distance(owner.transform.position, owner.player.position) > owner.attackRange)
        {
            owner.stateMachine.ChangeState(owner.movementState);
        }
    }
}


