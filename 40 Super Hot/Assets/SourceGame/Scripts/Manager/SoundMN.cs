using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMN : Singleton<SoundMN>
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioSourceLoop;
    [SerializeField] private List<SFX> SFXList = new List<SFX>();

    public void Mute(bool isMute)
    {
        audioSource.mute = isMute;
        audioSourceLoop.mute = isMute;
    }

    //public void StopClip()
    //{
    //    Debug.Log("22222");
    //    audioSource.Stop();
    //}

    public void PlayOneShot(SFXType sFXType)
    {
        SFX sfx = SFXList.Find(x => x.sFXType == sFXType);
        if (sfx != null)
        {
            audioSource.PlayOneShot(sfx.audioClip);
        }
            
    }

    public void PlayOneShot(string str)
    {
        SFX sfx = SFXList.Find(x => x.sFXType.ToString() == str);
        if (sfx != null)
            audioSource.PlayOneShot(sfx.audioClip);
    }

    public void PlayLoop(SFXType sFXType)
    {
        SFX sfx = SFXList.Find(x => x.sFXType == sFXType);
        if (sfx == null)
            return;

        audioSourceLoop.clip = sfx.audioClip;
        audioSourceLoop.loop = true;
        audioSourceLoop.Play();
    }

    public void StopLoop()
    {
        audioSourceLoop.Stop();
    }

    public void PlayWinSound(float reward)
    {
        int index = 1;
        if (reward <= 100f)
            index = 1;
        else if (reward > 100f && reward <= 200f)
            index = 2;
        else if (reward > 200f && reward <= 500f)
            index = 3;
        else if (reward > 500f && reward <= 1000f)
            index = 4;
        else
            index = 5;

        PlayOneShot("WIN" + index.ToString());
    }
}

[Serializable]
public class SFX
{
    public SFXType sFXType;
    public AudioClip audioClip;
}

public enum SFXType
{
    FASTBTN_CLICK, GAMBLE_LOSE, GAMBLE_START, GAMBLE_WIN, INFOBTN_CLICK, NUMBER_RUN_END, NUMBER_RUN_START, REEL_STOP, SHOW_LINE, SPINBTN_CLICK,
    UI_AUTO_START, UI_AUTO_STOP, UI_CHANGEBET, WIN1, WIN2, WIN3, WIN4, WIN5, COLLECT_END, SCATTER_SOUND, WILD_SOUND, GAMBLE_LOOP
}

