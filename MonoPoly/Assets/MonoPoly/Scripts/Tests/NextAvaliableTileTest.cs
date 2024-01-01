using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NextAvaliableTileTest
{
    public int TotalLength = 40;
    
    // A Test behaves as an ordinary method
    [Test]
    public void NextAvaliableTile_Pos39_Dice1()
    {
        int Result = GameManager.GetNextAvaliableTitle(39, 1, TotalLength);
        Debug.Log($"Result = [{Result}]");
        
        Assert.IsTrue(Result==0,$"Result = [{Result}]");
    }
    [Test]
    public void NextAvaliableTile_Pos39_Dice2()
    {
        int Result = GameManager.GetNextAvaliableTitle(39, 2, TotalLength);
        Debug.Log($"Result = [{Result}]");
        
        Assert.IsTrue(Result==0,$"Result = [{Result}]");
    }
    [Test]
    public void NextAvaliableTile_Pos39_Dice1to6()
    {
        int Result = GameManager.GetNextAvaliableTitle(39, 1, TotalLength);
        Debug.Log($"Result = [{Result}]");
        
        int Result2 = GameManager.GetNextAvaliableTitle(39, 2, TotalLength);
        Debug.Log($"Result2 = [{Result2}]");
        
        int Result3 = GameManager.GetNextAvaliableTitle(39, 3, TotalLength);
        Debug.Log($"Result3 = [{Result3}]");
        
        int Result4 = GameManager.GetNextAvaliableTitle(39, 4, TotalLength);
        Debug.Log($"Result4 = [{Result4}]");
        
        int Result5 = GameManager.GetNextAvaliableTitle(39, 5, TotalLength);
        Debug.Log($"Result5 = [{Result5}]");
        
        int Result6 = GameManager.GetNextAvaliableTitle(39, 6, TotalLength);
        Debug.Log($"Result4 = [{Result6}]");
        
        Assert.IsTrue(Result==0,$"Result = [{Result}]");
    }

    [Test]
    public void TestCreatObject()
    {
      // var returnValue=   ObjectPooler.CreateObject(typeof(GameObject));
      // Debug.Log($"Return value is of type {returnValue.GetType()}");
        
    }
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.

}
