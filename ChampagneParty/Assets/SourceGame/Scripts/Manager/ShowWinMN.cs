using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowWinMN : Singleton<ShowWinMN>
{
    public float timeToShowOneWin = 3f;
    public int dataIndex = 0;
    public GameObject fog;

    private void ShowFog(bool isShow = true)
    {
        //fog.SetActive(isShow);
    }
    public void ShowAllWin(bool isShow)
    {
        if (isShow)
        {
            GambleGameMN.Instance.played = true;
            dataIndex = 0;
            HideAllWin();
            for (int i = 0; i < ResultMN.Instance.winDatas.Count; i++)
            {
                WinData data = ResultMN.Instance.winDatas[i];
                bool isShowEach = ResultMN.Instance.winDatas.Count >= 2 ? false : true;
                LineWinning(data, i, isShowEach);
                ShowFog();
            }
            Invoke("ShowOnceWin", timeToShowOneWin);
        }
        else
        {
            CancelInvoke("ShowOnceWin");
        }
    }

    public void HideAllWin()
    {
        int column = GameMN.Instance.gameData.column;
        int row = GameMN.Instance.gameData.row;
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                Symbol symbol = SlotMN.Instance.GetSymbol(i, j);
                symbol.WinSetting(false, Color.white);
                symbol.SetBlurIcon(false);
            }
        }
        ShowFog(false);
    }

    public void ShowOnceWin()
    {
        if (ResultMN.Instance.winDatas.Count < 1)
        {
            CancelInvoke("ShowOnceWin");
            return;
        }

        HideAllWin();
        WinData data = ResultMN.Instance.winDatas[dataIndex];
        LineWinning(data, dataIndex, isShowEach : true);
        dataIndex++;
        if (dataIndex > ResultMN.Instance.winDatas.Count - 1)
            dataIndex = 0;

        NotificationPanel.Instance.Show(NotificationType.WIN);
        NotificationPanel.Instance.ShowWin(data);

        if (GambleGameMN.Instance.played)
        {
            SoundMN.Instance.PlayOneShot(SFXType.WIN);
        }

        ShowFog();
        Invoke("ShowOnceWin", timeToShowOneWin);
    }

    public void LineWinning(WinData data, int index, bool isShowEach)
    {
        if (data.line >= 0)
        {
            WinningLine line = GameMN.Instance.gameData.winningLines[data.line];
            ShowLineMN.Instance.ShowPreviewLine(data.line);
            for (int j = 0; j < line.positions.Count; j++)
            {
                if (j <= data.symbolCount - 1)
                {
                    Symbol symbol = SlotMN.Instance.GetSymbol(j, line.positions[j]);
                    symbol.WinSetting(true, ShowLineMN.Instance.GetColor(data.line));
                }
            }
        }
        else 
        {
            int column = GameMN.Instance.gameData.column;
            int row = GameMN.Instance.gameData.row;
            ShowLineMN.Instance.HideAll();
            for (int i = 0; i < column; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    Symbol symbol = SlotMN.Instance.GetSymbol(i, j);
                    if (symbol.data.type == SymbolType.SCATTER || (symbol.data.type == SymbolType.WILD))
                    {
                        symbol.WinSetting(true, Color.yellow);
                    }
                }
            }
        }
    }
}
