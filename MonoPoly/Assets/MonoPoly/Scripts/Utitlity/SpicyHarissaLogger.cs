#define SUPPRESS_MOVEMENT_LOG
#define SUPPRES_DICE_LOG

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
    public static string MOVEMENT_DEBUG_KEY = "MOVEMENT";
    public static string DICE_DEBUG_KEY = "DICE";

    //TODO: dont just declare things and not use them 
    public static void Log(string message, LogLevel logLevel, string DebugKey = "")
    {
        if (DebugKey == "MOVEMENT")
        {
#if SUPPRESS_MOVEMENT_LOG
            return;
#endif
        }

        if (DebugKey == "DICE")
        {
#if SUPPRES_DICE_LOG
            return;
#endif
        }

        Debug.Log(message);
    }

    public static void Log(string message, LogLevel logLevel, Color color, string DebugKey = "")
    {
        string ColorString = ColorUtility.ToHtmlStringRGB(color);
        Debug.Log($"<color=#{ColorString}>" + message + "</color>");
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