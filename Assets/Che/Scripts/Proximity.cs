using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NPCProximity2D : MonoBehaviour, IInteractable
{
    public Dialogue dialogueUI;
    public string npcName = "";
    [TextArea(2, 5)] public string[] lines;
    public bool startOnTriggerEnter = true;
    public bool playOnce = false;
    public string playerTag = "Player";

    private bool hasPlayed = false; // cant play again if true and playOnce is true
    private bool playerInRange = false;  // track if player is nearby

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("NPC trigger hit by: " + other.name);

        if (!other.CompareTag(playerTag) && other.name != "Player") return;
        playerInRange = true;
        if (playOnce && hasPlayed) return;
        if (startOnTriggerEnter)
            StartDialogue();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            playerInRange = false;
    }

    public void Interact()  // when you press space
    {
        if (!playerInRange) return;
        if (playOnce && hasPlayed) return;
        StartDialogue();
    }

    public void StartDialogue()
    {
        dialogueUI.Begin(lines, npcName);
        hasPlayed = true;
    }
}