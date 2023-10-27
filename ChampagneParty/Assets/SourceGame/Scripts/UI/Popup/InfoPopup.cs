using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InfoPopup : Popups
{
    [Header("Paytable Page")]
    [SerializeField] List<SymbolInfo> symInfoList = new List<SymbolInfo>();
    
    [Header("WinningLines Page")]
    [SerializeField] private LineInfo lineInfoPrefab;
    //[SerializeField] private Transform lineInfoHolder;

    [Header("Pages")]
    public List<GameObject> pageList = new List<GameObject>();
    public int pageIndex = 0;

    [SerializeField] private Button moreInfoBtn;
    [SerializeField] private GUIButton closePaytableBtn;

    private void Start() 
    {
        Init();
        moreInfoBtn.onClick.AddListener(ChangePage);
        closePaytableBtn.SetEvent(HidePopup);
    }

    private void Init()
    {
        PaytableSetting();
        //WinningLinesSetting();
    }

    public override void ShowPopup(bool isShow = true)
    {
        base.ShowPopup(isShow);
        closePaytableBtn.Show(true);
        if(!isShow) return;
        RefreshSymbolInfoRewards();
        ShowFirstPage();
    }

    public override void HidePopup()
    {
        base.HidePopup();
        closePaytableBtn.Show(false);
        UIMN.Instance.moreinfoBtn.Show(false);
        //SoundMN.Instance.PlayOneShot(SFXType.INFO_END);
    }

    private void RefreshSymbolInfoRewards()
    {
        foreach (SymbolInfo sym in symInfoList)
        {
            sym.ShowRewards();
        }
    }

    public virtual void ShowFirstPage()
    {
        pageIndex = 0;
        ShowPage(pageIndex);
    }

    private void ChangePage()
    {
        pageIndex++;

        if (pageIndex >= pageList.Count)
            pageIndex = 0;

        ShowPage(pageIndex);
    }

    private void ShowPage(int pageIndex)
    {
        SoundMN.Instance.PlayOneShot(SFXType.CHANGE_PAGE);
        Transform page = pageList[pageIndex].transform;
        page.SetSiblingIndex(page.parent.childCount - 1);
        RectTransform rect = page.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 300f);
        rect.DOAnchorPosY(0, 0.2f);
    }

    //public void WinningLinesSetting()
    //{
    //    for(int i = 0; i < GameMN.Instance.gameData.winningLines.Count; i++)
    //    {
    //        WinningLine data = GameMN.Instance.gameData.winningLines[i];
    //        //LineInfo line = Instantiate(lineInfoPrefab, lineInfoHolder);
    //        //line.Setting(i + 1, data.sprite);
    //    }
    //}

    public void PaytableSetting()
    {
        for(int i = 0; i < GameMN.Instance.gameData.symbols.Count; i++)
        {
            if(i != 11)
            {
                SymbolData data = GameMN.Instance.gameData.symbols[i];
                SymbolInfo sym = symInfoList[i];
                sym.Setting(data);
                sym.ShowRewards();
            }
        }
    }

    //private void HideAllPage(){
    //    foreach(GameObject page in pageList)
    //    {
    //        page.SetActive(false);
    //    }
    //}
}
