using UnityEngine;

public class CollisionDamager : MonoBehaviour
{
    [SerializeField] uint m_damage = 1;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out IDamageable damageable)) damageable.Damage(m_damage);
    }
}
