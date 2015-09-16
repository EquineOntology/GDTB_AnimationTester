using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class CodingShellHelper
{
    public static List<string> FindAllScripts()
    {        
        var allAssets = AssetDatabase.GetAllAssetPaths();
        var allScripts = new List<string>();
        
        foreach (var path in allAssets)
        {
            if (path.EndsWith(".cs") || path.EndsWith(".js"))
            {
                allScripts.Add(path);
            }
        }///QQQ bananas!
        return allScripts;
    }
    
    public static void CheckAllScriptsForQQQs(out List<string> scripts, out List<string> qqqs)
    {
        // Since we're rechecking everything, let's clear the two lists.
        scripts = new List<string>();
        qqqs = new List<string>();
        
        // First we collect all scripts in the project, and then we check them for QQQs.
        var AllScripts = FindAllScripts();
        foreach(var script in AllScripts)
        {
            var QQQsInScript = CheckScriptForQQQs(script);
            
            for(int i = 0; i < QQQsInScript.Count; i++)
            {
                // Since the string "QQQ" is repeated many times in these two files, its default value would give
                // a bunch of false positives in these two files. So either the token doesn't use the default value,
                // or we exclude these two files from the collection.
                if (CodingShell.QQQ != "QQQ" || (!script.EndsWith("CodingShell.cs") && !script.EndsWith("CodingShellHelper.cs")))
                {
                    scripts.Add(script);
                    qqqs.Add(QQQsInScript[i]);
                }
            }
        }
    }
    
//QQQMelonas
    private static List<string> CheckScriptForQQQs(string path)
    {
        var currentQQQs = new List<string>();

        var lines = File.ReadAllLines(path);
        string completeQQQ;            
        for (int i = 0; i < lines.Length; i++)
        {
            completeQQQ = "";
            if (lines[i].Contains(CodingShell.QQQ))
            {
                var index = lines[i].IndexOf(CodingShell.QQQ);
                var tempString = lines[i].Substring(index);
                tempString = tempString.Substring(CodingShell.QQQ.Length);
                tempString.Trim();
                completeQQQ += tempString;
                
                currentQQQs.Add(completeQQQ);
            }            
        }
        return currentQQQs;
    }
}