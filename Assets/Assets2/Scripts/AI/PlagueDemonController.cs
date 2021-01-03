using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DanesUnityLibrary;

public class PlagueDemonController : MonoBehaviour
{
    [Header("Ranges")]
    [SerializeField] public float stoppingRange;
    [SerializeField] public float aggroRange;

    [Header("Frog Spawn Attack")]
    [SerializeField] private GameObject plagueFrogPrefab;
    [SerializeField] private Transform frogSpawnPosition;
    [SerializeField] public int minFrogSpawnAmount;
    [SerializeField] public int maxFrogSpawnAmount;
    [SerializeField] public float spawnFrogsTime;

    [Header("References and Debug")]
    [SerializeField] public Transform player;
    [SerializeField] private bool debug;

    [HideInInspector] public NavMeshAgent navigation { get; private set; }
    [HideInInspector] public StateMachine<PlagueDemonController> stateMachine { get; private set; }
    [HideInInspector] public PlagueDemonIdle idleState { get; private set; }
    [HideInInspector] public PlagueDemonMove movementState { get; private set; }
    [HideInInspector] public PlaugeDemonSpawn spawnAttack { get; private set; }

    private void Awake()
    {
        stateMachine = new StateMachine<PlagueDemonController>(this);
        idleState = new PlagueDemonIdle();
        movementState = new PlagueDemonMove();
        spawnAttack = new PlaugeDemonSpawn();
    }

    private void Start()
    {
        navigation = GetComponent<NavMeshAgent>();
        stateMachine.ChangeState(idleState);
    }

    void Update()
    {
        stateMachine.Update();

        if(debug)
            print(stateMachine.currentState);
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stoppingRange);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, aggroRange);
        }
    }

    public void SpawnPlagueFrogs(int amount) 
    {
        for (int i = 0; i < amount; i++)
        {
            Instantiate(plagueFrogPrefab, frogSpawnPosition.position, transform.rotation);
        }
    }
}

public class PlagueDemonIdle : State<PlagueDemonController>
{
    public override void EnterState(PlagueDemonController owner)
    {
    }

    public override void ExitState(PlagueDemonController owner)
    {
    }

    public override void UpdateState(PlagueDemonController owner)
    {
        if (Vector3.Distance(owner.transform.position, owner.player.position) < owner.aggroRange)
        {
            owner.stateMachine.ChangeState(owner.movementState);
        }
    }
}

public class PlagueDemonMove : State<PlagueDemonController>
{
    private Timer spawnFrogsTimer;

    public override void EnterState(PlagueDemonController owner)
    {
        owner.navigation.SetDestination(owner.player.position);
        spawnFrogsTimer = new Timer(owner.spawnFrogsTime);
    }

    public override void ExitState(PlagueDemonController owner)
    {
    }

    public override void UpdateState(PlagueDemonController owner)
    {
        owner.navigation.SetDestination(owner.player.position);

        spawnFrogsTimer.UpdateTimer(Time.deltaTime);

        if (Vector3.Distance(owner.transform.position, owner.player.transform.position) < owner.stoppingRange)
        {
            owner.navigation.SetDestination(owner.transform.position);

            if (spawnFrogsTimer.Expired)
                owner.stateMachine.ChangeState(owner.spawnAttack);
        }
        /*else if (Vector3.Distance(owner.transform.position, owner.player.position) >)
        {

        }*/
    }
}

public class PlaugeDemonSpawn : State<PlagueDemonController>
{
    public override void EnterState(PlagueDemonController owner)
    {
        owner.SpawnPlagueFrogs(Random.Range(owner.minFrogSpawnAmount, owner.maxFrogSpawnAmount));
    }

    public override void ExitState(PlagueDemonController owner)
    {
    }

    public override void UpdateState(PlagueDemonController owner)
    {
        owner.stateMachine.ChangeState(owner.stateMachine.previousState);
    }
}
