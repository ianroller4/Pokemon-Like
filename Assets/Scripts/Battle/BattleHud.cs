using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleHud : MonoBehaviour {

    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text lvlText;
    [SerializeField] TMP_Text statusText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Color poisonColor;
    [SerializeField] Color burnColor;
    [SerializeField] Color paraColor;
    [SerializeField] Color sleepColor;
    [SerializeField] Color toxicColor;
    [SerializeField] Color frostColor;


    Pokemon _pokemon;
    

    public void setData (Pokemon pokemon) {

        _pokemon = pokemon;

        nameText.text = pokemon.pBase.Name;

        lvlText.text = "Lvl " + pokemon.level;

        hpBar.SetHP((float) pokemon.HP / pokemon.MaxHP);

        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;

    }

    void SetStatusText () {

        if (_pokemon.Status == null) {

            statusText.text = "";
            
        } else {

            if (_pokemon.Status.ID == ConditionID.Poison) {

                statusText.text = "PSN";
                statusText.color = poisonColor; 

            } else if (_pokemon.Status.ID == ConditionID.Burn) {

                statusText.text = "BRN";
                statusText.color = burnColor; 

            } else if (_pokemon.Status.ID == ConditionID.Paralysis) {

                statusText.text = "PAR";
                statusText.color = paraColor; 

            } else if (_pokemon.Status.ID == ConditionID.Frostbite) {

                statusText.text = "FRB";
                statusText.color = frostColor; 

            } else if (_pokemon.Status.ID == ConditionID.Sleep) {

                statusText.text = "SLP";
                statusText.color = sleepColor; 

            } else if (_pokemon.Status.ID == ConditionID.Toxic) {

                statusText.text = "TOX";
                statusText.color = toxicColor; 

            }

        }

    }

    public IEnumerator UpdateHP () {

        if (_pokemon.HPChanged) {
            yield return hpBar.SetHPSmooth((float) _pokemon.HP / _pokemon.MaxHP);
            _pokemon.HPChanged = false;
        }

    }
}
