using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Menu is anything that menu as the name describe is more static 
/// </summary>
[RequireComponent(typeof(RectTransform))]
public abstract class MenuBase : MonoBehaviour
{
    public abstract void Init<T>(InitalizationHandle<T> handle);
    public abstract void CloseWindow();
    protected virtual void ResetWindow(){}

}
