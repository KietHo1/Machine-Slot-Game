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

    public void PlayOneShot(SFXType sFXType)
    {
        SFX sfx = SFXList.Find(x => x.sFXType == sFXType);
        if(sfx != null)
            audioSource.PlayOneShot(sfx.audioClip);
    }

    public void PlayOneShot(string str)
    {
        SFX sfx = SFXList.Find(x => x.sFXType.ToString() == str);
        if(sfx != null)
            audioSource.PlayOneShot(sfx.audioClip);
    }

    public void PlayLoop(SFXType sFXType)
    {
        SFX sfx = SFXList.Find(x => x.sFXType == sFXType);
        if(sfx == null)
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
        if (reward <= 200f)
        {
            PlayOneShot(SFXType.nicewin);
        } 
        else if (reward > 200f && reward <= 500f)
        {
            PlayOneShot(SFXType.bigwin);
        }
        else if (reward > 500f)
            PlayOneShot(SFXType.megawin);
    }

    public void PlayWinSound()
    {
        WinData data = SymbolLargestReward(ResultMN.Instance.winDatas);
        PlaySoundToSymbol(data.symbol);
    }

    private WinData SymbolLargestReward(List<WinData> winDatas)
    {
        if (winDatas.Count == 0) return null;

        WinData symbolLargestReward = new WinData();
        foreach (WinData winData in winDatas)
        {
            if (winData.lineReward > symbolLargestReward.lineReward) symbolLargestReward = winData;
        }
        return symbolLargestReward;
    }

    public void PlaySoundToSymbol(int index)
    {
        if(index == 3)
        {
            PlayOneShot(SFXType.symb7);
        }
        else if(index == 4)
        {
            PlayOneShot(SFXType.symb1);
        }
        else if(index == 5 || index == 6)
        {
            PlayOneShot(SFXType.symb4);
        }
        else if(index == 7 || index == 8)
        {
            PlayOneShot(SFXType.symb2);
        }
        else if(index == 9)
        {
            PlayOneShot(SFXType.symb6);
        }
        else
        {
            PlayOneShot(SFXType.symb8);
        }
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
    audio_on, auto_off, auto_on, bigwin, btn, collect, collect1, countup11, countup12, countup13, countup14,
    deal_2, gamble_off, gamble_on, gambleprop, mainGameSounds, maxstep, megawin, mobileMainSounds, nicewin, onstart,
    panel_in, panel_out, reels, ScatterSound, shortSound, step1, step2, step3, step4, step5, step6, step7, step8, step9,
    step10, step11, step12, stop_reel2, stop_wild, symb1, symb2, symb3, symb4, symb5, symb6, symb7, symb8, WildSound, win1,
    win2, win3, winfinal, winline, winSounds
}
