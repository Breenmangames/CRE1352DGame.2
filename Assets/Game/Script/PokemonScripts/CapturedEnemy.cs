using UnityEngine;
using System.Linq;


[System.Serializable]
public class CapturedEnemy : MonoBehaviour
{
   
    public string enemyName;
    public GameObject enemyPrefab;    
    public float maxHealth;
    public float capturedAtHealth;
    public Sprite icon;           
    public bool isDeployed;

  
    [System.NonSerialized]
    public GameObject deployedInstance; // Reference to the instantiated GameObject when deployed, not serialized to avoid issues with prefab references

    public CapturedEnemy(EnemyStats stats)
    {
        enemyName = stats.enemyName;  // Copy the enemy's name from the stats
        enemyPrefab = stats.sourcePrefab; //Copy the prefab reference from the stats
        maxHealth = stats.maxHealth; //Copy the max health from the stats
        capturedAtHealth = stats.currentHealth; //Copy the current health at the time of capture from the stats
        icon = stats.icon; //Copy the icon from the stats
        isDeployed = false; // Initially, the captured enemy is not deployed
    }


}
