using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartyMemberUI : MonoBehaviour {

    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text lvlText;
    [SerializeField] HPBar hpBar;

    [SerializeField] Color highlight;

    Pokemon _pokemon;
    

    public void SetData (Pokemon pokemon) {

        _pokemon = pokemon;

        nameText.text = pokemon.pBase.Name;

        lvlText.text = "Lvl " + pokemon.level;

        hpBar.SetHP((float) pokemon.HP / pokemon.MaxHP);

    }

    public void HighlightName (bool selected) {

        if (selected) {

            nameText.color = highlight;
 
        } else {
            nameText.color = Color.black;
        }

    }

}
