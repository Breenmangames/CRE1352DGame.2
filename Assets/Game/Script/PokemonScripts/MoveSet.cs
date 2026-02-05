using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PokemonBasic;

[CreateAssetMenu(fileName = "MoveSet", menuName = "Pokemon/Create New Move")]
public class MoveSet : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField]  public PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp;

    public int PP { get; internal set; }
}
