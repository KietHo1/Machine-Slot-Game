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

    private bool isStart = true;

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

        if(!GameMN.Instance.isFreeSpin())
            NotificationPanel.Instance.Show(NotificationType.GOOD_LUCK);
        else
            NotificationPanel.Instance.ShowCountFreeSpin();

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
            return reels[column].pack2.symbols[row];
        else
            return reels[column].pack1.symbols[row];
    }
}

[System.Serializable]
public class SpinData
{
    public int numberOfSpins = 10;
    public int spinsDelta = 4;
    public float maxSpeed = 5;
}