using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeSpinPanel : MonoBehaviour
{
    [SerializeField] private Text contentTxt;

    public void Show(bool isStep1, float reward = 0)
    {
        gameObject.SetActive(true);
        string str = isStep1 ? ("15 FREE GAMES \n ALL PRIZES X3") : ("FEATURE WIN \n" + Ultility.GetMoneyFormated(reward) + "\n 15 FREE GAMES PLAYED");
        contentTxt.text = str;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
