using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class BattleHUD : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;

    public void SetData(PokemonScript pokmen)
    {
        nameText.text = pokmen._base.Name;
        levelText.text = "Lvl " + pokmen.level;
        hpBar.SetHP((float)pokmen.HP / pokmen.MaxHP);
    }
}
