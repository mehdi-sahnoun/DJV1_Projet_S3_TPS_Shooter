using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCaracter : MonoBehaviour, IDamageable
{
    [SerializeField] private int hitPoints = 10;
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float rollSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private TMP_Text data;
    [SerializeField] private GameObject gameOverUI;

    private CharacterController characterController;
    private Animator animator;

    private bool isWalking = false;
    private bool isRunning = false;
    private bool isRolling = false;
    private int ShootTriggerHash = Animator.StringToHash("shoot");
    private int DeadTriggerHash = Animator.StringToHash("dead");
    private int RollTriggerHash = Animator.StringToHash("roll");

    private Vector3 animatorInitialPosition;

    [SerializeField] private int bulletSpeed = 5;
    [SerializeField] private int bulletDamage = 10;

    private int initHP = 10; 

    public void setHP(int hp)
    {
        hitPoints = hp;
    }

    public void powerUp()
    {
        walkSpeed += 0.1f;
        runSpeed += 0.1f;
        bulletDamage += 5;
        hitPoints += 20;

        if(hitPoints > initHP)
            initHP = hitPoints;

        Debug.Log("walkspeed : " + walkSpeed);
        Debug.Log("runspeed : " + runSpeed);
        Debug.Log("bulletDamage : " + bulletDamage);
        Debug.Log("hitPoints : " + hitPoints);
        data.text = "Player : " + hitPoints + " / " + bulletDamage;
    }

    public float HitPointPercent => (float)hitPoints / initHP;

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
        characterController = GetComponent<CharacterController>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component not found in child objects!");
            }
        }

        if (animator != null)
        {
            animatorInitialPosition = animator.transform.localPosition;
        }

        initHP = hitPoints;
        data.text = "Player : " + hitPoints + " / " + bulletDamage;
    }

    void Update()
    {
        float horizontal = Input.GetKey(KeyCode.A) ? -1 : (Input.GetKey(KeyCode.D) ? 1 : 0);
        float vertical = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? -1 : 0);

        Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            isRunning = Input.GetKey(KeyCode.LeftControl);
            isWalking = !isRunning;

            float currentSpeed = isRunning ? runSpeed : walkSpeed;

            Vector3 moveDirection = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * inputDirection;
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
        }
        else
        {
            isWalking = false;
            isRunning = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger(RollTriggerHash);
        }

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger(ShootTriggerHash);
            StartCoroutine(WaitForAnimationAndShoot());
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
            animator.SetBool("isRunning", isRunning);
        }

        characterController.Move(Vector3.down * 9.81f * Time.deltaTime);
    }

    void LateUpdate()
    {
        if (animator != null)
        {
            animator.transform.localPosition = animatorInitialPosition;
        }
    }

    //private IEnumerator Roll()
    //{
    //    animator.SetTrigger(RollTriggerHash);

    //    float rollDuration = 1f; // Adjust this to match the length of the roll animation
    //    float rollTimer = 0f;

    //    while (rollTimer < rollDuration)
    //    {
    //        rollTimer += Time.deltaTime;
    //        characterController.Move(transform.forward * rollSpeed * Time.deltaTime);
    //        yield return null;
    //    }
    //}


    private IEnumerator WaitForAnimationAndShoot()
    {
        yield return new WaitForSeconds(0.5f);

        var direction = transform.rotation * Vector3.forward;
        GameObject bullet = Instantiate(bulletPrefab, transform.position + direction * 1f, transform.rotation).gameObject;
        bullet.transform.position = new Vector3(bullet.transform.position.x, bullet.transform.position.y + 1.5f, bullet.transform.position.z);
        bullet.GetComponent<Bullet>().setBulletSpeedAndDamage(bulletSpeed, bulletDamage);
        bullet.SetActive(true);
        Destroy(bullet, 5.0f);
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

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        gameOverUI.SetActive(true);
    }
}
