using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null)
        {
            
            controller.ChangeHealth(1); // Or create an IncreaseHealth method
            SoundEffectManager.PlaySoundEffect("HealthPickUpSound");
            Destroy(gameObject);
        }
    }
}