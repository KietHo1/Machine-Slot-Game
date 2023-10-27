using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;
    private int mainWidth = 1920, mainHeight = 1080, screenWidth, screenHeight;
    [SerializeField] private Transform mapBG;
    [SerializeField] private float defaultFOV, defaultMapBGScaleX, defaultMapBGScaleY;
    
    private void Awake() {
        mainCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        ChangeValue();
    }

    private void Update() {
        ChangeValue();
    }

    private void ChangeValue()
    {
        if (screenWidth == Screen.width && screenHeight == Screen.height) return;

        float mainDelta =  mainWidth * 1f / mainHeight; 
        float screenDelta = Screen.width * 1f / Screen.height;
            
        if(mainDelta >= screenDelta)
        {
            ChangeScaleBG(mainDelta / screenDelta);
            ChangeFOV(mainDelta / screenDelta);
        }
        else
        {
            ChangeScaleBG(screenDelta / mainDelta);
            ChangeFOV(1);
        }
      
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    private void ChangeFOV(float delta)
    {
        float fov = defaultFOV * delta;
        mainCamera.orthographicSize = fov;
    }

    private void ChangeScaleBG(float delta)
    {
        float scaleX = defaultMapBGScaleX * delta;
        float scaleY = defaultMapBGScaleY * delta;
        //Debug.Log(scaleX);
        //Debug.Log(scaleY);
        mapBG.localScale = new Vector3(scaleX, scaleY, 1);
    }
}
