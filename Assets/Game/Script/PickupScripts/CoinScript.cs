using UnityEngine;

public class CoinScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();


        if (controller != null) 
        {
            SoundEffectManager.PlaySoundEffect("CoinPickUpSound");
            Destroy(gameObject);
        }

    }
}
