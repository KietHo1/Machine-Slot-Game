using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColorLoop : MonoBehaviour
{
    [SerializeField] Color currentColor, color;
    [SerializeField] private int frameToAction = 0, frameDelayToStart = 0;
    private Image img;
    private Text txt;
    private int curFrame = 0, currFrameStartDelay = 0;
    
    bool isPlay = false;

    private void OnEnable() {
        isPlay = true;
        currFrameStartDelay = 0;
        curFrame = 0;
        img.color = Color.white;
    }

    private void OnDisable() {
        isPlay = false;
    }

    private void Awake() {
        img = GetComponent<Image>();
        txt = GetComponent<Text>();
    }
    private void Update() 
    {
        if(!isPlay)
            return;
        
        currFrameStartDelay++;
        if(currFrameStartDelay < frameDelayToStart)
            return;

        curFrame++;
        if(curFrame < frameToAction)
            return;
            
        curFrame = 0;

        if(img != null)
        {
            if(img.color == currentColor)
                img.color = color;
            else
                img.color = currentColor;
        }

        if(txt != null)
        {
            if(txt.color == currentColor)
                txt.color = color;
            else
                txt.color = currentColor;
        }
       
    }

}
