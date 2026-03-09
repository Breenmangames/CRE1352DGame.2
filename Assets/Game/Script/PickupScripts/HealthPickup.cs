using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        TestMovement controller = other.GetComponent<TestMovement>();

        if (controller != null)
        {
            // Call a proper "IncreaseHealth" method if you have one
            // For now, assuming you add this to TestMovement
            controller.ChangeHealth(1); // Or create an IncreaseHealth method
            SoundEffectManager.PlaySoundEffect("HealthPickUpSound");
            Destroy(gameObject);
        }
    }
}