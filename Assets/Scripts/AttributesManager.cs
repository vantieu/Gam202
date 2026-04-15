using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AttributesManager : MonoBehaviour
{
    public int health; // Máu hiện tại của nhân vật

    public int attack;// Sát thương tấn công cơ bản

    [Header("ENEMY Die")]
    bool isDead = false;

    public GameObject gemPrefab;
    // ====== ĐÒN CHÍ MẠNG (CRITICAL HIT) ======


    // Hệ số sát thương khi đánh chí mạng
    //  1.5 = damage tăng 150%
    public float critDamage = 1.5f;


    // Tỉ lệ đánh chí mạng
    // 0.5 = 50% khả năng ra đòn chí mạng
    public float critChance = 0.5f;

    // =======================
    // BIẾN LIÊN QUAN HEALTH BAR
    // =======================

    // Slider đại diện cho thanh máu
    private Slider healthSlider;

    // Canvas chứa thanh máu (World Space)
    private Transform healthCanvas;

    // Camera chính để thanh máu luôn quay về màn hình
    private Camera mainCamera;

    void Start()
    {
        // Lấy camera chính trong scene
        mainCamera = Camera.main;

        // Nếu object này KHÔNG PHẢI Enemy -> bỏ qua
        if (!CompareTag("Enemy")) return;

        // =======================
        // TÌM CANVAS THEO TÊN
        // (KHÔNG DÙNG GetChild(index) để tránh lỗi)
        // =======================
        healthCanvas = transform.Find("Canvas");

        // Nếu không tìm thấy Canvas -> báo lỗi
        if (healthCanvas == null)
        {
            Debug.LogError("Không tìm thấy Canvas trong Enemy: " + gameObject.name);
            return;
        }

        // =======================
        // TÌM HEALTH BAR (SLIDER)
        // =======================
        Transform bar = healthCanvas.Find("HeathBar");

        // Nếu không tìm thấy HeathBar -> báo lỗi
        if (bar == null)
        {
            Debug.LogError("Không tìm thấy HeathBar trong Enemy: " + gameObject.name);
            return;
        }

        // Lấy component Slider từ HeathBar
        healthSlider = bar.GetComponent<Slider>();

        // Nếu HeathBar không có Slider -> báo lỗi  
        if (healthSlider == null)
        {
            Debug.LogError("HealthBar không có Slider component");
            return;
        }

        // Gán giá trị max cho thanh máu
        healthSlider.maxValue = health;

        // Gán giá trị ban đầu
        healthSlider.value = health;
    }

    // =======================
    // UPDATE – CHẠY MỖI FRAME
    // =======================
    void Update()
    {
        // Nếu có Canvas và Camera
        if (healthCanvas != null && mainCamera != null)
        {
            // Làm cho thanh máu LUÔN QUAY VỀ CAMERA
            // => Enemy xoay hướng nào thì máu vẫn nhìn thẳng
            healthCanvas.LookAt(
                healthCanvas.position + mainCamera.transform.forward
            );

        }
    }

    // ====== NHẬN SÁT THƯƠNG ======


    // Hàm nhận sát thương
    // amount: lượng damage mà nhân vật bị trừ
    public void TakeDamage(int amount)
    {
        // Trừ máu theo lượng sát thương nhận vào
        health -= amount;

        // Nếu không phải Enemy thì không xử lý UI
        // Cập nhật thanh máu nếu là Enemy
        if (CompareTag("Enemy"))
        {
            if (healthSlider != null)
                healthSlider.value = health;
        }

        // Nếu máu <= 0
        if (health <= 0)
        {
            if (CompareTag("Enemy"))
                EnemyDie();

            if (CompareTag("Player"))
                PlayerDie();
        }

    }

    void EnemyDie()
    {
        // Nếu enemy đã chết thì thoát hàm ngay, tránh chạy lại nhiều lần
        if (isDead) return;

        // Đánh dấu enemy đã chết
        isDead = true;

        // In ra console để debug: tên enemy + trạng thái dead
        Debug.Log(gameObject.name + "Dead");

        // =======================
        // 0. TẮT AI (RẤT QUAN TRỌNG)
        // =======================

        // Lấy component EnemyController (script điều khiển AI)
        EnemyMovement enemyMovement = GetComponent<EnemyMovement>();

        // Nếu enemy có EnemyMovement thì tắt script này
        // → Ngăn AI tiếp tục di chuyển / tấn công sau khi chết
        if (enemyMovement != null)
            enemyMovement.enabled = false;

        // =======================
        // 1. TẮT NAVMESH AGENT
        // =======================

        // Lấy NavMeshAgent để dừng hệ thống pathfinding
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        // Kiểm tra enemy có dùng NavMeshAgent không
        if (agent != null)
        {
            // Dừng ngay việc di chuyển
            agent.isStopped = true;

            // Tắt hoàn toàn NavMeshAgent để tránh bug giật / trượt
            agent.enabled = false;
        }

        // =======================
        // 2. TẮT CHARACTER CONTROLLER
        // =======================

        // Lấy Character có EnemyMovement thì tắt script này
        CharacterController characterController = GetComponent<CharacterController>();

        // Nếu có thì tắt để enemy không còn tương tác vật lý
        if (characterController != null)
            characterController.enabled = false;

        // =======================
        // 3. TẮT COLLIDER
        // =======================

        // Lấy collider của enemy
        Collider collider = GetComponent<Collider>();

        // Tắt collider để:
        // - Không bị đánh thêm
        // - Không cản đường player
        if (collider != null)
            collider.enabled = false;

        // =======================
        // 4. BẬT ANIMATION CHẾT
        // =======================

        // Lấy Animator trong enemy hoặc trong các object con
        Animator animator = GetComponentInChildren<Animator>();

        // Kiểm tra enemy có Animator không
        if (animator != null)
        {
            // Tắt root motion để animation không kéo nhân vật di chuyển
            animator.applyRootMotion = false;

            // Set biến isDead = true để kích hoạt animation chết
            animator.SetBool("isDead", true);
        }

        // =======================
        // 4. SPAWM GEM
        // =======================

        if (gemPrefab != null)
        {
            GameObject gem = Instantiate(
                gemPrefab,
                transform.position + Vector3.up * 0.5f,
                Quaternion.identity
            );
            gem.SetActive(true);
        }


        // =======================
        // 6. HỦY ENEMY
        // =======================

        // Hủy enemy sau 2 giây
        // → Đủ thời gian cho animation chết chạy xong
        Destroy(gameObject, 2f);
    }

    void PlayerDie()
    {
        Debug.Log("Player Dead");


        // Tắt controller
        var cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;


        // Tắt movement script khác
        var scripts = GetComponents<MonoBehaviour>();
        foreach (var s in scripts)
        {
            if (s != this) s.enabled = false;
        }


        // Play animation chết
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("Dead");

        // Tắt NavMeshAgent an toàn
        var agent = GetComponent<NavMeshAgent>();
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        // Destroy object sau 3s
        Destroy(gameObject, 3f);
    }



    // ====== GÂY SÁT THƯƠNG ======


    // Hàm gây sát thương cho đối tượng khác
    // target: GameObject bị tấn công
    public void DealDamage(GameObject target)
    {
        // Lấy component AttributesManager của target
        // Dùng để truy cập máu và các chỉ số của target
        AttributesManager atm = target.GetComponent<AttributesManager>();


        // Nếu target không có AttributesManager
        // thì không thể gây damage → thoát hàm
        if (atm == null) return;


        // Khởi tạo damage ban đầu bằng attack cơ bản
        float totalDamage = attack;


        // Kiểm tra có ra đòn chí mạng hay không
        // Random.Range(0f, 1f) tạo số ngẫu nhiên từ 0 đến 1
        if (Random.Range(0f, 1f) < critChance)
        {
            // Nếu crit thì nhân damage lên theo hệ số crit
            totalDamage *= critDamage;


            // In log để biết đòn đánh là chí mạng
            Debug.Log("Critical Hit!");
        }


        // Truyền damage cuối cùng (đã tính crit nếu có)
        // Ép kiểu float → int trước khi trừ máu
        atm.TakeDamage((int)totalDamage);
    }
}
