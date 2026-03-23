using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questID;
    public string questName;
    public string questDescription;
    public List<QuestObjective> objectives;

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(questID))
        {
            questID = questName + Guid.NewGuid().ToString();
        }
    }

    [System.Serializable]
    public class QuestObjective
    {
        public string objectiveID; //Matches with IDs for items, enemies, etc., related to the quest
        public string description;
        public ObjectiveType type;
    }

    public enum ObjectiveType { ItemCollection, FightEnemy, LocationBased, Talk, Custom }
}
