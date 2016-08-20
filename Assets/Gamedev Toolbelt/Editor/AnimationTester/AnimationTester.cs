using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace com.immortalhydra.gdtb.animationtester
{
    public class AnimationTester : EditorWindow
    {
        public static AnimationTester Instance { get; private set; }
        public static bool IsOpen {
            get { return Instance != null; }
        }

        // Animatables: gameobjects with Animator component.
        private List<Animator> _animatables;
        private bool _collectedAnimatables = false;
        private string[] _animatableNames;
        private int _currentAnimatablesIndex = 0;

        // ControllersBackup: a backup of all original animator controllers.
        // Used to reassign the original controller to an animator.
        private Dictionary<int, RuntimeAnimatorController> _controllersBackup = new Dictionary<int, RuntimeAnimatorController>();

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



        [MenuItem ("Gamedev Toolbelt/Animation Tester")]
        static void Init ()
        {
            // Get existing open window or, if none exists, make a new one.
            var window = (AnimationTester)EditorWindow.GetWindow (typeof (AnimationTester));
            window.SetMinSize();
            window.Show();
        }

        private void OnEnable()
        {
            #if UNITY_5_3_OR_NEWER || UNITY_5_1 || UNITY_5_2
                titleContent = new GUIContent("AnimationTester");
            #else
                title = "AnimationTester";
            #endif

            Instance = this;

            /* Load current preferences (like colours, etc.).
             * We do this here so that most preferences are updated as soon as they're changed.
             */
            Preferences.GetAllPrefValues();

            LoadSkin();
            LoadStyles();

            // Populate list of gameobjects with animator, but only once.
            if(_collectedAnimatables == false)
            {
                _animatables = AnimationTesterHelper.GetObjectsWithAnimator();
                _collectedAnimatables = true;
                _animatableNames = AnimationTesterHelper.GetNames(_animatables);

                // Build the backups.
                _clipNamesBackup = AnimationTesterHelper.BuildClipNamesBackup(_animatables);
                _controllersBackup = AnimationTesterHelper.BuildControllersBackup(_animatables);

                //Debug.Log("Collected animatables!");
            }
        }


        private void OnHierarchyChange()
        {
            UpdateAnimatables();
            _currentAnimatablesIndex = 0;
            _currentClipIndex = 0;
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
            EditorGUILayout.LabelField(Constants.ANIMATABLES_LIST, EditorStyles.boldLabel, GUILayout.Width(Constants.LABEL_WIDTH));

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            var tempIndex = _currentAnimatablesIndex;
            _currentAnimatablesIndex = EditorGUILayout.Popup(_currentAnimatablesIndex, _animatableNames, GUILayout.Width(Constants.POPUP_WIDTH));

            // If the selected animatable changes, update the list of animations.
            if (tempIndex != _currentAnimatablesIndex && _currentAnimatablesIndex < _animatables.Count)
            {
                RevertToPreviousAnimator(_animatables[tempIndex]);
                _shouldUpdateClips = true;
            }

            GUILayout.Space(5);
            if(GUILayout.Button(Constants.UPDATE_ANIMATABLES_LIST, GUILayout.Width(Constants.BUTTON_WIDTH)))
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
            EditorGUILayout.LabelField(Constants.ANIMATABLE_CLIPS_LIST, EditorStyles.boldLabel, GUILayout.Width(Constants.LABEL_WIDTH));

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _currentClipIndex = EditorGUILayout.Popup(_currentClipIndex, _animatableClipNames, GUILayout.Width(Constants.POPUP_WIDTH));
            GUILayout.Space(5);
            if (GUILayout.Button(Constants.UPDATE_CLIP_NAMES_LIST, GUILayout.Width(Constants.BUTTON_WIDTH)))
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
            if(GUILayout.Button(Constants.PLAYBUTTON_TEXT, GUILayout.Width(Constants.BUTTON_WIDTH/2)))
            {
                if (!Application.isPlaying)
                {
                    var sceneWindow = (SceneView)EditorWindow.GetWindow(typeof(SceneView));
                    sceneWindow.ShowNotification(new GUIContent(Constants.ERROR_MUST_BE_IN_PLAY_MODE));
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


        /// Set the minSize of the window based on preferences.
        public void SetMinSize()
        {
            var window = GetWindow(typeof(AnimationTester)) as AnimationTester;
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                window.minSize = new Vector2(222f, 150f);
            }
            else
            {
                window.minSize = new Vector2(322f, 150f);
            }
        }


        /// Load CodeTODOs custom skin.
        public void LoadSkin()
        {
            /*
            _skin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
            */
        }


        /// Load custom styles and apply colors from preferences.
        public void LoadStyles()
        {
            /*
            _style_script = _skin.GetStyle("GDTB_CodeTODOs_script");
            _style_script.active.textColor = Preferences.Color_Tertiary;
            _style_script.normal.textColor = Preferences.Color_Tertiary;
            _style_task = _skin.GetStyle("GDTB_CodeTODOs_task");
            _style_task.active.textColor = Preferences.Color_Secondary;
            _style_task.normal.textColor = Preferences.Color_Secondary;
            _style_buttonText = _skin.GetStyle("GDTB_CodeTODOs_buttonText");
            _style_buttonText.active.textColor = Preferences.Color_Tertiary;
            _style_buttonText.normal.textColor = Preferences.Color_Tertiary;

            _skin.settings.selectionColor = Preferences.Color_Secondary;

            // Change scrollbar color.
            var scrollbar = Resources.Load(Constants.TEX_SCROLLBAR, typeof(Texture2D)) as Texture2D;
            #if UNITY_5 || UNITY_5_3_OR_NEWER
                scrollbar.SetPixel(0,0, Preferences.Color_Secondary);
            #else
                var pixels = scrollbar.GetPixels();
                // We do it like this because minimum texture size in older versions of Unity is 2x2.
                for(var i = 0; i < pixels.GetLength(0); i++)
                {
                    scrollbar.SetPixel(i, 0, Preferences.Color_Secondary);
                    scrollbar.SetPixel(i, 1, Preferences.Color_Secondary);
                }
            #endif

            scrollbar.Apply();
            _skin.verticalScrollbarThumb.normal.background = scrollbar;
            _skin.verticalScrollbarThumb.fixedWidth = 6;
            */
        }
    }
}