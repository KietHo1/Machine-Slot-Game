using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowLineMN : Singleton<ShowLineMN>
{
    [SerializeField] private LineRenderer linePrefab;
    private List<LineRenderer> lineRenderList = new List<LineRenderer>();

    public void ShowLine(WinningLine wLine, int lineIndex, int index)
    {
        HideAll();

        LineRenderer line = GetLine(index);
        line.startColor = GetColor(lineIndex);
        line.endColor = GetColor(lineIndex);

        Color color1 = LineMN.Instance.GetLineColor(lineIndex);
        //Color color2 = LineMN.Instance.GetLineHighlightColor(lineIndex);
        Highlight highlight = line.GetComponent<Highlight>();

        if (highlight != null)
            highlight.SetColor(color1/*, color2*/);

        line.gameObject.SetActive(true);
        line.positionCount = 5;
        for (int i = 0; i < wLine.positions.Count; i++)
        {
            Vector3 pos = SlotMN.Instance.GetSymbol(i, wLine.positions[i]).transform.position;
            line.SetPosition(i, pos);
        }
    }

    public Color GetColor(int lineIndex)
    {
        Color color = LineMN.Instance.GetLineColor(lineIndex);
        return color;
    }

    public void HideAll()
    {
        foreach (LineRenderer lineR in lineRenderList)
        {
            lineR.gameObject.SetActive(false);
        }

        foreach (Image line in LineMN.Instance.lineList)
        {
            Color newColor = new Color(1, 1, 1, 1);
            line.color = newColor;
        }

        foreach (Image line in LineMN.Instance.lineList1)
        {
            Color newColor = new Color(1, 1, 1, 1);
            line.color = newColor;
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
