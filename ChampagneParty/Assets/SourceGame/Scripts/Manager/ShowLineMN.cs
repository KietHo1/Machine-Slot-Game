using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowLineMN : Singleton<ShowLineMN>
{
    [SerializeField] private LineRenderer linePrefab;
    private List<LineRenderer> lineRenderList = new List<LineRenderer>();
    public PreviewLine[] previewLines = new PreviewLine[40];
    public void ShowLine(WinningLine wLine, int lineIndex, int index, bool isAll = false)
    {
        if(!isAll)
            HideAll();

        LineRenderer line = GetLine(index);
        line.gameObject.SetActive(true);
        line.startColor = GetColor(lineIndex);
        line.endColor = GetColor(lineIndex);
        line.positionCount = 7;

        line.SetPosition(0, Camera.main.ScreenToWorldPoint(previewLines[lineIndex].leftPos.position));

        for (int j = 0; j < wLine.positions.Count; j++)
        {
            Vector3 pos = SlotMN.Instance.GetSymbol(j, wLine.positions[j]).transform.position;
            line.SetPosition(j + 1, pos);
        }

        line.SetPosition(6, Camera.main.ScreenToWorldPoint(previewLines[lineIndex].rightPos.position));
    }

    int currentLineCount = 0;
    public void PreviewLines(int lineCount)
    {
        for (int i = 0; i < GameMN.Instance.gameData.winningLines.Count; i++)
        {
            if(i == lineCount - 1)
            {
                if (currentLineCount < lineCount || i == 0)
                    ShowPreviewLine(i);
            }
             
            if(i > lineCount - 1)
            {
                previewLines[i].linePrefab.Hide();
            }
        }

        currentLineCount = lineCount;
    }

    public void ShowPreviewLine(int i)
    {
        WinningLine wLine = GameMN.Instance.gameData.winningLines[i];
        ShowLine(wLine, i, i, false);
        previewLines[i].linePrefab.Show();
    }

    public Color GetColor(int lineIndex)
    {
        Color color = previewLines[lineIndex].linePrefab.GetColor();
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
            line.positionCount = 5;
            lineRenderList.Add(line);
            return line;
        }
        return lineRenderList[index];
    }
}

[System.Serializable]
public class PreviewLine
{
    public LinePrefab linePrefab;
    public Transform leftPos, rightPos;
}

