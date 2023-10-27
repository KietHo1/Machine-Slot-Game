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
}

[Serializable]
public class SFX
{
    public SFXType sFXType;
    public AudioClip audioClip;
}

public enum SFXType
{
    START_GAME, PANEL_IN, PANEL_OUT,
    
    STEP_1, STEP_2, STEP_3, STEP_4, STEP_5, STEP_6, STEP_7, STEP_8, STEP_9, STEP_10, STEP_11, STEP_12, STEP_13
    , STEP_14, STEP_15, STEP_16, STEP_17, STEP_18, STEP_19, STEP_20, STEP_21, STEP_22, STEP_23, STEP_24, STEP_25, 
    LINE_STEP_1, LINE_STEP_2, LINE_STEP_3, LINE_STEP_4, LINE_STEP_5,

    START_REEL, STOP_REEL, STOP_REEL_WILD_1, STOP_REEL_WILD_2, STOP_REEL_WILD_3,

    CLICK, AUTO_START, AUTO_STOP, 

    WIN_0, WIN_1, WIN_2, WIN_3, WIN_4, WIN_5, WIN_6, WIN_7, WIN_8, WIN_10, WIN_DONE,

    COUNT_UP, SHOW_CHOOSE_GAMBLE, GAMBLE_LOOP, GAMBLE_WIN_1, GAMBLE_WIN_2, GAMBLE_WIN_3,

    COLLECT, COLLECT_DONE, SOUND_ON,

    FREE_SPIN_INIT, FREE_SPIN_WAITING, FREE_SPIN_TRANSITION, FREE_SPIN_TICK, FREE_SPIN_TICK_DONE, FREE_SPIN_MELODY, FREE_SPIN_MELODY_DONE ,FREE_SPIN_REPLACE, ADD_FREE_SPIN, WIN_LINE_FREE_SPIN
}
