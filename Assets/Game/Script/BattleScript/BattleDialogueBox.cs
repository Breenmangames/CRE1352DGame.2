using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BattleDialogueBox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color highlightedColor;

    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;

    [SerializeField] Text ppTexts;
    [SerializeField] Text typeTexts;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        { 
        dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }

    }
    public void EnableDialogText(bool enabled)
    { 
        dialogText.enabled = enabled;
    }
    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }
    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }
    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; ++i)
        {
            if (i == selectedAction)
                actionTexts[i].color = highlightedColor;
            else
                actionTexts[i].color = Color.black;
        }
    }
    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0;i < moveTexts.Count; ++i)
        {
            if (i== selectedMove)
                moveTexts[i].color = highlightedColor;
            else 
                moveTexts[i].color = Color.black;
        }
        ppTexts.text = $"PP {move.PP}/{move.Base.PP}";
        typeTexts.text = move.Base.type.ToString();
    }
    public void SetMoveNames(List<Move> moves) 
    {
        for (int i = 0; i < moves.Count; ++i) 
        {
            if (i < moves.Count)
                moveTexts[i].text = moves[i].Base.name;
            else
                moveTexts[i].text = "-";


        }  
    }
}
