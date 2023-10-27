using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GambleCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image bg, mainSuitImg, suitImg, icon;
    [SerializeField] private Text numberTxt;

    [SerializeField] private Sprite cardFront, cardBack, jockerCard;
    [SerializeField] private Sprite[] suits = new Sprite[4], icons = new Sprite[4];

    public int id;
    public bool isBlock;

    public void OnPointerClick(PointerEventData eventData)
    {
        GambleGameMN.Instance.PickCard(id);
    }

    public void FaceSetting(bool isBack, int number = -1, int suit = 0)
    {
        isBlock = !isBack;

        if (bg != null)
            bg.sprite = isBack ? cardBack : cardFront;

        mainSuitImg.gameObject.SetActive(number != -1);

        if (numberTxt != null)
        {
            numberTxt.gameObject.SetActive(number != -1);
            icon.gameObject.SetActive(number != -1);
        }

        if (suitImg != null)
            suitImg.gameObject.SetActive(number != -1);

        if (number == -1)
            return;

        mainSuitImg.sprite = suits[suit];

        if (suitImg != null)
            suitImg.sprite = suits[suit];

        if (numberTxt != null)
        {
            if (number == 11) { numberTxt.text = "J"; icon.sprite = icons[0]; }
            if (number == 12) { numberTxt.text = "Q"; icon.sprite = icons[1]; }
            if (number == 13) { numberTxt.text = "K"; icon.sprite = icons[2]; }
            if (number == 14) { numberTxt.text = "A"; icon.sprite = icons[3]; }
            if (number == 15) 
            { 
                numberTxt.gameObject.SetActive(false); 
                mainSuitImg.gameObject.SetActive(false); 
                suitImg.gameObject.SetActive(false); 
                icon.gameObject.SetActive(false); 
                bg.sprite = jockerCard; 
            }
            if (number < 11) { icon.gameObject.SetActive(false); numberTxt.text = number.ToString(); }
        }

        if (numberTxt != null)
            numberTxt.color = suit < 2 ? Color.black : Color.red;
    }
}

