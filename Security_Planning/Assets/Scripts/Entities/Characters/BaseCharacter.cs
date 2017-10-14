﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour {

    public abstract void RequestPassword(IPasswordOpenable passwordOpenableObject);
    public abstract void InterruptRequestPassword();
}