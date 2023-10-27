using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleBackgroundImage : MonoBehaviour
{
    private float mainWidth = 1920f, mainHeight = 1080f, screenWidth, screenHeight;

    // Update is called once per frame
    void Update()
    {
        ChangeValue();
    }

    private void ChangeValue()
    {
        float mainDelta =  mainWidth / mainHeight; 
        float screenDelta = (float)Screen.width / (float)Screen.height;
        
        if((float)Screen.width == screenWidth && (float)Screen.height == screenHeight) 
            return;
        
        float scale = mainDelta / screenDelta;
        if(mainDelta < screenDelta)
            scale = screenDelta / mainDelta;

        transform.localScale = new Vector3(scale,scale,scale);

    }
}
