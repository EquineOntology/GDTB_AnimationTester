using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AnimationShellWindow : EditorWindow 
{	
	// Animatables: gameobjects with Animator component.
	private List<Animator> _animatables;
	private bool _collectedAnimatables = false;
	private string[] _animatableNames;	
	private int _currentAnimatablesIndex = 0;
	
	// AnimatableClips: Animation Clips of an animatable.
	private AnimationClip[] _animatableClips;
	private int _currentClipIndex = 0;
	private string[] _animatableClipNames;
	
	// CONSTANTS
	private const string ANIMATABLES_LIST = "Select gameobject: ";
	private const string ANIMATABLE_CLIPS_LIST = "Select clip: ";
	private const string UPDATE_ANIMATABLES_LIST = "Refresh list";
	private const string UPDATE_CLIP_NAMES_LIST = "Refresh list";
	
	private const string ERROR_NO_ANIMATABLES = "There are no gameobjects with an Animator component in the scene.";
    private const string ERROR_ANIMATABLE_NOT_FOUND = "The selected gameobject was not found.\nDid you remove the object while the window was open? If so, please click on \"Refresh list\" and try again.";
    private const int LABEL_WIDTH = 150;
	private const int POPUP_WIDTH = 150;
	private const int BUTTON_WIDTH = 100;

    private const int EDITOR_WINDOW_MINSIZE_X = 300;
    private const int EDITOR_WINDOW_MINSIZE_Y = 200;

    [MenuItem ("Testerizer/Load Animation Shell")]
	static void Init ()
	{		
		// Get existing open window or if none, make a new one.
		var window = (AnimationShellWindow)EditorWindow.GetWindow (typeof (AnimationShellWindow));
        window.minSize = new Vector2(EDITOR_WINDOW_MINSIZE_X, EDITOR_WINDOW_MINSIZE_Y);
        window.Show();
	}
	
	private void OnEnable()
	{
		// Populate list of gameobjects with animator, but only if it isn't already.
		if(_collectedAnimatables == false)
		{
			_animatables = CollectAnimatables();
			_collectedAnimatables = true;
			_animatableNames = GetAnimatableNames();
			//Debug.Log("Collected animatables!");
		}
	}
	
	private void OnGUI ()
	{
		EditorGUILayout.BeginVertical();
		GUILayout.Space(15);
		DrawListOfAnimatables();
		GUILayout.Space(15);
		
        if (_animatables != null && _currentAnimatablesIndex < _animatables.Count)
        {
            DrawListOfAnimations(_animatables[_currentAnimatablesIndex]);		
        }
        /*try
        {

            
        }
        catch(Exception)
        {
            //Debug.LogWarning(ERROR_NO_ANIMATABLES);
            _animatables = null;
            _currentAnimatablesIndex = -1;
            _collectedAnimatables = false;

            NullAnimatableClips();
        }
        if (_animatableClips != null)
        {
            NullAnimatableClips();
        }*/
        EditorGUILayout.EndVertical();
	}
	
	private void DrawListOfAnimatables()
	{
		//Debug.Log("Drawing list of animations");
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField(ANIMATABLES_LIST, EditorStyles.boldLabel, GUILayout.Width(LABEL_WIDTH));
						
		EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        var tempIndex = _currentAnimatablesIndex;
		_currentAnimatablesIndex = EditorGUILayout.Popup(_currentAnimatablesIndex, _animatableNames, GUILayout.Width(POPUP_WIDTH));

        // If the selected animatable changes, update the list of animations.
        if (tempIndex != _currentAnimatablesIndex && _currentAnimatablesIndex < _animatables.Count)
        {
            UpdateClipsAndNames(_animatables[_currentAnimatablesIndex]);
        }
        GUILayout.Space(5);
		if(GUILayout.Button(UPDATE_ANIMATABLES_LIST, GUILayout.Width(BUTTON_WIDTH)))
		{
			UpdateAnimatables();
		}
		EditorGUILayout.Space();
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.EndVertical();
	}
	
	private void DrawListOfAnimations(Animator animatable)
	{
		bool updatedClipLists = UpdateClipsAndNames(animatable);

        if (updatedClipLists == true)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(ANIMATABLE_CLIPS_LIST, EditorStyles.boldLabel, GUILayout.Width(LABEL_WIDTH));

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _currentClipIndex = EditorGUILayout.Popup(_currentClipIndex, _animatableClipNames, GUILayout.Width(POPUP_WIDTH));
            GUILayout.Space(5);
            if (GUILayout.Button(UPDATE_CLIP_NAMES_LIST, GUILayout.Width(BUTTON_WIDTH)))
            {
                UpdateClipsAndNames(animatable);
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
    }
	
	private void UpdateAnimatables()
	{
        if (_animatables != null)
        {
            _animatables.Clear();
        }
        _animatables = CollectAnimatables();
        _animatableNames = GetAnimatableNames();
        //Debug.Log("Updating \"animatables\" list");
    }
	
	private bool UpdateClipsAndNames(Animator animatable)
	{
        if (UnityEditor.AnimationUtility.GetAnimationClips(animatable.gameObject) != null)
        {
            _animatableClips = UnityEditor.AnimationUtility.GetAnimationClips(animatable.gameObject);
            _animatableClipNames = GetAnimationClipNames(_animatableClips);
            
			// Only return true if there is actually something in those arrays.
			if(_animatableClips.Length > 0)
			{
                return true;
            }
        }
        return false;
        //Debug.Log("Updating \"clips\" lists");
        
    }
	
	private List<Animator> CollectAnimatables()
	{
		var allObjectsInScene = GameObject.FindObjectsOfType<GameObject>();
        var animatorsInScene = new List<Animator>();
		
        foreach(var go in allObjectsInScene)
		{
			if(go.activeInHierarchy && go.GetComponent<Animator>() != null)
			{
				animatorsInScene.Add(go.GetComponent<Animator>());
            }
		}		
		return animatorsInScene;
	}
	
	private string[] GetAnimatableNames()
	{
		var names = new string[_animatables.Count];
		for(int i = 0; i < _animatables.Count; i++)
		{
			names[i] = _animatables[i].gameObject.name;
        }		
		return names;
	}
	
	private string[] GetAnimationClipNames(AnimationClip[] animationClips)
    {
        var nameHolder = new string[animationClips.Length];
        for (int i = 0; i < animationClips.Length; i++)
        {
            nameHolder[i] = animationClips[i].name;
        }
        return nameHolder;
    }
	
	private void NullAnimatableClips()
	{
		_animatableClips = null;
		_animatableClipNames = null;
		_currentClipIndex = 0;
	}

    private void OnHierarchyChange()
    {
        UpdateAnimatables();
        _currentAnimatablesIndex = 0;
        _currentClipIndex = 0;
    }
}