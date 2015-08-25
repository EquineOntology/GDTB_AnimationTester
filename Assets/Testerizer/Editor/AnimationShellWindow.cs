using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AnimationShellWindow : EditorWindow 
{	
	[SerializeField]
	List<Animator> _animatables;
	private bool _collectedAnimatables = false;
	private string[] _animatableNames;
	
	private int _currentAnimatablesIndex = 0;
	
	// CONSTANTS
	private const string ANIMATABLES_TEXT = "Animatables: ";
	
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
			Debug.Log("Collected animatables!");
		}
	}
	
	private void OnGUI ()
	{
		EditorGUILayout.BeginVertical();
		DrawListOfAnimatables();
		EditorGUILayout.EndVertical();
	}
	
	private void DrawListOfAnimatables()
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(ANIMATABLES_TEXT, EditorStyles.boldLabel);
		EditorGUILayout.Popup(_currentAnimatablesIndex, _animatableNames);
		EditorGUILayout.EndHorizontal();	
	}
	
	/// <summary>
	/// Updates the "animatables" list.
	/// </summary>
	private void UpdateAnimatables()
	{
		_animatables = CollectAnimatables();
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
	
	private string[] GetAnimatableNames(List<Animator> animatables)
	{
		var names = new string[animatables.Count];
		for(int i = 0; i < animatables.Count; i++)
		{
			names[i] = animatables[i].gameObject.name;	
		}		
		return names;
	}
}
