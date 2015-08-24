using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof (AnimationShell))]
public class AnimationShellInspector : Editor
{
    private AnimationShell _aniShell;

    private AnimBool _showButtons;
    private int _currentAnimationIndex = -1;

    // Constants, to easily modify labels and the such.
    private const int BUTTON_WIDTH = 50;
    private const string ERR_MUST_BE_IN_PLAY_MODE = "To play animations you need to be in Play mode.";
    private const string BUTTON_TEXT = "Play";
    private const string ANIM_NAME_TEXT = "Animation";
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
            EditorGUILayout.LabelField(_aniShell.AnimationClipNames[i], GUILayout.Width((Screen.width - 65) - 15));
            EditorGUILayout.EndHorizontal();
        }
    }

    //QQQ
    private string BeautifyString(string originalString)
    {
        var workString = originalString;

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
    }
}