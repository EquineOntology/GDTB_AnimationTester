using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

public class CodeTODOs : EditorWindow
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

    private const string PREFS_CODINGTODOS = "GDTB_CodingTODOs_Enable";
    private const string PREFS_QQQTEMPLATE = "GDTB_CodingTODOs_QQQTemplate";

    [MenuItem("Gamedev toolbelt/CodeTODOs")]
    public static void Init()
    {
        // Get existing open window or if none, make a new one.
        CodeTODOs window = (CodeTODOs)EditorWindow.GetWindow(typeof(CodeTODOs));
        window.Show();
    }

    public void OnEnable()
    {
        CheckPrefs();
        _qqqs = CodeTODOsHelper.CheckAllScriptsForQQQs(out _qqqTasks, out _qqqScripts);
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
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Width(BOX_WIDTH));            
            
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal();            
            GUILayout.Space(10);
            EditorGUILayout.LabelField(_qqqs[i].Task, EditorStyles.boldLabel, GUILayout.Width(BOX_WIDTH - 20));            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("In \"" + _qqqs[i].Script + "\".", GUILayout.Width(BOX_WIDTH - 20));

            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
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
            CodeTODOsHelper.FindAllScripts();
            CodeTODOsHelper.CheckAllScriptsForQQQs(out _qqqTasks, out _qqqScripts);
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
    }
    
    private void CheckPrefs()
    {
        if(!EditorPrefs.HasKey(PREFS_CODINGTODOS))
        {
            EditorPrefs.SetBool(PREFS_CODINGTODOS, true);
        }
        
        if(!EditorPrefs.HasKey(PREFS_QQQTEMPLATE))
        {
            EditorPrefs.SetString(PREFS_QQQTEMPLATE, "QQQ");
        }
        else
        {
            QQQTemplate = EditorPrefs.GetString(PREFS_QQQTEMPLATE, "QQQ");
        }
    }
}