using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NewScriptableObjectScript : ScriptableObject
{
    public string npcName; // Name of the NPC 
    public Sprite npcportrait; // Portrait image for the NPC
    public string[] dialogueLines; // Array of dialogue lines for the NPC
    public float typingSpeed = 0.05f; // Speed at which dialogue is displayed   
    public AudioClip voiceSound;
    public float voicePitch = 1f; // Pitch for the voice sound
    public bool[] autoProgessLines;
    public float autoProgressDelay = 1.5f; // Delay before automatically progressing to the next line
}
