using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpicyHarissaLogger 
{
   public static void Log(string message)
   {
      Debug.Log(message);
   }
   public static void LogError(string message)
   {
      Debug.LogError(message);
   }
   //Same as Log Error but it will not be ignored by any symbol, brief it will be on shipping build
   public static void LogErrorCritical(string message)
   {
      Debug.LogError(message);
   }
}
