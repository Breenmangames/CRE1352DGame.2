using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NPCProximity2D : MonoBehaviour, IInteractable
{
    [Header("References")]
    public Dialogue dialogueUI;
    [Header("Dialogue Data")]
    public string speakerName = "";
    [TextArea(2, 5)] public string[] lines;
    [Header("Behaviour")]
    public bool autoStartOnEnter = true;
    public bool playOnce = false;
    public string playerTag = "Player";

    private bool hasPlayed = false;
    private bool playerInRange = false;  // track if player is nearby

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("NPC trigger hit by: " + other.name + " | tag: " + other.tag);

        if (!other.CompareTag(playerTag) && other.name != "Player") return;
        playerInRange = true;

        if (playOnce && hasPlayed) return;
        if (autoStartOnEnter)
            StartDialogue();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            playerInRange = false;
    }

    public void Interact()  // called by PlayerController when E is pressed
    {
        if (!playerInRange) return;
        if (playOnce && hasPlayed) return;
        StartDialogue();
    }

    public void StartDialogue()
    {
        if (dialogueUI == null)
        {
            Debug.LogWarning($"{name}: Dialogue UI not assigned.");
            return;
        }
        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning($"{name}: No dialogue lines assigned.");
            return;
        }
        dialogueUI.Begin(lines, speakerName);
        hasPlayed = true;
    }
}