using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; } //Singleton Instance

    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject); //Make sure there's only one instance
    }

   public void ShowDialogueUI(bool show)
   {
        dialoguePanel.SetActive(show); //Toggle the UI's visibility
   }

   public void SetNPCinfo(string npcName, Sprite portrait)
   {
        nameText.text = npcName;
        portraitImage.sprite = portrait;
   }

   public void SetDialogueText(string text)
   {
        dialogueText.text = text;
   }
}
