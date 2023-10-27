using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SprAnim : MonoBehaviour
{
    private List<Sprite> cursprList = new List<Sprite>();
    [SerializeField] private List<Sprite> sprList = new List<Sprite>();
    [SerializeField] private List<Sprite> sprclickList = new List<Sprite>();
    [SerializeField] private SpriteRenderer rend;
    [SerializeField] private Image image;
    
    private int index;
    public bool isLoop, isPlay;
    private bool isOpposite, isClick;

    public int framePerSpr = 1;
    public int frameToStarts = 0;
    private int currFrame, currStartFrame;

    private void OnEnable() {
        isClick = false;
        cursprList = sprList;
    }

    private void Awake() {
        if(image == null)
            image = GetComponent<Image>();
        
        if(rend == null)
            rend = GetComponent<SpriteRenderer>();
            
        cursprList = sprList;
    }

    public void Show(bool isShow)
    {
        gameObject.SetActive(isShow);
        isPlay = isShow;
    }

    private void FixedUpdate() {
        if(!isPlay || cursprList.Count == 0) return;
        ChangeSpr();
    }

    private void ChangeSpr()
    {
        if((index >= cursprList.Count || index < 0) && !isClick)
            return;
        
        if(rend != null)
            rend.sprite = cursprList[index];
        
        if(image != null)
            image.sprite = cursprList[index];
        
        currStartFrame++;
        if(currStartFrame < frameToStarts && !isClick) return;

        currFrame++;
        if(currFrame < framePerSpr) return;
        currFrame = 0;

        if(isOpposite)
            index--;
        else
            index++;

        //NO OPPOSITE
        if(index >= cursprList.Count)
        {
            if(isLoop)
                index = 0;
            else
                isPlay = false;

            currStartFrame = 0;
            cursprList = sprList;
            isClick = false;
        }

        //OPPOSITE
        if(index < 0)
        {
            if(isLoop)
                index = cursprList.Count - 1;
            else
                isPlay = false;

            currStartFrame = 0;
            cursprList = sprList;
            isClick = false;
        }
    }

    public void Setting(bool isPlay = false, bool isOpposite = false, bool isClick = false)
    {
        if(sprList.Count == 0 && sprclickList.Count == 0)
        {
            this.isPlay = false;
            return;
        }

        if(isOpposite)
            index = sprList.Count - 1;
        else
            index = 0;

        cursprList = isClick ? sprclickList : sprList;

        this.isClick = isClick;
        this.isOpposite = isOpposite;
        this.isPlay = isPlay;
    }

    public void SetFirstFrameClickButton()
    {
        image.sprite = sprclickList[0];
    }

    public void SetLastFrameClickButton()
    {
        image.sprite = sprList[0];
    }

    public void SetSprList(List<Sprite> sprs)
    {
        sprList = sprs;
    }
}
