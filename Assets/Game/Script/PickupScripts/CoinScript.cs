using UnityEngine;
using UnityEngine.UIElements;

public class CoinScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private UIHandler uiController;

    private void Start()
    {
        UIHandler uiDocument = Object.FindFirstObjectByType<UIHandler>();
        uiController = uiDocument?.GetComponent<UIHandler>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SoundEffectManager.PlaySoundEffect("CoinPickUpSound");
            uiController.PickUpCoin();
            Destroy(gameObject);
        }
    }
}


