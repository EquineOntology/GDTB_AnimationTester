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

        // Animatables: gameobjects with Animator/Animation component.
        private List<Animator> _animators;
        private List<Animation> _animations;
        private bool _collectedAnimatables = false;
        private string[] _animatableNames;
        private int _currentAnimatablesIndex = 0;

        #if !UNITY_5_4_OR_NEWER
        // ControllersBackup: a backup of all original animator controllers.
        // Used to reassign the original controller to an animator for Unity<5.4.
        private Dictionary<int, RuntimeAnimatorController> _controllersBackup = new Dictionary<int, RuntimeAnimatorController>();
        #endif

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
        private int _buttonWidth = Constants.BUTTON_WIDTH;
        private int _buttonHeight = Constants.BUTTON_HEIGHT;
        private int _popupWidth = Constants.POPUP_WIDTH;
        private int _offset = 5;

        private GUISkin _skin;
        private GUIStyle _style_buttonText, _style_boldLabel;

        [MenuItem("Window/Gamedev Toolbelt/AnimationTester %t")]
        static void Init()
        {
            // If AnimationTester has not been initialized, or EditorPrefs have been lost for some reason, reset them to default.
            if(!EditorPrefs.HasKey("GDTB_AnimationTester_initialized") || EditorPrefs.GetBool("GDTB_AnimationTester_initialized", false) == false)
            {
                Preferences.InitPrefs();
            }

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

            // Populate list of gameobjects with animator/animation, but only once.
            if(_collectedAnimatables == false)
            {
                _animators = AnimationTesterHelper.GetObjectsWithAnimator();
                _animations = AnimationTesterHelper.GetObjectsWithAnimation();
                _collectedAnimatables = true;
                _animatableNames = AnimationTesterHelper.GetNames(_animators, _animations);

                // Build the backups.
                _clipNamesBackup = AnimationTesterHelper.BuildClipNamesBackup(_animators, _animations);

                #if !UNITY_5_4_OR_NEWER
                _controllersBackup = AnimationTesterHelper.BuildControllersBackup(_animators);
                #endif
            }
        }


        private void OnHierarchyChange()
        {
            UpdateAnimatables();
            _currentAnimatablesIndex = 0;
            _currentClipIndex = 0;
            _shouldUpdateClips = true;
        }


        private void OnGUI()
        {
            DrawWindowBackground();

            // If there are no animatables in the scene, tell the user.
            if (_animations.Count == 0 && _animators.Count == 0)
            {
                DrawNoAnimatablesMessage();
            }
            else
            {
                DrawListOfAnimatables();
                if (_currentAnimatablesIndex < (_animators.Count + _animations.Count))
                {
                    if(_currentAnimatablesIndex < _animators.Count)
                    {
                        DrawListOfAnimations(_animators[_currentAnimatablesIndex]);
                    }
                    else
                    {
                        DrawListOfAnimations(_animations[_currentAnimatablesIndex - _animators.Count]);
                    }
                }

                DrawPlay();
            }
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
            var label = "There are currently no gameobjects\nwith compatible animations in the scene.";
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


        // Draws the popup with the list of gameobjects with animatables, and the select button.
        private void DrawListOfAnimatables()
        {
            var labelRect = new Rect(_offset, _offset, _popupWidth, _buttonHeight);
            var popupRect = new Rect(_offset, _offset * 5, _popupWidth, _buttonHeight);
            EditorGUI.LabelField(labelRect, "Select gameobject:", _style_boldLabel);

            var tempIndex = _currentAnimatablesIndex;
            _currentAnimatablesIndex = EditorGUI.Popup(popupRect, _currentAnimatablesIndex, _animatableNames);
            // If the selected animatable changes, update the list of animations.
            if (tempIndex != _currentAnimatablesIndex && _currentAnimatablesIndex < (_animators.Count + _animations.Count))
            {
                #if !UNITY_5_4_OR_NEWER
                if(tempIndex < _animators.Count)
                {
                    RevertToPreviousAnimator(_animators[tempIndex]);
                }
                #endif

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
            if (Controls.Button(animatablesRect, animatablesContent))
            {
                UpdateAnimatables();
            }
        }

        private void Button_Animatables_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(_popupWidth + _offset * 3, _iconSize * 1.5f  + _offset / 2 - _buttonHeight / 2, _buttonWidth, _buttonHeight);
            aContent = new GUIContent("Select", "Select this gameobject");
        }
        private void Button_Animatables_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect (_popupWidth + _offset * 3,  _iconSize + _offset * 2.5f - _iconSize / 2, _iconSize, _iconSize);
            aContent = new GUIContent(DrawingUtils.Texture_Select, "Select this gameobject");
        }


        private void Button_Refresh_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(_popupWidth + _offset * 3,  _iconSize * 4 - _offset / 2 - _buttonHeight / 2, _buttonWidth, _buttonHeight);
            aContent = new GUIContent("Refresh", "Refresh list of animations");
        }
        private void Button_Refresh_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect (_popupWidth + _offset * 3, _iconSize * 3 +_offset * 3.5f - _iconSize / 2, _iconSize, _iconSize);
            aContent = new GUIContent(DrawingUtils.Texture_Refresh, "Refresh list of animations");
        }


        /// Draw the "Play" button.
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

            if(Controls.Button(playRect, playContent))
            {
                if(!Application.isPlaying)
                {
                    var sceneWindow = (SceneView)EditorWindow.GetWindow(typeof(SceneView));
                    sceneWindow.ShowNotification(new GUIContent("To play an animation you must be in Play mode."));
                }
                else
                {
                    if(_currentAnimatablesIndex < _animators.Count)
                    {
                        AnimationTesterHelper.PlayAnimation(_animators[_currentAnimatablesIndex], _animatableClips[_currentClipIndex]);
                    }
                    else
                    {
                        AnimationTesterHelper.PlayAnimation(_animations[_currentAnimatablesIndex - _animators.Count], _animatableClips[_currentClipIndex]);
                    }
                }
            }
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
            aContent = new GUIContent(DrawingUtils.Texture_Play, "Play selected clip");
        }


        /// Draw the "Settings" button.
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

            if(Controls.Button(settingsRect, settingsContent))
            {
                // Unfortunately EditorApplication.ExecuteMenuItem(...) doesn't work, so we have to rely on a bit of reflection.
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
                var type = assembly.GetType("UnityEditor.PreferencesWindow");
                var method = type.GetMethod("ShowPreferencesWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                method.Invoke(null, null);
            }
        }

        private void Button_Settings_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(_offset, position.height - _buttonHeight - _offset, _buttonWidth, _buttonHeight);
            aContent = new GUIContent("Settings", "Open Settings");
        }

        private void Button_Settings_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(_offset, position.height - _iconSize - _offset, _iconSize, _iconSize);
            aContent = new GUIContent(DrawingUtils.Texture_Settings, "Open Settings");
        }


        /// Update the list of animatables.
        private void UpdateAnimatables()
        {
            // If the user removes a controller from the scene in the middle of a draw call, the index in the for loop stays the same but _animatables.Count diminishes.
            // Since this is pretty much inevitable, and will correct itself next frame, what we do is swallow the exception and wait for the next draw call.
            try
            {
                #if !UNITY_5_4_OR_NEWER
                if( _animators.Count != 0 &&
                    _currentAnimatablesIndex < _animators.Count  &&
                    _animators[_currentAnimatablesIndex] != null)
                {
                    RevertToPreviousAnimator(_animators[_currentAnimatablesIndex]);
                }
                #endif

                _animators.Clear();
                _animations.Clear();
                _animators = AnimationTesterHelper.GetObjectsWithAnimator();
                _animations = AnimationTesterHelper.GetObjectsWithAnimation();
                _animatableNames = null;
                _animatableNames = AnimationTesterHelper.GetNames(_animators, _animations);
                _clipNamesBackup.Clear();
                _clipNamesBackup = AnimationTesterHelper.BuildClipNamesBackup(_animators, _animations);

                #if !UNITY_5_4_OR_NEWER
                _controllersBackup.Clear();
                _controllersBackup = AnimationTesterHelper.BuildControllersBackup(_animators);
                #endif
            }
            catch (System.Exception ) { }
        }


#region Animator

        // Draws the popup with the list of animations for objects with an Animator.
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

            // Refresh list from the animation controller, but only if in play mode (otherwise throws exception).
            if (Controls.Button(refreshRect, refreshContent))
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
        }

        /// Update the clips from an animatable
        private void UpdateClips(Animator animatable)
        {
            UpdateClipsList(animatable);
            UpdateClipNamesList(animatable);
        }

        /// Update the list of clip names from an animatable.
        private void UpdateClipNamesList(Animator animatable)
        {
            var key = animatable.GetInstanceID();
            _clipNamesBackup.TryGetValue(key, out _animatableClipNames);
        }

        /// Update the list of animationClips from an animatable.
        private void UpdateClipsList(Animator animatable)
        {
            _animatableClips = UnityEditor.AnimationUtility.GetAnimationClips(animatable.gameObject);
        }

        #if !UNITY_5_4_OR_NEWER
        /// Switch to the original animator.
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
        #endif

#endregion


#region Animation

        // Draw the popup with the list of animations for objects with an Animation.
        private void DrawListOfAnimations(Animation animatable)
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

            // Refresh list from the animation controller, but only if in play mode (otherwise throws exception).
            if (Controls.Button(refreshRect, refreshContent))
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
        }

        private void UpdateClips(Animation animatable)
        {
            UpdateClipsList(animatable);
            UpdateClipNamesList(animatable);
        }

        private void UpdateClipsList(Animation animatable)
        {
            var clips = new List<AnimationClip>();
            foreach(AnimationState state in animatable)
            {
                clips.Add(state.clip);
            }

            _animatableClips = new AnimationClip[clips.Count];
            for(var i = 0; i < clips.Count; i++)
            {
                _animatableClips[i] = clips[i];
            }
        }

        private void UpdateClipNamesList(Animation animatable)
        {
            var key = animatable.GetInstanceID();
            _clipNamesBackup.TryGetValue(key, out _animatableClipNames);
        }
#endregion


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


        // Unfortunately, IMGUI is not really responsive to events, e.g. changing the style of a button
        // (like when you press it) shows some pretty abysmal delays in the GUI, the button will light up
        // and down too late after the actual click. We force the UI to update more often instead.
        public void Update()
        {
            Repaint();
        }
    }
}
