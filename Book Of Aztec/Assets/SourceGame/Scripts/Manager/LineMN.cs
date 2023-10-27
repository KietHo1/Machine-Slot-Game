using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class LineMN : Singleton<LineMN>
{
    [Range(0, 255)]
    [SerializeField] private float opacityLockColor;
    [SerializeField] private Color[] lockColor = new Color[50];
    [SerializeField] private Color[] unlockColorList = new Color[50];
    public Image[] lineList = new Image[50];
    public Image[] lineList1 = new Image[50];
    [SerializeField] private Camera mainCamera;



    public void LineSetting()
    {
        AllLineLock();

        for (int i = 0; i < GameMN.Instance.GetLine(); i++)
        {
            lineList[i].gameObject.SetActive(true);
            lineList1[i].gameObject.SetActive(true);
            //lineList[i].color = unlockColorList[i];
            lineList[i].transform.GetChild(0).GetComponent<Text>().color = Color.black;
            //lineList1[i].color = unlockColorList[i];
            lineList1[i].transform.GetChild(0).GetComponent<Text>().color = Color.black;
        }
    }

    public void ShowWin(int index)
    {
        AllLineLock();

        foreach (Image line in lineList)
        {
            line.gameObject.SetActive(true);
            Color newColor = new Color(1, 1, 1, 130f / 255f);
            line.color = newColor;
        }

        foreach (Image line in lineList1)
        {
            line.gameObject.SetActive(true);
            Color newColor = new Color(1, 1, 1, 130f / 255f);
            line.color = newColor;
        }
        Color newColor1 = new Color(1, 1, 1, 1);
        lineList[index].color = newColor1;
        lineList1[index].color = newColor1;
    }

    private void AllLineLock()
    {
        for (int i = 0; i < lineList.Length; i++)
        {
            //Color newColor = new Color(unlockColorList[i].r, unlockColorList[i].g, unlockColorList[i].b, opacityLockColor / 255f);
            //lockColor[i] = newColor;
            lineList[i].gameObject.SetActive(false);
            lineList1[i].gameObject.SetActive(false);
            //lineList[i].color = lockColor[i];
            //lineList[i].transform.GetChild(0).GetComponent<Text>().color = lockColor[i];
            //lineList1[i].color = lockColor[i];
            //lineList1[i].transform.GetChild(0).GetComponent<Text>().color = lockColor[i];
        }
    }

    public Vector2 GetLinePos(int index)
    {
        return mainCamera.ScreenToWorldPoint(lineList[index].gameObject.transform.position);
    }

    public Color GetLineColor(int index)
    {
        return unlockColorList[index];
    }
    //public Color GetLineHighlightColor(int index)
    //{
    //    return lockColor[index];
    //}
}

