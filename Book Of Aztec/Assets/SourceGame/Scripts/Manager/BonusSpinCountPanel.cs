using System;
using UnityEngine;
using UnityEngine.UI;

public class BonusSpinCountPanel : Singleton<BonusSpinCountPanel>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Text countTxt, currentCountTxt;

    public void Show(bool isShow)
    {
        panel.SetActive(isShow);
        if (isShow)
            Setting();
    }

    private void Setting()
    {
        countTxt.GetComponent<NumberRun>().Run(GameMN.Instance.gameData.bonusSpinCount, -GameMN.Instance.gameData.bonusSpinCount, 5f, null, false);
        currentCountTxt.GetComponent<NumberRun>().Run(GameMN.Instance.bonusSpinData.bonusSpinCount, GameMN.Instance.gameData.bonusSpinCount, 5f, null, false);
    }
}
