using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog}

public class GameController : MonoBehaviour {

    GameState state;

    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera overWorld;

    private void Awake () {

        ConditionsDB.Init();

    }

    private void Start () {

        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
        DialogManager.Instance.OnShowDialog += StartDialog;
        DialogManager.Instance.OnCloseDialog += CloseDialog;

    }

    void CloseDialog () {
        
        if (state == GameState.Dialog) {
            state = GameState.FreeRoam;
        }

    }

    void StartDialog () {
        
        state = GameState.Dialog;

    }

    void StartBattle() {

        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        overWorld.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetPokemon();

        battleSystem.StartBattle(playerParty, wildPokemon);

    }

    void EndBattle (bool won) {
        
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        overWorld.gameObject.SetActive(true);

    }

    public void Update () {

        if (state == GameState.FreeRoam) {

            playerController.HandleUpdate();

        } else if (state == GameState.Battle) {

            battleSystem.HandleUpdate();

        } else if (state == GameState.Dialog) {
            DialogManager.Instance.HandleUpdate();
        }

    }
    
}
