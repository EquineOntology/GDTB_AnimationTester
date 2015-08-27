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

   private string[] GetAnimationClipNames(AnimationClip[] animationClips)
    {
        var nameHolder = new string[animationClips.Length];
        for (int i = 0; i < animationClips.Length; i++)
        {
            nameHolder[i] = animationClips[i].name;
        }
        return nameHolder;
    }

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

    public void PlayAnimation(string animationName)
    {
        var clip = GetAnimationClipFromName(animationName);
        var controller = CreateControllerFromClip(clip);
        _animator.runtimeAnimatorController = controller;
        _animator.Play(clip.name);
    }

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

    private void InitializeVariables()
    {
        _clips = UnityEditor.AnimationUtility.GetAnimationClips(this.gameObject);
        AnimationClipNames = GetAnimationClipNames(_clips);
    }
}