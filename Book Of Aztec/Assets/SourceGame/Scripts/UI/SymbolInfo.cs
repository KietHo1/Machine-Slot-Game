using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SymbolInfo : MonoBehaviour
{
    public SymbolData data;
    public Image img;
    public GameObject rewards;
    public Text pricePrefab;
    public int fontSize;

    public void Setting(SymbolData data, bool isBonus = false)
    {
        this.data = data;
        img.sprite = data.symbol;
        if(isBonus)
            img.sprite = data.bonusSymbol;
        rewards.SetActive(false);
        rewards.SetActive(true);
    }

    public void ShowRewards()
    {
        RemoveAllRewards();

        for(int i = data.rewards.Count - 1; i >= 0; i--)
        {
            float reward = data.rewards[i];
            if(reward > 0)
            {
                Text priceTxt = Instantiate(pricePrefab, rewards.transform);
                priceTxt.fontSize = fontSize;
                if (GameMN.Instance.bonusSpinData.isBonusSpin == false)
                {
                    priceTxt.color = new Color(0.9647058823529412f, 0.9058823529411765f, 0.5019607843137255f);
                    priceTxt.text = PriceString(reward);
                }
                else
                {
                    priceTxt.color = new Color(0.3254901960784314f, 0.2352941176470588f, 0.1490196078431373f);
                    priceTxt.text = PriceBonusString(i, reward);
                }
            }
        }
    }

    private string PriceString(float reward)
    {
        return Ultility.GetMoneyFormated((GameMN.Instance.GetBet() * reward));
    }

    private string PriceBonusString(int index, float reward)
    {
        return index + 1 + " " + "-" + "   " + Ultility.GetMoneyFormated((GameMN.Instance.GetBet() * reward));
    }


    private void RemoveAllRewards()
    {
        foreach(Transform child in rewards.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
