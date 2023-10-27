using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopup : Popups
{
    [SerializeField] private GUIButton exitBtn, nextBtn, previousBtn, paginationBtn;
    [SerializeField] public Text numberPageTxt;

    [Header("Paytable Page")]
    [SerializeField] List<SymbolInfo> symInfoList = new List<SymbolInfo>();

    [Header("WinningLines Page")]
    [SerializeField] private LineInfo lineInfoPrefab;
    [SerializeField] private Transform lineInfoHolder;

    [Header("Pages")]
    public List<GameObject> pageList = new List<GameObject>();
    public int pageIndex = 0;

    private void Start()
    {
        Init();
    }
    public override void Awake()
    {
        nextBtn.SetEvent(delegate { NextPage(); SoundMN.Instance.PlayOneShot(SFXType.CLICK); });
        paginationBtn.SetEvent(delegate { NextPage(); SoundMN.Instance.PlayOneShot(SFXType.CLICK); });
        previousBtn.SetEvent(delegate { PreviousPage(); SoundMN.Instance.PlayOneShot(SFXType.CLICK); });
        exitBtn.SetEvent(delegate { HidePopup(); SoundMN.Instance.PlayOneShot(SFXType.CLICK); });
    }
    private void Init()
    {
        PaytableSetting();
        //WinningLinesSetting();
    }

    public virtual void ChangePageSetting()
    {

    }

    public override void ShowPopup(bool isShow = true)
    {
        base.ShowPopup(isShow);
        if (!isShow) return;
        ShowFirstPage();
        ShowPage();
        //SpecialRewardsSetting();
        RefreshSymbolInfoRewards();
    }
    public virtual void ShowFirstPage()
    {
        pageIndex = 0;
        numberPageTxt.text = (pageIndex + 1).ToString() + "/" + pageList.Count;
    }
    
    public void WinningLinesSetting()
    {
        for (int i = 0; i < GameMN.Instance.gameData.winningLines.Count; i++)
        {
            WinningLine data = GameMN.Instance.gameData.winningLines[i];
            LineInfo line = Instantiate(lineInfoPrefab, lineInfoHolder);
            line.Setting(i + 1, data.sprite);
        }
    }

    //private void SpecialRewardsSetting()
    //{
    //    specialRewardTxt.text = "3.     " + Ultility.GetMoneyFormated(GameMN.Instance.GetTotalBet());
    //}

    public void PaytableSetting()
    {
        for (int i = 0; i < symInfoList.Count; i++)
        {
            SymbolData data = GameMN.Instance.gameData.symbols[i];
            if (i == 3 || i == 7 || i == 8) continue;
            SymbolInfo sym = symInfoList[i];
            sym.Setting(data);
            sym.ShowRewards();
        }
    }

    private void RefreshSymbolInfoRewards()
    {
        foreach (SymbolInfo sym in symInfoList)
        {
            sym.ShowRewards();
        }
    }

    public void ChangePageIndex(int index)
    {
        pageIndex = index;
        numberPageTxt.text = (pageIndex + 1).ToString() + "/" + pageList.Count;
        ShowPage();
    }

    private void ShowPage()
    {
        HideAllPage();
        pageList[pageIndex].SetActive(true);
    }

    private void HideAllPage()
    {
        foreach (GameObject page in pageList)
        {
            page.SetActive(false);
        }
    }
    public void NextPage()
    {
        if (pageIndex < pageList.Count - 1)
        {
            pageIndex++;
        }
        else
        {
            pageIndex = 0;
        }
        ChangePageIndex(pageIndex);
    }
    public void PreviousPage()
    {
        if (pageIndex > 0)
        {
            pageIndex--;
        }
        else
        {
            TotalPageNumber();
        }
        ChangePageIndex(pageIndex);
    }
    public void TotalPageNumber()
    {
        pageIndex = pageList.Count - 1;
    }
}
