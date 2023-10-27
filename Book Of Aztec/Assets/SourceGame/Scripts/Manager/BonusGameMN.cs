  using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BonusGameMN : Singleton<BonusGameMN>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Action endBonus;
    [SerializeField] private Image[] symImgs = new Image[8];
    [SerializeField] private Sprite[] chooseSyms = new Sprite[8];
    [SerializeField] private Sprite[] choosenSyms = new Sprite[8];
    [SerializeField] private int[] ignoreSyms = new int[0];
    [SerializeField] private List<int> symIndexList = new List<int>();
    private int symbolChoosen = 0;
    private int currentSymbolIndex = 0;
    private int stepCount = 0;
    private List<int> symbolOccurList = new List<int>();

    private void Start()
    {
        CreateSymbolOccurList();
    }

    private void CreateSymbolOccurList()
    {
        symbolOccurList = Ultility.CreateSymbolOccurList(GameMN.Instance.gameData.symbols, ignoreSyms);
    }

    public void Show(bool isShow, Action endBonus = null)
    {
        panel.SetActive(isShow);
        this.endBonus = endBonus;
        if(isShow)
            StartCoroutine(PlayGame());
    }

    IEnumerator PlayGame()
    {
        ShowChooseSymbol(0);
        Ultility.ShuffleIntList(symbolOccurList);

        symbolChoosen = symbolOccurList[0];
        int index = symIndexList.FindIndex(x => x == symbolChoosen);
        stepCount = symImgs.Length * 3 + index;

        yield return new WaitForSeconds(2f);
        for (int i = 0; i < stepCount; i ++)
        {
            
            currentSymbolIndex++;
            if (currentSymbolIndex > symImgs.Length - 1)
                currentSymbolIndex = 0;

            ShowChooseSymbol(currentSymbolIndex);
            SoundMN.Instance.PlayOneShot(SFXType.FREE_SPIN_TICK);
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(1f);
        ShowFinalSymbol(currentSymbolIndex);
        SoundMN.Instance.PlayOneShot(SFXType.FREE_SPIN_TICK_DONE);

        yield return new WaitForSeconds(1f);
        EndGame();
    }

    private void HideImage()
    {
        foreach (Image img in symImgs)
        {
            img.gameObject.SetActive(false);
        }
    }

    private void ShowChooseSymbol(int index)
    {
        HideImage();
        symImgs[index].sprite = chooseSyms[index];
        symImgs[index].gameObject.SetActive(true);
    }
    
    private void ShowFinalSymbol(int index)
    {
        HideImage();
        symImgs[index].sprite = choosenSyms[index];
        symImgs[index].gameObject.SetActive(true);
    }

    private void EndGame()
    {
        endBonus?.Invoke();
    }

    public int GetChoosenSymbol()
    {
        return symbolChoosen;
    }
}
