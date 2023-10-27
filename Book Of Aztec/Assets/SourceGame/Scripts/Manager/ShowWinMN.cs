using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ShowWinMN : Singleton<ShowWinMN>
{
    public float timeToShowOneWin = 3f;
    public int dataIndex = 0;

    public void ShowAllWin(bool isShow)
    {
        if (isShow)
        {
            dataIndex = 0;
            HideAllWin();

            for (int i = 0; i < ResultMN.Instance.winDatas.Count; i++)
            {
                WinData data = ResultMN.Instance.winDatas[i];
                if (!data.isBonus)
                {
                    LineWinning(data);
                }
            }
            
            Invoke("ShowOnceWin", timeToShowOneWin);
        }
        else
        {
            CancelInvoke("ShowOnceWin");
        }
    }
    private void HideBorder()
    {
        int column = GameMN.Instance.gameData.column;
        int row = GameMN.Instance.gameData.row;
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                Symbol symbol = SlotMN.Instance.GetSymbol(i, j);
                symbol.ShowBorder(false);
            }
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
                symbol.SetWinColor(Color.white);
                symbol.WinSetting(false);
                symbol.SetBlurIcon(false);
            }
        }
    }

    private void ShowOnceWin()
    {
        if (ResultMN.Instance.winDatas.Count < 1)
        {
            CancelInvoke("ShowOnceWin");
            return;
        }

        WinData data = ResultMN.Instance.winDatas[dataIndex];
        if (!data.isBonus)
        {
            HideBorder();
            HideAllWin();

            LineWinning(data);
            dataIndex++;
            if (dataIndex > ResultMN.Instance.winDatas.Count - 1)
                dataIndex = 0;

            if (!GameMN.Instance.autoPlayData.isAutoPlay)
            {
                NotificationPanel.Instance.Show(NotificationType.WIN);
                NotificationPanel.Instance.ShowWin(data);  
            }
            else
            {
                NotificationPanel.Instance.Show(NotificationType.TOTAL_LINE);
            }
        }
        
        Invoke("ShowOnceWin", timeToShowOneWin);
    }

    public void ShowOnceWinBonus(WinData data)
    {
        HideBorder();
        HideAllWin();

        WinningLine line = GameMN.Instance.gameData.winningLines[data.line];
        ShowLineMN.Instance.ShowLine(line, data.line, dataIndex);
        LineMN.Instance.ShowWin(data.line);
        for (int j = 0; j < line.positions.Count; j++)
        {
            Symbol symbol = SlotMN.Instance.GetSymbol(j, line.positions[j]);
            if (GameMN.Instance.gameData.symbols.FindIndex(x => x == symbol.data) == data.symbol)
            {
                symbol.SetWinColor(data.line);
                symbol.ShowBorder(true);
                symbol.ShowBonusAnim();
            }
        }
    }

    public void LineWinning(WinData data)
    {
        if (data.line != -1)
        {
            WinningLine line = GameMN.Instance.gameData.winningLines[data.line];
            ShowLineMN.Instance.ShowLine(line, data.line, dataIndex);
            LineMN.Instance.ShowWin(data.line);
            for (int j = 0; j < line.positions.Count; j++)
            {
                if (j <= data.symbolCount - 1)
                {
                    Symbol symbol = SlotMN.Instance.GetSymbol(j, line.positions[j]);
                    symbol.SetWinColor(data.line);
                    symbol.ShowBorder(true);
                    symbol.WinSetting(true);
                }
            }
        }
        else
        {
            ShowLineMN.Instance.HideAll();
            int column = GameMN.Instance.gameData.column;
            int row = GameMN.Instance.gameData.row;
            for (int i = 0; i < column; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    Symbol symbol = SlotMN.Instance.GetSymbol(i, j);
                    if (symbol.data.type == SymbolType.WILD)
                    {
                        symbol.SetWinColor(0);
                        symbol.ShowBorder(true);
                        SymbolData scatterData = GameMN.Instance.gameData.symbols.Find(x => x.type == SymbolType.SCATTER);
                        symbol.WinSettingData2(true, scatterData);
                    }
                }
            }
        }
    }
}
