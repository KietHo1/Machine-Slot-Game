using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ultility
{
    public static void ShuffleIntArray(int[] list)
    {
        var count = list.Length;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    public static bool isWin(int occur)
    {
        if(occur == 0) 
            return false;

        List<int> list = new List<int>();
        for(int i=0 ; i < 100; i++)
        {
            if(i <= occur)
                list.Add(1);
            else
                list.Add(0);
        }
        ShuffleIntList(list);
        return list[0] == 1 ? true : false;
    }

    public static void ShuffleIntList(List<int> list)
    {
        var count = list.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    public static int GetResultRandom(int occurLengt, int[] occur)
    {
        int[] array = new int[100]; 
        int index = 0;
        for(int i = 0; i < occurLengt; i++)
        {
            for(int j = 0; j < occur[i]; j++)
            {
                array[index] = i;
                index ++;
            }
        }

        ShuffleIntArray(array);
        return array[0];
    }

    public static List<int> CreateSymbolOccurList(List<SymbolData> symbols)
    {
        List<int> symbolOccurList = new List<int>();
        for(int i = 0; i < symbols.Count; i++)
        {
            SymbolData data = symbols[i];
            for(int j = 0; j < data.appearOccur; j++)
            {
                symbolOccurList.Add(i);
            }
        }
        return symbolOccurList;
    }

    public static IEnumerator NumberRun(float start, float end, Text txt)
    {
        float delta = end - start;
        float startValue = start;
        float time = 20;
        for(int i = 0 ; i < time; i++)
        {
            startValue += (delta / time);
            txt.text = GetMoneyFormated(startValue);
            yield return new WaitForSeconds(0.001f);
        }

        startValue = end;
        txt.text = GetMoneyFormated(startValue);
    }

    public static IEnumerator NumberRunFollowTime(float start, float end, Text txt, float time)
    {
        float delta = end - start;
        float startValue = start;
        for (int i = 0; i < time; i++)
        {
            startValue += (delta / time);
            txt.text = GetMoneyFormated(startValue);
            yield return new WaitForSeconds(0.001f);
        }

        startValue = end;
        txt.text = GetMoneyFormated(startValue);
    }

    public static string GetMoneyFormated(float money)
    {
        return GameMN.Instance.currency[(int)GameMN.Instance.gameData.currency] + money.ToString("0.00");
    }
}
