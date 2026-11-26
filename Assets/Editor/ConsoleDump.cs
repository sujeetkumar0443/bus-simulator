using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

[InitializeOnLoad]
public static class ConsoleDump
{
    static HashSet<string> seen = new HashSet<string>();
    static string path = "BusTest/console_dump.txt";
    static ConsoleDump()
    {
        Application.logMessageReceived -= OnLog;
        Application.logMessageReceived += OnLog;
    }
    static void OnLog(string msg, string stack, LogType t)
    {
        if (t == LogType.Log || t == LogType.Warning) return;
        string key = msg + stack;
        if (!seen.Add(key) || seen.Count > 40) return;
        Directory.CreateDirectory("BusTest");
        File.AppendAllText(path, "[" + t + "] " + msg + "\n" + stack + "\n----\n");
    }
}
