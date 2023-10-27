using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineInfo : MonoBehaviour
{
    public Image lineImg;
    public Text lineTxt;

    public void Setting(int index, Sprite spr)
    {
        lineImg.sprite = spr;
        lineTxt.text = "LINE " + index.ToString();
    }
}
