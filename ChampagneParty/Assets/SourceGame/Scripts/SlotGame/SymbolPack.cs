using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolPack : MonoBehaviour
{
    public List<Symbol> symbols = new List<Symbol>();

    public void Init(float _height, Symbol symbolPrefab)
    {
        int row = GameMN.Instance.gameData.row;
        transform.localPosition = new Vector3(transform.localPosition.x, _height * ((float)row / 2) / 100, transform.localPosition.z);
        
        float height = Math.Abs(-_height);
        float minPosY = -height;
        for(int i = row - 1; i >= 0; i--)
        {
            Symbol s = Instantiate(symbolPrefab, transform);
            float posY = minPosY + (height * i);
            s.transform.localPosition = new Vector3(s.transform.localPosition.x, posY / 100f, s.transform.localPosition.z);
            s.gameObject.name = "Symbol_0" + i; 
            symbols.Insert(0, s);
        }
    }

    public void SetPos(float posY)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, posY, transform.localPosition.z);
    }

    public virtual void SetRandomIcon()
    {
        int rand = GetRandom(0, SlotMN.GetSymbolCount());
        for(int i = 0; i < symbols.Count ; i++)
        {
            SymbolData symbolData = GameMN.Instance.gameData.symbols[rand];
            symbols[i].Setting(symbolData);
        }
    }

    public virtual void SetFinalIcon(int reelIndex)
    {
        int row = GameMN.Instance.gameData.row;
        for(int i = 0 ; i < row; i++)
        {
            int symbol = ResultMN.Instance.symbolResult.GetSymbol(reelIndex, i);
            SymbolData data = GameMN.Instance.gameData.symbols[symbol];
            symbols[i].Setting(data);
            symbols[i].SetBlurIcon(true);
        }
    }

    public void SetLayer(int layer)
    {
        foreach(Symbol symbol in symbols)
        {
            symbol.SetLayer(layer);
        }
    }

    public void SetBlurIcon(bool isBlur)
    {
        foreach(Symbol symbol in symbols)
        {
            symbol.SetBlurIcon(isBlur);
        }
    }

    public int GetRandom(int min ,int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public void CheckFinalSymbol()
    {
        Symbol sScatter = symbols.Find(x => x.data.type == SymbolType.SCATTER);
        if(sScatter != null)
        {
            //SlotMN.Instance.CountScatter();
            return;
        }

        SoundMN.Instance.PlayOneShot(SFXType.REEL_STOP);
    }
}
