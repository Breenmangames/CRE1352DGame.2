
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
    public static bool IsActive { get; private set; } // any code can check if dialogue is active by calling Dialogue.IsActive

    void Awake()
    {
        Hide();
        if (dialogueText != null) dialogueText.text = "";
    }

    void Update()
    {
        if (!dialogueActive) return; // only check for input if dialogue is active

        if (Input.GetMouseButtonDown(0))
        {
            if (dialogueText.text == currentLines[index])
            {
                NextLine(); // left click advances dialogue IF the current line is fully displayed
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


        if (typingRoutine != null) StopCoroutine(typingRoutine); // stop the typewriter before starting a new one
        typingRoutine = StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        dialogueText.text = ""; // clear text before typing new line
        foreach (char c in currentLines[index])
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed); // delay between each character
        }
    }

    private void NextLine()
    {
        if (index < currentLines.Length - 1)
        {
            index++;
            if (typingRoutine != null) StopCoroutine(typingRoutine);
            typingRoutine = StartCoroutine(TypeLine()); // start typing the next line
        }

        else
        {
            EndDialogue();
        }
    }
    public void EndDialogue() // can be called by other scripts to end dialogue early
    {
        IsActive = false;
        dialogueActive = false;
        if (typingRoutine != null) StopCoroutine(typingRoutine); // stop any ongoing typewriter effect
        Hide();

    }
    private void Show() => dialoguePanel.SetActive(true);
    private void Hide() => dialoguePanel.SetActive(false);
}
