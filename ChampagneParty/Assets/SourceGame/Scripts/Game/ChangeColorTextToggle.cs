using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColorTextToggle : MonoBehaviour
{
    private Toggle tg;
    [SerializeField] private Color color;
    [SerializeField] private Text txt;

    private void Awake() {
        tg = GetComponent<Toggle>();
        tg.onValueChanged.AddListener(ChangeColor);
    }

    private void ChangeColor(bool noUse){
        if(tg.isOn)
            txt.color = color;
        else
            txt.color = Color.white;
    }
}
