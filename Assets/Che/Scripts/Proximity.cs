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
        if (col) col.isTrigger = true; // ensure the collider is set to trigger so it doesnt block movement
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag) && other.name != "Player") return;
        playerInRange = true;
        if (playOnce && hasPlayed) return;

        if (startOnTriggerEnter)
            StartDialogue(); // automatically start dialogue when player enters trigger
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
        dialogueUI.Begin(lines, npcName); // start the dialogue with the specified lines and npc name
        hasPlayed = true;
    }
}