using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AutoplayPopup : MonoBehaviour
{
    
    public GUIButton[] autoplayValueBtns;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject autoplayBtnPanel;
    private int autoIndex = 0;
    private int numberAuto = 0;

    void Awake() 
    {
        SettingEvent();
    }

    private void SettingEvent()
    {
        for (int i = 0; i < autoplayValueBtns.Length; i++)
        {
            int n = i;
            autoplayValueBtns[i].SetEvent(delegate { ChangeNumberAuto(n); AutoPlay(); });
        }
    }

    
    public void GotoLobby()
    {

    }

    public void ShowPopup(bool isShow = true)
    {
        if (isShow)
        {
            autoplayBtnPanel.transform.DORotate(new Vector3(0, 0, 90), 0.5f).SetEase(Ease.Linear).
                OnComplete(() => {
                    UIMN.Instance.autoPlayBtn.Show(false);
                    autoplayBtnPanel.transform.rotation = Quaternion.identity;
                    panel.transform.localScale = new Vector3(0.1f, 1, 1);
                    panel.SetActive(isShow);
                    panel.transform.DOScaleX(1f, 1f); 
                });        
        }
        else panel.SetActive(isShow);
    }

    private void ChangeNumberAuto(int index, bool isInfinity = false)
    {
        if (isInfinity)
        {
            //autoIndex = GameMN.Instance.gameData.NumberAutoPlay.Count - 1;
            //autoPlayTxt.text = GameMN.Instance.gameData.NumberAutoPlay[autoIndex].ToString();
            return;
        }

        autoIndex = index;

        //if (autoIndex >= GameMN.Instance.gameData.NumberAutoPlay.Count)
        //    autoIndex = 0;

        //if (autoIndex < 0)
        //    autoIndex = GameMN.Instance.gameData.NumberAutoPlay.Count - 1;

        //numberAuto = GameMN.Instance.gameData.NumberAutoPlay[autoIndex];

        //autoPlayTxt.text = GameMN.Instance.gameData.NumberAutoPlay[autoIndex].ToString();
        //GameMN.Instance.autoPlayData.ValueSetting(true);

    }

    private void AutoPlay()
    {
        ShowPopup(false);
        //GameMN.Instance.autoPlayData.ValueSetting(true);
        UIMN.Instance.PlaySlot();
        GameMN.Instance.SetSpinType(SpinType.AUTO_SPIN);
    }

}
