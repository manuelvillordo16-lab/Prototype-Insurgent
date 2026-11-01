using UnityEngine;

public class LegDamage : MonoBehaviour
{
    private bool canDealDamage = false;
    private float currentDamage = 0f;

    public void EnableDamage(float damage)
    {
        canDealDamage = true;
        currentDamage = damage;
    }

    public void DisableDamage()
    {
        canDealDamage = false;
        currentDamage = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage) return;

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(currentDamage);
            canDealDamage = false; 
        }
    }
}