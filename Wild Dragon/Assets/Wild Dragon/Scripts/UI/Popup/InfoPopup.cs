using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InfoPopup : Popups
{
    [Header("Paytable Page")]
    [SerializeField] List<SymbolInfo> symInfoList = new List<SymbolInfo>();
    
    [Header("Pages")]
    public List<GameObject> pageList = new List<GameObject>();
    public int pageIndex = 0; 

    [SerializeField] private Button afterInfoBtn;
    [SerializeField] private Button previousInfoBtn;
    float timeToShow = 0.4f;
    [SerializeField] private Text pageNumberTxt;
    
    public void Start() 
    {
        Init();
        afterInfoBtn.onClick.AddListener(delegate { SoundMN.Instance.PlayOneShot(SFXType.btn); ChangePageAfter(); });
        previousInfoBtn.onClick.AddListener(delegate { SoundMN.Instance.PlayOneShot(SFXType.btn); ChangePageBefore(); });
        pageNumberTxt.text = (pageIndex + 1).ToString();
    }

    public void Init()
    {
        PaytableSetting();
    }

    public override void ShowPopup(bool isShow = true)
    {
        base.ShowPopup(isShow);
        if (!isShow)
            return;
        //SoundMN.Instance.PlayOneShot(SFXType.INFO_START);
        //RefreshSymbolInfoRewards();
        ShowFirstPage();
    }

    public override void HidePopup()
    {
        base.HidePopup();
        SoundMN.Instance.PlayOneShot(SFXType.btn);
    }

    public void PaytableSetting()
    {
        for(int i = 0; i < GameMN.Instance.gameData.symbols.Count; i++)
        {
            SymbolData data = GameMN.Instance.gameData.symbols[i];
            SymbolInfo sym = symInfoList[i];
            sym.Setting(data);
            if (i == 1 || i == 2 || i == 6 || i == 8) continue;
            sym.ShowRewards();
        }
    }

    //private void RefreshSymbolInfoRewards()
    //{
    //    foreach(SymbolInfo sym in symInfoList)
    //    {
    //        sym.ShowRewards();
    //    }
    //}

    private void ShowFirstPage()
    {
        pageIndex = 0;
        ShowPage(pageIndex);
        pageNumberTxt.text = (pageIndex + 1).ToString() + " / 3";
    }

    private void ChangePageAfter()
    {
        pageIndex ++;

        if(pageIndex >= pageList.Count)
            pageIndex = 0;

        ShowPage(pageIndex);

        pageNumberTxt.text = (pageIndex + 1).ToString() + " / 3";

        //SoundMN.Instance.PlayOneShot(SFXType.MORE_INFO);
    }

    private void ChangePageBefore()
    {
        pageIndex--;

        if (pageIndex < 0)
            pageIndex = pageList.Count - 1;

        ShowPage(pageIndex);

        pageNumberTxt.text = (pageIndex + 1).ToString() + "/3";

        //SoundMN.Instance.PlayOneShot(SFXType.MORE_INFO);
    }

    private void ShowPage(int pageIndex)
    {
        Transform page = pageList[pageIndex].transform;
        page.SetSiblingIndex(page.parent.childCount - 1);
        RectTransform rect = page.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 300f);
        rect.DOAnchorPosY(0, 0.2f);
    }
}
