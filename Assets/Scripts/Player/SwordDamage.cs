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
        TryDealDamage(other);
    }

    // OnTriggerEnter se llama UNA sola vez, en el momento en que los colliders empiezan a tocarse.
    // Si la espada ya estaba adentro del collider (por ejemplo el trigger grande del boss)
    // cuando se abre la ventana de daño, Enter ya paso y no vuelve a llamarse.
    // OnTriggerStay se llama en cada frame mientras siguen superpuestos, asi que lo cubre.
    private void OnTriggerStay(Collider other)
    {
        TryDealDamage(other);
    }

    private void TryDealDamage(Collider other)
    {
        if (!canDealDamage) return;

        // Los colliders del propio jugador estan pegados a la espada: no golpearse a si mismo
        if (other.GetComponentInParent<PlayerHealth>() != null) return;

        // InParent: el collider que toca la espada puede ser un hijo del objeto que tiene la vida
        IDamageable damageable = other.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(currentDamage);
            canDealDamage = false;   // un solo golpe por ataque
        }
    }
}