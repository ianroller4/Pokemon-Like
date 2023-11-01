using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")] // Makes it easier to create new pokemons by making a menu shortcut when right clicking
public class PokemonBase : ScriptableObject {

    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite front;
    [SerializeField] Sprite back;

    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    // Base Stats
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMove> learnableMoves;

    public string Name {
        get { return name; }
    }

    public string Description {
        get { return description; }
    }

    public Sprite Front {
        get { return front; }
    }

    public Sprite Back {
        get { return back; }
    }

    public PokemonType Type1 {
        get { return type1; }
    }

    public PokemonType Type2 {
        get { return type2; }
    }

    public int MaxHP {
        get { return maxHP; }
    }

    public int Attack {
        get { return attack; }
    }

    public int Defense {
        get { return defense; }
    }

    public int SpAttack {
        get { return spAttack; }
    }

    public int SpDefense {
        get { return spDefense; }
    }

    public int Speed {
        get { return speed; }
    }

    public List<LearnableMove> LearnableMoves {
        get { return learnableMoves; }
    }
    
}

[System.Serializable]
public class LearnableMove {

    [SerializeField] MoveBase moveBase;

    [SerializeField] int level;

    public MoveBase MoveBase {
        get { return moveBase; }
    }

    public int Level {
        get { return level; }
    }

}

public enum PokemonType {

    None,
    Normal,
    Fire,
    Water,
    Grass,
    Electric,
    Ice,
    Flying,
    Fighting,
    Psychic,
    Dark,
    Ghost,
    Rock,
    Ground,
    Bug,
    Dragon,
    Fairy,
    Steel,
    Poison
}

public enum Stat {

    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,
    Accuracy,
    Evasion

}

public class TypeChart {

    static float[][] chart = 
    {
        //                          Nor    Fir    Wat    Gra    Ele    Ice    Fly    Fig    Psy    Dar    Gho    Roc    Gro    Bug    Dra    Fai    Ste    Poi
        /* Nor */    new float[] {  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  0f  , 0.5f ,  1f  ,  1f  ,  1f  ,  1f  , 0.5f ,  1f},  // Done

        /* Fir */    new float[] {  1f  , 0.5f , 0.5f ,  2f  ,  1f  ,  2f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  , 0.5f ,  1f  ,  2f  , 0.5f ,  1f  ,  2f  ,  1f},  // Done

        /* Wat */    new float[] {  1f  ,  2f  , 0.5f , 0.5f ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  2f  ,  2f  ,  1f  , 0.5f ,  1f  ,  1f  ,  1f},  // Done

        /* Gra */    new float[] {  1f  , 0.5f ,  2f  , 0.5f ,  1f  ,  1f  , 0.5f ,  1f  ,  1f  ,  1f  ,  1f  ,  2f  ,  2f  , 0.5f , 0.5f ,  1f  , 0.5f , 0.5f}, // Done

        /* Ele */    new float[] {  1f  ,  1f  ,  2f  , 0.5f , 0.5f ,  1f  ,  2f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  0f  ,  1f  , 0.5f ,  1f  ,  1f  ,  1f},  // Done

        /* Ice */    new float[] {  1f  , 0.5f , 0.5f ,  2f  ,  1f  , 0.5f ,  2f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  2f  ,  1f  ,  2f  ,  1f  , 0.5f ,  1f},  // Done

        /* Fly */    new float[] {  1f  ,  1f  ,  1f  ,  2f  , 0.5f ,  1f  ,  1f  ,  2f  ,  1f  ,  1f  ,  1f  , 0.5f ,  1f  ,  2f  ,  1f  ,  1f  , 0.5f ,  1f},  // Done

        /* Fig */    new float[] {  2f  ,  1f  ,  1f  ,  1f  ,  1f  ,  2f  , 0.5f ,  1f  , 0.5f ,  2f  ,  0f  ,  2f  ,  1f  , 0.5f ,  1f  ,  1f  ,  2f  , 0.5f}, // Done

        /* Psy */    new float[] {  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  2f  , 0.5f ,  0f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  , 0.5f ,  2f},  // Done

        /* Dar */    new float[] {  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  , 0.5f ,  2f  , 0.5f ,  2f  ,  1f  ,  1f  ,  1f  ,  1f  , 0.5f ,  1f  ,  1f},  // Done

        /* Gho */    new float[] {  0f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  2f  , 0.5f ,  2f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f},  // Done

        /* Roc */    new float[] {  1f  ,  2f  ,  1f  ,  1f  ,  1f  ,  2f  ,  2f  , 0.5f ,  1f  ,  1f  ,  1f  ,  1f  , 0.5f ,  2f  ,  1f  ,  1f  , 0.5f ,  1f},  // Done

        /* Gro */    new float[] {  1f  ,  2f  ,  1f  , 0.5f ,  2f  ,  1f  ,  0f  ,  1f  ,  1f  ,  1f  ,  1f  ,  2f  ,  1f  , 0.5f ,  1f  ,  1f  ,  2f  ,  2f},  // Done

        /* Bug */    new float[] {  1f  , 0.5f ,  1f  ,  2f  ,  1f  ,  1f  , 0.5f , 0.5f ,  2f  ,  2f  , 0.5f ,  1f  ,  1f  ,  1f  ,  1f  , 0.5f , 0.5f , 0.5f}, // Done

        /* Dra */    new float[] {  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  2f  ,  0f  , 0.5f ,  1f},  // Done

        /* Fai */    new float[] {  1f  , 0.5f ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  2f  ,  1f  ,  2f  ,  1f  ,  1f  ,  1f  ,  1f  ,  2f  ,  1f  , 0.5f , 0.5f}, // Done

        /* Ste */    new float[] {  1f  , 0.5f , 0.5f ,  1f  , 0.5f ,  2f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  2f  ,  1f  ,  1f  ,  1f  ,  2f  , 0.5f ,  1f},  // Done

        /* Poi */    new float[] {  1f  ,  1f  ,  1f  ,  2f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  ,  1f  , 0.5f , 0.5f , 0.5f ,  1f  ,  1f  ,  2f  ,  0f  , 0.5f}  // Done
    };

    public static float GetEffectivness (PokemonType attackType, PokemonType defenseType) {

        if (attackType == PokemonType.None || defenseType == PokemonType.None) {
            return 1;
        }

        int row = (int) attackType - 1;
        int col = (int) defenseType - 1;

        return chart[row][col];

    }
    
}
