using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace com.immortalhydra.gdtb.animationtester
{
    public class AnimationTester : EditorWindow
    {
        // Animatables: gameobjects with Animator component.
        private List<Animator> _animatables;
        private bool _collectedAnimatables = false;
        private string[] _animatableNames;
        private int _currentAnimatablesIndex = 0;

        // ControllersBackup: a backup of all original animator controllers.
        // Used to reassign the original controller to an animator.
        private Dictionary<int, RuntimeAnimatorController> _controllersBackup = new Dictionary<int, RuntimeAnimatorController>();
        private bool _controllerBackupBuilt = false;

        // AnimatableClips: Animation Clips of an animatable.
        private AnimationClip[] _animatableClips;
        private int _currentClipIndex = 0;
        private string[] _animatableClipNames;
        private bool _shouldUpdateClips = true;

        // ClipNamesBackup: a "backup" of the clip names of an animatable.
        // This is needed because otherwise, when refreshing the list of clips when another animatable is selected,
        // the controller that will be examined is the test one, which only has a single clip in it.
        private Dictionary<int,string[]> _clipNamesBackup = new  Dictionary<int,string[]>();

        // CONSTANTS
        private const string ANIMATABLES_LIST = "Select gameobject: ";
        private const string ANIMATABLE_CLIPS_LIST = "Select clip: ";
        private const string UPDATE_ANIMATABLES_LIST = "Refresh list";
        private const string UPDATE_CLIP_NAMES_LIST = "Refresh list";
        private const string PLAYBUTTON_TEXT = "Play";

        private const int LABEL_WIDTH = 150;
        private const int POPUP_WIDTH = 150;
        private const int BUTTON_WIDTH = 100;

        private const int EDITOR_WINDOW_MINSIZE_X = 300;
        private const int EDITOR_WINDOW_MINSIZE_Y = 160;

        private const string ERROR_NO_ANIMATABLES = "There are no gameobjects with an Animator component in the scene.";
        private const string ERROR_ANIMATABLE_NOT_FOUND = "The selected gameobject was not found.\nDid you remove the object while the window was open? If so, please click on \"Refresh list\" and try again.";
        private const string ERROR_MUST_BE_IN_PLAY_MODE = "To play an animation you must be in Play mode.";


        [MenuItem ("Gamedev Toolbelt/Animation Tester")]
        static void Init ()
        {
            // Get existing open window or, if none exists, make a new one.
            var window = (AnimationTester)EditorWindow.GetWindow (typeof (AnimationTester));
            window.minSize = new Vector2(EDITOR_WINDOW_MINSIZE_X, EDITOR_WINDOW_MINSIZE_Y);
            window.Show();
        }

        private void OnEnable()
        {
            // Populate list of gameobjects with animator, but only once.
            if(_collectedAnimatables == false)
            {
                _animatables = AnimationTesterHelper.GetObjectsWithAnimator();
                _collectedAnimatables = true;
                _animatableNames = AnimationTesterHelper.GetNames(_animatables);

                // Build the backups.
                _clipNamesBackup = AnimationTesterHelper.BuildClipNamesBackup(_animatables);
                _controllersBackup = AnimationTesterHelper.BuildControllersBackup(_animatables);
                _controllerBackupBuilt = true;

                //Debug.Log("Collected animatables!");
            }
        }

        private void OnGUI ()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Space(15);
            DrawListOfAnimatables();
            GUILayout.Space(15);
            if (_animatables != null && _currentAnimatablesIndex < _animatables.Count)
            {
                DrawListOfAnimations(_animatables[_currentAnimatablesIndex]);
            }
            EditorGUILayout.EndVertical();
        }

        // Draws the popup with the list of gameobjects with animatables
        private void DrawListOfAnimatables()
        {
            //Debug.Log("Drawing list of animations");
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(ANIMATABLES_LIST, EditorStyles.boldLabel, GUILayout.Width(LABEL_WIDTH));

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            var tempIndex = _currentAnimatablesIndex;
            _currentAnimatablesIndex = EditorGUILayout.Popup(_currentAnimatablesIndex, _animatableNames, GUILayout.Width(POPUP_WIDTH));

            // If the selected animatable changes, update the list of animations.
            if (tempIndex != _currentAnimatablesIndex && _currentAnimatablesIndex < _animatables.Count)
            {
                RevertToPreviousAnimator(_animatables[tempIndex]);
                _shouldUpdateClips = true;
            }

            GUILayout.Space(5);
            if(GUILayout.Button(UPDATE_ANIMATABLES_LIST, GUILayout.Width(BUTTON_WIDTH)))
            {
                UpdateAnimatables();
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        // Draws the popup with the list of animations.
        private void DrawListOfAnimations(Animator animatable)
        {
            if (_shouldUpdateClips == true)
            {
                UpdateClips(animatable);
                _shouldUpdateClips = false;
            }
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(ANIMATABLE_CLIPS_LIST, EditorStyles.boldLabel, GUILayout.Width(LABEL_WIDTH));

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _currentClipIndex = EditorGUILayout.Popup(_currentClipIndex, _animatableClipNames, GUILayout.Width(POPUP_WIDTH));
            GUILayout.Space(5);
            if (GUILayout.Button(UPDATE_CLIP_NAMES_LIST, GUILayout.Width(BUTTON_WIDTH)))
            {

                UpdateClips(animatable);
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);
            DrawPlayButton();
            EditorGUILayout.EndVertical();
        }

        private void DrawPlayButton()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if(GUILayout.Button(PLAYBUTTON_TEXT, GUILayout.Width(BUTTON_WIDTH/2)))
            {
                if (!Application.isPlaying)
                {
                    var sceneWindow = (SceneView)EditorWindow.GetWindow(typeof(SceneView));
                    sceneWindow.ShowNotification(new GUIContent(ERROR_MUST_BE_IN_PLAY_MODE));
                }
                else
                {
                    AnimationTesterHelper.PlayAnimation(_animatables[_currentAnimatablesIndex], _animatableClips[_currentClipIndex]);
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }

        private void UpdateAnimatables()
        {
            if(_animatables[_currentAnimatablesIndex] != null)
            {
                RevertToPreviousAnimator(_animatables[_currentAnimatablesIndex]);
            }
            _animatables.Clear();
            _animatables = AnimationTesterHelper.GetObjectsWithAnimator();
            _animatableNames = null;
            _animatableNames = AnimationTesterHelper.GetNames(_animatables);
            _clipNamesBackup.Clear();
            _clipNamesBackup = AnimationTesterHelper.BuildClipNamesBackup(_animatables);
            _controllersBackup.Clear();
            _controllersBackup = AnimationTesterHelper.BuildControllersBackup(_animatables);
            //Debug.Log("Updating \"animatables\" list");
        }

        private void UpdateClips(Animator animatable)
        {
            UpdateClipsList(animatable);
            UpdateClipNamesList(animatable);
        }

        private void UpdateClipNamesList(Animator animatable)
        {
            var key = animatable.GetInstanceID();
            _clipNamesBackup.TryGetValue(key, out _animatableClipNames);
            //Debug.Log("Updating \"clips\" lists");
        }

        private void UpdateClipsList(Animator animatable)
        {
            _animatableClips = UnityEditor.AnimationUtility.GetAnimationClips(animatable.gameObject);
        }

        private void RevertToPreviousAnimator(Animator anim)
        {
            var key = anim.GetInstanceID();
            RuntimeAnimatorController originalAnimator;

            var gottenValue = _controllersBackup.TryGetValue(key, out originalAnimator);

            if (gottenValue == true)
            {
                anim.runtimeAnimatorController = originalAnimator;
            }
        }

        private void OnHierarchyChange()
        {
            UpdateAnimatables();
            _currentAnimatablesIndex = 0;
            _currentClipIndex = 0;
        }
    }
}