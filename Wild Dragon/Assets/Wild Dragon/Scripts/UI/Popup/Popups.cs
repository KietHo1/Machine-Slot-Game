using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popups : MonoBehaviour
{
    [SerializeField] private Button exitBtn;
    public GameObject panel;

    public virtual void Awake() 
    {
        if(exitBtn != null)
            exitBtn.onClick.AddListener(HidePopup);
    }

    public virtual void ShowPopup(bool isShow = true)
    {
        panel.SetActive(isShow);
    }

    public virtual void HidePopup()
    {
        panel.SetActive(false);
    }
}
