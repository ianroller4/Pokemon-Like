using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartyScreen : MonoBehaviour {

    [SerializeField] TMP_Text messageText;

    PartyMemberUI[] memberSlots;

    List<Pokemon> pokemon;

    public void Init () {

        memberSlots = GetComponentsInChildren<PartyMemberUI>();

    }

    public void SetPartyData (List<Pokemon> pokemon) {

        this.pokemon = pokemon;

        for (int i = 0; i < memberSlots.Length; i++) {

            if (i < pokemon.Count) {

                memberSlots[i].SetData(pokemon[i]);

            } else {

                memberSlots[i].gameObject.SetActive(false);

            }

        }

        messageText.text = "Choose a Pokemon";

    }

    public void UpdateMemberSelection (int selected) {

        for (int i = 0; i < pokemon.Count; i++) {

            if (i == selected) {

                memberSlots[i].HighlightName(true);

            } else {

                memberSlots[i].HighlightName(false);

            }

        }

    }

    public void SetMessageText (string msg) {

        messageText.text = msg;

    }
    
}
