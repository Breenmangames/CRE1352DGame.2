using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PokemonScript 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    public PokemonBasic _base { get; set; }
    public int level { get; set; }

    public int HP { get; set; }

    public List<Move> Moves { get; set; }

    public PokemonScript(PokemonBasic pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;
        HP = _base.maxHP;


        //generates the moveset based on level and learnable moves
        Moves = new List<Move>();
        foreach (var move in _base.LearnableMoves)
        {
            if (move.Level <= level)
                Moves.Add(new Move(move.Base));

            if (Moves.Count >= 4)
                break;
        }
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((_base.attack * level) / 100f) + 5; }
    }
    public int Defense
    {
        get { return Mathf.FloorToInt((_base.defense * level) / 100f) + 5; }
    }
    public int Speed
    {
        get { return Mathf.FloorToInt((_base.speed * level) / 100f) + 5; }
    }
    public int SpecialAttack
    {
        get { return Mathf.FloorToInt((_base.specialAttack * level) / 100f) + 5; }
    }
    public int SpecialDefense
    {
        get { return Mathf.FloorToInt((_base.specialDefense * level) / 100f) + 5; }
    }
    public int MaxHP
    {
        get { return Mathf.FloorToInt((_base.maxHP * level) / 100f) + 10 + level; }
    }


}
