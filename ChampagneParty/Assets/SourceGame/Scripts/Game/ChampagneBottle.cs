using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ChampagneBottle : MonoBehaviour
{
    private GUIButton guiButton;
    private Image img;
    private Action evt;
    [SerializeField] private Sprite normalBottleSpr, openedBottleSpr;
    [SerializeField] private Text txt;

    private void Awake()
    {
        img = GetComponent<Image>();
        guiButton = GetComponent<GUIButton>();
    }

    private void Start()
    {
        guiButton.SetEvent(BottleChoosing);
    }

    public void SetEvent(Action evt)
    {
        this.evt = evt;
    }

    public void SetNewBottle()
    {
        guiButton.Disable(false);
        img.sprite = normalBottleSpr;
        txt.text = " ";
    }

    private void BottleChoosing()
    {
        if (BonusGameMN.Instance.CanNotChoose())
            return;

        guiButton.Disable(true);
        img.sprite = openedBottleSpr;
        // ?????
        evt?.Invoke();
        ShowReward();
    }

    public void ShowReward(bool ischoosing = true)
    {
        txt.color = ischoosing ? Color.red : Color.black;
        txt.text = Ultility.GetMoneyFormated(BonusGameMN.Instance.GetReward());
    }
}
