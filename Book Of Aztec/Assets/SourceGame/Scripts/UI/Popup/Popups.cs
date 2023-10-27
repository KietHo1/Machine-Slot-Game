using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popups : MonoBehaviour
{
    
    [SerializeField] private GameObject panel;

    

    public virtual void Awake() {
        
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
