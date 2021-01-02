using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BloodDemonController : MonoBehaviour
{
    [SerializeField] public Transform player;
    [SerializeField] public float aggroRange;
    [SerializeField] public float attackRange;
    [SerializeField] private bool debug;

    [HideInInspector] public NavMeshAgent navigation { get; private set; }

    [HideInInspector] public StateMachine<BloodDemonController> stateMachine { get; private set; }
    [HideInInspector] public BloodDemonIdle idleState { get; private set; }
    [HideInInspector] public BloodDemonMove movementState { get; private set; }
    [HideInInspector] public BloodDemonAttack attackState { get; private set; }

    private void Awake()
    {
        stateMachine = new StateMachine<BloodDemonController>(this);
        idleState = new BloodDemonIdle();
        movementState = new BloodDemonMove();
        attackState = new BloodDemonAttack();
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

public class BloodDemonIdle : State<BloodDemonController>
{
    public override void EnterState(BloodDemonController owner)
    {
    }

    public override void ExitState(BloodDemonController owner)
    {
    }

    public override void UpdateState(BloodDemonController owner)
    {
        if (Vector3.Distance(owner.transform.position, owner.player.position) < owner.aggroRange)
        {
            owner.stateMachine.ChangeState(owner.movementState);
        }
    }
}

public class BloodDemonMove : State<BloodDemonController>
{
    public override void EnterState(BloodDemonController owner)
    {
        owner.navigation.SetDestination(owner.player.position);
    }

    public override void ExitState(BloodDemonController owner)
    {
    }

    public override void UpdateState(BloodDemonController owner)
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

public class BloodDemonAttack : State<BloodDemonController>
{
    public override void EnterState(BloodDemonController owner)
    {
        Debug.Log("Attack!");
    }

    public override void ExitState(BloodDemonController owner)
    {

    }

    public override void UpdateState(BloodDemonController owner)
    {
        if (Vector3.Distance(owner.transform.position, owner.player.position) > owner.attackRange)
        {
            owner.stateMachine.ChangeState(owner.movementState);
        }
    }
}
