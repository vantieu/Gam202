using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 2f;          // Khoảng cách đánh
    public float attackCooldown = 1.5f;     // Delay giữa các đòn
    public Transform player;

    private Animator animator;
    private float cooldownTimer;

    private bool isAttacking = false;       // Trạng thái attack (dùng cho movement check)

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        cooldownTimer += Time.deltaTime;

        float distance = Vector3.Distance(transform.position, player.position);

        // ================== TRONG RANGE → ATTACK ==================
        if (distance <= attackRange)
        {
            // Đang attack
            isAttacking = true;

            LookAtPlayer();

            // Nếu đủ cooldown → trigger attack
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0f;

                animator.SetTrigger("isAttack");
            }
        }
        else
        {
            // ❗ Ra khỏi range → KHÔNG attack nữa
            isAttacking = false;
        }
    }

    // ================== XOAY MẶT ==================
    void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                10f * Time.deltaTime
            );
        }
    }

    // ================== TRẠNG THÁI ATTACK ==================
    public bool IsAttacking()
    {
        return isAttacking;
    }

    // ================== GÂY DAMAGE (Animation Event) ==================
    public void DealDamage()
    {
        AttributesManager myStats = GetComponent<AttributesManager>();

        if (myStats == null) return;

        myStats.DealDamage(player.gameObject);
    }
}