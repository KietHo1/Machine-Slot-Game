using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusGamePopup : MonoBehaviour
{
    [SerializeField] private ChampagneBottle[] champagneBottles = new ChampagneBottle[5];
    [SerializeField] private GameObject panel;

    private void Start()
    {
        SetEvent();
    }

    private void SetEvent()
    {
        foreach (ChampagneBottle bottle in champagneBottles)
        {
            bottle.SetEvent(BonusGameMN.Instance.BottleChoosing);
        }
    }

    public void Show()
    {
        panel.SetActive(true);
        foreach (ChampagneBottle bottle in champagneBottles)
        {
            bottle.SetNewBottle();
        }
        SoundMN.Instance.PlayOneShot(SFXType.PARTY_INTRO);
        SoundMN.Instance.PlayLoop(SFXType.PARTY_LOOP);
    }

    public void Hide()
    {
        panel.SetActive(false);
        SoundMN.Instance.StopLoop();
        SoundMN.Instance.PlayOneShot(SFXType.PARTY_END);
    }
}
