using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Events;

public class SlotMN : Singleton<SlotMN>
{
    public List<SlotReel> reels = new List<SlotReel>();
    public List<Transform> reelsParent = new List<Transform>();
    public SlotReel reelPrefab;

    public Transform topPos, bottomPos;
    public SpinData spinData;
    private int numberOfStopReel = 0;
    private int scatterCount = 0;
    private int wildCount = 0;


    void Start()
    {
        CreateReels();
    }

    private void CreateReels()
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
        //ShowLineMN.Instance.HideAllRewardText();
        //UIMN.Instance.ShowTopReward(false);

        //ResultMN.Instance.CheckBonus();

        StopCoroutine(IERunSlots());
        StartCoroutine(IERunSlots());

        //CreateActiveYellowPanel(ResultMN.Instance.isHaveBonus());
    }

    //private void Update() {
    //    if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
    //    {
    //        if(!isStopClicked)
    //        {
    //            foreach(SlotReel reel in reels)
    //                reel.ReelStoping();
    //            isStopClicked = true;
    //        }
    //    }
    //}

    bool isStopClicked = false;
    public void StopSpining()
    {
        if (!isStopClicked)
        {
            foreach (SlotReel reel in reels)
                reel.ReelStoping();
            isStopClicked = true;
        }
    }

    private IEnumerator IERunSlots()
    {
        while (!ResultMN.Instance.HaveResultToRun())
            yield return null;

        isStopClicked = false;
        numberOfStopReel = 0;
        NotificationPanel.Instance.Show(NotificationType.GOOD_LUCK);
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
        return reels[column].pack1.symbols[row];
    }

    public void CountScatter()
    {
        scatterCount++;
        SoundMN.Instance.PlayOneShot(SFXType.SCATTER_SOUND);
        if (scatterCount > 5)
            scatterCount = 5;
    }

    public void CountWild()
    {
        wildCount++;
        SoundMN.Instance.PlayOneShot(SFXType.WILD_SOUND);
        if (wildCount > 5)
            wildCount = 5;
    }
}

[System.Serializable]
public class SpinData
{
    public int numberOfSpins = 9;
    public int spinsDelta = 3;
    public float maxSpeed = 8;

    public void ValueSetting(int numberOfSpins, int spinsDelta, float maxSpeed)
    {
        this.numberOfSpins = numberOfSpins;
        this.spinsDelta = spinsDelta;
        this.maxSpeed = maxSpeed;
    }
}