using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PoisonSwampZone : MonoBehaviour
{
    [Tooltip("Poison buildup per second while inside this zone.")]
    public float poisonRate = 20f;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerMovementController controller = other.GetComponent<PlayerMovementController>();
            if (controller != null)
            {
                Debug.Log("Sludged!");
                controller.Sludged(true);
            }
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerMovementController controller = other.GetComponent<PlayerMovementController>();
            if (controller != null)
            {
                Debug.Log("No longer sludged!");
                controller.Sludged(false);
            }
        }
    }
}