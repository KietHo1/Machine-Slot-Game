using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BetLineSliderMN : Singleton<BetLineSliderMN>
{
    [SerializeField] private Slider lineSlider, betSlider;
    [SerializeField] private Image lineImg, betImg;
    [SerializeField] private Transform lineNumberHolder, betNumberHolder;
    [SerializeField] private GameObject numberPrefab;
    private List<GameObject> numberLineList = new List<GameObject>();
    private List<GameObject> numberBetList = new List<GameObject>();
    [SerializeField] private Color chooseColor;
    [SerializeField] private Button exitBtn, minusBetBtn, plusBetBtn, minusLineBtn, plusLineBtn;
    [SerializeField] private GUIButton leftShowBtn, rightShowBtn;
    [SerializeField] private RectTransform linePanel, betPanel;
    [SerializeField] private InfoPopup infoPopup;
    IEnumerator Start()
    {
        exitBtn.onClick.AddListener(delegate { Show(false); });
        leftShowBtn.SetEvent(delegate { Show(); });
        rightShowBtn.SetEvent(delegate { Show(); });
        minusBetBtn.onClick.AddListener(delegate { UIMN.Instance.ChangeBetValue(-1); ChangeBetBySliderBTN(); });
        plusBetBtn.onClick.AddListener(delegate { UIMN.Instance.ChangeBetValue(1); ChangeBetBySliderBTN(); });
        minusLineBtn.onClick.AddListener(delegate { UIMN.Instance.ChangeLines(-1); ChangeLineBySliderBTN(); });
        plusLineBtn.onClick.AddListener(delegate { UIMN.Instance.ChangeLines(1); ChangeLineBySliderBTN(); });
        Init();

        Show();
        yield return new WaitForSeconds(2f);
        Show(false);
    }
    public void BetLineSetting(bool isOff)
    {
        leftShowBtn.Disable(isOff);
        rightShowBtn.Disable(isOff);
    }
    private void Show(bool isShow = true)
    {
        exitBtn.gameObject.SetActive(isShow);
        ApplyLine();
        ApplyBet();

        float posXLinePanel = isShow ? 0 : -150f;
        float posXBetPanel = isShow ? 0 : 150f;

        if (!isShow)
        {
            SoundMN.Instance.PlayOneShot(SFXType.panel_out);
            linePanel.DOAnchorPosX(posXLinePanel, 0.5f).OnComplete(() => { linePanel.DOScaleX(0f, 0f); });
            betPanel.DOAnchorPosX(posXBetPanel, 0.5f).OnComplete(() => { betPanel.DOScaleX(0f, 0f); });
            //SoundMN.Instance.PlayOneShot(SFXType.PANEL_OUT);
        }
        else
        {
            SoundMN.Instance.PlayOneShot(SFXType.panel_in);
            linePanel.DOScaleX(1f, 0f).OnComplete(() => { linePanel.DOAnchorPosX(posXLinePanel, 0.5f); });
            betPanel.DOScaleX(1f, 0f).OnComplete(() => { betPanel.DOAnchorPosX(posXBetPanel, 0.5f); });
            //SoundMN.Instance.PlayOneShot(SFXType.PANEL_IN);
        }
    }

    private void Init()
    {
        //for (int i = GameMN.Instance.gameData.lineCountList.Count - 1; i >= 0; i--)
        //{
        //    GameObject number = Instantiate(numberPrefab, lineNumberHolder);
        //    GetText(number).text = GameMN.Instance.gameData.lineCountList[i].ToString();
        //    GetText(number).color = Color.white;
        //    number.name = GameMN.Instance.gameData.lineCountList[i].ToString();
        //    numberLineList.Add(number);
        //}

        //for (int i = GameMN.Instance.gameData.bets.Count - 1; i >= 0; i--)
        //{
        //    GameObject number = Instantiate(numberPrefab, betNumberHolder);
        //    GetText(number).text = GameMN.Instance.gameData.bets[i].ToString();
        //    GetText(number).color = Color.white;
        //    number.name = GameMN.Instance.gameData.bets[i].ToString();
        //    numberBetList.Add(number);
        //}

        //numberLineList.Reverse();
        //numberBetList.Reverse();
    }

    public void ChangeLineBySlider()
    {
        //float delta = 1 / (float)GameMN.Instance.gameData.lineCountList.Count;
        //for (int i = 0; i < GameMN.Instance.gameData.lineCountList.Count; i++)
        //{
        //    float value = (i + 1) * delta;
        //    if (lineSlider.value < value)
        //    {
        //        lineImg.fillAmount = value;
        //        GameMN.Instance.currentLinesIndex = i;
        //        ShowCurrentNumber(numberLineList, GameMN.Instance.currentLinesIndex);
        //        UIMN.Instance.CheckSoundLine();
        //        UIMN.Instance.LineSetting();
        //        UIMN.Instance.BetSetting();
        //        UIMN.Instance.TotalBetSetting();
        //        ShowLineMN.Instance.ShowLinePanel(GameMN.Instance.gameData.lineCountList[GameMN.Instance.currentLinesIndex]);
        //        //ShowLineMN.Instance.ShowLinePreview(GameMN.Instance.gameData.lineCountList[GameMN.Instance.currentLinesIndex]);
        //        infoPopup.PaytableSetting();
        //        NotificationPanel.Instance.Show(NotificationType.SHOWBETLINE);
        //        return;
        //    }

        //}
    }

    public void ChangeBetBySlider()
    {
        //float delta = 1 / (float)GameMN.Instance.gameData.bets.Count;
        //for (int i = 0; i < GameMN.Instance.gameData.bets.Count; i++)
        //{
        //    float value = (i + 1) * delta;
        //    if (betSlider.value < value)
        //    {
        //        betImg.fillAmount = value;
        //        GameMN.Instance.currentBetIndex = i;
        //        UIMN.Instance.CheckSoundBet();
        //        ShowCurrentNumber(numberBetList, GameMN.Instance.currentBetIndex);
        //        UIMN.Instance.LineSetting();
        //        UIMN.Instance.BetSetting();
        //        UIMN.Instance.TotalBetSetting();
        //        infoPopup.PaytableSetting();
        //        NotificationPanel.Instance.Show(NotificationType.SHOWBETLINE);
        //        return;
        //    }

        //}
    }

    public void ChangeLineBySliderBTN()
    {
        //ApplyLine();
        //float delta = 1 / (float)GameMN.Instance.gameData.lineCountList.Count;
        //int i = GameMN.Instance.currentLinesIndex;
        //float value = (i + 1) * delta;
        //if (lineSlider.value < value)
        //{
        //    lineImg.fillAmount = value;
        //    GameMN.Instance.currentLinesIndex = i;
        //    UIMN.Instance.CheckSoundLine();
        //    ShowCurrentNumber(numberLineList, GameMN.Instance.currentLinesIndex);
        //    infoPopup.PaytableSetting();
        //    ShowLineMN.Instance.ShowLinePanel(GameMN.Instance.gameData.lineCountList[GameMN.Instance.currentLinesIndex]);
        //    NotificationPanel.Instance.Show(NotificationType.SHOWBETLINE);
        //}
    }

    public void ChangeBetBySliderBTN()
    {
        //ApplyBet();
        //float delta = 1 / (float)GameMN.Instance.gameData.bets.Count;
        //int i = GameMN.Instance.currentBetIndex;
        //float value = (i + 1) * delta;
        //if (lineSlider.value < value)
        //{
        //    betImg.fillAmount = value;
        //    GameMN.Instance.currentBetIndex = i;
        //    UIMN.Instance.CheckSoundBet();
        //    ShowCurrentNumber(numberBetList, GameMN.Instance.currentBetIndex);
        //    infoPopup.PaytableSetting();
        //    NotificationPanel.Instance.Show(NotificationType.SHOWBETLINE);
        //}
    }

    private void ApplyLine()
    {
        //float delta = 1 / (float)GameMN.Instance.gameData.lineCountList.Count;
        //for (int i = 0; i < GameMN.Instance.gameData.lineCountList.Count; i++)
        //{
        //    if (GameMN.Instance.currentLinesIndex == i)
        //    {
        //        float value = (i + 1) * delta;
        //        lineImg.fillAmount = value;
        //    }
        //}

        //ShowCurrentNumber(numberLineList, GameMN.Instance.currentLinesIndex);
    }

    private void ApplyBet()
    {
        //float delta = 1 / (float)GameMN.Instance.gameData.bets.Count;
        //for (int i = 0; i < GameMN.Instance.gameData.bets.Count; i++)
        //{
        //    if (GameMN.Instance.currentBetIndex == i)
        //    {
        //        float value = (i + 1) * delta;
        //        betImg.fillAmount = value;
        //    }
        //}

        //ShowCurrentNumber(numberBetList, GameMN.Instance.currentBetIndex);
    }

    private void ShowCurrentNumber(List<GameObject> numberList, int index)
    {
        for (int i = 0; i < numberList.Count; i++)
        {
            if (i == 0 || i == numberList.Count - 1)
            {
                NumberShow(numberList[0], true, Color.white);
                NumberShow(numberList[numberList.Count - 1], true, Color.white);
                continue;
            }

            NumberShow(numberList[i], false, Color.white);
        }

        NumberShow(numberList[index], true, chooseColor);
    }


    private void NumberShow(GameObject number, bool isShow, Color color)
    {
        float oppacity = isShow ? 1 : 0;
        GetImage(number).color = new Color(GetImage(number).color.r, GetImage(number).color.g, GetImage(number).color.b, oppacity);
        GetText(number).color = new Color(color.r, color.g, color.b, oppacity);

    }

    private Text GetText(GameObject number)
    {
        return number.transform.GetChild(0).GetComponent<Text>();
    }

    private Image GetImage(GameObject number)
    {
        return number.GetComponent<Image>();
    }
}
