using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    public float attackDamage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            HealthManager health = other.GetComponent<HealthManager>();
            if (health != null)
            {
                health.TakeDamage(attackDamage);
            }
        }
    }
} 