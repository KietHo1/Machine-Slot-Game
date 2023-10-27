using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class NumberRun : MonoBehaviour
{
    private Text txt;
    private TMP_Text tmpTxt;
    [SerializeField] private string frontStr;
    private float startValue, endValue;
    private bool isEndCount = true;
    private float spd;
    private Action numberRunEnd;
    [SerializeField] private bool isMoney = true;
    private bool isMouse = true;

    void Awake()
    {
        txt = GetComponent<Text>();
        tmpTxt = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (isEndCount)
            return;

        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && isMouse)
        {
            End();
            return;
        }

        if (startValue > endValue)
        {
            startValue -= Time.deltaTime * spd;
            if (startValue <= endValue)
            {
                startValue = endValue;
                End();
            }
        }

        if (startValue < endValue)
        {
            startValue += Time.deltaTime * spd;
            if (startValue >= endValue)
            {
                startValue = endValue;
                End();
            }
        }

        if (startValue == endValue)
            End();

        if (txt != null)
        {
            if (isMoney)
                txt.text = frontStr + Ultility.GetMoneyFormated(startValue);
            else
                txt.text = frontStr + ((int)startValue).ToString();
        }
           
        if(tmpTxt != null)
        {
            if (isMoney)
                tmpTxt.text = frontStr + Ultility.GetMoneyFormated(startValue);
            else
                tmpTxt.text = frontStr + ((int)startValue).ToString();
        }
    }

    public void Run(float startValue, float value, float maxTime, Action numberRunEnd, bool _isMouse = true)
    {
        this.isMouse = _isMouse;
        this.startValue = startValue;
        this.endValue = startValue + value;

        spd = 0f;
        float timeToRun = Mathf.Abs(value) / 10f;
        if (!isMouse)
            timeToRun = Mathf.Abs(value) / 2.5f;
        if (timeToRun > maxTime)
            timeToRun = maxTime;

        while (spd * timeToRun < Mathf.Abs(value))
            spd += 0.1f;

        isEndCount = false;

        this.numberRunEnd = numberRunEnd;
    }

    private void End()
    {
        isEndCount = true;
        numberRunEnd?.Invoke();
        if (txt != null)
        {
            if (isMoney)
                txt.text = frontStr + Ultility.GetMoneyFormated(endValue);
            else
                txt.text = frontStr + ((int)endValue).ToString();
        }

        if (tmpTxt != null)
        {
            if (isMoney)
                tmpTxt.text = frontStr + Ultility.GetMoneyFormated(endValue);
            else
                tmpTxt.text = frontStr + ((int)endValue).ToString();
        }
    }
}

