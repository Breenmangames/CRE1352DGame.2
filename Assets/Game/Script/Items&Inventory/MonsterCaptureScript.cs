using UnityEngine;

public class MonsterCaptureScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool isCaptured = false;
     void Start()
    {

    }

    void Update()
    {
        if (isCaptured)
        {
            // Logic for when the monster is captured
            // For example, you could disable the monster's movement and add it to the player's inventory
            Debug.Log(gameObject.name + " has been captured!");
            // Add code here to add the monster to the player's inventory
            Destroy(gameObject); // Remove the monster from the scene
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CaptureItem"))
        {
            // Logic for when the capture item collides with the monster
            // For example, you could add a random chance for the monster to break free
            float captureChance = Random.Range(0f, 1f);
            if (captureChance > 0.5f) // 50% chance to capture
            {
                isCaptured = true;
            }
            else
            {
                Debug.Log(gameObject.name + " broke free!");
                // Add code here to return the monster to its original position if it breaks free
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("CaptureItem"))
        {
            // Logic for when the capture item exits the monster's collider
            // You could reset any temporary states here if needed
        }
    }







}




/* logic for script.  
 * i want a capture item that can be used on a monster to capture it. 
 * the monster will then be added to the player's inventory and can be used in battle. 
 * the monster will have a chance to break free from the capture item, and if it does, it will return to its original position.  
 * if the capture is successful, the monster will be removed from the scene and added to the player's inventory. */