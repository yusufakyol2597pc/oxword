using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger
{
    public static void Log(string message)
    {
        Debug.Log(message);
    }

    public static void Log(string method, string message)
    {
        Debug.Log(method + ": " + message);
    }
}
