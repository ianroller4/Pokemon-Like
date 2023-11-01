using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour {
    
    [SerializeField] bool isPlayerUnit;

    public bool IsPlayerUnit {
        get { return isPlayerUnit; }
    }

    public Pokemon pokemon { get; set; }

    Image image;

    Vector3 orignalPos;

    Color originalColor;

    private void Awake() {

        image = GetComponent<Image>();

        orignalPos = image.transform.localPosition;

        originalColor = image.color;

    }

    public void Setup (Pokemon poke) {
 
        pokemon = poke;

        if (isPlayerUnit) {
            image.sprite = pokemon.pBase.Back;
        } else {
            image.sprite = pokemon.pBase.Front;
        }

        image.color = originalColor;

        PlayEnterAnimation();
    }

    public void PlayEnterAnimation () {

        if (isPlayerUnit) {
            image.transform.localPosition = new Vector3(-500f, orignalPos.y);
        } else {
            image.transform.localPosition = new Vector3(500f, orignalPos.y);
        }

        image.transform.DOLocalMoveX(orignalPos.x, 1f);

    }

    public void PlayAttackAnimation () {

        var sequence = DOTween.Sequence();

        if (isPlayerUnit) {
            sequence.Append (image.transform.DOLocalMoveX(orignalPos.x + 50f, 0.25f));
        } else {
            sequence.Append (image.transform.DOLocalMoveX(orignalPos.x - 50f, 0.25f));
        }

        sequence.Append (image.transform.DOLocalMoveX(orignalPos.x, 0.25f));

    }

    public void PlayHitAnimation () {

        var sequence = DOTween.Sequence();

        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));

    }

    public void PlayFaintAnimation () {

        var sequence = DOTween.Sequence();

        sequence.Append(image.transform.DOLocalMoveY(orignalPos.y - 150, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f)); // Join will let the 2 sequences play together

    }

}
