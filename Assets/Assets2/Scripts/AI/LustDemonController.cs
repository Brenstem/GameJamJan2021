using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DanesUnityLibrary;

public class LustDemonController : MonoBehaviour
{
    [Header("Ranges")]
    [SerializeField] public float aggroRange;
    [SerializeField] public float attackRange;

    [Header("Whip attack")]
    [SerializeField] private GameObject whipHitBox;
    [SerializeField] public float whipCooldown;

    [HideInInspector] public HitBoxController whipHitBoxController;
    
    [Header("References and debug")]
    [SerializeField] public Transform player;
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

        whipHitBoxController = whipHitBox.GetComponent<HitBoxController>();

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
    private Timer attackTimer;

    public override void EnterState(LustDemonController owner)
    {
        Debug.Log("Attack!");
        owner.whipHitBoxController.ExposeHitBox();
        attackTimer = new Timer(owner.whipCooldown);
    }

    public override void ExitState(LustDemonController owner)
    {
        attackTimer.Reset();
    }

    public override void UpdateState(LustDemonController owner)
    {
        attackTimer.UpdateTimer(Time.deltaTime);

        if (Vector3.Distance(owner.transform.position, owner.player.position) > owner.attackRange)
        {
            owner.stateMachine.ChangeState(owner.movementState);
        }
        else if (attackTimer.Expired)
        {
            Debug.Log("Attack!");
            owner.whipHitBoxController.ExposeHitBox();
            attackTimer.Reset();
        }
    }
}
