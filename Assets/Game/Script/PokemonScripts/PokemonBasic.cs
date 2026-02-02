using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PokemonBasic", menuName = "Pokemon/Create New Pokemon")]
public class PokemonBasic : ScriptableObject
{
    [SerializeField] string pokemonName;


    [TextArea]
    [SerializeField] string description;



    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;


    [SerializeField] public PokemonType type1;
    [SerializeField] public PokemonType type2;


    // Base Stats
    [SerializeField] public int maxHP;
    [SerializeField] public int attack;
    [SerializeField] public int defense;
    [SerializeField] public int speed;
    [SerializeField] public int specialAttack;
    [SerializeField] public int specialDefense;

    [SerializeField] public List<LearnableMove> learnableMoves;

    public class LearnableMove
    {
        [SerializeField] public MoveSet moveBase;
        [SerializeField] public int level;
        public MoveSet MoveBase
        {
            get { return moveBase; }
        }
        public int Level
        {
            get { return level; }
        }

        public MoveSet Base { get; internal set; }
    }


    public string GetName()
    {
        return pokemonName;
    }
    public string Name
    {
        get { return pokemonName; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }

    public enum PokemonType
    {
        None,
        Normal,
        Fire,
        Water,
        Electric,
        Grass,
        Ice,
        Fighting,
        Poison,
        Ground,
        Flying,
        Psychic,
        Bug,
        Rock,
        Ghost,
        Dragon
    }
}