
using UnityEngine;
using TMPro;
using System.Collections;

public class Dialogue : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed = 0.03f;

    private int index;
    private bool dialogueActive = false;
    private Coroutine typingRoutine;

    void Awake()
    {
        Hide();
        textComponent.text = "";
    }

    void Update()
    {
        if (!dialogueActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                if (typingRoutine != null) StopCoroutine(typingRoutine);
                textComponent.text = lines[index];
            }
        }
    }

    public void Begin()
    {
        dialoguePanel.SetActive(true);
        dialogueActive = true;
        index = 0;

        if (typingRoutine != null) StopCoroutine(typingRoutine);
        typingRoutine = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        textComponent.text = "";
        foreach (char c in lines[index])
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            if (typingRoutine != null) StopCoroutine(typingRoutine);
            typingRoutine = StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        dialogueActive = false;
        Hide();
    }

    void Hide()
    {
        dialoguePanel.SetActive(false);
    }
}
