using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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

    private void Start()
    {
       StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        PlayerHUD.SetData(playerUnit.Pokemon);
        EnemyUnit.Setup();
        EnemyHUD.SetData(EnemyUnit.Pokemon);

       yield return DialogueBox.TypeDialog($"You have stumbled on a Wild {EnemyUnit.Pokemon.Base.Name} !");
       yield return new WaitForSeconds(1f);

        PlayerAction();

    }
    void PlayerAction() 
    {
        battleState = BattleState.PlayerAction;
        StartCoroutine(DialogueBox.TypeDialog("Choose an Action!"));
        DialogueBox.EnableActionSelector(true);
    }
}
