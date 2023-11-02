using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour {

    public float moveSpeed; // Movement speed of the player
    private Vector2 input;
    private Character character;

    public event Action OnEncountered;
    public event Action<Collider2D> InTrainerView;

    private void Awake () {
        character = GetComponent<Character>();
    }

    public void HandleUpdate() {
        if (!character.isMoving) {
            input.x = Input.GetAxisRaw("Horizontal"); // GetAxisRaw will result in values of either -1 or 1
            input.y = Input.GetAxisRaw("Vertical");

            // Removes diagonal movement
            if (input.x != 0 ) {
                input.y = 0;
            }

            if (input != Vector2.zero) {

                StartCoroutine(character.Move(input, OnMoveOver)); // CheckForEncounters is an action here, will be called when action is invoke in move funtion
            }
        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z)) {

            Interact();

        }
    }

    void Interact () {

        var facingDir = new Vector3(character.Animator.moveX, character.Animator.moveY);

        var interactPos = transform.position + facingDir; // Gets position in front of player

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);

        if (collider != null) {

            collider.GetComponent<Interactable>()?.Interact(transform);

        }

    }

    private void OnMoveOver () {

        CheckForEncounter();
        CheckIfInTrainerView();

    }

    private void CheckForEncounter () {

        if (Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.GrassLayer) != null) {

            if (UnityEngine.Random.Range(1, 101) <= 10) { // 1 in 10 times a pokemon will appear

                character.Animator.isMoving = false;
                OnEncountered();

            }

        }

    }

    private void CheckIfInTrainerView () {

        var collider = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.FovLayer);

        if (collider != null) { // Checks if in trainers view

            character.Animator.isMoving = false;
            InTrainerView?.Invoke(collider);

        }

    }
}
