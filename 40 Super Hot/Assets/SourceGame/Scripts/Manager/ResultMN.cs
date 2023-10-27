using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultMN : Singleton<ResultMN>
{
    public List<WinData> winDatas = new List<WinData>();
    public ReelResult[] reelResults = new ReelResult[0];
    public SymbolResult symbolResult = null;
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

    private void CreateResult()
    {
        winDatas = new List<WinData>();
        Ultility.ShuffleIntList(symbolOccurList);
        int column = GameMN.Instance.gameData.column;
        int row = GameMN.Instance.gameData.row;
        for (int i = 0; i < reelResults.Length; i++)
        {
            reelResults[i].CreateResult(row, symbolOccurList);
        }
        SymbolResult symResult = new SymbolResult(reelResults, row);
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
            if (reward > 0 || countScatter == 5)
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
                            if (GameMN.Instance.gameData.symbols[symbol].type == SymbolType.SCATTER || firstSymData.type == SymbolType.BONUS)
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

    private void CreateWindatas(int symbol, int matchCount, int line)
    {
        float reward = GameMN.Instance.gameData.symbols[symbol].rewards[matchCount - 1];

        bool haveBonus = false;
        SymbolData SymData = GameMN.Instance.gameData.symbols[symbol];
        if (SymData.type == SymbolType.BONUS)
        {
            if (matchCount == 5)
                haveBonus = true;
        }

        if (reward > 0 || haveBonus)
        {
            WinData data = new WinData();
            data.line = line;
            data.symbolCount = matchCount;
            data.symbol = symbol;
            data.lineReward = reward * GameMN.Instance.GetBet();
            winDatas.Add(data);
        }
    }

    public float GetLineReward()
    {
        float reward = 0f;
        foreach (WinData data in winDatas)
        {
            reward += data.lineReward;
        }
        return reward;
    }
}

public class SymbolResult
{
    public int[,] symbolResult;

    public SymbolResult()
    {

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
