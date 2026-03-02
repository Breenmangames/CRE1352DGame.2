
using UnityEngine;

public class NPCProximity2D : MonoBehaviour
{
    public Dialogue dialogueUI;
    public bool playOnce = true;
    private bool hasPlayed = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (playOnce && hasPlayed) return;

        dialogueUI.Begin();
        hasPlayed = true;
    }
}
