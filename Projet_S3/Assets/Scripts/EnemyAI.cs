using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour, IDamageable
{
    [SerializeField] private int hitPoints = 1;
    [SerializeField] private Transform player; // Target to follow
    [SerializeField] private GameObject bulletPrefab; // Bullet prefab for shooting
    [SerializeField] private GameObject healPrefab; // Heal prefab to spawn after death
    [SerializeField] private float moveDuration = 3f; // How long the enemy moves
    [SerializeField] private float stopDuration = 2f; // How long the enemy stops
    [SerializeField] private float shootCooldown = 0.5f; // Time between shots
    [SerializeField] private int minShots = 2; // Minimum shots per stop
    [SerializeField] private int maxShots = 4; // Maximum shots per stop
    [SerializeField] private float angularSpeed = 360f; // Rotation speed when aiming

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private static readonly int IsRunningHash = Animator.StringToHash("isRunning");
    private static readonly int ShootHash = Animator.StringToHash("shoot");
    private int DeadTriggerHash = Animator.StringToHash("dead");

    private bool isShooting = false; // Is the enemy currently shooting?
    private float moveTimer = 0f; // Timer for movement
    private Vector3 animatorInitialPosition;

    private int initialHitPoints = 1;
    public float HitPointPercent => (float)hitPoints / initialHitPoints;


    [SerializeField] private int bulletSpeed = 5; // Speed of the bullet
    [SerializeField] private int bulletDamage = 10; // Speed of the bullet


    public void setHP(int hp)
    {
        hitPoints = hp;
    }

    public void setBulletSpeedAndDamage(int speed, int damage)
    {
        bulletDamage = damage;
        bulletSpeed = speed;
    }

    public int getHp()
    {
        return this.hitPoints;
    }


    void Start()
    {
        initialHitPoints = hitPoints;

        navMeshAgent = GetComponent<NavMeshAgent>();

        // Find the Animator in child objects
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found in child objects!");
        }
        else
        {
            // Store the initial position of the Animator's GameObject
            animatorInitialPosition = animator.transform.localPosition;
        }

        // Start the movement and shooting routine
        StartCoroutine(MovementAndShootingRoutine());
    }

    private IEnumerator MovementAndShootingRoutine()
    {
        while (true)
        {
            // Move towards the player for a duration
            moveTimer = 0f;
            navMeshAgent.isStopped = false;

            if (animator != null)
                animator.SetBool(IsRunningHash, true);

            while (moveTimer < moveDuration)
            {
                moveTimer += Time.deltaTime;

                // Continuously update the destination to follow the player
                if (player != null)
                {
                    navMeshAgent.SetDestination(player.position);
                }

                yield return null;
            }

            // Stop moving and begin shooting
            navMeshAgent.isStopped = true;

            if (animator != null)
                animator.SetBool(IsRunningHash, false);

            isShooting = true;
            yield return StartCoroutine(ShootMultipleTimes());

            // Reset shooting state
            isShooting = false;
        }
    }

    private IEnumerator ShootMultipleTimes()
    {
        int shotsToFire = Random.Range(minShots, maxShots + 1);

        for (int i = 0; i < shotsToFire; i++)
        {
            if (player != null)
            {
                // Rotate towards the player
                Vector3 direction = (player.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, angularSpeed * Time.deltaTime);

                // Trigger the shooting animation
                if (animator != null)
                {
                    animator.SetTrigger(ShootHash);
                }

                if (bulletPrefab != null)
                {
                    Vector3 bulletSpawnPosition = transform.position + direction * 1f;
                    bulletSpawnPosition.y += 1.5f; // Adjust height above ground

                    GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition, transform.rotation);
                    bullet.GetComponent<Bullet>().setBulletSpeedAndDamage(bulletSpeed, bulletDamage);
                    bullet.SetActive(true);
                    Destroy(bullet, 5f); // Destroy bullet after 5 seconds
                }

                yield return new WaitForSeconds(shootCooldown);
            }
        }

        // Wait for the stop duration after shooting
        yield return new WaitForSeconds(stopDuration);
    }

    void LateUpdate()
    {
        // Reset the Animator's GameObject to its initial local position
        if (animator != null)
        {
            animator.transform.localPosition = animatorInitialPosition;
        }
    }

    public void ApplyDamage(int value)
    {
        hitPoints -= value;

        if (hitPoints <= 0)
        {
            animator.SetTrigger(DeadTriggerHash);
            StartCoroutine(WaitForAnimationAndDie());
        }
    }

    private IEnumerator WaitForAnimationAndDie()
    {
        yield return new WaitForSeconds(5f);/////////////////////not that good of a solution

        GameObject heal = Instantiate(healPrefab, transform.position, transform.rotation).gameObject;
        heal.transform.position = new Vector3(heal.transform.position.x, heal.transform.position.y + 1.0f, heal.transform.position.z);
        heal.SetActive(true);
        Destroy(gameObject);
    }

    ///// for the game manager
    public delegate void EnemyDestroyedDelegate(EnemyAI enemy);
    public static event EnemyDestroyedDelegate OnEnemyDestroyed;

    private void OnDestroy()
    {
        OnEnemyDestroyed?.Invoke(this); // Notify the GameManager
    }
}
