using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DanesUnityLibrary;


public class BloodDemonController : MonoBehaviour
{
    [Header("Ranges")]
    [SerializeField] public float aggroRange;
    [SerializeField] public float attackRange;
    [SerializeField] public float dashMultiplyer;

    [Header("References and debug")]
    [SerializeField] public Transform player;
    [SerializeField] private bool debug;

    [Header("Attack")]
    [SerializeField] private GameObject hitHitBox;
    [SerializeField] public float hitCooldown;

    [HideInInspector] public HitBoxController hitHitBoxController { get; private set; }
    [HideInInspector] public NavMeshAgent navigation { get; private set; }

    [HideInInspector] public StateMachine<BloodDemonController> stateMachine { get; private set; }
    [HideInInspector] public BloodDemonIdle idleState { get; private set; }
    [HideInInspector] public BloodDemonMove movementState { get; private set; }
    [HideInInspector] public BloodDemonAttack attackState { get; private set; }
    [HideInInspector] public BloodDemonDash dashState { get; private set; }
 
    private void Awake()
    {
        stateMachine = new StateMachine<BloodDemonController>(this);
        idleState = new BloodDemonIdle();
        movementState = new BloodDemonMove();
        attackState = new BloodDemonAttack();
        dashState = new BloodDemonDash();
    }

    private void Start()
    {
        hitHitBoxController = hitHitBox.GetComponent<HitBoxController>();
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
        owner.navigation.enabled = true;
        owner.navigation.SetDestination(owner.player.position);
    }

    public override void ExitState(BloodDemonController owner)
    {
    }

    public override void UpdateState(BloodDemonController owner)
    {
        owner.navigation.SetDestination(owner.player.position);
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
        owner.hitHitBoxController.ExposeHitBox();
    }

    public override void ExitState(BloodDemonController owner)
    {
    }

    public override void UpdateState(BloodDemonController owner)
    {
        owner.stateMachine.ChangeState(owner.dashState);
    }
}

public class BloodDemonDash : State<BloodDemonController>
{
    Vector3 newVector = Vector3.zero;
    private Timer dashTimer;

    public override void EnterState(BloodDemonController owner)
    {
        dashTimer = new Timer(owner.hitCooldown);

        owner.navigation.enabled = false;
        owner.GetComponent<Rigidbody>().isKinematic = false;
        owner.GetComponent<Rigidbody>().useGravity = true;

        newVector = owner.player.position - owner.transform.position;
        owner.GetComponent<Rigidbody>().AddForce(new Vector3(-newVector.x * owner.dashMultiplyer * 10000, -newVector.y * owner.dashMultiplyer, 0)); //den börjar använda gravity eftersom den inte ska snappa ner
    }

    public override void ExitState(BloodDemonController owner)
    {
        owner.navigation.enabled = true;
        owner.GetComponent<Rigidbody>().isKinematic = true;
        owner.GetComponent<Rigidbody>().useGravity = false;
        dashTimer.Reset();
    }

    public override void UpdateState(BloodDemonController owner)
    {
        dashTimer.UpdateTimer(Time.deltaTime);

        if (dashTimer.Expired)
        {
            owner.stateMachine.ChangeState(owner.idleState);
        }
    }
}