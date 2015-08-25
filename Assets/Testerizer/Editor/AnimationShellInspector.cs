using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof (AnimationShell))]
public class AnimationShellInspector : Editor
{
    private AnimationShell _aniShell;

    private AnimBool _showButtons;
    private int _currentAnimationIndex = -1;
    private string[] _beautifulNames;

    // Constants, to easily modify labels and the such.
    private const int BUTTON_WIDTH = 50;
    private const string ERR_MUST_BE_IN_PLAY_MODE = "To play animations you need to be in Play mode.";
    private const string BUTTON_TEXT = "Play";
    private const string ANIM_NAME_TEXT = "Animations";
    private const string NOW_PLAYING_TEXT = "Playing: ";

    public void OnEnable()
    {
        InitializeVariables();
        _showButtons.valueChanged.AddListener(Repaint);
    }

    /// <summary>
    /// Draw the inspector.
    /// </summary>
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginVertical();
        if (_currentAnimationIndex != -1)
        {
            DrawCurrentAnimationStats();
        }
        EditorGUILayout.EndVertical();

        _showButtons.target = EditorGUILayout.ToggleLeft("Show Animations", _showButtons.target);

        if (EditorGUILayout.BeginFadeGroup(_showButtons.faded))
        {
            DrawAnimationsPanel();
        }
        EditorGUILayout.EndFadeGroup();
    }

    /// <summary>
    /// Draw the stats of the animation currently playing.
    /// </summary>
    private void DrawCurrentAnimationStats()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(NOW_PLAYING_TEXT, EditorStyles.boldLabel, GUILayout.Width((Screen.width - 65)/2 - 15));
        EditorGUILayout.LabelField(_aniShell.AnimationClipNames[_currentAnimationIndex], GUILayout.Width((Screen.width - 65)/2 - 15));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Draw the list of animations.
    /// </summary>
    private void DrawAnimationsPanel()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.Width(BUTTON_WIDTH));
        EditorGUILayout.LabelField(ANIM_NAME_TEXT, EditorStyles.boldLabel, GUILayout.Width((Screen.width - 65)/2 - 15));
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < _aniShell.AnimationClipNames.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(BUTTON_TEXT, GUILayout.Width(BUTTON_WIDTH)))
            {
                if (!Application.isPlaying)
                {
                    Debug.LogWarning(ERR_MUST_BE_IN_PLAY_MODE);
                }
                else
                {
                    _aniShell.PlayAnimation(_aniShell.AnimationClipNames[i]);
                    _currentAnimationIndex = i;
                }
            }
            EditorGUILayout.LabelField(_beautifulNames[i], GUILayout.Width((Screen.width - 65) - 15));
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// Makes a camelCase string pretty.
    /// </summary>
    /// <param name="originalString">The string you want beautified.</param>
    /// <returns>A beautifully subdivided and capitalized string.</returns>
    private string BeautifyString(string originalString)
    {
        var workString = originalString;        
        workString = SplitCamelCase(workString);
        
        var beautifulString = workString;        
        return beautifulString;
    }

    /// <summary>
    /// Initializations.
    /// </summary>
    private void InitializeVariables()
    {
        _aniShell = serializedObject.targetObject as AnimationShell;
        _showButtons = new AnimBool(false);   
        _beautifulNames = new string[_aniShell.AnimationClipNames.Length];
        
        for(int i = 0; i < _aniShell.AnimationClipNames.Length; i++)
        {
            _beautifulNames[i] = BeautifyString(_aniShell.AnimationClipNames[i]);
        }
    }
    
    /// <summary>
    /// Splits a camelCase string in words.
    /// </summary>
    /// <param name="originalString">The string you want split.</param>
    /// <returns>A string with divided words.</returns>    
    public string SplitCamelCase(string originalString)
    {
        var splitString = System.Text.RegularExpressions.Regex.Replace(originalString, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        return splitString;      
    }
}