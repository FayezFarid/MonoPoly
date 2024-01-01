using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public interface IPoolableObject
{
    bool CanBePooled { get; set; }
    void PoolObject();
    
}


public static class ObjectPooler
{
    private static Dictionary<Type, Object> PrefabPool = new Dictionary<Type, Object>()
    {
        { MatchSettings.SingletonInstance.PlayerPrefab.GetType(), MatchSettings.SingletonInstance.PlayerPrefab }
    };
    private static Dictionary<Type, List<IPoolableObject>> ObjectsPool = new Dictionary<Type, List<IPoolableObject>>();

    // public static IPoolableObject RequestObject(Type type)
    // {
    //     if (ObjectsPool.ContainsKey(type))
    //     {
    //         var poolableObject= GetFirstAvaliableObject(ObjectsPool[type]);
    //         if (poolableObject == null)
    //         {
    //             
    //         }
    //     }
    //
    //     return null;
    // }

    // public static Object CreateObject(Type type )
    // {
    //     var objectToInstantite = PrefabPool[type];
    //     var objectInstance = MonoBehaviour.Instantiate(objectToInstantite);
    //     var casted= Convert.ChangeType(objectInstance, type);
    //     Debug.Log($"casted ");
    //     return casted;
    // }
    // private static IPoolableObject GetFirstAvaliableObject(List<IPoolableObject> list)
    // {
    //     foreach (var item in list)
    //     {
    //         if (item.CanBePooled)
    //         {
    //             return item;
    //         }
    //     }
    //
    //     return null;
    // }
}
