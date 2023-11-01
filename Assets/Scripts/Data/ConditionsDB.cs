using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB {

    public static void Init () {

        foreach (var kvp in ConditionList) {

            var conditionID = kvp.Key;
            var condition = kvp.Value;

            condition.ID = conditionID;

        }

    }

    public static Dictionary<ConditionID, Conditions> ConditionList { get; set; } = new Dictionary<ConditionID, Conditions>() {

        {ConditionID.Poison, new Conditions() { 
            Name = "Poison", 
            StartMsg = "has been poisoned", 
            OnAfterTurn = PoisonEffect
            }
        },
        {ConditionID.Burn, new Conditions() {
            Name = "Burn", 
            StartMsg = "has been burned", 
            OnAfterTurn = BurnEffect
            }
        },
        {ConditionID.Frostbite, new Conditions() { 
            Name = "Frostbite", 
            StartMsg = "has been frostbitten", 
            OnAfterTurn = FrostbiteEffect
            }
        },
        {ConditionID.Paralysis, new Conditions() { 
            Name = "Paralysis", 
            StartMsg = "has been paralyzed", 
            OnBeforeMove = ParalysisEffect
            }
        },
        {ConditionID.Sleep, new Conditions() { 
            Name = "Sleep", 
            StartMsg = "has fallen asleep",
            OnStart = GetSleepTurns, 
            OnBeforeMove = SleepEffect
            }
        },
        {ConditionID.Confusion, new Conditions() { 
            Name = "Confusion", 
            StartMsg = "is confused",
            OnStart = GetConfusionTurns, 
            OnBeforeMove = ConfusionEffect
            }
        }

    };

    static void PoisonEffect (Pokemon pokemon) {

        pokemon.UpdateHP(pokemon.MaxHP / 8);
        pokemon.StatusChanges.Enqueue($"{pokemon.pBase.Name} was hurt due to poison");

    }

    static void ToxicEffect (Pokemon pokemon) {

    }

    static void BurnEffect (Pokemon pokemon) {

        pokemon.UpdateHP(pokemon.MaxHP / 16);
        pokemon.StatusChanges.Enqueue($"{pokemon.pBase.Name} was hurt due to its burn");

    }

    static bool SleepEffect (Pokemon pokemon) {

        bool result;

        if (pokemon.StatusTime <= 0) {

            pokemon.CureStatus();
            pokemon.StatusChanges.Enqueue($"{pokemon.pBase.Name} woke up!");  
            
            result = true;
        } else {

            pokemon.StatusTime--;
            pokemon.StatusChanges.Enqueue($"{pokemon.pBase.Name} is sleeping");       

            result = false;
        }

        return result;

    }

    static void GetSleepTurns (Pokemon pokemon) {

        pokemon.StatusTime = Random.Range(1, 4);
        Debug.Log($"{pokemon.pBase.Name} will be asleep for {pokemon.StatusTime} turns");

    }

    static void FrostbiteEffect (Pokemon pokemon) {

        pokemon.UpdateHP(pokemon.MaxHP / 16);
        pokemon.StatusChanges.Enqueue($"{pokemon.pBase.Name} was hurt due to its frostbite");

    }

    static bool ParalysisEffect (Pokemon pokemon) {

        bool result;

        if (Random.Range(1, 5) == 1) {
            result = false;
            pokemon.StatusChanges.Enqueue($"{pokemon.pBase.Name} is paralyzed! It can not move!");
        } else {
            result = true;
        }

        return result;

    }

    static void GetConfusionTurns (Pokemon pokemon) {

        pokemon.VolatileStatusTime = Random.Range(1, 5);
        Debug.Log($"{pokemon.pBase.Name} will be confused for {pokemon.VolatileStatusTime} turns");

    }

    static bool ConfusionEffect (Pokemon pokemon) {

        bool result;

        if (pokemon.VolatileStatusTime <= 0) {

            pokemon.CureVolatileStatus();
            pokemon.StatusChanges.Enqueue($"{pokemon.pBase.Name} snapped out of confusion!");
            result = true;

        } else {

            pokemon.VolatileStatusTime--;
            if (Random.Range(1, 3) == 1) {

                result = true;

            } else {

                // Hurt by confusion
                pokemon.StatusChanges.Enqueue($"{pokemon.pBase.Name} is confused");
                pokemon.UpdateHP(pokemon.MaxHP / 2);
                pokemon.StatusChanges.Enqueue("It hurt itself in confusion");
                result = false;

            }

        }

        return result;


    }

}

public enum ConditionID {

    None,
    Poison,
    Toxic,
    Burn,
    Sleep,
    Frostbite,
    Paralysis,
    Confusion

}
