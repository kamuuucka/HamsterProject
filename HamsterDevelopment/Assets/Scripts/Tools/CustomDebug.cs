using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum DebugColor
{
    grey,
    white,
    red,
    yellow,
    green,
    orange
}

public static class SuperDebug
{
    /// <summary>
    /// Log the message with a specific color. If no color used, it will be grey.
    /// </summary>
    /// <param name="message">Text of your message.</param>
    /// <param name="color">DebugColor that you want to use.</param>
    /// <param name="callerFilePath">Path to the file that called this method.</param>
    public static void Log(string message, DebugColor color = DebugColor.grey, [CallerFilePath] string callerFilePath = "")
    {
        var logColor = GetHashValue(color);
        Debug.Log($"{ShowScriptName(callerFilePath)}<color={logColor}>{message}");
    }

    /// <summary>
    /// Specific method for displaying bool values. True is always green, red is always false.
    /// </summary>
    /// <param name="value">Bool value.</param>
    /// <param name="message">Custom message about the bool.</param>
    /// <param name="callerFilePath">Path to the file that called this method.</param>
    public static void LogBool(bool value, string message = "Value of the boolean is: ", [CallerFilePath] string callerFilePath = "")
    {
        var boolColor = value ? DebugColor.green : DebugColor.red;
        
        Debug.Log($"{ShowScriptName(callerFilePath)}{message}<color={boolColor}>{value}");
    }
    
    /// <summary>
    /// Method used to display the name of the file that called any of the methods in this script.
    /// </summary>
    /// <param name="callerFilePath">Path to the file that called this method.</param>
    /// <returns>Name of the file that called the method.</returns>
    private static string ShowScriptName(string callerFilePath)
    {
        var scriptName = System.IO.Path.GetFileNameWithoutExtension(callerFilePath);
        return $"<color={DebugColor.white}>[Called from {scriptName}] ";
    }

    /// <summary>
    /// Gets hash value of any of the colors.
    /// </summary>
    /// <param name="color">DebugColor color that's value needs to be displayed.</param>
    /// <returns>Hash value of a specific color.</returns>
    private static string GetHashValue(DebugColor color)
    {
        return color switch
        {
            DebugColor.grey => "#cdd1ce",
            DebugColor.white => "#ffffff",
            DebugColor.red => "#ff0000",
            DebugColor.yellow => "#f2eb66",
            DebugColor.green => "#00ff00",
            DebugColor.orange => "#f2ca66",
            _ => "#000000"
        };
    }
    
    //public static void DisplayMessage(string message, 
    // [CallerFilePath] string callerFilePath = "")
    // {
    //     // Extract just the script name from the full path
    //     string scriptName = System.IO.Path.GetFileNameWithoutExtension(callerFilePath);
    //     
    //     Console.WriteLine($"[Called from {scriptName}] {message}");
    // }
}
