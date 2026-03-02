
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NPCProximity2D : MonoBehaviour
{
    [Header("References")]
    public Dialogue dialogueUI;         // Assign the Dialogue component from the Canvas

    [Header("Dialogue Data")]
    [TextArea(2, 5)] public string[] lines; // Different per NPC

    [Header("Behaviour")]
    public bool autoStartOnEnter = true;
    public bool playOnce = false;
    public string playerTag = "Player";

    private bool hasPlayed = false;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (playOnce && hasPlayed) return;

        if (autoStartOnEnter)
        {
            StartDialogue();
        }
        else
        {
            // Optional: show "Press E to talk" prompt and start on key press instead.
        }
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

        dialogueUI.Begin(lines);
        hasPlayed = true;
    }
}
