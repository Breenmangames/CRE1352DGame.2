
using UnityEngine;
using TMPro;
using System.Collections;

public class Dialogue : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI nameComponent;

    [Header("Typing")]
    public float textSpeed = 0.03f;

    private string[] currentLines;
    private int index;
    private bool dialogueActive = false;
    private Coroutine typingRoutine;
    public static bool IsActive { get; private set; }

    void Awake()
    {
        Hide();
        if (textComponent != null) textComponent.text = "";
    }

    void Update()
    {
        if (!dialogueActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == currentLines[index])
            {
                NextLine();
            }
            else
            {
                if (typingRoutine != null) StopCoroutine(typingRoutine);
                textComponent.text = currentLines[index];
            }
        }
    }

    /// <summary>
    /// Starts dialogue using the lines provided by an NPC.
    /// </summary>
    public void Begin(string[] lines, string speakerName = "")
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("Dialogue.Begin called with no lines.");
            return;
        }

        if (nameComponent != null)
        {
            nameComponent.text = speakerName;
            nameComponent.gameObject.SetActive(speakerName != "");
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
        textComponent.text = "";
        foreach (char c in currentLines[index])
        {
            textComponent.text += c;
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
