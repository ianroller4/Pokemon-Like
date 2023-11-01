using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move")]

public class MoveBase : ScriptableObject {

    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type;
    [SerializeField] MoveCategory category;

    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHits;
    [SerializeField] int pp;
    [SerializeField] int priority;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondEffects;
    [SerializeField] MoveTarget target;

    public string Name {
        get { return name; }
    }

    public string Description {
        get { return description; }
    }

    public MoveCategory Category {
        get { return category; }
    }

    public PokemonType Type {
        get { return type; }
    }

    public int Power {
        get { return power; }
    }

    public int Accuracy {
        get { return accuracy; }
    }

    public bool AlwaysHits {
        get { return alwaysHits; }
    }

    public int Pp {
        get { return pp; }
    }
    public int Priority {
        get { return priority; }
    }

    public MoveEffects Effects {
        get { return effects; }
    }

    public List<SecondaryEffects> SecondEffects {
        get { return secondEffects; }
    }

    public MoveTarget Target {
        get { return target; }
    }

}

[System.Serializable]
public class MoveEffects {

    [SerializeField] List<StatMods> mods;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;

    public List<StatMods> Mods {
        get { return mods; }
    } 

    public ConditionID Status {
        get { return status; }
    }

    public ConditionID VolatileStatus {
        get { return volatileStatus; }
    }

}

[System.Serializable]
public class SecondaryEffects : MoveEffects {

    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance {
        get { return chance; }
    }

    public MoveTarget Target {
        get { return target; }
    }

}

[System.Serializable]
public class StatMods {

    public Stat stat;
    public int mod;

}

public enum MoveCategory {

    Status,
    Physical,
    Special

}

public enum MoveTarget {

    Foe,
    Self

}

