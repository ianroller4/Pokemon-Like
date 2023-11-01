using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour {

    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;

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

}
