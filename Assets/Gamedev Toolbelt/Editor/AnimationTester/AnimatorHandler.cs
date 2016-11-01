using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Animations;
#if UNITY_5_4_OR_NEWER
using UnityEngine.Experimental.Director;
#endif

namespace com.immortalhydra.gdtb.animationtester
{
    public static class AnimatorHandler
    {

#region METHODS

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


    #if UNITY_5_4_OR_NEWER

        public static void PlayAnimation (Animator animator, AnimationClip clip)
        {
            var playableClip = AnimationClipPlayable.Create(clip);
            animator.Play(playableClip);
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

    #endif

#endregion

    }
}