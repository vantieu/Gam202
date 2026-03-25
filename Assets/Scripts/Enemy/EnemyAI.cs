using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;              // Tốc độ di chuyển
    public float rotationSpeed = 5f;      // Tốc độ xoay

    [Header("Patrol")]
    public float patrolRange = 10f;       // Bán kính đi random
    public float waitTime = 2f;           // Thời gian đứng chờ khi tới điểm

    [Header("Chase")]
    public float detectionRange = 8f;     // Khoảng cách phát hiện player
    public Transform player;              // Player cần đuổi

    private CharacterController controller;
    private Animator animator;

    private Vector3 startPosition;        // Vị trí ban đầu (gốc patrol)
    private Vector3 targetPosition;       // Điểm random đang đi tới
    private float waitTimer;

    private bool isChasing = false;       // Đang đuổi player
    private bool returningToStart = false;// Đang quay về vị trí ban đầu

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Lấy Animator từ object con
        animator = GetComponentInChildren<Animator>();

        startPosition = transform.position;

        // Chọn điểm random đầu tiên
        SetNewTarget();
    }

    void Update()
    {
        // Khoảng cách tới player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Khoảng cách từ player tới vùng patrol (để không đuổi quá xa)
        float distanceFromStart = Vector3.Distance(player.position, startPosition);

        // ================== CHASE ==================
        // Nếu player trong phạm vi phát hiện + vẫn trong vùng patrol
        if (distanceToPlayer <= detectionRange && distanceFromStart <= patrolRange)
        {
            isChasing = true;
            returningToStart = false;

            // Di chuyển tới player
            MoveTo(player.position);
        }
        else
        {
            // Nếu đang đuổi mà player chạy mất → quay về
            if (isChasing)
            {
                isChasing = false;
                returningToStart = true;
            }

            // ================== RETURN ==================
            if (returningToStart)
            {
                ReturnToStart();
            }
            // ================== PATROL ==================
            else
            {
                Patrol();
            }
        }
    }

    void Patrol()
    {
        float distance = Vector3.Distance(transform.position, targetPosition);

        // Nếu đã tới điểm random
        if (distance < 1f)
        {
            waitTimer += Time.deltaTime;

            // Đứng yên
            animator.SetBool("isWalk", false);

            // Đợi xong thì đi tiếp
            if (waitTimer >= waitTime)
            {
                SetNewTarget();
                waitTimer = 0f;
            }
        }
        else
        {
            // Di chuyển tới điểm random
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

            // Tiếp tục đi random
            SetNewTarget();
        }
        else
        {
            // Di chuyển về gốc
            MoveTo(startPosition);
        }
    }

    void SetNewTarget()
    {
        // Tạo điểm random trong hình cầu
        Vector3 random = Random.insideUnitSphere * patrolRange;

        // Không thay đổi trục Y (tránh bay lên trời)
        random.y = 0;

        // Tính vị trí mục tiêu
        targetPosition = startPosition + random;
    }

    void MoveTo(Vector3 target)
    {
        // Lấy hướng di chuyển
        Vector3 direction = (target - transform.position).normalized;

        // ===== XOAY MẶT =====
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

    // ===== VẼ VÒNG TRONG SCENE =====
    void OnDrawGizmosSelected()
    {
        // Vòng patrol (xanh)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRange);

        // Vòng phát hiện player (đỏ)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}