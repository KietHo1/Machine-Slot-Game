using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BonusGameMN : Singleton<BonusGameMN>
{
    public Action endBonus;
    public virtual void ShowBonusPopup(Action endBonus)
    {
        this.endBonus = endBonus;
    }

    public virtual void SetFortuneBet(int status)
    {
        
    }

    public virtual void CreateActiveYellowPanel(bool isBonus = false)
    {

    }

    public virtual void MoveYellowPanel(int index, float time = 0)
    {

    }

    public virtual void PlayBonusGame()
    {
        
    }

    public virtual void EndGame()
    {

    }
}
