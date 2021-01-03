using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DanesUnityLibrary;

public class PlagueFrogController : MonoBehaviour
{
    [Header("Ranges")]
    [SerializeField] public float aggroRange;
    [SerializeField] public float attackRange;

    [Header("References and debug")]
    [SerializeField] private bool debug;
    [SerializeField] public Transform player;

    [Header("Attack")]
    [SerializeField] private GameObject biteHitBox;
    [SerializeField] public float biteCooldown;


    [HideInInspector] public HitBoxController biteHitBoxController { get; private set; }

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
        biteHitBoxController = biteHitBox.GetComponent<HitBoxController>();
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
        owner.navigation.SetDestination(new Vector3(owner.player.position.x, 0));
    }

    public override void ExitState(PlagueFrogController owner)
    {
    }

    public override void UpdateState(PlagueFrogController owner)
    {
        owner.navigation.SetDestination(new Vector3(owner.player.position.x, 0));

        Debug.Log(owner.navigation.destination);

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
    private Timer attackTimer;

    public override void EnterState(PlagueFrogController owner)
    {
        Debug.Log("Attack!");
        owner.biteHitBoxController.ExposeHitBox();
        attackTimer = new Timer(owner.biteCooldown);
    }

    public override void ExitState(PlagueFrogController owner)
    {
        attackTimer.Reset();
    }

    public override void UpdateState(PlagueFrogController owner)
    {
        attackTimer.UpdateTimer(Time.deltaTime);

        if (Vector3.Distance(owner.transform.position, owner.player.position) > owner.attackRange)
        {
            owner.stateMachine.ChangeState(owner.movementState);
        }
        else if (attackTimer.Expired)
        {
            Debug.Log("Attack!");
            owner.biteHitBoxController.ExposeHitBox();
            attackTimer.Reset();
        }
    }
}


