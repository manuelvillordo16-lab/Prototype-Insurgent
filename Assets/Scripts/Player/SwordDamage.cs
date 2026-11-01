using UnityEngine;

public class SwordDamage : MonoBehaviour
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

        IDamageable damageable = other.GetComponent<IDamageable>();
       
        if (damageable != null)
        {
            damageable.TakeDamage(currentDamage);
            canDealDamage = false; 
        }
    }

}
