using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace com.immortalhydra.gdtb.animationtester
{
    public class WindowMain : EditorWindow
    {

#region FIELDS AND PROPERTIES

    // Constants.
    private int _iconSize = Constants.ICON_SIZE;
    private int _buttonWidth = Constants.BUTTON_WIDTH;
    private int _buttonHeight = Constants.BUTTON_HEIGHT;
    private int _popupWidth = Constants.POPUP_WIDTH;

	// Fields.
    // Animatables: gameobjects with Animator/Animation component.
    private List<Animator> _animators;
    private List<Animation> _animations;
    private bool _collectedAnimatables = false;
    private string[] _animatableNames;
    private int _currentAnimatablesIndex = 0;

    // ControllersBackup: a backup of all original animator controllers.
    // Used to reassign the original controller to an animator for Unity<5.4.
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
    private int _offset = 5;
    private GUISkin _skin;
    private GUIStyle _style_buttonText, _style_boldLabel;

	// Properties.
    public static WindowMain Instance { get; private set; }
    public static bool IsOpen {
        get { return Instance != null; }
    }
#endregion

#region MONOBEHAVIOUR METHODS

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
                _animators = AnimatorHandler.GetObjectsWithAnimator();
                _animations = AnimationHandler.GetObjectsWithAnimation();
                _collectedAnimatables = true;
                _animatableNames = AnimationClipHandler.GetNames(_animators, _animations);

                // Build the backups.
                _clipNamesBackup = AnimationClipHandler.BuildClipNamesBackup(_animators, _animations);

                #if !UNITY_5_4_OR_NEWER
                _controllersBackup = AnimatorHandler.BuildControllersBackup(_animators);
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
                    DrawListOfAnimations();
                }

                DrawPlay();
            }
            DrawSettings();
        }


        // Unfortunately, IMGUI is not really responsive to events, e.g. changing the style of a button
        // (like when you press it) shows some pretty abysmal delays in the GUI, the button will light up
        // and down too late after the actual click. We force the UI to update more often instead.
        public void Update()
        {
            Repaint();
        }


        public void OnDestroy()
        {
            CloseOtherWindows();
        }
#endregion

#region METHODS

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




        [MenuItem("Window/Gamedev Toolbelt/AnimationTester/Open AnimationTester %t", false, 1)]
        private static void Init()
        {
            // If AnimationTester has not been initialized, or EditorPrefs have been lost for some reason, reset them to default.
            if(!EditorPrefs.HasKey("GDTB_AnimationTester_initialized") || EditorPrefs.GetBool("GDTB_AnimationTester_initialized", false) == false)
            {
                Preferences.InitPrefs();
            }

            // Get existing open window or, if none exists, make a new one.
            var window = (WindowMain)EditorWindow.GetWindow (typeof (WindowMain));
            window.minSize = new Vector2(270f, 150f);
            window.LoadSkin();
            window.LoadStyles();

            window.Show();

            if(Preferences.ShowWelcome == true)
            {
                WindowWelcome.Init();
            }
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
                if(tempIndex < _animators.Count)
                {
                    RevertToPreviousAnimator(_animators[tempIndex]);
                }

                UpdateClips();
            }

            Rect prevGameobjectRect;
            GUIContent prevGameobjectContent;
            SetupButton_PreviousGameobject(out prevGameobjectRect, out prevGameobjectContent);

            if (Controls.Button(prevGameobjectRect, prevGameobjectContent))
            {
                PreviousAnimatable();
                UpdateClips();
            }

            Rect nextGameobjectRect;
            GUIContent nextGameobjectContent;
            SetupButton_NextGameobject(out nextGameobjectRect, out nextGameobjectContent);

            if (Controls.Button(nextGameobjectRect, nextGameobjectContent))
            {
                NextAnimatable();
                UpdateClips();
            }
        }


        // Draws the popup with the list of animations for currently selected animatable.
        private void DrawListOfAnimations()
        {
            if (_shouldUpdateClips == true)
            {
                UpdateClips();
                _shouldUpdateClips = false;
            }

            var labelRect = new Rect(_offset, _iconSize * 2.5f, _popupWidth, _buttonHeight);
            var popupRect = new Rect(_offset, _iconSize * 2.3f + _offset * 5, _popupWidth, _buttonHeight);

            EditorGUI.LabelField(labelRect, "Select clip:", _style_boldLabel);
            _currentClipIndex = EditorGUI.Popup(popupRect, _currentClipIndex, _animatableClipNames);


            Rect prevClipRect;
            GUIContent prevClipContent;

            SetupButton_PreviousClip(out prevClipRect, out prevClipContent);
            if (Controls.Button(prevClipRect, prevClipContent))
            {
                PreviousClip();
            }

            Rect nextClipRect;
            GUIContent nextClipContent;

            SetupButton_NextClip(out nextClipRect, out nextClipContent);
            if (Controls.Button(nextClipRect, nextClipContent))
            {
                NextClip();
            }
        }


        /// Draw the "Play" button.
        private void DrawPlay()
        {
            Rect playRect;
            GUIContent playContent;


            SetupButton_Play(out playRect, out playContent);
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
                        AnimatorHandler.PlayAnimation(_animators[_currentAnimatablesIndex], _animatableClips[_currentClipIndex]);
                    }
                    else
                    {
                        AnimationHandler.PlayAnimation(_animations[_currentAnimatablesIndex - _animators.Count], _animatableClips[_currentClipIndex]);
                    }
                }
            }
        }


        /// Draw the "Settings" button.
        private void DrawSettings()
        {
            Rect settingsRect;
            GUIContent settingsContent;

            SetupButton_Settings(out settingsRect, out settingsContent);

            if(Controls.Button(settingsRect, settingsContent))
            {
                // Unfortunately EditorApplication.ExecuteMenuItem(...) doesn't work, so we have to rely on a bit of reflection.
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
                var type = assembly.GetType("UnityEditor.PreferencesWindow");
                var method = type.GetMethod("ShowPreferencesWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                method.Invoke(null, null);
            }
        }


        private void SetupButton_PreviousGameobject(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(_popupWidth + _offset * 2, _iconSize * 1.5f  + _offset / 2 - _buttonHeight / 2, _buttonWidth - 20, _buttonHeight);
            aContent = new GUIContent("Prev", "Previous gameobject");
        }

        private void SetupButton_NextGameobject(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(_popupWidth + _buttonWidth - _offset, _iconSize * 1.5f  + _offset / 2 - _buttonHeight / 2, _buttonWidth - 20, _buttonHeight);
            aContent = new GUIContent("Next", "Next gameobject");
        }


        private void SetupButton_PreviousClip(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(_popupWidth + _offset * 2,  _iconSize * 4 - _offset / 2 - _buttonHeight / 2, _buttonWidth - 20, _buttonHeight);
            aContent = new GUIContent("Prev", "Previous clip");
        }

        private void SetupButton_NextClip(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(_popupWidth + _buttonWidth - _offset,  _iconSize * 4 - _offset / 2 - _buttonHeight / 2, _buttonWidth - 20, _buttonHeight);
            aContent = new GUIContent("Next", "Next clip");
        }


        private void SetupButton_Play(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(0,0, _buttonWidth, _buttonHeight);
            aRect.x = position.width / 2 - _buttonWidth / 2;
            aRect.y = _iconSize * 4 + _offset * 3.5f;
            aContent = new GUIContent("Play", "Play selected clip");
        }


        private void SetupButton_Settings(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(_offset, position.height - _buttonHeight - _offset, _buttonWidth, _buttonHeight);
            aContent = new GUIContent("Settings", "Open Settings");
        }


        // Change index of currently selected animatable to previous one in the list, and go around to the last one if index == 0.
        private void PreviousAnimatable()
        {
            // First, we revert the current animator to its original runtimeController.
            if(_currentAnimatablesIndex < _animators.Count)
            {
                RevertToPreviousAnimator(_animators[_currentAnimatablesIndex]);
            }

            if(_currentAnimatablesIndex != 0)
            {
                _currentAnimatablesIndex--;
            }
            else
            {
                _currentAnimatablesIndex = _animators.Count + _animations.Count - 1;
            }
        }


        // Change index of currently selected animatable to next one in the list, and go around to the first one if max index.
        private void NextAnimatable()
        {
            // First, we revert the current animator to its original runtimeController.
            if(_currentAnimatablesIndex < _animators.Count)
            {
                RevertToPreviousAnimator(_animators[_currentAnimatablesIndex]);
            }

            if(_currentAnimatablesIndex < (_animators.Count + _animations.Count - 1))
            {
                _currentAnimatablesIndex++;
            }
            else if (_currentAnimatablesIndex == (_animators.Count + _animations.Count - 1))
            {
                _currentAnimatablesIndex = 0;
            }
        }


        // Change index of currently selected clip to previous one in the list, and go around to the last one if index == 0.
        private void PreviousClip()
        {
            if(_currentClipIndex > 0)
            {
                _currentClipIndex--;
            }
            else
            {
                _currentClipIndex = _animatableClips.Length - 1;
            }
        }


        // Change index of currently selected clip to next one in the list, and go around to the first one if max index.
        private void NextClip()
        {
            if(_currentClipIndex < _animatableClips.Length - 1)
            {
                _currentClipIndex++;
            }
            else
            {
                _currentClipIndex = 0;
            }
        }


        /// Update the list of animatables.
        private void UpdateAnimatables()
        {
            // If the user removes a controller from the scene in the middle of a draw call, the index in the for loop stays the same but _animatables.Count diminishes.
            // Since this is pretty much inevitable, and will correct itself next frame, what we do is swallow the exception and wait for the next draw call.
            try
            {
                if( _animators.Count != 0 &&
                    _currentAnimatablesIndex < _animators.Count  &&
                    _animators[_currentAnimatablesIndex] != null)
                {
                    RevertToPreviousAnimator(_animators[_currentAnimatablesIndex]);
                }

                _animators.Clear();
                _animations.Clear();
                _animators = AnimatorHandler.GetObjectsWithAnimator();
                _animations = AnimationHandler.GetObjectsWithAnimation();
                _animatableNames = null;
                _animatableNames = AnimationClipHandler.GetNames(_animators, _animations);
                _clipNamesBackup.Clear();
                _clipNamesBackup = AnimationClipHandler.BuildClipNamesBackup(_animators, _animations);

                _controllersBackup.Clear();
                _controllersBackup = AnimatorHandler.BuildControllersBackup(_animators);

            }
            catch (System.Exception) {}
        }


        private void UpdateClips()
        {
            if(_currentAnimatablesIndex < _animators.Count)
            {
                UpdateClips(_animators[_currentAnimatablesIndex]);
            }
            else
            {
                UpdateClips(_animations[_currentAnimatablesIndex - _animators.Count]);
            }
        }


    #region Animator
        /// Update the clips from an animatable
        private void UpdateClips(Animator animatable)
        {
            UpdateClipsList(animatable);
            UpdateClipNamesList(animatable);
            _currentClipIndex = 0;
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
    #endregion


    #region Animation
        private void UpdateClips(Animation animatable)
        {
            UpdateClipsList(animatable);
            UpdateClipNamesList(animatable);
            _currentClipIndex = 0;
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


        /// Switch to the original animator.
        private void RevertToPreviousAnimator(Animator animator)
        {
            var key = animator.GetInstanceID();
            RuntimeAnimatorController originalAnimator;

            var gottenValue = _controllersBackup.TryGetValue(key, out originalAnimator);

            if (gottenValue == true)
            {
                animator.runtimeAnimatorController = originalAnimator;
            }
        }


        /// Close open sub-windows (add, edit) when opening prefs.
        private void CloseOtherWindows()
        {
            if (WindowWelcome.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowWelcome)).Close();
            }

            if (WindowHelp.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowHelp)).Close();
            }
        }

#endregion

    }
}
