
using UnityEngine;
using TMPro;
using System.Collections;

public class Dialogue : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerName;

    public float textSpeed = 0.03f;

    private string[] currentLines;
    private int index;
    private bool dialogueActive = false;
    private Coroutine typingRoutine;
    public static bool IsActive { get; private set; }

    void Awake()
    {
        Hide();
        if (dialogueText != null) dialogueText.text = "";
    }

    void Update()
    {
        if (!dialogueActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (dialogueText.text == currentLines[index])
            {
                NextLine();
            }

            else
            {
                if (typingRoutine != null) StopCoroutine(typingRoutine);
                dialogueText.text = currentLines[index];
            }
        }
    }


    public void Begin(string[] lines, string speakerName = "")
    {
        if (lines == null || lines.Length == 0)
        {
            return;
        }

        if (this.speakerName != null)
        {
            this.speakerName.text = speakerName;
            this.speakerName.gameObject.SetActive(speakerName != " ");
        }
        currentLines = lines;
        index = 0;
        IsActive = true;
        dialogueActive = true;

        Show();


        if (typingRoutine != null) StopCoroutine(typingRoutine);
        typingRoutine = StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        dialogueText.text = "";
        foreach (char c in currentLines[index])
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void NextLine()
    {
        if (index < currentLines.Length - 1)
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
        IsActive = false;
        dialogueActive = false;
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        Hide();

    }
    private void Show() => dialoguePanel.SetActive(true);
    private void Hide() => dialoguePanel.SetActive(false);
}
