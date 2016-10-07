using UnityEngine;
using UnityEditor.Animations;
using System.Collections.Generic;
#if UNITY_5_4_OR_NEWER
using UnityEngine.Experimental.Director;
#endif

namespace com.immortalhydra.gdtb.animationtester
{
    public static class AnimationTesterHelper
    {

        #if UNITY_5_4_OR_NEWER
        public static void PlayAnimation (Animator animator, AnimationClip clip)
        {
            var playableClip = AnimationClipPlayable.Create(clip);
            animator.Play(playableClip);
        }
        public static void PlayAnimation (Animation animation, AnimationClip clip)
        {
            animation.Play(clip.name);
        }


        #else
        public static void PlayAnimation(Animator animator, AnimationClip clip)
        {
            var controller = CreateControllerFromClip(clip);
            CloneParameters(controller, animator);
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
        private static void CloneParameters(AnimatorController controller, Animator animator)
        {
            controller.parameters = animator.parameters;
        }
        public static Dictionary <int, RuntimeAnimatorController> BuildControllersBackup(List<Animator> animators)
        {
            var backup = new Dictionary<int, RuntimeAnimatorController>();

            foreach(var x in animators)
            {
                var key = x.GetInstanceID();
                var controller = x.runtimeAnimatorController;
                backup.Add(key, controller);
            }

            return backup;
        }
        #endif


        public static string[] GetNames(AnimationClip[] animationClips)
        {
            var nameHolder = new string[animationClips.Length];
            for (int i = 0; i < animationClips.Length; i++)
            {
                nameHolder[i] = animationClips[i].name;
            }
            return nameHolder;
        }


        public static string[] GetNames(List<Animator> animators, List<Animation> animations)
        {
            var nameHolder = new string[animators.Count + animations.Count];
            for (int i = 0; i < animators.Count; i++)
            {
                nameHolder[i] = animators[i].gameObject.name;
            }
            for (int j = 0; j < animations.Count; j++)
            {
                nameHolder[animators.Count + j] = animations[j].gameObject.name;
            }

            return nameHolder;
        }


        public static string[] GetNames(Animator animator)
        {
            var clips = UnityEditor.AnimationUtility.GetAnimationClips(animator.gameObject);
            var names = GetNames(clips);

            return names;
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


        public static List<Animation> GetObjectsWithAnimation()
        {
            var allObjectsInScene = GameObject.FindObjectsOfType<GameObject>();
            var gos = new List<Animation>();

            foreach (var go in allObjectsInScene)
            {
                if (go.activeInHierarchy && go.GetComponent<Animation>() != null)
                {
                    gos.Add(go.GetComponent<Animation>());
                }
            }

            return gos;
        }


        public static Dictionary<int, string[]> BuildClipNamesBackup(List<Animator> animators, List<Animation> animations)
        {
            var backup = new Dictionary<int, string[]>();

            foreach (Animator anim in animators)
            {
                var key = anim.GetInstanceID();
                string[] clipNames;

                if (UnityEditor.AnimationUtility.GetAnimationClips(anim.gameObject) != null)
                {
                    clipNames = GetNames(anim);
                }
                else
                {
                    clipNames = new string[] { "" };
                }

                backup.Add(key, clipNames);
            }

            foreach (Animation animation in animations)
            {
                var key = animation.GetInstanceID();
                var clipNames = new List<string>();
                foreach(AnimationState state in animation)
                {
                    if(state.clip != null)
                    {
                        clipNames.Add(state.clip.name);
                    }
                    else
                    {
                        clipNames.Add("");
                    }
                }

                var namesArray = new string[clipNames.Count];
                for(var i = 0; i < clipNames.Count; i++)
                {
                    namesArray[i] = clipNames[i];
                }
                backup.Add(key, namesArray);
            }

            return backup;
        }
    }
}
