using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlackPanel : MonoBehaviour
{
    private void Start() 
    {
        Image image = GetComponent<Image>();
        image.enabled = true;
        image.DOColor(new Color(0f,0f,0f, 0f), 1f).OnComplete(() => gameObject.SetActive(false));
    }
}
