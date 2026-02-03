using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class BattleUnit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] PokemonBasic _base;
    [SerializeField] public int level;
    [SerializeField] public bool isPlayerUnit;

    public PokemonScript Pokemon {  get; set; }
    public void Setup()
    {
       Pokemon = new PokemonScript(_base, level);
        if (isPlayerUnit)
            GetComponent<Image>().sprite = Pokemon.Base.backSprite;
        else
            GetComponent<Image>().sprite = Pokemon.Base.frontSprite;

    }
}
