using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LogLevel
{
   Verbose,
   Standard,
   Critical,
}
public static class SpicyHarissaLogger
{
   public static readonly string[] DebugStrings = new[] { "Tiles", "GameManager" };
   //TODO: dont just declare things and not use them 
   public static void Log(string message,LogLevel logLevel,string DebugKey="")
   {
      Debug.Log(message);
   }
   public static void Log(string message,LogLevel logLevel,Color color,string DebugKey="")
   {
   
      string ColorString =    ColorUtility.ToHtmlStringRGB(color);
      Debug.Log($"<color=#{ColorString}>"+ message +"</color>");
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
