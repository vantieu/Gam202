using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;              // Tốc độ di chuyển
    public float rotationSpeed = 5f;      // Tốc độ xoay

    [Header("Patrol")]
    public float patrolRange = 10f;       // Bán kính đi random
    public float waitTime = 2f;           // Thời gian đứng chờ

    [Header("Chase")]
    public float detectionRange = 8f;     // Khoảng cách phát hiện player
    public Transform player;

    private CharacterController controller;
    private Animator animator;
    private EnemyAttack attackScript;     // Tham chiếu script attack

    private Vector3 startPosition;        // Vị trí ban đầu
    private Vector3 targetPosition;       // Điểm random
    private float waitTimer;

    private bool isChasing = false;       // Đang đuổi player
    private bool returningToStart = false;// Đang quay về

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Lấy animator từ object con
        animator = GetComponentInChildren<Animator>();

        // Lấy script attack
        attackScript = GetComponent<EnemyAttack>();

        startPosition = transform.position;

        // Tạo điểm random đầu tiên
        SetNewTarget();
    }

    void Update()
    {
        // Nếu đang attack → dừng
        if (attackScript != null && attackScript.IsAttacking())
        {
            animator.SetBool("isWalk", false); // ❗ không walk
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Khoảng cách từ player tới vùng patrol
        float distanceFromStart = Vector3.Distance(player.position, startPosition);

        // ===== CHASE =====
        if (distanceToPlayer <= detectionRange && distanceFromStart <= patrolRange)
        {
            isChasing = true;
            returningToStart = false;

            // Di chuyển tới player
            MoveTo(player.position);
        }
        else
        {
            // Nếu đang đuổi mà player chạy mất
            if (isChasing)
            {
                isChasing = false;
                returningToStart = true;
            }

            // ===== RETURN =====
            if (returningToStart)
            {
                ReturnToStart();
            }
            // ===== PATROL =====
            else
            {
                Patrol();
            }
        }
    }

    void Patrol()
    {
        float distance = Vector3.Distance(transform.position, targetPosition);

        // Nếu tới điểm
        if (distance < 1f)
        {
            waitTimer += Time.deltaTime;

            // Đứng yên → idle
            animator.SetBool("isWalk", false);

            // Đợi xong → đi tiếp
            if (waitTimer >= waitTime)
            {
                SetNewTarget();
                waitTimer = 0f;
            }
        }
        else
        {
            MoveTo(targetPosition);
        }
    }

    void ReturnToStart()
    {
        float distance = Vector3.Distance(transform.position, startPosition);

        // Nếu đã về vị trí ban đầu
        if (distance < 1f)
        {
            returningToStart = false;
            SetNewTarget();
        }
        else
        {
            MoveTo(startPosition);
        }
    }

    void SetNewTarget()
    {
        // Random trong hình cầu
        Vector3 random = Random.insideUnitSphere * patrolRange;

        // Không bay lên trời
        random.y = 0;

        targetPosition = startPosition + random;
    }

    public void MoveTo(Vector3 target)
    {
        // Tính hướng
        Vector3 direction = (target - transform.position).normalized;

        // ===== XOAY =====
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Xoay mượt
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // ===== DI CHUYỂN =====
        controller.Move(direction * speed * Time.deltaTime);

        // Bật animation đi
        animator.SetBool("isWalk", true);
    }

    // Vẽ vùng trong Scene
    void OnDrawGizmosSelected()
    {
        // 🔵 Patrol
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRange);

        // 🔴 Detection
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}