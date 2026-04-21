using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewNPCDialogue", menuName ="NPC Dialogue")]

public class NPCDialogue : ScriptableObject
{
  public string npcName;
  public Sprite npcPortrait;
  public string[] dialogueLines;
  public bool[] autoProgressLines;
  public bool[] endDialogueLines; //Mark when dialogue should end
  public float autoProgressDelay = 1.5f;
  public float typingSpeed = 0.05f;
  public AudioClip voiceSound;
  public float voicePitch = 1f;

  public DialogueChoice[] choices;

  public int questInProgressIndex;
  public int questCompletedIndex;
  public Quest quest; //The quest given by the NPC
}

[System.Serializable]

public class DialogueChoice
{
    public int dialogueIndex; //Dialogue lines where the choices will appear
    public string[] choices; //Your response options
    public int[] nextDialogueIndexes; //Where your choices lead
    public bool[] givesQuest; //If choice gives quest
}
