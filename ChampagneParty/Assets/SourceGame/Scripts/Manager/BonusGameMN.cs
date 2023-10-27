using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEngine.Networking.UnityWebRequest;

public class BonusGameMN : Singleton<BonusGameMN>
{
    public Action endBonus;
    [SerializeField] private BonusGamePopup bonusGamePopup;
    private int bottleChoosingCount = 0, maxChoose = 2;
    private bool canNotChoose = false;
    private float reward = 0f;

    public void SetEndBonusEvent(Action endBonus = null)
    {
        this.endBonus = endBonus;
    }

    public void ShowGamePanel()
    {
        bonusGamePopup.Show();
        bottleChoosingCount = 0;
        canNotChoose = false;
        reward = 0;
        NotificationPanel.Instance.Show(NotificationType.PARTY_BONUS);
    }

    public void BottleChoosing()
    {
        bottleChoosingCount++;
        if (bottleChoosingCount >= maxChoose)
        {
            canNotChoose = true;
            Invoke("Collect", 2f);
        }
    }

    public float GetReward()
    {
        bool isBottleWin = Ultility.isWin(Mathf.RoundToInt(GameMN.Instance.gameData.bonusWinOccur));
        if (!isBottleWin)
            return 0;

        if (bottleChoosingCount == 1)
        {
            if (GameMN.Instance.currentRewards * 2 > GameSetting.max_win)
                return 0;

            if(GameMN.Instance.currentRewards > 0)
            {
                reward += GameMN.Instance.currentRewards;
                return GameMN.Instance.currentRewards;
            }
            else
            {
                reward += 5;
                return 5;
            }
            
        }

        if(bottleChoosingCount == 2)
        {
            if (GameMN.Instance.currentRewards * 4 > GameSetting.max_win)
                return 0;

            if (GameMN.Instance.currentRewards > 0)
            {
                reward += GameMN.Instance.currentRewards * 2;
                return GameMN.Instance.currentRewards * 2;
            }
            else
            {
                reward += 10;
                return 10;
            }
        }

        return 0;
    }

    private void Collect()
    {
        UIMN.Instance.RewardSetting(reward);
        SoundMN.Instance.PlayOneShot(SFXType.PARTYBOTTLECLOSE);
        bonusGamePopup.Hide();
        //SoundMN.Instance.StopLoop();
        endBonus?.Invoke();
    }

    public bool CanNotChoose()
    {
        return canNotChoose;
    }

}