using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class AnimationShellWindow : EditorWindow 
{	
	// Animatables: gameobjects with Animator component.
	//[SerializeField]
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
	private const string UPDATE_ANIMATABLES_LIST = "Update";
	private const string UPDATE_CLIP_NAMES_LIST = "Update";
	
	private const int LABEL_WIDTH = 150;
	private const int POPUP_WIDTH = 150;
	private const int BUTTON_WIDTH = 50;
	
	[MenuItem ("Testerizer/Load Animation Shell")]
	static void Init ()
	{		
		// Get existing open window or if none, make a new one.
		var window = (AnimationShellWindow)EditorWindow.GetWindow (typeof (AnimationShellWindow));
		window.Show();
	}
	
	private void OnEnable()
	{
		// Populate list of gameobjects with animator, but only if it isn't already.
		if(_collectedAnimatables == false)
		{
			_animatables = CollectAnimatables();
			_collectedAnimatables = true;
			_animatableNames = GetAnimatableNames(_animatables);
			//Debug.Log("Collected animatables!");
		}
	}
	
	private void OnGUI ()
	{
		EditorGUILayout.BeginVertical();
		GUILayout.Space(10);
		DrawListOfAnimatables();
		GUILayout.Space(10);
		if(_animatables != null)
		{
			try
			{
				DrawListOfAnimations(_animatables[_currentAnimatablesIndex]);
				//Debug.Log("Drawing list of animations");
			}
			catch(Exception e)
			{
				Debug.Log(e.Source);
				_animatables = null;
				_currentAnimatablesIndex = -1;
				_collectedAnimatables = false;
				
				NullAnimatableClips();
			}
		}
		else
		{
			if(_animatableClips != null)
			{
				NullAnimatableClips();
			}
		}
		EditorGUILayout.EndVertical();
	}
	
	private void DrawListOfAnimatables()
	{
		EditorGUILayout.BeginVertical();	
		EditorGUILayout.LabelField(ANIMATABLES_LIST, EditorStyles.boldLabel, GUILayout.Width(LABEL_WIDTH));
						
		EditorGUILayout.BeginHorizontal();
		_currentAnimatablesIndex = EditorGUILayout.Popup(_currentAnimatablesIndex, _animatableNames, GUILayout.Width(POPUP_WIDTH));
		GUILayout.Space(5);
		if(GUILayout.Button(UPDATE_ANIMATABLES_LIST, GUILayout.Width(BUTTON_WIDTH)))
		{
			UpdateAnimatables();
			Repaint();
		}
		EditorGUILayout.Space();
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.EndVertical();
	}
	
	private void DrawListOfAnimations(Animator animatable)
	{
		if(_animatableClips == null)
		{			
			UpdateClipsAndNames(animatable);
		}
		
		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField(ANIMATABLE_CLIPS_LIST, EditorStyles.boldLabel, GUILayout.Width(LABEL_WIDTH));
		
		EditorGUILayout.BeginHorizontal();
		_currentClipIndex = EditorGUILayout.Popup(_currentClipIndex, _animatableClipNames, GUILayout.Width(POPUP_WIDTH));
		GUILayout.Space(5);
		if(GUILayout.Button(UPDATE_CLIP_NAMES_LIST, GUILayout.Width(BUTTON_WIDTH)))
		{
			UpdateClipsAndNames(animatable);
			Repaint();
		}
		EditorGUILayout.Space();
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.EndVertical();		
	}
	
	/// <summary>
	/// Updates the "animatables" list.
	/// </summary>
	private void UpdateAnimatables()
	{
		_animatables = CollectAnimatables();
		//Debug.Log("Updating \"animatables\" list");
	}
	
	/// <summary>
	/// Updates an animator's list of clips (and their names).
	/// </summary>
	/// <param name="animatable">The animator whose clips you want listed.</param>
	private void UpdateClipsAndNames(Animator animatable)
	{
		_animatableClips = UnityEditor.AnimationUtility.GetAnimationClips(animatable.gameObject);
		_animatableClipNames = GetAnimationClipNames(_animatableClips);
		//Debug.Log("Updating \"clips\" list");
	}
	
	/// <summary>
	/// Collects all objects with an Animator component in the scene, and puts them in a list.
	/// </summary>
	/// <returns>A List<Animator> populated with all objects with an animator in the scene.</returns>
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
	
	/// <summary>
    /// Creates an array containing the names of the animatable gameobjects.
    /// </summary>
    /// <param name="animatables">The list of animatables.</param>
    /// <returns>An array populated with the names.</returns>
	private string[] GetAnimatableNames(List<Animator> animatables)
	{
		var names = new string[animatables.Count];
		for(int i = 0; i < animatables.Count; i++)
		{
			names[i] = animatables[i].gameObject.name;	
		}		
		return names;
	}
	
	/// <summary>
    /// Creates an array containing the names of the clips.
    /// </summary>
    /// <param name="animationClips">The array containing the animation clips.</param>
    /// <returns>An array populated with the clip names.</returns>
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
		_currentClipIndex = -1;
	}
}
