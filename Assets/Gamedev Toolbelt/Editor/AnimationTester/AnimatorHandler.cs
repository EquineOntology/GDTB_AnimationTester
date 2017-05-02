using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
#if UNITY_5_4 || UNITY_5_6
using UnityEngine.Experimental.Director;
#endif

namespace com.immortalhydra.gdtb.animationtester
{
    public static class AnimatorHandler
    {

#region FIELDS

    #if UNITY_5_6_OR_NEWER
        private static List<PlayableGraph> _playableGraphs = new List<PlayableGraph>();
    #endif

#endregion

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


    #if UNITY_5_6_OR_NEWER

        static AnimatorHandler() {
            EditorApplication.playmodeStateChanged += ModeChanged;
        }

        public static void PlayAnimation (Animator animator, AnimationClip clip)
        {
            var graph = PlayableGraph.CreateGraph();
            var output = graph.CreateAnimationOutput("Animation", animator);
            var playableClip = graph.CreateAnimationClipPlayable(clip);
            output.sourcePlayable = playableClip;
            graph.CreateAnimationClipPlayable(clip);
            graph.Play();

            _playableGraphs.Add(graph);
        }

        public static void ModeChanged ()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode &&
                EditorApplication.isPlaying )
            {
                DisposeOfPlayableGraphs();
            }
        }

        private static void DisposeOfPlayableGraphs()
        {
            foreach (var graph in _playableGraphs)
            {
                graph.Destroy();
            }
        }

    #elif UNITY_5_4

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