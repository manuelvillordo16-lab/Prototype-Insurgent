using UnityEngine;

public class IDamageable : MonoBehaviour
{
    public interface IDamageable
    {
        void TakeDamage(float amount);
    }
}
