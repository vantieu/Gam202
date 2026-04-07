using UnityEngine;

public class WeaponAttributes : MonoBehaviour
{
    // Attributes của người chơi (Player)
    // Kéo Player (Ninja) có AttributesManager vào đây
    public AttributesManager atm;
    // public GameObject Player;

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra va chạm với Enemy
        if (other.CompareTag("Enemy"))
        {
            // GỌI DealDamage để áp dụng:
            // - Damage cơ bản
            // - Critical Hit
            // - Logic chết
            atm.DealDamage(other.gameObject);
        }
        // Kiểm tra va chạm với Player
        if (other.CompareTag("Player"))
        {
            // GỌI DealDamage để áp dụng:
            // - Damage cơ bản
            // - Critical Hit
            // - Logic chết
            // atm = Player.GetComponent<AttributesManager>();
            atm.DealDamage(other.gameObject);
        }
    }

    // public void damage()
    // {
    //     atm = Player.GetComponent<AttributesManager>();
    //     atm.DealDamage(Player);
    // }
}
