using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultMN : Singleton<ResultMN>
{
    public SymbolResult symbolResult = null;
    private List<int> symbolOccurList = new List<int>();
    public List<WinData> winDatas = new List<WinData>();
    public bool isLineWin;
    public bool isHaveBonusGame = false;
    private int createWinningMaxCount = 500;

    public void CreateSymbolOccurList()
    {
        symbolOccurList = Ultility.CreateSymbolOccurList(GameMN.Instance.gameData.symbols);
    }

    public bool HaveResultToRun()
    {
        float lineWinOccur = (float)GameSetting.percent * ((float)GameMN.Instance.GetLine() / (float)GameMN.Instance.gameData.winningLines.Count);
        isLineWin = Ultility.isWin(Mathf.RoundToInt(lineWinOccur));

        int count = 0;
        while (isLineWin != IsHaveLineWin() && count <= createWinningMaxCount)
        {
            count++;
            if (count > createWinningMaxCount)
            {
                isLineWin = false;
                count = 0;
            }
        }

        return true;
    }

    private void CreateResult()
    {
        winDatas = new List<WinData>();
        Ultility.ShuffleIntList(symbolOccurList);
        int column = GameMN.Instance.gameData.column;
        int row = GameMN.Instance.gameData.row;
        SymbolResult symResult = new SymbolResult(column, row, symbolOccurList);
        symbolResult = symResult;
    }

    private bool IsHaveLineWin()
    {

        CreateResult();

        List<Combo> combos = new List<Combo>();
        for (int i = 0; i < GameMN.Instance.GetLine(); i++)
        {
            if (i >= GameMN.Instance.GetLine())
                break;

            WinningLine line = GameMN.Instance.gameData.winningLines[i];
            Combo combo = new Combo();
            for (int j = 0; j < line.positions.Count; j++)
            {
                int symbol = symbolResult.GetSymbol(j, line.positions[j]);
                combo.array[j] = symbol;
            }

            combos.Add(combo);
        }
        CheckComboCreateWinData(combos);
        CheckScatter();

        if (isLineWin)
        {
            float reward = 0f;
            foreach (WinData data in winDatas)
                reward += data.lineReward;

            if (reward > GameSetting.max_win)
                return false;
        }

        return winDatas.Count > 0;
    }

    private void CheckScatter()
    {
        int column = GameMN.Instance.gameData.column;
        int row = GameMN.Instance.gameData.row;
        int countScatter = 0;
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                int sym = symbolResult.GetSymbol(i, j);
                SymbolData SymData = GameMN.Instance.gameData.symbols[sym];
                if (SymData.type == SymbolType.SCATTER)
                    countScatter++;
            }
        }

        if (countScatter == 0)
            return;

        SymbolData scatterSymData = GameMN.Instance.gameData.symbols.Find(x => x.type == SymbolType.SCATTER);
        int symIndex = GameMN.Instance.gameData.symbols.FindIndex(x => x.type == SymbolType.SCATTER);
        if (countScatter > scatterSymData.rewards.Count)
            countScatter = scatterSymData.rewards.Count;

        if (scatterSymData != null)
        {
            float reward = scatterSymData.rewards[countScatter - 1];
            if (reward > 0)
            {
                WinData data = new WinData();
                data.line = -1;
                data.symbolCount = countScatter;
                data.symbol = symIndex;
                data.lineReward = reward * GameMN.Instance.GetBet();
                winDatas.Add(data);
            }
        }
    }

    private void CheckComboCreateWinData(List<Combo> combos)
    {
        for (int i = 0; i < combos.Count; i++)
        {
            int firstSymbol = 0;
            int matchCount = 0;

            float wildReward = 0f;
            int wildMatchCount = 0;

            int firstSym = combos[i].array[0];
            SymbolData firstSymData = GameMN.Instance.gameData.symbols[firstSym];

            for (int j = 0; j < combos[i].array.Length; j++)
            {
                int symbol = combos[i].array[j];
                if (firstSymData.type == SymbolType.SCATTER)
                {
                    matchCount = 1;
                    break;
                }

                if (matchCount == 0)
                {
                    firstSymbol = symbol;
                    matchCount++;
                }
                else
                {
                    if (firstSymbol == symbol)
                    {
                        matchCount++;
                        if (matchCount >= 3 && firstSymData.type == SymbolType.WILD)
                        {
                            wildMatchCount = matchCount;
                            wildReward = GameMN.Instance.gameData.symbols[firstSymbol].rewards[matchCount - 1];
                        }
                    }
                    else
                    {
                        firstSymData = GameMN.Instance.gameData.symbols[firstSymbol];
                        SymbolData nextSymData = GameMN.Instance.gameData.symbols[symbol];
                        if (firstSymData.type == SymbolType.WILD)
                        {
                            if (GameMN.Instance.gameData.symbols[symbol].type == SymbolType.SCATTER)
                                break;

                            matchCount++;
                            firstSymbol = symbol;
                        }
                        else if (nextSymData.type == SymbolType.WILD)
                        {
                            matchCount++;
                        }
                        else
                            break;
                    }
                }
            }

            if (wildReward > GameMN.Instance.gameData.symbols[firstSymbol].rewards[matchCount - 1])
            {
                matchCount = wildMatchCount;
                firstSymbol = GameMN.Instance.gameData.symbols.FindIndex(x => x.type == SymbolType.WILD);
            }

            CreateWindatas(firstSymbol, matchCount, i);
        }
    }

    private void CreateWindatas(int firstSymbol, int matchCount, int line)
    {
        float reward = GameMN.Instance.gameData.symbols[firstSymbol].rewards[matchCount - 1];
        if (reward > 0)
        {
            WinData data = new WinData();
            data.line = line;
            data.symbolCount = matchCount;
            data.symbol = firstSymbol;
            data.lineReward = reward * GameMN.Instance.GetBet();
            winDatas.Add(data);
        }
    }

    //private bool isDoubleReward(int symbol, int isHaveWild)
    //{
    //    return GameMN.Instance.gameData.symbols[symbol].type != SymbolType.WILD && isHaveWild == 1;
    //}

    public float GetLineReward()
    {
        float reward = 0f;
        foreach (WinData data in winDatas)
        {
            reward += data.lineReward;
        }
        return reward;
    }

    public void CheckBonus()
    {
        isHaveBonusGame = Ultility.isWin(GameMN.Instance.gameData.winOccur.bonus);
    }

    public bool isHaveBonus()
    {
        return isHaveBonusGame;
    }
}
