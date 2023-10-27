using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GambleCard : MonoBehaviour
{
    [SerializeField] private Image bg, mainSuitImg, suitImg;
    [SerializeField] private Text numberTxt, winText;

    [SerializeField] private Sprite cardFront, cardBack, cardBackRed;
    [SerializeField] private Sprite[] suits = new Sprite[4];
    [SerializeField] private bool isMain = false;

    public void FaceSetting(bool isBack, int number = -1, int suit = 0)
    {
        if (isMain)
        {
            if (isBack)
            {
                SoundMN.Instance.PlayLoop(SFXType.GAMBLE_LOOP);
                StartCoroutine(BackFaceAnim());
            }
            else
            {
                SoundMN.Instance.StopLoop();
                StopAllCoroutines();
            }
        }

        if (bg != null)
            bg.sprite = isBack ? cardBack : cardFront;

        mainSuitImg.gameObject.SetActive(number != -1);

        if (numberTxt != null)
            numberTxt.gameObject.SetActive(number != -1);

        if (suitImg != null)
            suitImg.gameObject.SetActive(number != -1);

        if (number == -1)
            return;

        mainSuitImg.sprite = suits[suit];

        if (suitImg != null)
            suitImg.sprite = suits[suit];

        if (numberTxt != null)
            numberTxt.text = number.ToString();

        if (numberTxt != null)
            numberTxt.color = suit < 2 ? Color.black : Color.red;
    }

    public void ShowWinText(bool isShow = true)
    {
        winText.gameObject.SetActive(isShow);
    }
    IEnumerator BackFaceAnim()
    {
        if (!isMain)
            yield break;

        yield return new WaitForSeconds(0.2f);
        bg.sprite = cardBack;
        yield return new WaitForSeconds(0.2f);
        bg.sprite = cardBackRed;
        
        StartCoroutine(BackFaceAnim());
    }
}
