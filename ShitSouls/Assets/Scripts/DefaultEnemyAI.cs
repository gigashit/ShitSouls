using UnityEngine;

public class DefaultEnemyAI : MonoBehaviour
{
    [SerializeField] private GameObject damageCollider;
    public float attackDuration = 0.5f;

    public void Attack()
    {
        damageCollider.SetActive(true);
        Invoke(nameof(DisableDamageCollider), attackDuration);
    }

    private void DisableDamageCollider()
    {
        damageCollider.SetActive(false);
    }
}
