using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Symbol : MonoBehaviour
{
    public SymbolData data;
    [SerializeField] private SpriteRenderer animSR;
    [SerializeField] private GameObject border;
    [SerializeField] private int framePerSpr = 1;
    private SpriteRenderer sR;
    private bool isShowWin = false;
    private int animIndex = 0;
    private Color borderColor = Color.white;
    int currFrame; 

    private SymbolData data2;
    private bool useBaseAnim = true;

    private void Awake()
    {
        sR = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SetBlurIcon(false);
    }

    private void Update()
    {
        if (!isShowWin || data.anims.Count == 0) return;

        currFrame++;
        if (currFrame < framePerSpr) return;
        currFrame = 0;

        if (useBaseAnim)
            ShowWinningSpriteAnim();
        else
            ShowWinningSpriteAnimData2();
    }

    public void Setting(SymbolData data)
    {
        this.data = data;
    }

    public void SetBlurIcon(bool blur)
    {
        ShowBorder(false);
        sR.sprite = blur ? data.blurSymbol : data.symbol;
        if (isBonusSymbol())
            sR.sprite = data.bonusSymbol;
        isShowWin = false;
        sR.enabled = true;
        animSR.gameObject.SetActive(false);

    }

    private bool isBonusSymbol()
    {
        if (!GameMN.Instance.isBonusSpin())
            return false;

        return GameMN.Instance.gameData.symbols.FindIndex(x => x == data) == BonusGameMN.Instance.GetChoosenSymbol();
    }
    public void SetWinColor(Color color)
    {
        this.borderColor = color;
    }

    public void SetWinColor(int lineIndex)
    {
        Color color1 = LineMN.Instance.GetLineColor(lineIndex);
        //Color color2 = LineMN.Instance.GetLineHighlightColor(lineIndex);
        this.borderColor = color1;
        Highlight highlight = border.GetComponent<Highlight>();
        if (highlight != null)
            highlight.SetColor(color1/*, color2*/);
    }

    public void WinSetting(bool isShow)
    {
        this.isShowWin = isShow;
        animIndex = 0;
        useBaseAnim = true;
    }

    public void WinSettingData2(bool isShow, SymbolData symbolData)
    {
        data2 = symbolData;
        useBaseAnim = false;

        this.isShowWin = isShow;
        animIndex = 0;
    }

    private void ShowWinningSpriteAnimData2()
    {
        animSR.gameObject.SetActive(true);
        animSR.sprite = data2.anims[animIndex];
        animSR.transform.localScale = new Vector3(1f, 1f, 1f);
        animIndex++;
        if (animIndex >= data2.anims.Count)
            animIndex = 0;
    }

    private void ShowWinningSpriteAnim()
    {
        animSR.gameObject.SetActive(true);
        animSR.sprite = data.anims[animIndex];
        animSR.transform.localScale = new Vector3(1f, 1f, 1f);
        animIndex++;
        if (animIndex >= data.anims.Count)
            animIndex = 0;
    }
   
    public void SetLayer(int layer)
    {
        sR.sortingOrder = layer;
        animSR.sortingOrder = layer + 2;
        border.GetComponent<SpriteRenderer>().sortingOrder = layer + 3;
    }

    public void ShowBorder(bool isShow = true)
    {
        border.SetActive(isShow);
        border.GetComponent<SpriteRenderer>().color = borderColor;

        animSR.gameObject.SetActive(isShow);
        animSR.sprite = data.symbol;
        sR.enabled = !isShow;
    }

    public void ShowBonusAnim()
    {
        animSR.sprite = data.bonusSymbol;
        animSR.transform.localScale = Vector3.one;
    }
}
