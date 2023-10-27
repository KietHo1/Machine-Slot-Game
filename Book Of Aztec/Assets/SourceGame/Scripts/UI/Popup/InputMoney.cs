using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputMoney : MonoBehaviour
{
    [SerializeField] private List<Button> numberBtn = new List<Button>();
    [SerializeField] private Button removeAllBtn, removeEachBtn;
    [SerializeField] private Text moneyTxt, balanceTxt, totalBetTxt;
    private string currentMoneyStr = "";
    private float currentMoney = 0f;

    private void Awake() {
        removeAllBtn.onClick.AddListener(RemoveAll);
        removeEachBtn.onClick.AddListener(RemoveEach);

        for(int i = 0; i < numberBtn.Count; i++)
        {
            int n = i;
            numberBtn[i].onClick.AddListener(delegate{InputNumber(n);});
        }
    }

    public void Init()
    {
        Result("", 0f);
        balanceTxt.text = "Balance: " + Ultility.GetMoneyFormated(UserData.balance_point);
        totalBetTxt.text = "Total Bet: " + Ultility.GetMoneyFormated(GameMN.Instance.GetTotalBet());
    }

    public float GetMoney()
    {
        return currentMoney / 100;
    }

    private string GetMoneyStr()
    {
        return Ultility.GetMoneyFormated(GetMoney());
    }

    private void InputNumber(int number)
    {
        string str = number.ToString();
        Result(currentMoneyStr += str, float.Parse(currentMoneyStr));
    }

    private void RemoveAll()
    {
        Result("", 0f);
    }

    private void RemoveEach()
    {
        string str = "";
        for(int i = 0; i < currentMoneyStr.Length - 1; i++)
        {
            str += currentMoneyStr[i];
        }
        currentMoneyStr = str;
        if(currentMoneyStr.Length > 0)
            currentMoney = float.Parse(currentMoneyStr);
        else
            currentMoney = 0f;
        
        Result(str, currentMoney);
    }

    private void Result(string str, float money)
    {
        currentMoneyStr = str;
        currentMoney = money;

        if(currentMoney == 0)
            currentMoneyStr = "";

        moneyTxt.text = GetMoneyStr();
    }
}
