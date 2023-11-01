using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character : MonoBehaviour {

    public float moveSpeed; // Movement speed of the player

    public bool isMoving { get; private set;}
    CharacterAnimator animator;

    public CharacterAnimator Animator {
        get { return animator; }
    }

    private void Awake () {

        animator = GetComponent<CharacterAnimator>();

    }

    public IEnumerator Move (Vector2 moveVec, Action OnMoveOver=null) { // Function to do something over some period of time

        animator.moveX = Mathf.Clamp(moveVec.x, -1f, 1f);
        animator.moveY = Mathf.Clamp(moveVec.y, -1f, 1f);
        var targetPos = transform.position; // This initially gets where the player is

        // We then add the values of input to get the new position we want to move to
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;

        if (IsPathClear(targetPos)) {

            isMoving = true;

            while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) { // Checks if there actually a difference between starting and target positions

                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime); // Moves the player in the direction a small bit at a time
                yield return null; // Will stop the above function but then go back to while loop???

            }

            transform.position = targetPos; // Sets the players position to target position

            isMoving = false;

            OnMoveOver?.Invoke();

        }

    }

    public void HandleUpdate () {

        animator.isMoving = isMoving;

    }

    private bool IsPathClear (Vector3 targetPos) {

        var diff = targetPos - transform.position;
        var dir = diff.normalized;

        bool result;

        bool collision = Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, GameLayers.i.SolidObjectsLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer);

        if (collision) {
            result = false;
        } else {
            result = true;
        }

        return result;

    }

    private bool isWalkable (Vector3 targetPos) {

        bool result = true;

        if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.i.SolidObjectsLayer | GameLayers.i.InteractableLayer) != null) {

            result = false;

        }

        return result;        

    }

    public void LookTowards (Vector3 targetPos) {

        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xdiff == 0 || ydiff == 0) {

            animator.moveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.moveY = Mathf.Clamp(ydiff, -1f, 1f);

        } else {
            Debug.LogError ("Error in LookTowards (Character Script): Cannot look diagonally");
        }

    }

}
