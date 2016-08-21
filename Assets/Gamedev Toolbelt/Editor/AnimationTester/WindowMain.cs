using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace com.immortalhydra.gdtb.animationtester
{
    public class WindowMain : EditorWindow
    {
        public static WindowMain Instance { get; private set; }
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


        // Layouting and styling;
        private int _iconSize = Constants.ICON_SIZE;
        private int _buttonWidth = 70;
        private int _buttonHeight = 18;
        private int _popupWidth = Constants.POPUP_WIDTH;
        private int _offset = 5;

        private GUISkin _skin;
        private GUIStyle _style_buttonText, _style_boldLabel;

        [MenuItem("Window/Gamedev Toolbelt/AnimationTester %q")]
        static void Init()
        {
            // Get existing open window or, if none exists, make a new one.
            var window = (WindowMain)EditorWindow.GetWindow (typeof (WindowMain));
            window.SetMinSize();
            window.LoadSkin();
            window.LoadStyles();

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

            // Load current preferences (like colours, etc.).
            // We do this here so that most preferences are updated as soon as they're changed.
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
            }
        }


        private void OnHierarchyChange()
        {
            UpdateAnimatables();
            _currentAnimatablesIndex = 0;
            _currentClipIndex = 0;
        }


        private void OnGUI()
        {
            DrawWindowBackground();

            // If the list is empty, tell the user.
            if (_animatables.Count == 0)
            {
                DrawNoAnimatablesMessage();
            }

            DrawListOfAnimatables();
            if (_animatables != null && _currentAnimatablesIndex < _animatables.Count)
            {
                DrawListOfAnimations(_animatables[_currentAnimatablesIndex]);
            }

            DrawPlay();
            DrawSettings();
        }


        /// Draw the background texture.
        private void DrawWindowBackground()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), Preferences.Color_Primary);
        }


        /// If there are no Animatables, tell the user.
        private void DrawNoAnimatablesMessage()
        {
            var label = "There are currently no gameobjects with compatible animations in the scene.";
            var labelContent = new GUIContent(label);

            Vector2 labelSize;
            #if UNITY_5_3_OR_NEWER
                labelSize = EditorStyles.centeredGreyMiniLabel.CalcSize(labelContent);
            #else
                labelSize = EditorStyles.wordWrappedMiniLabel.CalcSize(labelContent);
            #endif

            var labelRect = new Rect(position.width / 2 - labelSize.x / 2, position.height / 2 - labelSize.y / 2 - _offset * 2.5f, labelSize.x, labelSize.y);
            #if UNITY_5_3_OR_NEWER
                EditorGUI.LabelField(labelRect, labelContent, EditorStyles.centeredGreyMiniLabel);
            #else
                EditorGUI.LabelField(labelRect, labelContent, EditorStyles.wordWrappedMiniLabel);
            #endif
        }


        // Draws the popup with the list of gameobjects with animatables
        private void DrawListOfAnimatables()
        {
            var labelRect = new Rect(_offset, _offset, _popupWidth, _buttonHeight);
            var popupRect = new Rect(_offset, _offset * 5, _popupWidth, _buttonHeight);
            //Debug.Log("Drawing list of animations");
            EditorGUI.LabelField(labelRect, "Select gameobject:", _style_boldLabel);

            var tempIndex = _currentAnimatablesIndex;
            _currentAnimatablesIndex = EditorGUI.Popup(popupRect, _currentAnimatablesIndex, _animatableNames);

            // If the selected animatable changes, update the list of animations.
            if (tempIndex != _currentAnimatablesIndex && _currentAnimatablesIndex < _animatables.Count)
            {
                RevertToPreviousAnimator(_animatables[tempIndex]);
                _shouldUpdateClips = true;
            }

            Rect animatablesRect;
            GUIContent animatablesContent;
            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    Button_Animatables_default(out animatablesRect, out animatablesContent);
                    break;
				case ButtonsDisplayFormat.COOL_ICONS:
                default:
                    Button_Animatables_icon(out animatablesRect, out animatablesContent);
                    break;
            }

            // Update list of animatables.
            if (GUI.Button(animatablesRect, animatablesContent))
            {
                UpdateAnimatables();
            }
            DrawingUtils.DrawButton(animatablesRect, Preferences.ButtonsDisplay, DrawingUtils.Texture_Complete, animatablesContent.text, _style_buttonText);
        }

        private void Button_Animatables_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(_popupWidth + _offset * 3, _iconSize * 1.5f  + _offset / 2 - _buttonHeight / 2, _buttonWidth, _buttonHeight);
            aContent = new GUIContent("Select", "Select this gameobject");
        }
        private void Button_Animatables_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect (_popupWidth + _offset * 3,  _iconSize + _offset * 2.5f - _iconSize / 2, _iconSize, _iconSize);
            aContent = new GUIContent("", "Select this gameobject");
        }


        // Draws the popup with the list of animations.
        private void DrawListOfAnimations(Animator animatable)
        {
            if (_shouldUpdateClips == true)
            {
                UpdateClips(animatable);
                _shouldUpdateClips = false;
            }

            var labelRect = new Rect(_offset, _iconSize * 2.5f, _popupWidth, _buttonHeight);
            var popupRect = new Rect(_offset, _iconSize * 2.3f + _offset * 5, _popupWidth, _buttonHeight);

            EditorGUI.LabelField(labelRect, "Select clip:", _style_boldLabel);
            _currentClipIndex = EditorGUI.Popup(popupRect, _currentClipIndex, _animatableClipNames);

            Rect refreshRect;
            GUIContent refreshContent;
            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    Button_Refresh_default(out refreshRect, out refreshContent);
                    break;
				case ButtonsDisplayFormat.COOL_ICONS:
                default:
                    Button_Refresh_icon(out refreshRect, out refreshContent);
                    break;
            }

            if (GUI.Button(refreshRect, refreshContent))
            {
                if (!Application.isPlaying)
                {
                    var sceneWindow = (SceneView)EditorWindow.GetWindow(typeof(SceneView));
                    sceneWindow.ShowNotification(new GUIContent("To refresh the list of animations you must be in Play mode."));
                }
                else
                {
                    UpdateClips(animatable);
                }
            }
            DrawingUtils.DrawButton(refreshRect, Preferences.ButtonsDisplay, DrawingUtils.Texture_Refresh, refreshContent.text, _style_buttonText);
        }

        private void Button_Refresh_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(_popupWidth + _offset * 3,  _iconSize * 4 - _offset / 2 - _buttonHeight / 2, _buttonWidth, _buttonHeight);
            aContent = new GUIContent("Refresh", "Refresh list of animations");
        }
        private void Button_Refresh_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect (_popupWidth + _offset * 3, _iconSize * 3 +_offset * 3.5f - _iconSize / 2, _iconSize, _iconSize);
            aContent = new GUIContent("", "Refresh list of animations");
        }


        private void DrawPlay()
        {
            Rect playRect;
            GUIContent playContent;
            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    Button_Play_default(out playRect, out playContent);
                    break;
				case ButtonsDisplayFormat.COOL_ICONS:
                default:
                    Button_Play_icon(out playRect, out playContent);
                    break;
            }

            if(GUI.Button(playRect, playContent))
            {
                if (!Application.isPlaying)
                {
                    var sceneWindow = (SceneView)EditorWindow.GetWindow(typeof(SceneView));
                    sceneWindow.ShowNotification(new GUIContent("To play an animation you must be in Play mode."));
                }
                else
                {
                    AnimationTesterHelper.PlayAnimation(_animatables[_currentAnimatablesIndex], _animatableClips[_currentClipIndex]);
                }
            }
            DrawingUtils.DrawButton(playRect, Preferences.ButtonsDisplay, DrawingUtils.Texture_Play, playContent.text, _style_buttonText);
        }

        private void Button_Play_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(0,0, _buttonWidth, _buttonHeight);
            aRect.x = ((_popupWidth + _offset * 3) + _buttonWidth) / 2 - _buttonWidth / 2;
            aRect.y = _iconSize * 4 + _offset * 3.5f;
            aContent = new GUIContent("Play", "Play selected clip");
        }
        private void Button_Play_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect (0, 0, _iconSize, _iconSize);
            aRect.x = ((_popupWidth + _offset * 3) + _iconSize) / 2 - _iconSize / 2;
            aRect.y = _iconSize * 4 + _offset * 3.5f;
            aContent = new GUIContent("", "Play selected clip");
        }


        private void DrawSettings()
        {
            Rect settingsRect;
            GUIContent settingsContent;
            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    Button_Settings_default(out settingsRect, out settingsContent);
                    break;
				case ButtonsDisplayFormat.COOL_ICONS:
                default:
                    Button_Settings_icon(out settingsRect, out settingsContent);
                    break;
            }

            if(GUI.Button(settingsRect, settingsContent))
            {
                // Unfortunately EditorApplication.ExecuteMenuItem(...) doesn't work, so we have to rely on a bit of reflection.
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
                var type = assembly.GetType("UnityEditor.PreferencesWindow");
                var method = type.GetMethod("ShowPreferencesWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                method.Invoke(null, null);
            }
            DrawingUtils.DrawButton(settingsRect, Preferences.ButtonsDisplay, DrawingUtils.Texture_Settings, settingsContent.text, _style_buttonText);
        }

        private void Button_Settings_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(_offset, position.height - _buttonHeight - _offset, _buttonWidth, _buttonHeight);
            aContent = new GUIContent("Settings", "Open Settings");
        }

        private void Button_Settings_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(_offset, position.height - _iconSize - _offset, _iconSize, _iconSize);
            aContent = new GUIContent("", "Open Settings");
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


        /// Load custom skin.
        public void LoadSkin()
        {
            _skin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }


        /// Load custom styles and apply colors from preferences.
        public void LoadStyles()
        {
            _style_buttonText = _skin.GetStyle("GDTB_AnimationTester_buttonText");
            _style_buttonText.active.textColor = Preferences.Color_Tertiary;
            _style_buttonText.normal.textColor = Preferences.Color_Tertiary;

            _style_boldLabel = _skin.GetStyle("GDTB_AnimationTester_bold");
            _style_boldLabel.active.textColor = Preferences.Color_Secondary;
            _style_boldLabel.normal.textColor = Preferences.Color_Secondary;

            _skin.settings.selectionColor = Preferences.Color_Secondary;
        }


        /// Set the minSize of the window based on preferences.
        public void SetMinSize()
        {
            var window = GetWindow(typeof(WindowMain)) as WindowMain;
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                window.minSize = new Vector2(190f, 123f);
            }
            else
            {
                window.minSize = new Vector2(243f, 121f);
            }
        }
    }
}
