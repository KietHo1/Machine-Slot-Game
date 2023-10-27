using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMN : Singleton<SoundMN>
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioSourceLoop;
    [SerializeField] private List<SFX> SFXList = new List<SFX>();

    public void Mute(int status)
    {
        bool isMute = status == 0 ? false : true;
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
   AUTO_PLAY, REEL_STOP, CHANGE_BET, CHANGE_PAGE, WIN,
   GAMBLE_WIN, GAMBLE_LOSE, COLLECT, FREE_INTRO, PARTYBOTTLECLOSE,
   PARTY_END, PARTY_INTRO, PARTY_LOOP, SPIN
}
