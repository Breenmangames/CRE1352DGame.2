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
    public GameObject deployedInstance;

    public CapturedEnemy(EnemyStats stats)
    {
        enemyName = stats.enemyName;
        enemyPrefab = stats.sourcePrefab;
        maxHealth = stats.maxHealth;
        capturedAtHealth = stats.currentHealth;
        icon = stats.icon;
        isDeployed = false;
    }


}
