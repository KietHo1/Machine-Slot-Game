using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/GameData")]
public class GameData : ScriptableObject
{
    public string gameName;
    public int column;
    public int row;
    public float betMultipler = 1f;
    public int bonusWinOccur = 50;
    public List<SymbolData> symbols = new List<SymbolData>();
    public List<WinningLine> winningLines = new List<WinningLine>();
    public Currency currency = Currency.DOLLAR;
}

[System.Serializable]
public class SymbolData
{
    public SymbolType type = SymbolType.NORMAL;
    public Sprite symbol, blurSymbol;
    public List<Sprite> anims = new List<Sprite>();
    public List<float> rewards = new List<float>();
    public float appearOccur = 0;
}

[System.Serializable]
public class WinningLine
{
    public List<int> positions = new List<int>();
    public Sprite sprite;
}

[System.Serializable]
public class Combo
{
    public int[] array = new int[5];
}

public enum SymbolType
{
    NORMAL, BONUS, FREESPIN, SCATTER, JACKPOT, WILD
}

public enum Currency
{
    DOLLAR, EURO, LIRA
}
