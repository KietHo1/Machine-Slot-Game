using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;

public class SlotMN : Singleton<SlotMN>
{
    public List<SlotReel> reels = new List<SlotReel>();
    public List<Transform> reelsParent = new List<Transform>();
    public SlotReel reelPrefab;

    public Transform topPos, bottomPos;
    public SpinData spinData;
    private int numberOfStopReel = 0;

    private bool isStart = true;
    private int scatterCount = 0;
    private int wildCount = 0;

    public List<Sprite> scatterWildSpriteList;
   

    public void CreateReels()
    {
        for (int i = 0; i < GameMN.Instance.gameData.column; i++)
        {
            SlotReel reel = Instantiate(reelPrefab, reelsParent[i]);
            reel.transform.localPosition = new Vector3(reel.transform.localPosition.x, topPos.localPosition.y, reel.transform.localPosition.z);
            reel.Init(i, spinData, topPos.localPosition.y, bottomPos.localPosition.y);
            reels.Add(reel);
        }
    }

    private void CountReelStop()
    {
        numberOfStopReel++;
    }

    public void RunSlots()
    {
        if (GameMN.Instance.gameActivity != GameActivity.NORMAL) return;
        GameMN.Instance.GameStatus(GameActivity.SPIN);
        ShowWinMN.Instance.ShowAllWin(false);
        ShowLineMN.Instance.HideAll();

        //if(!GameMN.Instance.isFreeSpin())
        //    SoundMN.Instance.PlayLoop(SFXType.REEL_LOOP);
        //else
        //    SoundMN.Instance.PlayLoop(SFXType.FREE_SPIN_LOOP);

        isStart = false;
        StopCoroutine(IERunSlots());
        StartCoroutine(IERunSlots());
    }

    public void ReelStoping()
    {
        if (!isStopClicked)
        {
            foreach (SlotReel reel in reels)
                reel.ReelStoping();
            isStopClicked = true;
        }
    }

    bool isStopClicked = false;
    private IEnumerator IERunSlots()
    {
        while (!ResultMN.Instance.HaveResultToRun())
            yield return null;

        NotificationPanel.Instance.Show(NotificationType.GOOD_LUCK);
        scatterCount = 0;
        isStopClicked = false;
        numberOfStopReel = 0;

        foreach (SlotReel reel in reels)
        {
            reel.FixSpinData();
            reel.Move(CountReelStop);
            
        }

        while (numberOfStopReel != GameMN.Instance.gameData.column)
            yield return new WaitForSeconds(0.1f);

        yield return new WaitForSeconds(0.1f);
        GameMN.Instance.SpinStop();
    }

    public static int GetSymbolCount()
    {
        return GameMN.Instance.gameData.symbols.Count;
    }

    public Symbol GetSymbol(int column, int row)
    {
        if (isStart)
        {
            return reels[column].pack2.symbols[row];
        }
        else
        {
            return reels[column].pack1.symbols[row];
        }
    }

    public void CountScatter()
    {
        scatterCount++;
        if (scatterCount > 5)
            scatterCount = 5;
        SoundMN.Instance.PlayOneShot(SFXType.ScatterSound);
    }

    public void CountWild()
    {
        wildCount++;
        if (wildCount > 5)
            wildCount = 5;
        SoundMN.Instance.PlayOneShot(SFXType.WildSound);
    }

    //public void SoundToWildScatterSpin()
    //{
    //    foreach (SlotReel reel in reels)
    //    {
    //        int row = GameMN.Instance.gameData.row;
    //        for (int i = row - 1; i >= 0; i--)
    //        {
    //            if (reel.pack1.symbols[i].data.symbol == scatterWildSpriteList[0]
    //                || reel.pack1.symbols[i].data.symbol == scatterWildSpriteList[1]
    //                || reel.pack1.symbols[i].data.symbol == scatterWildSpriteList[2])
    //            {
    //                SoundMN.Instance.PlayOneShot(SFXType.WildSound);
    //            }
    //            if (reel.pack1.symbols[i].data.symbol == scatterWildSpriteList[3])
    //            {
    //                SoundMN.Instance.PlayOneShot(SFXType.ScatterSound);
    //            }
    //        }
    //    }
    //}
}

[System.Serializable]
public class SpinData
{
    public int numberOfSpins = 10;
    public int spinsDelta = 4;
    public float maxSpeed = 5;

    public void ValueSetting(int numberOfSpins, int spinsDelta, float maxSpeed)
    {
        this.numberOfSpins = numberOfSpins;
        this.spinsDelta = spinsDelta;
        this.maxSpeed = maxSpeed;
    }
}