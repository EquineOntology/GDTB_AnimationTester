using UnityEngine;
using UnityEditor.Animations;
using System.Collections.Generic;

namespace com.immortalhydra.gdtb.animationtester
{
    public static class AnimationClipHandler
    {

#region METHODS

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
                    clipNames = new[] { "" };
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

#endregion

    }
}
