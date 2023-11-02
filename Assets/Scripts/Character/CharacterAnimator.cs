using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour {

    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] FacingDirections defaultDirection = FacingDirections.Down;

    // Parameters
    public float moveX { get; set; }
    public float moveY { get; set; }
    public bool isMoving { get; set; }

    // States
    SpriteAnimator walkDown;
    SpriteAnimator walkUp;
    SpriteAnimator walkRight;
    SpriteAnimator walkLeft;
    bool wasMoving;

    SpriteAnimator currentAnimation;

    // References
    SpriteRenderer spriteRenderer;

    private void Start () {

        spriteRenderer = GetComponent<SpriteRenderer>();

        walkDown = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUp = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkRight = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkLeft = new SpriteAnimator(walkLeftSprites, spriteRenderer);

        SetFacingDirection(defaultDirection);

        currentAnimation = walkDown;

    }

    private void Update () {

        var prevAnim = currentAnimation;

        if (moveX == 1) {

            currentAnimation = walkRight;

        } else if (moveX == -1) {

            currentAnimation = walkLeft;

        } else if (moveY == 1) {

            currentAnimation = walkUp;

        } else if (moveY == -1) {
            
            currentAnimation = walkDown;

        }

        if (currentAnimation != prevAnim || isMoving != wasMoving) {
            currentAnimation.Start();
        }

        if (isMoving) {
            currentAnimation.HandleUpdate();
        } else {
            spriteRenderer.sprite = currentAnimation.Frames[0];
        }

        wasMoving = isMoving;

    }

    public void SetFacingDirection (FacingDirections dir) {

        switch (dir) {

            case FacingDirections.Up:
                moveY = 1;
                break;

            case FacingDirections.Down:
                moveY = -1;
                break;

            case FacingDirections.Left:
                moveX = -1;
                break;

            case FacingDirections.Right:
                moveX = 1;
                break;
        }

    }

    public FacingDirections DefaultDirection {
        get { return defaultDirection; }
    }

}

public enum FacingDirections { Up, Down, Left, Right}
