using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    [Header("Daño")]
    [SerializeField] private float lightDamage = 15f;
    [SerializeField] private float heavyDamage = 35f;

    private bool canDealDamage = false;
    private bool isHeavyAttack = false;

    public void EnableDamage(bool heavy) //Summons PlayerCombat
    {
        canDealDamage = true;
        isHeavyAttack = heavy;
    }

    public void DisableDamage()
    {
        canDealDamage = false;
    }

    private void OnTriggerEnter(Collider other) // Hit detection        Mari debe tener script de los enemigos
    {
        if (!canDealDamage) return;
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            float damage = isHeavyAttack ? heavyDamage : lightDamage;
            enemyHealth.TakeDamage(damage);
        }
    }

}
