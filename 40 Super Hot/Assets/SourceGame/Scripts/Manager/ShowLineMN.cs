using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowLineMN : Singleton<ShowLineMN>
{
    [SerializeField] private LineRenderer linePrefab;
    private List<LineRenderer> lineRenderList = new List<LineRenderer>();
    [SerializeField] private GameObject[] lines = new GameObject[40];
    [SerializeField] private Color[] darkColor = new Color[40];

    private void Start()
    {
        HideAll();
    }

    public void ShowLine(WinningLine wLine, int lineIndex, int index, bool isShowEach = false)
    {
        if (isShowEach)
            HideAll();

        LineRenderer line = GetLine(index);
        line.startColor = GetColor(lineIndex);
        line.endColor = GetColor(lineIndex);
        line.gameObject.SetActive(true);

        Color color1 = GetColor(lineIndex);
        Color color2 = GetDarkColor(lineIndex);
        Highlight highlight = line.GetComponent<Highlight>();
        if (highlight != null)
            highlight.SetColor(color1, color2);

        line.SetPosition(0, lines[lineIndex].transform.position);
        for (int i = 0; i < wLine.positions.Count; i++)
        {
            Vector3 pos = SlotMN.Instance.GetSymbol(i, wLine.positions[i]).transform.position;
            line.SetPosition(i, pos);
        }
    }

    public Color GetColor(int lineIndex)
    {
        Color color = lines[lineIndex].GetComponent<SpriteRenderer>().color;
        return color;
    }

    public Color GetDarkColor(int lineIndex)
    {
        Color color = darkColor[lineIndex];
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
