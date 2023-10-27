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
        LineMN.Instance.LineSetting();
        SoundMN.Instance.PlayOneShot(SFXType.START_REEL);
        if (!GameMN.Instance.isBonusSpin())
            NotificationPanel.Instance.Show(NotificationType.GOOD_LUCK);
        else
            NotificationPanel.Instance.Show(NotificationType.BONUSSPIN_CURRENT);

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

    public bool isStopClicked = false;
    private IEnumerator IERunSlots()
    {
        while (!ResultMN.Instance.HaveResultToRun())
            yield return null;

        BetLineSliderMN.Instance.BetLineSetting(true);
        SettingPanel.Instance.SettingPanelSetting(false);

        wildCount = 0;
        isStopClicked = false;
        numberOfStopReel = 0;
        GameMN.Instance.bonusSpinData.expandingWin = 0;

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
        wildCount++;
        if (wildCount > 5)
            wildCount = 5;
        SoundMN.Instance.PlayOneShot("STOP_REEL_WILD_" + wildCount);
    }
}

[System.Serializable]
public class SpinData
{
    public int numberOfSpins = 10;
    public int spinsDelta = 4;
    public float maxSpeed = 5;
}