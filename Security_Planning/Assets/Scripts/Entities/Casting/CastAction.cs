using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastAction
{
    private Action interruptAction;
    private Action successAction;
    
    private bool started;

    public float TimeLeft { get; private set; }
    public float CastTime { get; private set; }

    public CastAction(float castTime, Action interruptAction, Action successAction)
    {
        CastTime = castTime;
        TimeLeft = castTime;
        this.interruptAction = interruptAction;
        this.successAction = successAction;
    }

    public void StartCast()
    {
        started = true;
    }

    public void Update()
    {
        if (started)
        {
            TimeLeft -= Time.deltaTime;
            if (TimeLeft <= 0)
            {
                successAction();
                CastManager.Instance.CastingFinished(this);
            }
        }
    }

    public void Interrupt()
    {
        interruptAction();
        CastManager.Instance.CastingFinished(this);
    }
}
