using UnityEngine;
using UnityEditor.Animations;
using System.Collections.Generic;

public static class AnimationShellHelper {
	
	public static void PlayAnimation(Animator animator, AnimationClip clip)
	{
        var controller = CreateControllerFromClip(clip);
        animator.runtimeAnimatorController = controller;
        animator.Play(clip.name);
	}
	
	private static AnimatorController CreateControllerFromClip(AnimationClip clip)
    {
        var tempClip = clip;
        tempClip.wrapMode = WrapMode.Loop;
        var controller = new AnimatorController();
        controller.name = "TestController";
        controller.AddLayer("Base");
        controller.layers[0].stateMachine.AddState(tempClip.name);
        controller.layers[0].stateMachine.states[0].state.motion = tempClip;

        return controller;
    }
	
	public static string[] GetNames(AnimationClip[] animationClips)
    {
        var nameHolder = new string[animationClips.Length];
        for (int i = 0; i < animationClips.Length; i++)
        {
            nameHolder[i] = animationClips[i].name;
        }
        return nameHolder;
    }
	
	public static string[] GetNames(List<Animator> animatables)
	{
		var nameHolder = new string[animatables.Count];
		for(int i = 0; i < animatables.Count; i++)
		{
			nameHolder[i] = animatables[i].gameObject.name;
        }		
		return nameHolder;
	}

    public static List<Animator> GetObjectsWithAnimator()
    {
        var allObjectsInScene = GameObject.FindObjectsOfType<GameObject>();
        var gos = new List<Animator>();

        foreach (var go in allObjectsInScene)
        {
            if (go.activeInHierarchy && go.GetComponent<Animator>() != null)
            {
                gos.Add(go.GetComponent<Animator>());
            }
        }
        return gos;
    }

   /* public static AnimationClip GetClipByName(string name, Animator animator)
    {
		var clips = UnityEditor.AnimationUtility.GetAnimationClips(animator.gameObject);
        var clipNames = GetNames(clips);
		
        for (int i = 0; i < clipNames.Length; i++)
        {
            if (clipNames[i] == name)
            {
                return clips[i];
            }
        }
        return null;
    }*/
	
	
}
