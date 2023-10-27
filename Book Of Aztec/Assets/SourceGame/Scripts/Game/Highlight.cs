using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private LineRenderer lineRenderer;
    private Color color1, color2;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void SetColor(Color color1/*, Color color2*/)
    {
        this.color1 = color1;
        //this.color2 = color2;
    }
    
    private void OnEnable() {
        StartCoroutine(Play());
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    IEnumerator Play()
    {
        yield return new WaitForSeconds(0.2f);
        if(spriteRenderer != null)
            spriteRenderer.color = color1;
        
        if(lineRenderer != null)
        {
            lineRenderer.startColor = color1;
            lineRenderer.endColor = color1;
        }

        yield return new WaitForSeconds(0.2f);

        if(spriteRenderer != null)
            spriteRenderer.color = color2;
        
        if(lineRenderer != null)
        {
            lineRenderer.startColor = color2;
            lineRenderer.endColor = color2;
        }

        StartCoroutine(Play());
    }
}
