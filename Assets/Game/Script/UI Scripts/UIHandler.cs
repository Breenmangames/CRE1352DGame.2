using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class UIHandler : MonoBehaviour
{
    private VisualElement m_Healthbar;
    private IntegerField m_Coins;

    private int m_CurrentCoins = 0;
    public static UIHandler instance { get; private set; }


    private void Awake()
    {
        instance = this;
    }




    // Start is called before the first frame update
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        m_Healthbar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
        SetHealthValue(1.0f);
        m_Coins = uiDocument.rootVisualElement.Q<IntegerField>("CoinCount");
        SetCoinValue(0);

    }

   

     public void PickUpCoin(int coinValue = 1)
    {
        m_CurrentCoins += coinValue;
        SetCoinValue(m_CurrentCoins);
    }

    void SetCoinValue(int coins)
    {
       m_Coins.value = coins;
    }


    public void SetHealthValue(float percentage)
    {
        m_Healthbar.style.width = Length.Percent(100 * percentage);


    }
}
