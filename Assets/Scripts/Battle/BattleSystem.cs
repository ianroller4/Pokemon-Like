using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Random=UnityEngine.Random;

public enum BattleState {Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, BattleOver}
public enum BattleAction {Move, SwitchPokemon, UseItem, Run}

public class BattleSystem : MonoBehaviour {

    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;

    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud enemyHud;

    [SerializeField] BattleDialogBox dialogBox;

    [SerializeField] PartyScreen partyScreen;

    BattleState state;
    BattleState? prevState;

    int currentAction;
    int currentMove;
    int currentMember;

    public event Action<bool> OnBattleOver;

    PokemonParty playerParty;
    Pokemon wildPokemon;

    public void StartBattle (PokemonParty playerParty, Pokemon wildPokemon) {

        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle() {

        playerUnit.Setup(playerParty.GetHealthyPokemon());

        enemyUnit.Setup(wildPokemon);

        playerHud.setData(playerUnit.pokemon);

        enemyHud.setData(enemyUnit.pokemon);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.pokemon.Moves);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.pokemon.pBase.Name} appeared.");

        ActionSelection();

    }

    void BattleOver (bool won) {

        state = BattleState.BattleOver;

        playerParty.Pokemon.ForEach(p => p.OnBattleOver());

        OnBattleOver(won);

    }

    void ActionSelection() {

        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Choose an action");
        dialogBox.EnableActionSelector(true);

    }

    void OpenPartyScreen () {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemon);
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection() {
        state = BattleState.MoveSelection;

        dialogBox.EnableActionSelector(false);

        dialogBox.EnableDialogText(false);

        dialogBox.EnableMoveSelector(true);

    }

    IEnumerator RunTurns (BattleAction playerAction) {

        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move) {

            playerUnit.pokemon.CurrentMove = playerUnit.pokemon.Moves[currentMove];
            enemyUnit.pokemon.CurrentMove = enemyUnit.pokemon.GetRandomMove();

            int playerMovePriority = playerUnit.pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.pokemon.CurrentMove.Base.Priority;

            // Check who goes first
            bool playerGoesFirst;

            if (enemyMovePriority > playerMovePriority) {
                playerGoesFirst = false;
            } else if (enemyMovePriority < playerMovePriority) {
                playerGoesFirst = true;
            } else {
                playerGoesFirst = GoesFirst();
            }

            BattleUnit first;
            BattleUnit second;

            if (playerGoesFirst) {

                first = playerUnit;
                second = enemyUnit;

            } else {

                first = enemyUnit;
                second = playerUnit;

            }

            var secondPokemon = second.pokemon;

            // First Turn
            yield return RunMove (first, second, first.pokemon.CurrentMove);
            if (state == BattleState.BattleOver) {
                yield break;
            }
            yield return RunAfterTurn(first);

            if (state == BattleState.BattleOver) {
                yield break;
            }

            if (secondPokemon.HP > 0) {
                // Second Turn
                yield return RunMove (second, first, second.pokemon.CurrentMove);
                if (state == BattleState.BattleOver) {
                    yield break;
                }
                yield return RunAfterTurn(second);

                if (state == BattleState.BattleOver) {
                    yield break;
                }
            }

        } else if (playerAction == BattleAction.SwitchPokemon) {

            var selectedPokemon = playerParty.Pokemon[currentMember];
            state = BattleState.Busy;
            yield return SwitchPokemon(selectedPokemon);

            // Enemy Turn
            var enemyMove = enemyUnit.pokemon.GetRandomMove();
            yield return RunMove (enemyUnit, playerUnit, enemyUnit.pokemon.CurrentMove);
            if (state == BattleState.BattleOver) {
                yield break;
            }
            yield return RunAfterTurn(enemyUnit);

        }

        if (state != BattleState.BattleOver) {
            ActionSelection();
        }

    }

    public bool GoesFirst () {

        bool result;

        int playerSpeed  = playerUnit.pokemon.Speed; 
        int enemySpeed = enemyUnit.pokemon.Speed;

        if (playerSpeed > enemySpeed) {

            result = true;

        } else if (playerSpeed < enemySpeed) {

            result = false;

        } else {

            if (Random.Range(0, 100) < 50) {

                result = true;

            } else {

                result = false;

            }

        }

        return result;

    }

    IEnumerator RunMove (BattleUnit sourceUnit, BattleUnit targetUnit, Move move) {

        bool canMove = sourceUnit.pokemon.OnBeforeMove();

        if (!canMove) {
            yield return ShowStatusChanges(sourceUnit.pokemon);
            if (sourceUnit.IsPlayerUnit) {
                yield return playerHud.UpdateHP();
            } else {
                yield return enemyHud.UpdateHP();
            }
            if (sourceUnit.pokemon.HP <= 0) {
                yield return dialogBox.TypeDialog($"{sourceUnit.pokemon.pBase.Name} fainted");
                sourceUnit.PlayFaintAnimation();

                yield return new WaitForSeconds(2f);

                CheckForBattleOver(sourceUnit);

            }
            yield break;
        }

        yield return ShowStatusChanges(sourceUnit.pokemon);

        move.Pp--;

        yield return dialogBox.TypeDialog($"{sourceUnit.pokemon.pBase.Name} used {move.Base.Name}");

        if (Hits(move, sourceUnit.pokemon, targetUnit.pokemon)) {

            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);

            targetUnit.PlayHitAnimation();

            if (move.Base.Category == MoveCategory.Status) {

                yield return RunMoveEffects(move.Base.Effects, sourceUnit.pokemon, targetUnit.pokemon, move.Base.Target);

            } else {

                var damageDetails = targetUnit.pokemon.TakeDamage(move, sourceUnit.pokemon);

                if (targetUnit.IsPlayerUnit) {
                    yield return playerHud.UpdateHP();
                } else {
                    yield return enemyHud.UpdateHP();
                }

                yield return ShowDamageDetails(damageDetails);

            }

            if (move.Base.SecondEffects != null && move.Base.SecondEffects.Count > 0 && targetUnit.pokemon.HP > 0) {

                foreach (var sec in move.Base.SecondEffects) {

                    var rand = Random.Range(1, 101);
                    if (rand <= sec.Chance) {

                        yield return RunMoveEffects(sec, sourceUnit.pokemon, targetUnit.pokemon, sec.Target);

                    }

                }

            }

            if (targetUnit.pokemon.HP <= 0) {
                yield return dialogBox.TypeDialog($"{targetUnit.pokemon.pBase.Name} fainted");
                targetUnit.PlayFaintAnimation();

                yield return new WaitForSeconds(2f);

                CheckForBattleOver(targetUnit);

            }

        } else {

            yield return dialogBox.TypeDialog($"{sourceUnit.pokemon.pBase.Name} missed!");

        }

    }

    IEnumerator RunAfterTurn (BattleUnit sourceUnit) {

        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        // Status 
        sourceUnit.pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.pokemon);
        if (sourceUnit.IsPlayerUnit) {
            yield return playerHud.UpdateHP();
        } else {
            yield return enemyHud.UpdateHP();
        }
        if (sourceUnit.pokemon.HP <= 0) {
            yield return dialogBox.TypeDialog($"{sourceUnit.pokemon.pBase.Name} fainted");
            sourceUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);

            CheckForBattleOver(sourceUnit);

        }

    }

    IEnumerator RunMoveEffects (MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget) {

        if (effects.Mods != null) {

            if (moveTarget == MoveTarget.Self) {

                source.ApplyMods(effects.Mods);

            } else {

                target.ApplyMods(effects.Mods);

            }
        }

        // Status Condition
        if (effects.Status != ConditionID.None) {

            target.SetStatus(effects.Status);

        }

        // Volatile Status Condition
        if (effects.VolatileStatus != ConditionID.None) {

            target.SetVolatileStatus(effects.VolatileStatus);

        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);

    }

    bool Hits (Move move, Pokemon source, Pokemon target) {

        bool hit;

        if (move.Base.AlwaysHits) {
            hit = true;
        } else {
            float acc = move.Base.Accuracy;

            int accuracy = source.StatModifiers[Stat.Accuracy];
            int evasion = source.StatModifiers[Stat.Evasion];

            var modifierLevels = new float[] {1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f};

            if (accuracy > 0) {
                acc *= modifierLevels[accuracy];
            } else {
                acc /= modifierLevels[-accuracy];
            }

            if (evasion > 0) {
                acc /= modifierLevels[evasion];
            } else {
                acc *= modifierLevels[-evasion];
            }

            int randNum = Random.Range(1, 101);

            if (randNum <= acc) {
                hit = true;
            } else {
                hit = false;
            }
        }

        return hit;

    }

    IEnumerator ShowStatusChanges (Pokemon pokemon) {

        while (pokemon.StatusChanges.Count > 0) {

            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);

        }

    }

    void CheckForBattleOver (BattleUnit faintedUnit) {

        if (faintedUnit.IsPlayerUnit) {

            var nextPokemon = playerParty.GetHealthyPokemon();

            if (nextPokemon != null) {

                OpenPartyScreen();

            } else {

                BattleOver(false);

            }

        } else {

            BattleOver(true);

        }

    }

    IEnumerator ShowDamageDetails (DamageDetails dd) {

        if (dd.Crit > 1f) {

            yield return dialogBox.TypeDialog("A critical hit!");

        }

        if (dd.Type > 1f) {

            yield return dialogBox.TypeDialog("It's super effective");

        } else if (dd.Type < 1f) {

            yield return dialogBox.TypeDialog("It's not very effective");

        }

    }

    public void HandleUpdate () {

        if (state == BattleState.ActionSelection) {

            HandleActionSelector();

        } else if (state == BattleState.MoveSelection) {

            HandleMoveSelector();

        } else if (state == BattleState.PartyScreen) {
            HandlePartySelector();
        }

    }

    void HandleActionSelector() {
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            ++currentAction;
            
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            --currentAction;

        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            currentAction += 2;

        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            currentAction -= 2;

        }

        currentAction = Mathf.Clamp(currentAction, 0, 3); // Keeps current action between 0 and 3

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z)) {

            if (currentAction == 0) {

                // Fight
                MoveSelection();

            } else if (currentAction == 1) {

                // Bag

            } else if (currentAction == 2) {

                // Party
                prevState = state;
                OpenPartyScreen();

            } else if (currentAction == 3) {

                // Run

            }

        }
    }

    void HandleMoveSelector() {

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            ++currentMove;
            
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            --currentMove;

        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            currentMove += 2;

        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            currentMove -= 2;

        }

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.pokemon.Moves.Count - 1); // Keeps current move between 0 and 3

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z)) {

            var move = playerUnit.pokemon.Moves[currentMove];

            if (move.Pp != 0) {
                dialogBox.EnableMoveSelector(false);

                dialogBox.EnableDialogText(true);

                StartCoroutine( RunTurns(BattleAction.Move) );
            }

            

        } else if (Input.GetKeyDown(KeyCode.X)) {

            dialogBox.EnableMoveSelector(false);

            dialogBox.EnableDialogText(true);

            ActionSelection();

        }

    }

    void HandlePartySelector () {

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            ++currentMember;
            
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            --currentMember;

        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            currentMember += 2;

        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            currentMember -= 2;

        }

        currentMove = Mathf.Clamp(currentMove, 0, playerParty.Pokemon.Count - 1); // Keeps current move between 0 and 6

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z)) {

            var selectedMember = playerParty.Pokemon[currentMember];

            if (selectedMember.HP <= 0) {

                partyScreen.SetMessageText("Pokemon is fainted");
                return;

            }
            if (selectedMember == playerUnit.pokemon) {

                partyScreen.SetMessageText("Pokemon is already in battle");
                return;

            }

            partyScreen.gameObject.SetActive(false);

            if (prevState == BattleState.ActionSelection) {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            } else {
                state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));
            }

        } else if (Input.GetKeyDown(KeyCode.X)) {

            partyScreen.gameObject.SetActive(false);
            ActionSelection();

        }

    }

    IEnumerator SwitchPokemon (Pokemon newPokemon) {

        if (playerUnit.pokemon.HP > 0) {

            yield return dialogBox.TypeDialog($"Good job {playerUnit.pokemon.pBase.Name}!");

            playerUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);

        }

        playerUnit.Setup(newPokemon);

        playerHud.setData(newPokemon);

        dialogBox.SetMoveNames(newPokemon.Moves);

        yield return dialogBox.TypeDialog($"Go! {newPokemon.pBase.Name}!");

        state = BattleState.RunningTurn;

    }
    
}
