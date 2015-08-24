using UnityEngine;
using UnityEditor;

[CustomEditor(typeof (Animator))]
public class AnimatorInspector : Editor
{
    private bool _showAddAnimationGemButton = false;
    private Animator _anim;
    private GameObject _targetObject;

    public void OnEnable()
    {
		InitializeVariables ();

        if (HasAnimationGem() == false)
        {
            _showAddAnimationGemButton = true;
        }
        else
        {
            _showAddAnimationGemButton = false;
        }
    }

    public override void OnInspectorGUI()
    {
		serializedObject.Update ();
        DrawDefaultInspector();

        if (_showAddAnimationGemButton)
        {
			EditorGUILayout.BeginVertical();
			EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
            if (GUILayout.Button("Testerize", GUILayout.Width(75)))
            {
                _targetObject.AddComponent<AnimationShell>();
            }
			EditorGUILayout.Space();
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Separator();
			EditorGUILayout.EndVertical();
        }
    }

    private bool HasAnimationGem()
    {
        if (_targetObject.GetComponent<AnimationShell>() == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

	public void InitializeVariables()
	{
		_anim = serializedObject.targetObject as Animator;
		_targetObject = _anim.gameObject;
	}
}