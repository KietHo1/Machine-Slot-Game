using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GambleCard : MonoBehaviour
{
    [SerializeField] private Image bg;

    [SerializeField] private Sprite cardFront, cardBack1, cardBack2;
    [SerializeField] private Sprite[] suits = new Sprite[4];

    public void FaceSetting(bool isBack, int suit = 0)
    {
        if (bg != null)
            bg.sprite = isBack ? cardBack1 : cardFront;

        if (!isBack)
            bg.sprite = suits[suit];
    }

    public void SetCardBack(string cardBack)
    {
        if (cardBack == "cardback1") bg.sprite = cardBack1;
        if (cardBack == "cardback2") bg.sprite = cardBack2;
    }
}
