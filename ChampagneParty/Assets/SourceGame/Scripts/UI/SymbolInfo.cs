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

    public void Setting(SymbolData data)
    {
        this.data = data;
        img.sprite = data.symbol;
        //rewards.SetActive(false);
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
                priceTxt.text = PriceString(i, reward);
            }
        }
    }

    private string PriceString(int index , float reward)
    {
        return Ultility.GetMoneyFormated((GameMN.Instance.GetBet() * reward));
    }

    private void RemoveAllRewards()
    {
        foreach(Transform child in rewards.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
