using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour {

    [SerializeField] GameObject exclamation;
    [SerializeField] Dialog dialog;
    [SerializeField] GameObject fov;
    Character character;

    private void Awake () {
        character = GetComponent<Character>();
    }

    private void Start () {

        SetFovRotation(character.Animator.DefaultDirection);

    }

    public IEnumerator TriggerTrainerBattle (PlayerController player) {

        // Show Exclamation
        exclamation.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        exclamation.SetActive(false);

        // Walk towards player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;

        moveVec = new Vector2 (Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return character.Move(moveVec);

        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, OnFinished));

    }

    void OnFinished () {

        Debug.Log("Starting Trainer Battle");

    }

    public void SetFovRotation (FacingDirections dir) {

        float angle = 0f;

        switch (dir) {

            case FacingDirections.Up:
                angle = 180f;
                break;

            case FacingDirections.Left:
                angle = 270f;
                break;

            case FacingDirections.Right:
                angle = 90f;
                break;

            case FacingDirections.Down:
                angle = 0f;
                break;
        }

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);

    }
    
}
