using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

public class CodingShell : EditorWindow
{
    public static string QQQTemplate = "QQQ";
    public static int charLimitBeforeNewline = 60;

    // QQQ
    public ReorderableList _reorderableQQQs;
    public List<QQQ> _qqqs = new List<QQQ>();

    //private List<int> _priorities = new List<int>();
    private List<string> _qqqScripts = new List<string>();
    private List<string> _qqqTasks = new List<string>();

    // CONSTANTS (labels, etc).
    private const int BUTTON_WIDTH = 100;
    private const int BOX_WIDTH = 400;
    private const string LIST_QQQS = "Refresh list";

    [MenuItem("Testerizer/Load Coding Shell")]
    public static void Init()
    {       
        // Get existing open window or if none, make a new one.
        CodingShell window = (CodingShell)EditorWindow.GetWindow(typeof(CodingShell));
        window.Show();
    }

    public void OnEnable()
    {
        _qqqs = CodingShellHelper.CheckAllScriptsForQQQs(out _qqqTasks, out _qqqScripts);
        /*_reorderableQQQs = new ReorderableList(_qqqs, typeof(QQQ), true, true, true, true);

        _reorderableQQQs.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = _reorderableQQQs.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("Task"));
            EditorGUI.PropertyField(
                new Rect(rect.x + 60, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("Script"));
        };*/
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        DrawQQQList();
        EditorGUILayout.Space();
        DrawListButton();
        EditorGUILayout.BeginVertical();
    }
    
    private void DrawQQQList()
    {
        EditorGUILayout.BeginVertical();
        
        for(int i= 0; i< _qqqs.Count; i++)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(BOX_WIDTH));
            
            EditorGUILayout.BeginHorizontal();            
            GUILayout.Space(10);
            EditorGUILayout.LabelField(_qqqs[i].Task, EditorStyles.boldLabel);            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("In \"" + _qqqs[i].Script + "\".");

            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.EndVertical();

        //_reorderableQQQs.DoLayoutList();
        //_reorderableQQQs.DoList(new Rect(20, 20, 200, 200));
    }

    private void DrawListButton()
    {
        EditorGUILayout.BeginHorizontal();    
        EditorGUILayout.Space();
        if (GUILayout.Button(LIST_QQQS, GUILayout.Width(BUTTON_WIDTH)))
        {
            CodingShellHelper.FindAllScripts();
            CodingShellHelper.CheckAllScriptsForQQQs(out _qqqTasks, out _qqqScripts);
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
    }

    
}