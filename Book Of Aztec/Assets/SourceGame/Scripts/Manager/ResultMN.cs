using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultMN : Singleton<ResultMN>
{
    public List<WinData> winDatas = new List<WinData>();
    public ReelResult[] reelResults = new ReelResult[0];
    public SymbolResult symbolResult = null;
    public SymbolResult symbolResultBonus = null;
    private List<int> symbolOccurList = new List<int>();
    private bool isLineWin;
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

    private bool IsHaveLineWin()
    {

        CreateResult();

        List<Combo> comboList = new List<Combo>();
        for (int i = 0; i < GameMN.Instance.GetLine(); i++)
        {
            WinningLine line = GameMN.Instance.gameData.winningLines[i];
            Combo combo = new Combo();
            for (int j = 0; j < line.positions.Count; j++)
            {
                int symbol = symbolResult.GetSymbol(j, line.positions[j]);
                combo.array[j] = symbol;
            }

            comboList.Add(combo);
        }
        CheckComboCreateWinData(comboList);
        CheckScatter();

        if(GameMN.Instance.isBonusSpin())
            CreateBonusResult();

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

    private void CreateResult()
    {
        winDatas = new List<WinData>();
        Ultility.ShuffleIntList(symbolOccurList);
        int column = GameMN.Instance.gameData.column;
        int row = GameMN.Instance.gameData.row;
        for (int i = 0; i < column; i++)
        {
            reelResults[i].CreateResult(row, symbolOccurList);
        }
        SymbolResult symResult = new SymbolResult(reelResults, row);
        symbolResult = symResult;
    }

    public void changeSymbol(int line, int indexSymbol, bool isChange = false, int row = 0)
    {
        if (isChange)
        {
            int row_ = GameMN.Instance.gameData.row;
            for (int i = 0; i < row_; i++)
            {
                symbolResultBonus.SetSymbol(line, i, indexSymbol);
            }
        }
        else
            symbolResultBonus.SetSymbol(line, row, indexSymbol);
    }

    private void CreateBonusResult()
    {
        
        int column = GameMN.Instance.gameData.column;
        int row = GameMN.Instance.gameData.row;

        symbolResultBonus = new SymbolResult(column, row);

        for (int i = 0; i < column; i++)
        {
            bool isHaveBonusSymbol = false;
            for (int j = row - 1; j >= 0; j--)
            {
                int sym = symbolResult.GetSymbol(i, j);

                if (sym == BonusGameMN.Instance.GetChoosenSymbol())
                    isHaveBonusSymbol = true;

                if (isHaveBonusSymbol)
                    changeSymbol(i, BonusGameMN.Instance.GetChoosenSymbol(), true, sym);
                else
                    changeSymbol(i, sym, false, j);
            }
        }

        List<Combo> comboList = new List<Combo>();
        for (int i = 0; i < GameMN.Instance.GetLine(); i++)
        {
            WinningLine line = GameMN.Instance.gameData.winningLines[i];
            Combo combo = new Combo();

            int bonusCount = 0;
            for (int j = 0; j < line.positions.Count; j++)
            {
                int symbol = symbolResultBonus.GetSymbol(j, line.positions[j]);
                if(symbol == BonusGameMN.Instance.GetChoosenSymbol())
                {
                    combo.array[bonusCount] = symbol;
                    bonusCount++;
                }
            }

            combo.bonusCount = bonusCount;
            comboList.Add(combo);
        }

        for(int i = 0; i < comboList.Count; i++)
        {
            int symbol = comboList[i].array[0];
            int matchCount = comboList[i].bonusCount;

            if (matchCount == 0)
                continue;

            float reward = GameMN.Instance.gameData.symbols[symbol].rewards[matchCount - 1];
            if (reward > 0)
            {
                WinData data = new WinData();
                data.line = i;
                data.symbolCount = matchCount;
                data.symbol = symbol;
                data.lineReward = reward * GameMN.Instance.GetBet();
                data.isBonus = true;
                winDatas.Add(data);
            }
        }
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
                if (SymData.type == SymbolType.WILD)
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

    private void CheckComboCreateWinData(List<Combo> comboList)
    {
        for (int i = 0; i < comboList.Count; i++)
        {
            int firstSymbol = 0;
            int matchCount = 0;

            float wildReward = 0f;
            int wildMatchCount = 0;

            int firstSym = comboList[i].array[0];
            SymbolData firstSymData = GameMN.Instance.gameData.symbols[firstSym];

            for (int j = 0; j < comboList[i].array.Length; j++)
            {
                int symbol = comboList[i].array[j];

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

    public float GetLineReward()
    {
        float reward = 0f;
        foreach (WinData data in winDatas)
        {
            if (!data.isBonus)
                reward += data.lineReward;
        }
        return reward;
    }

    public float GetBonusReward()
    {
        float reward = 0f;
        foreach (WinData data in winDatas)
        {
            if (data.isBonus)
                reward += data.lineReward;
        }
        return reward;
    }

    public int GetSymbolHighestReward()
    {
        float reward = 0f;
        int symbol = 0;
        for (int i = 0; i < winDatas.Count; i++)
        {
            if (winDatas[i].lineReward > reward)
            {
                reward = winDatas[i].lineReward;
                symbol = winDatas[i].symbol;
            }
        }
        return symbol;
    }

    public bool CheckDuplicateSymbolWithBonus(int column, int row)
    {
        if (symbolResult.GetSymbol(column, row) == symbolResultBonus.GetSymbol(column, row))
            return true;

        return false;
    }
}

public class SymbolResult
{
    public int[,] symbolResult;

    public SymbolResult(int column, int row)
    {
        symbolResult = new int[column, row];
    }

    public SymbolResult(ReelResult[] reelResults, int row)
    {
        int symbolIndex = 0;
        symbolResult = new int[reelResults.Length, row];
        for (int i = 0; i < reelResults.Length; i++)
        {
            for (int j = 0; j < row; j++)
            {
                symbolResult[i, j] = reelResults[i].GetResult(j);
                symbolIndex++;
            }
        }
    }

    public void Debugs(int column, int row)
    {
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                Debug.Log("Column: " + i + "_" + "row: " + j + "_" + "Symbol: " + symbolResult[i, j]);
            }
        }
    }

    public int GetSymbol(int column, int row)
    {
        return symbolResult[column, row];
    }

    public void SetSymbol(int column, int row, int symbol)
    {
        symbolResult[column, row] = symbol;
    }
}


[System.Serializable]
public class ReelResult
{
    public ReelCondition[] reelConditions = new ReelCondition[0];
    private int[] symbolResult = new int[0];

    public void CreateResult(int row, List<int> symbolOccurList)
    {
        while (!isValid(row, symbolOccurList))
        {

        }
    }

    public bool isValid(int row, List<int> symbolOccurList)
    {
        symbolResult = new int[row];
        int startIndex = UnityEngine.Random.Range(0, symbolOccurList.Count - row);
        for (int i = 0; i < row; i++)
        {
            symbolResult[i] = symbolOccurList[startIndex + i];
        }

        for (int i = 0; i < reelConditions.Length; i++)
        {
            if (!reelConditions[i].CheckPass(symbolResult))
                return false;
        }

        return true;
    }

    public int GetResult(int index)
    {
        return symbolResult[index];
    }
}

[System.Serializable]
public class ReelCondition
{
    public ReelConditionType reelConditionType;
    public SymbolType symbolType = SymbolType.NORMAL;

    public bool CheckPass(int[] symbolResult)
    {
        if (reelConditionType == ReelConditionType.APPEAR_ONE_SYMBOL)
            return PassAppearOne(symbolResult);

        if (reelConditionType == ReelConditionType.IGNORE_SYMBOL)
            return PassIgnoreSymbol(symbolResult);

        return true;
    }

    public bool PassIgnoreSymbol(int[] symbolResult)
    {
        for (int i = 0; i < symbolResult.Length; i++)
        {
            if (GameMN.Instance.gameData.symbols[symbolResult[i]].type == symbolType)
                return false;
        }

        return true;
    }

    public bool PassAppearOne(int[] symbolResult)
    {
        int count = 0;
        for (int i = 0; i < symbolResult.Length; i++)
        {
            if (GameMN.Instance.gameData.symbols[symbolResult[i]].type == symbolType)
                count++;

            if (count == 2)
                return false;
        }

        return true;
    }
}

public enum ReelConditionType
{
    APPEAR_ONE_SYMBOL, IGNORE_SYMBOL
}
