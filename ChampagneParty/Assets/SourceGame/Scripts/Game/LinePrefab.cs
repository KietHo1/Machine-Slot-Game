using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinePrefab : MonoBehaviour
{
    [SerializeField] private Text betTxt, lineTxt;
    [SerializeField] private Image img, blackBG;

    public void SetBet(float value)
    {
        betTxt.text = Ultility.GetMoneyFormated(value);
    }

    public void SetLine(int value)
    {
        lineTxt.text = value.ToString();
    }

    public Color GetColor()
    {
        return img.color;
    }

    public void Hide()
    {
        blackBG.GetComponent<Highlight>().enabled = false;
        blackBG.GetComponent<Image>().enabled = true;
    }

    public void Show()
    {
        if(blackBG.GetComponent<Highlight>().enabled)
            blackBG.GetComponent<Highlight>().Play();

        blackBG.GetComponent<Highlight>().enabled = true;
    }
}
