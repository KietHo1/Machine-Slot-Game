using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class SettingPanel : Singleton<SettingPanel>, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Toggle soundTg;
    [SerializeField] private Button infoBtn;

    [SerializeField] private GUIButton showBtn;
    [SerializeField] private RectTransform settingPanel;

    IEnumerator Start()
    {
        soundTg.onValueChanged.AddListener(delegate { SoundMN.Instance.Mute(!soundTg.isOn); SoundMN.Instance.PlayOneShot(SFXType.SOUND_ON); });
        infoBtn.onClick.AddListener(delegate { UIMN.Instance.ShowInfoPage(); SoundMN.Instance.PlayOneShot(SFXType.CLICK); });

        Show();
        yield return new WaitForSeconds(2f);
        Hide();
    }
    public void SettingPanelSetting(bool isShow)
    {
        showBtn.Show(isShow);
    }

    private void Show()
    {
        float posYSettingPanel = 475f;
        settingPanel.DOScaleY(1f, 0f).OnComplete(() => { settingPanel.DOAnchorPosY(posYSettingPanel, 0.5f); });
        SoundMN.Instance.PlayOneShot(SFXType.PANEL_IN);
    }

    public void Hide()
    {
        float posYSettingPanel = 610f;
        settingPanel.DOAnchorPosY(posYSettingPanel, 0.5f).OnComplete(() => { settingPanel.DOScaleY(0f, 0f); });
        SoundMN.Instance.PlayOneShot(SFXType.PANEL_OUT);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Show();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Hide();
    }
}
