using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprAnim : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprList = new List<Sprite>();
    private SpriteRenderer rend;
    private int index;
    private bool isLoop, isOpposite;
    private bool isPlay;

    public int framePerSpr = 1;
    int currFrame;

    private void Awake() {
        rend = GetComponent<SpriteRenderer>();
    }

    public void Show(bool isShow)
    {
        gameObject.SetActive(isShow);
        isPlay = isShow;
    }

    private void FixedUpdate() {
        if(!isPlay) return;
        ChangeSpr();
    }

    private void ChangeSpr()
    {
        if(index >= sprList.Count || index < 0)
            return;
        
        rend.sprite = sprList[index];

        currFrame++;
        if(currFrame < framePerSpr) return;
        currFrame = 0;

        if(isOpposite)
            index--;
        else
            index++;

        if(index >= sprList.Count)
        {
            if(isLoop)
                index = 0;
            else
                isPlay = false;
        }

        if(index < 0)
        {
            if(isLoop)
                index = sprList.Count - 1;
            else
                isPlay = false;
        }
    }

    public void Setting(bool isPlay = false, bool isOpposite = false, bool isLoop = false)
    {
        if(sprList.Count == 0)
        {
            this.isPlay = false;
            return;
        }

        if(isOpposite)
            index = sprList.Count - 1;
        else
            index = 0;
        
        this.isLoop = isLoop;
        this.isOpposite = isOpposite;
        this.isPlay = isPlay;
    }
}
