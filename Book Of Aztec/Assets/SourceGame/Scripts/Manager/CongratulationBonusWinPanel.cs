using System;
using UnityEngine;
using UnityEngine.UI;

public class CongratulationBonusWinPanel : Singleton<CongratulationBonusWinPanel>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Text rewardTxt, playedTxt;

    public void Show(bool isShow)
    {
        panel.SetActive(isShow);
        if(isShow)
            Setting();
    }

    private void Setting()
    {
        rewardTxt.GetComponent<NumberRun>().Run(0, GameMN.Instance.currentRewards, 1, null);
        playedTxt.text = "BONUSSPINS PLAYED: " + GameMN.Instance.bonusSpinData.playedCount;
    }
}
