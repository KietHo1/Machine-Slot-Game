using UnityEngine;
using DG.Tweening;
using System.Collections;
public class Symbol : MonoBehaviour
{
    public SymbolData data;
    [SerializeField] private SpriteRenderer animSR;
    private SpriteRenderer sR;
    private bool isShowWin = false;
    private int animIndex = 0;
    private int currentFrame = 0, frameToPlay = 5;
    [SerializeField] private GameObject border;
    private Color borderColor = Color.white;
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

        currentFrame++;
        if (currentFrame < frameToPlay) return;
        currentFrame = 0;
        ShowWinningSpriteAnim();
    }

    internal void Setting(SymbolData data)
    {
        this.data = data;
    }

    public void SetBlurIcon(bool blur)
    {
        ShowBorder(false);
        sR.sprite = blur ? data.blurSymbol : data.symbol;
        isShowWin = false;
        sR.enabled = true;
        animSR.gameObject.SetActive(false);

    }

    public void SetWinColor(int lineIndex)
    {
        Color color1 = ShowLineMN.Instance.GetColor(lineIndex);
        Color color2 = ShowLineMN.Instance.GetDarkColor(lineIndex);
        this.borderColor = color1;
        Highlight highlight = border.GetComponent<Highlight>();
        if (highlight != null)
            highlight.SetColor(color1, color2);
    }

    public void WinSetting(bool isShow)
    {
        this.isShowWin = isShow;

        animIndex = 0;

        if (data.anims.Count <= 0)
            ShowWinningAnim(isShow);
    }

    private void ShowWinningSpriteAnim()
    {
        animSR.gameObject.SetActive(true);
        animSR.sprite = data.anims[animIndex];
        animIndex++;
        if (animIndex >= data.anims.Count)
            animIndex = 0;
    }
    private void ShowWinningAnim(bool isShow)
    {
        if (!isShow)
            return;

        //StartCoroutine(IEAnim());
    }

    //IEnumerator IEAnim()
    //{
    //    animSR.transform.DORotate(new Vector3(-360,0,0), 0.7f, RotateMode.FastBeyond360).SetEase(Ease.InOutQuart);
    //    animSR.transform.DOScaleX(1.2f, 0.35f);
    //    yield return new WaitForSeconds(0.35f);
    //    animSR.transform.DOScaleX(1f, 0.35f);
    //}

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
        animSR.sprite = data.anims[0];
        sR.enabled = !isShow;
    }
}
