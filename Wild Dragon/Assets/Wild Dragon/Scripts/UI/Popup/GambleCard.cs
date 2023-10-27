using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GambleCard : MonoBehaviour
{
    [SerializeField] private Image bg, mainSuitImg, suitImg;
    public Text numberTxt;

    [SerializeField] private Sprite cardFront, cardBack, cardBackRed;
    [SerializeField] private Sprite[] suits = new Sprite[4];
    [SerializeField] private bool isMain = false;

    public void FaceSetting(bool isBack, int number = -1, int suit = 0)
    {
        if (isMain)
        {
            if (isBack)
            {
                StartCoroutine(BackFaceAnim());
                SoundMN.Instance.PlayLoop(SFXType.deal_2);
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
            numberTxt.text = "A";

        if (numberTxt != null)
            numberTxt.color = suit < 2 ? Color.black : Color.red;
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
