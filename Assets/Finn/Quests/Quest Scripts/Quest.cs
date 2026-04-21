using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questID;
    public string questName;
    public string questDescription;
    public List<QuestObjective> objectives;
    public List<QuestReward> questRewards;

    //Called when scriptable object is edited
    private void OnEnable()
    {
        if (string.IsNullOrEmpty(questID))
        {
            questID = questName + Guid.NewGuid().ToString();
        }
    }
}

[System.Serializable]
public class QuestObjective
{
    public string objectiveID; //Matches with IDs for items, enemies, etc., related to the quest
    public string description;
    public ObjectiveType type;
    public int requiredAmount;
    public int currentAmount;

    public bool IsCompleted => currentAmount >= requiredAmount;
}

public enum ObjectiveType { ItemCollection, FightEnemy, LocationBased, Talk, Custom }

[System.Serializable]
public class QuestProgress
{
    public Quest quest;
    public List<QuestObjective> objectives;

    public QuestProgress(Quest quest)
    {
        this.quest = quest;
        objectives = new List<QuestObjective>();

        //Copy to avoid changing the original
        foreach (var obj in quest.objectives)
        {
            objectives.Add(new QuestObjective()
            {
                objectiveID = obj.objectiveID,
                description = obj.description,
                type = obj.type,
                requiredAmount = obj.requiredAmount,
                currentAmount = 0
            });
        }
    }

    public bool IsCompleted => objectives.TrueForAll(o => o.IsCompleted);

    public string QuestID => quest.questID;
}

[System.Serializable]

public class QuestReward
{
    public RewardType type;
    public int rewardID; //ItemID etc
    public int amount = 1;
}

public enum RewardType {  Item, Gold, Experience, Custom }
