using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public enum EffectType
{
    Prison,
    PrisonPass,
}

public interface IPlayerCallbacks
{
    void OnMoneyAdd(ref int moneyValue);
    void OnPlayerTurnStarted();
    void OnPlayerTurnEnded();
}

public interface IStatusEffectDefinition
{
    void OnMoneyAdd(StatusEffectInstance instance, ref int moneyValue);
    void OnTurnPass(StatusEffectInstance instance);
}

public abstract class StatusEffectDefinition : ScriptableObject, IStatusEffectDefinition /*,IPlayerCallbacks*/
{
    /// <summary>
    /// The Duration is by turn
    /// </summary>
    /// <returns></returns>
    [SerializeField] private int duration;

    /// <summary>
    /// Status Effect filter
    /// </summary>
    [SerializeField] private EffectType effectType;

    public virtual EffectType EffectType => effectType;

    public string description;

    public int Duration => duration;

    //Interface?

    public virtual void OnMoneyAdd(StatusEffectInstance instance, ref int moneyValue)
    {
    }

    public virtual void OnPlayerTurnStarted(StatusEffectInstance instance)
    {
    }

    public virtual void OnTurnPass(StatusEffectInstance instance)
    {
        instance.CurrentDuration--;
    }
}

public class StatusEffectInstance : IPlayerCallbacks
{
    public static Action<StatusEffectInstance> OnEffectEnded;


    private StatusEffectDefinition _statusEffectDefinition;
    private int _currentDuration;

    public EffectType EffectType => _statusEffectDefinition.EffectType;
    public int CurrentDuration
    {
        get => _currentDuration;
        set
        {
            _currentDuration = value;
            if (_currentDuration >= _statusEffectDefinition.Duration)
            {
                EffectEnded();
            }
        }
    }

    public StatusEffectInstance(StatusEffectDefinition statusEffectDefinition)
    {
        _statusEffectDefinition = statusEffectDefinition;
        _currentDuration = 0;
    }

    #region IPlayerCallbacks

    public void OnMoneyAdd(ref int moneyValue)
    {
        _statusEffectDefinition.OnMoneyAdd(this, ref moneyValue);
    }

    public void OnPlayerTurnStarted()
    {
        _statusEffectDefinition.OnPlayerTurnStarted(this);
    }

    public void OnPlayerTurnEnded()
    {
        _statusEffectDefinition.OnTurnPass(this);
    }

    #endregion

    private void EffectEnded()
    {
        OnEffectEnded?.Invoke(this);
    }
}