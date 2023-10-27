using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowLineMN : Singleton<ShowLineMN>
{
    [SerializeField] private LineRenderer linePrefab;
    [SerializeField] private List<Image> leftLinePanelList = new List<Image>();
    [SerializeField] private List<Image> rightLinePanelList = new List<Image>();
    [SerializeField] private List<Color> colorLineList = new List<Color>();
    private List<LineRenderer> lineRenderList = new List<LineRenderer>();

    public void ShowLine(WinningLine wLine, int lineIndex, int index, bool isAll = false)
    {
        if(!isAll)
            HideAll();

        LineRenderer line = GetLine(index);
        line.startColor = GetColor(lineIndex);
        line.endColor = GetColor(lineIndex);

        line.gameObject.SetActive(true);
        line.positionCount = 7;

        line.SetPosition(0, Camera.main.ScreenToWorldPoint(leftLinePanelList[lineIndex].gameObject.transform.position));

        for (int j = 0; j < wLine.positions.Count; j++)
        {
            Vector3 pos = SlotMN.Instance.GetSymbol(j, wLine.positions[j]).transform.position;
            line.SetPosition(j + 1, pos);
        }

        line.SetPosition(6, Camera.main.ScreenToWorldPoint(rightLinePanelList[lineIndex].gameObject.transform.position));
    }

    public Color GetColor(int lineIndex){
        Color color = colorLineList[lineIndex];
        return color;
    }

    public void HideAll()
    {
        foreach (LineRenderer lineR in lineRenderList)
        {
            lineR.gameObject.SetActive(false);
        }
    }

    private LineRenderer GetLine(int index)
    {
        if (index > lineRenderList.Count - 1)
        {
            LineRenderer line = Instantiate(linePrefab, transform);
            line.positionCount = 6;
            lineRenderList.Add(line);
            return line;
        }
        return lineRenderList[index];
    }

    public void ShowLinePanel(int lineCount)
    {
        foreach (Image line in leftLinePanelList)
        {
            line.gameObject.SetActive(false);
        }

        foreach (Image line in rightLinePanelList)
        {
            line.gameObject.SetActive(false);
        }

        for (int i = 0; i < leftLinePanelList.Count; i++)
        {
            if(i < lineCount)
            {
                leftLinePanelList[i].gameObject.SetActive(true);
                rightLinePanelList[i].gameObject.SetActive(true);
            }
        }
    }

    public void ShowLinePreview(int lineCount)
    {
        HideAll();
        CancelInvoke("HideAll");
        for (int i = 0; i < GameMN.Instance.gameData.winningLines.Count; i++)
        {
            if (i < lineCount)
            {
                ShowLine(GameMN.Instance.gameData.winningLines[i], i , i, isAll: true);
            }
        }

        Invoke("HideAll", 3f);
    }
}
