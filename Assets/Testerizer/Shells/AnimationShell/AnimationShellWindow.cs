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
    private bool _shouldUpdateClips = true;

    // CONSTANTS
    private const string ANIMATABLES_LIST = "Select gameobject: ";
	private const string ANIMATABLE_CLIPS_LIST = "Select clip: ";
	private const string UPDATE_ANIMATABLES_LIST = "Refresh list";
	private const string UPDATE_CLIP_NAMES_LIST = "Refresh list";
    private const string PLAYBUTTON_TEXT = "Play";
    
    private const int LABEL_WIDTH = 150;
	private const int POPUP_WIDTH = 150;
	private const int BUTTON_WIDTH = 100;
    
    private const int EDITOR_WINDOW_MINSIZE_X = 300;
    private const int EDITOR_WINDOW_MINSIZE_Y = 160;
    
	private const string ERROR_NO_ANIMATABLES = "There are no gameobjects with an Animator component in the scene.";
    private const string ERROR_ANIMATABLE_NOT_FOUND = "The selected gameobject was not found.\nDid you remove the object while the window was open? If so, please click on \"Refresh list\" and try again.";
    private const string ERROR_MUST_BE_IN_PLAY_MODE = "To play animations you need to be in Play mode.";
 

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
			_animatables = AnimationShellHelper.GetObjectsWithAnimator();
			_collectedAnimatables = true;
			_animatableNames = AnimationShellHelper.GetNames(_animatables);
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
        EditorGUILayout.EndVertical();
	}
	
    // Draws the popup with the list of gameobjects with animatables
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
            _shouldUpdateClips = false;
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
	
    // Draws the popup with the list of animations.
	private void DrawListOfAnimations(Animator animatable)
	{
		bool updatedClipLists = true;

        if (_shouldUpdateClips == true)
        {
            UpdateClipsAndNames(animatable);
            _shouldUpdateClips = false;
        }

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
                    
            GUILayout.Space(20);
            DrawPlayButton();
            EditorGUILayout.EndVertical();
        }
    }

    private void DrawPlayButton()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if(GUILayout.Button(PLAYBUTTON_TEXT, GUILayout.Width(BUTTON_WIDTH/2)))
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning(ERROR_MUST_BE_IN_PLAY_MODE);
            }
            else
            {
                AnimationShellHelper.PlayAnimation(_animatables[_currentAnimatablesIndex], _animatableClips[_currentClipIndex]);
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
    }

    private void UpdateAnimatables()
	{
        if (_animatables != null)
        {
            _animatables.Clear();
        }
        _animatables = AnimationShellHelper.GetObjectsWithAnimator();
        _animatableNames = AnimationShellHelper.GetNames(_animatables);
        //Debug.Log("Updating \"animatables\" list");
    }

    private bool UpdateClipsAndNames(Animator animatable)
    {
        _shouldUpdateClips = false;

        if (UnityEditor.AnimationUtility.GetAnimationClips(animatable.gameObject) != null)
        {
            _animatableClips = UnityEditor.AnimationUtility.GetAnimationClips(animatable.gameObject);
            _animatableClipNames = AnimationShellHelper.GetNames(_animatableClips);

            // Only return true if there is actually something in those arrays.
            if (_animatableClips.Length > 0)
            {
                return true;
            }
        }
        return false;
        //Debug.Log("Updating \"clips\" lists");        
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