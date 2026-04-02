using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class UIHandler : MonoBehaviour
{
    private VisualElement m_Healthbar;
    private IntegerField m_Coins;

    
    private IntegerField m_HealthPotion;
    private IntegerField m_SpeedPotion;
    private IntegerField m_AttackPotion;

    private int m_CurrentCoins = 0;
    private int m_CurrentHealthPotions = 0;
    private int m_CurrentSpeedPotions = 0;
    private int m_CurrentAttackPotions = 0;
    public static UIHandler instance { get; private set; }


    private void Awake()
    {
        instance = this;
    }

  /* private void OnEnable()
    {
        Loot.OnItemLooted += PickUpCoin();
    }*/
    



    // Start is called before the first frame update
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        m_Healthbar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
        SetHealthValue(1.0f);
        m_Coins = uiDocument.rootVisualElement.Q<IntegerField>("CoinCount");
        SetCoinValue(0);
        m_HealthPotion = uiDocument.rootVisualElement.Q<IntegerField>("HPPotionCount");
        m_SpeedPotion = uiDocument.rootVisualElement.Q<IntegerField>("SpeedPotionCount");
        m_AttackPotion = uiDocument.rootVisualElement.Q<IntegerField>("AtkPotionCount");
        

    }

   

     public void PickUpCoin(int coinValue = 1)
    {
        m_CurrentCoins += coinValue;
        SetCoinValue(m_CurrentCoins);
    }

    public void PickUpHealthPotion(int HPpotionValue = 1)
    {
        m_CurrentHealthPotions += HPpotionValue; 
        SetHPPotionValue(m_CurrentHealthPotions);
    }

    public void PickUpSpeedPotion(int SpeedpotionValue = 1)
    {
        m_CurrentSpeedPotions += SpeedpotionValue; 
        SetSpeedPotionValue(m_CurrentSpeedPotions);
    }

    public void PickUpAttackPotion(int AttackpotionValue = 1)
    {
        m_CurrentAttackPotions += AttackpotionValue; 
        SetAttackPotionValue(m_CurrentAttackPotions);
    }

    void SetCoinValue(int coins)
    {
       m_Coins.value = coins;
    }

    void SetHPPotionValue(int HealthPotionValue)
    {
        m_HealthPotion.value = HealthPotionValue; 
    }
    void SetSpeedPotionValue(int SpeedPotionValue)
    {
        m_SpeedPotion.value = SpeedPotionValue;
    }
    void SetAttackPotionValue(int AttackPotionValue)
    {
        m_AttackPotion.value = AttackPotionValue;
    }


    public void SetHealthValue(float percentage)
    {
        m_Healthbar.style.width = Length.Percent(100 * percentage);


    }
}