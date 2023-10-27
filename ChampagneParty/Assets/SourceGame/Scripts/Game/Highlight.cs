using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Highlight : MonoBehaviour
{
    private int count = 0;
    private LineRenderer lineRenderer;
    private Image img;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        img = GetComponent<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable() 
    {
        Play();
    }

    private void OnDisable() 
    {
        StopAllCoroutines();
    }

    public void Play()
    {
        count = 6;
        StartCoroutine(IEPlay());
    }

    IEnumerator IEPlay()
    {
        count--;

        yield return new WaitForSeconds(0.075f);
        if (lineRenderer != null)
            lineRenderer.enabled = true;

        if (img != null)
            img.enabled = true;

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        yield return new WaitForSeconds(0.075f);
        if (lineRenderer != null)
            lineRenderer.enabled = false;

        if (img != null)
            img.enabled = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (count > 0)
            StartCoroutine(IEPlay());
        else
            StopAllCoroutines();
    }
}
