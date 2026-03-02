using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();


        if (controller != null) //&& controller.health < controller.maxHealth
        {
            controller.ChangeHealth(1);
            SoundEffectManager.PlaySoundEffect("HealthPickUpSound");
            Destroy(gameObject);
        }

    }
}
