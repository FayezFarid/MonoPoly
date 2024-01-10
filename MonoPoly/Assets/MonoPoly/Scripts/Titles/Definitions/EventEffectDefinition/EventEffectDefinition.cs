using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class EventEffectDefinition : ScriptableObject
{
    public abstract List<Action> GetActions(GameManager gameManager,Action onEnd);
}