using UnityEngine;
using System.Collections.Generic;

namespace com.immortalhydra.gdtb.animationtester
{
    public static class AnimationHandler
    {
        public static void PlayAnimation (Animation animation, AnimationClip clip)
        {
            animation.Play(clip.name);
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
    }
}