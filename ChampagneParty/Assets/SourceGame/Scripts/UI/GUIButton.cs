using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class GUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button btn;
    private Image img;
    [SerializeField] private Text txt;
    [SerializeField] private List<Sprite> sprStatusList = new List<Sprite>();
    [SerializeField] private List<string> desStatusList = new List<string>();
    [SerializeField] private Sprite enableSpr, disableSpr, hoverSpr; 
    private int index = 0;
    private Action evt;
    private bool isDisable = false;
    [SerializeField] private Material highlightMat;

    private void Awake() 
    {
        btn = gameObject.GetComponent<Button>();
        img = gameObject.GetComponent<Image>();
    }

    private void Start() 
    {
        SetImage();
        SetTxt();
        btn.onClick.AddListener(ClickEvent);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDisable)
            return;

        if (disableSpr == null || enableSpr == null || hoverSpr == null)
        {
            if (btn == null)
                btn = GetComponent<Button>();
        }
        else
        {
            if (img == null)
                img = GetComponent<Image>();

            img.sprite = hoverSpr;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDisable)
            return;

        if (disableSpr == null || enableSpr == null || hoverSpr == null)
        {
            if (btn == null)
                btn = GetComponent<Button>();
        }
        else
        {
            if (img == null)
                img = GetComponent<Image>();

            img.sprite = enableSpr;
        }
    }

    private void ClickEvent()
    {
        if (isDisable)
            return;

        ChangeStatus();
        evt?.Invoke();

        SprAnim sprAnim = GetComponent<SprAnim>();

        if (sprAnim)
            sprAnim.Setting(isPlay: true, false, true);
    }

    private void ChangeStatus()
    {
        index++;
        if (index >= desStatusList.Count)
            index = 0;

        SetImage();
        SetTxt();
    }

    public int GetStatusIndex()
    {
        return index;
    }

    public void Show(bool isShow)
    {
        gameObject.SetActive(isShow);
        Disable(!isShow);
    }

    public void Disable(bool disable)
    {
        if (!gameObject.activeSelf)
            return;

        isDisable = disable;
        btn.enabled = !disable;
        if (disableSpr == null || enableSpr == null)
        {
            if (btn == null)
                btn = GetComponent<Button>();
        }
        else
        {
            if (img == null)
                img = GetComponent<Image>();

            img.material = disable ? null : highlightMat;
            Sprite spr = disable ? disableSpr : enableSpr;
            img.sprite = spr;
        }
    }

    public void SetEvent(Action evt)
    {
        this.evt = evt;
    }

    private void SetImage()
    {
        if (index >= sprStatusList.Count)
            return;

        img.sprite = sprStatusList[index];
    }

    private void SetTxt()
    {
        if (index >= desStatusList.Count)
            return;

        if (txt != null)
            txt.text = desStatusList[index];
    }

    public void SetTxt(string str)
    {
        txt.text = str;
    }

    public void SetTextColor(Color color)
    {
        txt.color = color;
    }
}
