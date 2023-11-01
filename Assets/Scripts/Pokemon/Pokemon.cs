using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]

public class Pokemon {

    [SerializeField] PokemonBase _base;
    [SerializeField] int _level;

    public PokemonBase pBase { 
        get {
            return _base;
        }
    }

    public int level { 
        get {
            return _level;
        }
    }

    public int HP { get; set; }
    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatModifiers { get; private set; }
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    public Conditions Status { get; private set; }
    public Conditions VolatileStatus { get; private set; }

    public int StatusTime { get; set; }
    public int VolatileStatusTime { get; set; }

    public bool HPChanged { get; set; }

    public event System.Action OnStatusChanged;

    public void Init () {

        Moves = new List<Move>();

        foreach (var move in pBase.LearnableMoves) {
            if (move.Level <= level) {
                Moves.Add(new Move(move.MoveBase));
            }

            if (Moves.Count >= 4) {
                break;
            }
        }

        CalculateStats();
        
        HP = MaxHP;

        ResetStatMods();
        Status = null;
        VolatileStatus = null;

    }

    void ResetStatMods () {
        StatModifiers = new Dictionary<Stat, int>() {

            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0},
            {Stat.Accuracy, 0},
            {Stat.Evasion, 0}
        };
    }

    void CalculateStats() {

        Stats = new Dictionary<Stat, int>();

        Stats.Add(Stat.Attack, Mathf.FloorToInt((pBase.Attack * level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((pBase.Defense * level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((pBase.SpAttack * level) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((pBase.SpDefense * level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((pBase.Speed * level) / 100f) + 5);

        MaxHP = Mathf.FloorToInt((pBase.MaxHP * level) / 100f) + 10 + level;

    }

    int GetStat (Stat stat) {

        int statVal = Stats[stat];

        // Apply stat modifier

        int modifier = StatModifiers[stat];
        var modifierLevels = new float[] {1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f};

        if (modifier >= 0) {
            statVal = Mathf.FloorToInt(statVal * modifierLevels[modifier]);
        } else {

            modifier *= -1;

            statVal = Mathf.FloorToInt(statVal / modifierLevels[modifier]);
        }

        return statVal;

    }

    public void ApplyMods (List<StatMods> statMods) {

        foreach (var m in statMods) {

            var stat = m.stat;
            var mod = m.mod;

            StatModifiers[stat] = Mathf.Clamp (StatModifiers[stat] + mod, -6, 6);

            if (mod > 0) {

                StatusChanges.Enqueue($"{pBase.Name}'s {stat} has increased!");

            } else {

                StatusChanges.Enqueue($"{pBase.Name}'s {stat} has decreased!");     
            }

            Debug.Log ($"{stat} has been changed to {StatModifiers[stat]}");

        }

    }

    public int Attack {
        get { return GetStat(Stat.Attack); }
    }

    public int Defense {
        get { return GetStat(Stat.Defense); }
    }

    public int SpAttack {
        get { return GetStat(Stat.SpAttack); }
    }

    public int SpDefense {
        get { return GetStat(Stat.SpDefense); }
    }

    public int Speed {
        get { return GetStat(Stat.Speed); }
    }

    public int MaxHP { get; private set; }

    public DamageDetails TakeDamage (Move move, Pokemon attacker) {

        int damage = 0;

        var damageDetails = new DamageDetails() {

                Type = 1f,
                Crit = 1f,
                Fainted = false

        };

        if (move.Base.Category != MoveCategory.Status) {

            float crit = 1f;

            if (Random.value * 100f <= 6.25) {
                crit = 2f;
                damageDetails.Crit = crit;
            }

            float type = TypeChart.GetEffectivness(move.Base.Type, this.pBase.Type1) * TypeChart.GetEffectivness(move.Base.Type, this.pBase.Type2);

            damageDetails.Type = type;

            float attack;
            float defense;

            if (move.Base.Category == MoveCategory.Special) {
                attack = attacker.SpAttack;
                defense = SpDefense;
            } else {
                attack = attacker.Attack;
                defense = Defense;
            }

            float modifier = Random.Range(0.85f, 1f) * type * crit;
            float a = (2 * attacker.level + 10) / 250f;
            float d = a * move.Base.Power * ((float) attack / defense) + 2;
            damage = Mathf.FloorToInt(d * modifier);
        }

        UpdateHP(damage);

        return damageDetails;

    }

    public void UpdateHP (int damage) {

        if (damage < 1) {
            damage = 1;
        }

        HP = Mathf.Clamp(HP - damage, 0, MaxHP);
        HPChanged = true;

    }

    public void SetStatus (ConditionID conditionID) {

        if (Status == null) {

            Status = ConditionsDB.ConditionList[conditionID];

            Status?.OnStart?.Invoke(this);

            StatusChanges.Enqueue($"{pBase.Name} {Status.StartMsg}");

            OnStatusChanged?.Invoke();
            
        }

    }

    public void SetVolatileStatus (ConditionID conditionID) {

        if (VolatileStatus == null) {

            VolatileStatus = ConditionsDB.ConditionList[conditionID];

            VolatileStatus?.OnStart?.Invoke(this);

            StatusChanges.Enqueue($"{pBase.Name} {VolatileStatus.StartMsg}");
            
        }

    }      

    public void CureStatus () {

        Status = null;
        OnStatusChanged?.Invoke();

    }

    public void CureVolatileStatus () {

        VolatileStatus = null;

    }

    public Move GetRandomMove() {
        var movesWithPP = Moves.Where(x => x.Pp > 0).ToList(); // Currently breaks if pokemon has no pp on all moves
        int r = Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];
    }

    public bool OnBeforeMove() {

        bool result = true; // The result if a pokemon can move or not

        if (Status?.OnBeforeMove != null) {

            if (!Status.OnBeforeMove(this)) {
                result = false;
            }

        }

        if (VolatileStatus?.OnBeforeMove != null) {

            if (!VolatileStatus.OnBeforeMove(this)) {
                result = false;
            }

        }

        return result;

    }

    public void OnAfterTurn () {

        Status?.OnAfterTurn?.Invoke(this); // ? will allow it to only execute code to the right if not null
        VolatileStatus?.OnAfterTurn?.Invoke(this);

    }

    public void OnBattleOver() {

        VolatileStatus = null;
        ResetStatMods();

    }

}

public class DamageDetails {

    public bool Fainted { get; set; }

    public float Crit { get; set; }

    public float Type { get; set; }

}
