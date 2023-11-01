using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator {

    SpriteRenderer spriteRenderer;
    List<Sprite> frames;
    float frameRate;

    int currentFrame;
    float timer;

    public SpriteAnimator (List<Sprite> F, SpriteRenderer SR, float FR = 0.16f) { // frame rate is set to a default

        spriteRenderer = SR;
        frames = F;
        frameRate = FR;

    }

    public void Start () {

        currentFrame = 0;
        timer = 0;

        spriteRenderer.sprite = frames[currentFrame];

    }

    public void HandleUpdate () {

        timer += Time.deltaTime;

        if (timer > frameRate) {

            currentFrame = (currentFrame + 1) % frames.Count;

            spriteRenderer.sprite = frames[currentFrame];
            timer -= frameRate;

        }

    }

    public List<Sprite> Frames {
        get { return frames; }
    }

}
