using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }
public class BattleSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit EnemyUnit;
    [SerializeField] BattleHUD PlayerHUD;
    [SerializeField] BattleHUD EnemyHUD;
    [SerializeField] BattleDialogueBox DialogueBox;

    BattleState battleState;
    int currentAction;
    int currentMove;

    private void Start()
    {
      // StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()   
    {
        playerUnit.Setup();
        PlayerHUD.SetData(playerUnit.Pokemon);
        EnemyUnit.Setup();
        EnemyHUD.SetData(EnemyUnit.Pokemon);

        DialogueBox.SetMoveNames(playerUnit.Pokemon.Moves);

       yield return DialogueBox.TypeDialog($"You have stumbled on a Wild {EnemyUnit.Pokemon.Base.Name} !");
       yield return new WaitForSeconds(1f);

        PlayerAction();
        Debug.Log("you are working!");
    }
    void PlayerAction() 
    {
        battleState = BattleState.PlayerAction;
        StartCoroutine(DialogueBox.TypeDialog("Choose an Action!"));
        DialogueBox.EnableActionSelector(true);
    }

    void PlayerMove() 
    {
        battleState = BattleState.PlayerMove;
        DialogueBox.EnableActionSelector(false);
        DialogueBox.EnableDialogText(false);
        DialogueBox.EnableMoveSelector(true );
    }

    private void Update()
    {
        if (battleState == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (battleState == BattleState.PlayerMove) 
        {
            HandleMoveSelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 0)
                ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
                --currentAction;
        }

        DialogueBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                PlayerMove();
                //fight selected
            }

            else if (currentAction == 1)
            {
                //run selected
            }
        }
    }
    void HandleMoveSelection() 
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 1)
                ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
                --currentMove;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 2)
                currentMove +=2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 0)
                currentMove -=2;
        }
        DialogueBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);
    }


}
