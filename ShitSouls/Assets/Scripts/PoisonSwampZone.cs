using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PoisonSwampZone : MonoBehaviour
{
    [Tooltip("Poison buildup per second while inside this zone.")]
    public float poisonRate = 20f;

    private void OnTriggerStay(Collider other)
    {
        StatusEffectManager status = other.GetComponent<StatusEffectManager>();
        if (status != null)
        {
            status.AddBuildup("Poison", poisonRate * Time.deltaTime);
        }
        else
        {
            Debug.LogError("StatusEffectManager not found");
        }
    }
}