using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InputMoneyPanel : MonoBehaviour
{
    [SerializeField] private InputMoney inputMoney;
    private Action<float> evt;
    [SerializeField] private Button backBtn, doneBtn;

    private void Awake() {
        backBtn.onClick.AddListener(Back);
        doneBtn.onClick.AddListener(Done);
    }


    public void GetAction(Action<float> evt)
    {
        gameObject.SetActive(true);
        inputMoney.Init();
        this.evt = evt;
    }

    private void Back()
    {
        if (inputMoney.GetMoney() != 0) inputMoney.RemoveAll();
        else gameObject.SetActive(false);
    }

    private void Done()
    {
        //Back();
        evt?.Invoke(inputMoney.GetMoney());
        gameObject.SetActive(false);
    }
}
