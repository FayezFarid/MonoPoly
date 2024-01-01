using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct InitalizationHandle<T> 
{
    public T Data;

    public InitalizationHandle(T data)
    {
        Data = data;
    }
    //Common Data.
}