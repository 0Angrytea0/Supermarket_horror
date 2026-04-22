using UnityEngine;
using UnityEngine.AI;

public class MonsterPatrol : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float waitTime = 1f;

    [Header("Alert")]
    public Transform player;
    public float alertDistance = 4f;
    public float suspiciousLookTime = 2f;
    public float catchDelay = 1f;
    public float rotationSpeed = 5f;

    [Header("Alert Sound")]
    public AudioSource alertAudioSource;
    public AudioClip alertClip;
    public float alertVolumeMultiplier = 2f;

    private NavMeshAgent agent;
    private MonsterVision vision;
    private PlayerHideController playerHideController;

    private int currentPoint = 0;
    private float waitTimer;
    private bool waiting;

    private bool isAlerted = false;
    private float suspiciousTimer = 0f;
    private float catchTimer = 0f;
    private Animation anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<MonsterVision>();
        anim = GetComponentInChildren<Animation>();

        if (player != null)
            playerHideController = player.GetComponent<PlayerHideController>();

        if (patrolPoints.Length > 0 && agent.isOnNavMesh)
        {
            agent.SetDestination(patrolPoints[currentPoint].position);
        }
    }

    void Update()
    {
        if (!agent.isOnNavMesh) return;

        bool playerVisible = false;
        bool playerCloseAndClear = false;

        if (vision != null)
            playerVisible = vision.CanSeePlayer();

        playerCloseAndClear = IsPlayerCloseAndUnblocked();

        if (!isAlerted && (playerVisible || playerCloseAndClear))
        {
            StartAlert();
        }

        if (isAlerted)
        {
            HandleAlertState(playerVisible || playerCloseAndClear);
            return;
        }

        HandlePatrol();
    }

    void HandlePatrol()
    {
        if (patrolPoints.Length == 0) return;

        if (!waiting && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            waiting = true;
            waitTimer = waitTime;
        }

        if (waiting)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                currentPoint = (currentPoint + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentPoint].position);
                waiting = false;
            }
        }
    }

    void StartAlert()
    {
        if (alertAudioSource != null && alertClip != null)
        {
            alertAudioSource.pitch = Random.Range(0.95f, 1.05f);
            alertAudioSource.PlayOneShot(alertClip, alertVolumeMultiplier);
        }

        if (anim != null)
            anim.enabled = false;

        isAlerted = true;
        suspiciousTimer = suspiciousLookTime;
        catchTimer = catchDelay;

        agent.isStopped = true;
    }

    void HandleAlertState(bool playerStillDetectable)
    {
        if (player != null)
        {
            Vector3 lookDirection = player.position - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        if (playerStillDetectable)
        {
            catchTimer -= Time.deltaTime;

            if (catchTimer <= 0f)
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.LoseGame();
            }

            return;
        }

        suspiciousTimer -= Time.deltaTime;

        if (suspiciousTimer <= 0f)
        {
            EndAlert();
        }
    }

    void EndAlert()
    {
        if (anim != null)
            anim.enabled = true;

        isAlerted = false;
        waiting = false;
        agent.isStopped = false;

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[currentPoint].position);
    }

    bool IsPlayerCloseAndUnblocked()
    {
        if (player == null) return false;

        if (playerHideController != null && playerHideController.isHidden)
            return false;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > alertDistance)
            return false;

        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 target = player.position + Vector3.up * 1.2f;
        Vector3 direction = target - origin;
        float rayDistance = direction.magnitude;

        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, rayDistance))
        {
            if (hit.transform.root == player)
                return true;
        }

        return false;
    }
}