using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DanesUnityLibrary;

public class IceDemonController : MonoBehaviour
{
    [Header("Ranges")]
    [SerializeField] public float aggroRange;
    [SerializeField] public float attackRange;

    [Header("Blow attack")]
    [SerializeField] private GameObject blowHitBox;
    [SerializeField] public float blowCooldown;
    [HideInInspector] public HitBoxController blowHitBoxController { get; private set; }

    [Header("Shield")]
    [SerializeField] [Range(0, 1f)] private float shieldChance;
    [SerializeField] public float shieldModeDuration;

    [Header("References and debug")]
    [SerializeField] public Transform player;
    [SerializeField] private bool debug;

    [HideInInspector] public Health health { get; private set; }
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
        health = GetComponent<Health>();
        stateMachine.ChangeState(idleState);

        blowHitBoxController = blowHitBox.GetComponent<HitBoxController>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        health.TookDamage += ActivateShield;

        stateMachine.Update();

        if (Input.GetKeyDown(KeyCode.Space))
        {

        }

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

    public void ActivateShield(Debuffs debuff)
    {
        if (Random.Range(0, 1f) > shieldChance)
        {
            stateMachine.ChangeState(shieldState);
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

public class IceDemonAttack : State<IceDemonController>
{
    private Timer attackTimer;

    public override void EnterState(IceDemonController owner)
    {
        Debug.Log("Attack!");
        owner.blowHitBoxController.ExposeHitBox();
        attackTimer = new Timer(owner.blowCooldown);
    }

    public override void ExitState(IceDemonController owner)
    {
        attackTimer.Reset();

    }

    public override void UpdateState(IceDemonController owner)
    {
        attackTimer.UpdateTimer(Time.deltaTime);

        if (Vector3.Distance(owner.transform.position, owner.player.position) > owner.attackRange)
        {
            owner.stateMachine.ChangeState(owner.movementState);
        }
        else if (attackTimer.Expired)
        {
            Debug.Log("Attack!");
            owner.blowHitBoxController.ExposeHitBox();
            attackTimer.Reset();
        }
    }
}

public class IceDemonShield : State<IceDemonController>
{
    private Timer shieldDurationTimer;

    public override void EnterState(IceDemonController owner)
    {
        shieldDurationTimer = new Timer(owner.shieldModeDuration);

        Debug.Log("Shield!");
        owner.health.invulnerable = true;
    }

    public override void ExitState(IceDemonController owner)
    {
        owner.health.invulnerable = false;
    }

    public override void UpdateState(IceDemonController owner)
    {
        shieldDurationTimer.UpdateTimer(Time.deltaTime);

        owner.navigation.SetDestination(owner.transform.position);

        if (shieldDurationTimer.Expired)
        {
            owner.stateMachine.ChangeState(owner.stateMachine.previousState);
        }
    }
}

