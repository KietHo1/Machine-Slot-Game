using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NumberRun : MonoBehaviour
{
    private Text txt;
    [SerializeField] private string frontStr;
    private float startValue, endValue;
    private bool isEndCount = true;
    private float spd;
    private Action numberRunEnd;

    void Awake()
    {
        txt = GetComponent<Text>();
    }

    void Update()
    {
        if (isEndCount)
            return;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
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
            txt.text = frontStr + Ultility.GetMoneyFormated(startValue);
    }

    public void Run(float startValue, float value, float maxTime, Action numberRunEnd)
    {
        this.startValue = startValue;
        this.endValue = startValue + value;

        spd = 0f;
        float timeToRun = Mathf.Abs(value) / 10f;
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
            txt.text = frontStr + Ultility.GetMoneyFormated(endValue);
    }
}

