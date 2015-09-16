using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CodingShell : EditorWindow
{
    public static string QQQ = "QQQ";

    private List<string> _qqqScripts = new List<string>();
    private List<string> _qqqs = new List<string>();

    // CONSTANTS (labels, etc).
    private const string LIST_QQQS = "List QQQs";

    [MenuItem("Testerizer/Load Coding Shell")]
    public static void Init()
    {
        // Get existing open window or if none, make a new one.
        CodingShell window = (CodingShell)EditorWindow.GetWindow(typeof(CodingShell));
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.Space();
        if (GUILayout.Button(LIST_QQQS))
        {
            CodingShellHelper.FindAllScripts();
            CodingShellHelper.CheckAllScriptsForQQQs(out _qqqScripts, out _qqqs);
            for (int i = 0; i < _qqqs.Count; i++)
            {
                Debug.Log(i + ": " + _qqqs[i] + " (in \"" + _qqqScripts[i] + "\").");
            }
        }
        EditorGUILayout.Space();

        EditorGUILayout.EndHorizontal();
    }
}
