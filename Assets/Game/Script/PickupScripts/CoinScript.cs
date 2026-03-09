using UnityEngine;
using UnityEngine.UIElements;

public class CoinScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (other.CompareTag("Player"))
        {
            // Reference your UI manager and call PickUpCoin
            // FIX: Use FindFirstObjectByType instead of FindObjectOfType (CS0618)
            // FIX: Remove call to PickUpCoin on UIDocument (CS1061)
            UIDocument uiDocument = Object.FindFirstObjectByType<UIDocument>();
            SoundEffectManager.PlaySoundEffect("CoinPickUpSound");
            //UIDocument.Instance.PickUpCoin(); // Call the method on your UI manager to update the coin count
            Destroy(gameObject);
        }
        Destroy(gameObject);
        }
    }


