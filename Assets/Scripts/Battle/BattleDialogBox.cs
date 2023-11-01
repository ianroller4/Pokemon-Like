using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleDialogBox : MonoBehaviour {

    [SerializeField] TMP_Text dialogText;

    [SerializeField] int lettersPerSecond;

    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;

    [SerializeField] List<TMP_Text> actionTexts;
    [SerializeField] List<TMP_Text> moveTexts;

    [SerializeField] TMP_Text ppText;
    [SerializeField] TMP_Text typeText;

    [SerializeField] Color highlight;


    public void SetDialog (string dialog) {

        dialogText.text = dialog;

    }

    public IEnumerator TypeDialog (string dialog) {

        dialogText.text = "";

        foreach (var letter in dialog.ToCharArray()) {

            dialogText.text += letter;
            yield return new WaitForSeconds(1f/lettersPerSecond); // Waits a 1/30 of a second

        }

        yield return new WaitForSeconds(1f);

    }

    public void EnableDialogText (bool enabled) {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector (bool enabled) {
        actionSelector.SetActive(enabled);
    }

   public void EnableMoveSelector (bool enabled) {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void UpdateActionSelection (int selectedAction) {

        for (int i = 0; i < actionTexts.Count; i++) {
            if (i == selectedAction) {
                actionTexts[i].color = highlight;
            } else {
                actionTexts[i].color = Color.black;
            }
        }
    }

     public void UpdateMoveSelection (int selectedAction, Move move) {

        for (int i = 0; i < moveTexts.Count; i++) {
            if (i == selectedAction) {
                moveTexts[i].color = highlight;
            } else {
                moveTexts[i].color = Color.black;
            }
        }

        ppText.text = $"PP {move.Pp}/{move.Base.Pp}";
        typeText.text = move.Base.Type.ToString();

        if (move.Pp == 0) {
            ppText.color = Color.red;
        } else {
            ppText.color = Color.black;
        }
    }

    public void SetMoveNames (List<Move> moves) {

        for (int i = 0; i < moveTexts.Count; i++) {

            if (i < moves.Count) {

                moveTexts[i].text = moves[i].Base.Name;

            } else {

                moveTexts[i].text = "-";

            }

        }

    }
    
    
}
