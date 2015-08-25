using UnityEngine;
using UnityEditor.Animations;

[ExecuteInEditMode]

public class AnimationShell : MonoBehaviour
{
    [SerializeField] // If you don't, the script won't work. 
    private Animator _animator;

    [SerializeField] // If you don't, the script won't work. 
    public string[] AnimationClipNames;

    private AnimationClip[] _clips;
 
    private void Awake()
    {
        if (!Application.isPlaying)
        {
            AssignAnimator();
        }

        InitializeVariables();
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

    /// <summary>
    /// Creates a custom AnimatorController containing a single animation clip.
    /// </summary>
    /// <param name="clip">The clip will be added to the new controller.</param>
    /// <returns>The new AnimatorController.</returns>
    private AnimatorController CreateControllerFromClip(AnimationClip clip)
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

    /// <summary>
    /// Plays an animation through a custom AnimatorController.
    /// </summary>
    /// <param name="animationName">The name of the clip to play.</param>
    public void PlayAnimation(string animationName)
    {
        var clip = GetAnimationClipFromName(animationName);
        var controller = CreateControllerFromClip(clip);
        _animator.runtimeAnimatorController = controller;
        _animator.Play(clip.name);
    }

    /// <summary>
    /// Get the reference to an Animation Clip from its name.
    /// </summary>
    /// <param name="clipName">The name of the clip.</param>
    /// <returns>Reference to the AnimationClip, if the clip exists.</returns>
    private AnimationClip GetAnimationClipFromName(string clipName)
    {
        for (int i = 0; i < AnimationClipNames.Length; i++)
        {
            if (AnimationClipNames[i] == clipName)
            {
                return _clips[i];
            }
        }
        return null;
    }

    /// <summary>
    /// If the function has an animator get a reference to it, otherwise add an Animator to the gameobject.
    /// </summary>
    private void AssignAnimator()
    {
        if (GetComponent<Animator>() != null)
        {
            _animator = GetComponent<Animator>();
        }
        else
        {
            gameObject.AddComponent<Animator>();
            _animator = GetComponent<Animator>();
        }
    }

    /// <summary>
    /// Initializations.
    /// </summary>
    private void InitializeVariables()
    {
        _clips = UnityEditor.AnimationUtility.GetAnimationClips(this.gameObject);
        AnimationClipNames = GetAnimationClipNames(_clips);
    }
}