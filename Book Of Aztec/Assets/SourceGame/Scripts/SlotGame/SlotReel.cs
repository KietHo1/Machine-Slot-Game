using UnityEngine;
using System;
using System.Collections.Generic;
using DG.Tweening;

public class SlotReel : MonoBehaviour
{
    public int reelIndex = 0;
    public int symbolLayer = 10;

    [SerializeField] private SymbolPack symbolPackPrefab;
    [SerializeField] private Symbol symbolPrefab;
    public SymbolPack pack1, pack2;
    private bool pack1Showing = true;
    private float time = 0f;
    private int numberOfMove = 0, currentMove = 0;
    private bool isSpin = false;
    private float maxSpeed, currentSpeed;
    private float startPos, endPos;
    [SerializeField] private float showPos, hidePos;
    [SerializeField] private float symbolHeight;
    private Action countToStopEvt;

    private void Update()
    {
        if (!isSpin) return;
        Moving();
    }

    public void Init(int reelIndex, SpinData spinData, float startPos, float endPos)
    {
        this.reelIndex = reelIndex;
        this.startPos = startPos;
        this.endPos = endPos;
        this.maxSpeed = spinData.maxSpeed;
        numberOfMove = spinData.numberOfSpins + spinData.spinsDelta * reelIndex;

        pack1 = Instantiate(symbolPackPrefab, transform);
        pack2 = Instantiate(symbolPackPrefab, transform);

        pack1.Init(symbolHeight, symbolPrefab);
        pack2.Init(-symbolHeight, symbolPrefab);

        pack1.SetLayer(symbolLayer);
        pack2.SetLayer(symbolLayer);

        pack1.SetRandomIcon();
        pack2.SetRandomIcon();

        showPos = pack2.transform.localPosition.y;
        hidePos = pack1.transform.localPosition.y;
    }

    public void Move(Action countToStopEvt)
    {
        isSpin = true;
        currentMove = 0;
        currentSpeed = 0;

        pack1.SetBlurIcon(true);
        pack2.SetBlurIcon(true);

        this.countToStopEvt = countToStopEvt;
    }

    public void ReelKingMove(SymbolResult symbolResult, Action countToStopEvt)
    {
        isSpin = true;
        currentMove = 0;
        currentSpeed = 0;

        pack1.SetBlurIcon(true);
        pack2.SetBlurIcon(true);

        this.countToStopEvt = countToStopEvt;
    }

    private void Moving()
    {
        if (currentMove < maxSpeed - 1)
            currentSpeed += Time.deltaTime * maxSpeed;
        else
            currentSpeed = maxSpeed;

        time += (Time.deltaTime * currentSpeed);
        float posY = Mathf.Lerp(startPos, endPos, time);
        transform.localPosition = new Vector3(transform.localPosition.x, posY, transform.localPosition.z);

        if (time >= 1)
        {
            time = 0;

            pack1Showing = !pack1Showing;

            pack1.SetPos(pack1Showing ? showPos : hidePos);
            pack2.SetPos(pack1Showing ? hidePos : showPos);

            currentMove++;
            if (currentMove == numberOfMove)
                Stop();
            else if (currentMove == numberOfMove - 1)
                pack1.SetFinalIcon(reelIndex);
            else
            {
                if (pack1Showing)
                    pack2.SetRandomIcon();
                else
                    pack1.SetRandomIcon();
            }

            transform.localPosition = new Vector3(transform.localPosition.x, startPos, transform.localPosition.z);
        }
    }

    public void ReelStoping()
    {
        if (currentMove < numberOfMove - 2)
        {
            if (currentMove % 2 == 0)
                currentMove = numberOfMove - 4;
            else
                currentMove = numberOfMove - 3;
        }
    }

    public void FixSpinData()
    {
        if (maxSpeed < 3)
            maxSpeed = 3;

        if (numberOfMove < 2)
            numberOfMove = 2;

        if (numberOfMove % 2 != 0)
            numberOfMove -= 1;
    }

    private void Stop()
    {
        isSpin = false;
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOLocalMoveY(startPos - 1f, 1 / maxSpeed));
        mySequence.Append(transform.DOLocalMoveY(startPos, 1 / maxSpeed));
        mySequence.OnComplete(() => countToStopEvt?.Invoke());
        pack1.SetBlurIcon(false);
        pack2.SetBlurIcon(false);

        pack1.CheckFinalSymbol();
    }

    public int GetRandom(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
}