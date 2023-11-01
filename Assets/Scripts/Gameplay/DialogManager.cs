using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DialogManager : MonoBehaviour {

    [SerializeField] GameObject dialogBox;
    [SerializeField] TMP_Text dialogText;
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    public static DialogManager Instance { get; private set; }

    int currentLine = 0;

    Dialog dialog;
    Action onDialogFinished;

    bool isTyping;
    public bool isShowing { get; private set; }

    private void Awake () {

        Instance = this;

    }

    public IEnumerator ShowDialog (Dialog dialog, Action onFinished=null) {

        yield return new WaitForEndOfFrame(); // Fixes some edge case

        OnShowDialog?.Invoke();

        isShowing = true;
        this.dialog = dialog;
        onDialogFinished = onFinished;

        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
        currentLine++;

    }

    public void HandleUpdate () {

        if (Input.GetKeyDown(KeyCode.Z) && !isTyping) {

            if (currentLine < dialog.Lines.Count) {

                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
                currentLine++;

            } else {

                currentLine = 0;
                isShowing = false;
                dialogBox.SetActive(false);
                onDialogFinished?.Invoke();
                OnCloseDialog?.Invoke();

            }

        }

    }

    public IEnumerator TypeDialog (string line) {

        isTyping = true;

        dialogText.text = "";

        foreach (var letter in line.ToCharArray()) {

            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond); // Waits a 1/30 of a second

        }

        isTyping = false;

    }

}
