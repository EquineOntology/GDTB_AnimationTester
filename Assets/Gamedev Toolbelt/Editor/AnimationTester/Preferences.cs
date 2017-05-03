using UnityEngine;
using UnityEditor;

namespace com.immortalhydra.gdtb.animationtester
{
    public class Preferences
    {
#region FIELDS AND PROPERTIES
        // Welcome window.
        private const string _PREFS_ANIMATIONTESTER_WELCOME = "GDTB_AnimationTester_Welcome";
        private static bool _showWelcome = true;
        private const bool _SHOW_WELCOME_DEFAULT = true;

        public static bool ShowWelcome
        {
            get { return _showWelcome; }
        }

        // Primary color.
        private const string _PREFS_ANIMATIONTESTER_COLOR_PRIMARY = "GDTB_AnimationTester_Primary";
        private static Color _primary = new Color(56, 56, 56, 1);
        private static Color _primaryDark = new Color(56, 56, 56, 1);
        private static Color _primaryLight = new Color(255, 255, 255, 1);
        private static Color _primaryDefault = new Color(56, 56, 56, 1);
        public static Color Color_Primary
        {
            get { return _primary; }
        }

        // Secondary color.
        private const string _PREFS_ANIMATIONTESTER_COLOR_SECONDARY = "GDTB_AnimationTester_Secondary";
        private static Color _secondary = new Color(0, 162, 219, 1);
        private static Color _secondaryDark = new Color(0, 162, 219, 1);
        private static Color _secondaryLight = new Color(100, 171, 255, 1);
        private static Color _secondaryDefault = new Color(0, 162, 219, 1);
        public static Color Color_Secondary
        {
            get { return _secondary; }
        }

        // Tertiary color.
        private const string _PREFS_ANIMATIONTESTER_COLOR_TERTIARY = "GDTB_AnimationTester_Tertiary";
        private static Color _tertiary = new Color(255, 248, 248, 1);
        private static Color _tertiaryDark = new Color(255, 248, 248, 1);
        private static Color _tertiaryLight = new Color(56, 56, 56, 1);
        private static Color _tertiaryDefault = new Color(255, 248, 248, 1);
        public static Color Color_Tertiary
        {
            get { return _tertiary; }
        }

        // Custom shortcut
        private const string _PREFS_ANIMATIONTESTER_SHORTCUT = "GDTB_AnimationTester_Shortcut";
        private static string _shortcut = "%|t";
        private static string _newShortcut;
        private const string _SHORTCUT_DEFAULT = "%|t";

        public static string Shortcut
        {
            get { return _shortcut; }
        }
        private static bool[] _modifierKeys = new bool[] { false, false, false }; // Ctrl/Cmd, Alt, Shift.
        private static int _mainShortcutKeyIndex;
        // Want absolute control over values.
        private static string[] _shortcutKeys = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "LEFT", "RIGHT", "UP", "DOWN", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "HOME", "END", "PGUP", "PGDN" };


        private static Vector2 _scrollPosition = new Vector2(-1, 0);
#endregion

#region METHODS

        [PreferenceItem("AnimationTester")]
        public static void PreferencesGUI()
        {
            GetAllPrefValues();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);
            EditorGUILayout.LabelField("General settings", EditorStyles.boldLabel);
            _showWelcome = EditorGUILayout.Toggle("Show Welcome window", _showWelcome);
            _newShortcut = DrawShortcutSelector();
            GUILayout.Space(20);
            EditorGUILayout.LabelField("UI", EditorStyles.boldLabel);
            _primary = EditorGUILayout.ColorField("Background", _primary);
            _secondary = EditorGUILayout.ColorField("Bold text and button color", _secondary);
            _tertiary = EditorGUILayout.ColorField("Text color", _tertiary);
            EditorGUILayout.Separator();
            DrawThemeButtons();
            GUILayout.Space(20);
            DrawResetButton();
            EditorGUILayout.EndScrollView();

            if (GUI.changed)
            {
                SetPrefValues();
            }
        }


        /// If EditorPrefs have been lost or have never been initialized, we want to set them to their default values.
        public static void InitPrefs()
        {
            ResetPrefsToDefault();
            EditorPrefs.SetBool("GDTB_AnimationTester_initialized", true);
        }


        /// Set the value of ShowWelcome.
        public static void SetWelcome(bool val)
        {
            EditorPrefs.SetBool(_PREFS_ANIMATIONTESTER_WELCOME, val);
        }


        /// If preferences have keys already saved in EditorPrefs, get them. Otherwise, set them.
        public static void GetAllPrefValues()
        {
            _showWelcome = GetPrefValue(_PREFS_ANIMATIONTESTER_WELCOME, _SHOW_WELCOME_DEFAULT);
            GetColorPrefs();
            _shortcut = GetPrefValue(_PREFS_ANIMATIONTESTER_SHORTCUT, _SHORTCUT_DEFAULT); // Shortcut.
            ParseShortcutValues();
        }


        /// Load color preferences.
        public static void GetColorPrefs()
        {
            _primary = GetPrefValue(_PREFS_ANIMATIONTESTER_COLOR_PRIMARY, RGBA.GetNormalizedColor(_primaryDefault)); // PRIMARY color.
            _secondary = GetPrefValue(_PREFS_ANIMATIONTESTER_COLOR_SECONDARY, RGBA.GetNormalizedColor(_secondaryDefault)); // SECONDARY color.
            _tertiary = GetPrefValue(_PREFS_ANIMATIONTESTER_COLOR_TERTIARY, RGBA.GetNormalizedColor(_tertiaryDefault)); // TERTIARY color.

            // If all colors are the same, there's been some issue. Revert to initial dark scheme.
            if(_primary == _secondary && _primary == _tertiary)
            {
                _primary = RGBA.GetNormalizedColor(_primaryDefault);
                _secondary = RGBA.GetNormalizedColor(_secondaryDefault);
                _tertiary = RGBA.GetNormalizedColor(_tertiaryDefault);
            }
        }




        /// Draw the shortcut selector.
        private static string DrawShortcutSelector()
        {
            // Differentiate between Mac Editor (CMD) and Win editor (CTRL).
            var platformKey = Application.platform == RuntimePlatform.OSXEditor ? "CMD" : "CTRL";
            var shortcut = "";
            ParseShortcutValues();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Shortcut ");
            GUILayout.Space(20);
            _modifierKeys[0] = GUILayout.Toggle(_modifierKeys[0], platformKey, EditorStyles.miniButton, GUILayout.Width(50));
            _modifierKeys[1] = GUILayout.Toggle(_modifierKeys[1], "ALT", EditorStyles.miniButton, GUILayout.Width(40));
            _modifierKeys[2] = GUILayout.Toggle(_modifierKeys[2], "SHIFT", EditorStyles.miniButton, GUILayout.Width(60));
            _mainShortcutKeyIndex = EditorGUILayout.Popup(_mainShortcutKeyIndex, _shortcutKeys, GUILayout.Width(60));
            GUILayout.EndHorizontal();

            // Generate shortcut string.
            if (_modifierKeys[0])
            {
                shortcut += "%|";
            }
            if (_modifierKeys[1])
            {
                shortcut += "&|";
            }
            if (_modifierKeys[2])
            {
                shortcut += "#|";
            }
            shortcut += _shortcutKeys[_mainShortcutKeyIndex];

            return shortcut;
        }


        /// Draw Apply colors - Load dark theme - load light theme.
        private static void DrawThemeButtons()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Apply new colors"))
            {
                ReloadSkins();
                RepaintOpenWindows();
            }
            if (GUILayout.Button("Load dark theme"))
            {
                // Get confirmation through dialog (or not if the user doesn't want to).
                if (EditorUtility.DisplayDialog("Change to dark theme?", "Are you sure you want to change the color scheme to the dark (default) theme?", "Change color scheme", "Cancel"))
                {
                    _primary = RGBA.GetNormalizedColor(_primaryDark);
                    _secondary = RGBA.GetNormalizedColor(_secondaryDark);
                    _tertiary = RGBA.GetNormalizedColor(_tertiaryDark);
                    SetColorPrefs();
                    GetColorPrefs();

                    ReloadSkins();

                    RepaintOpenWindows();
                }
            }
            if (GUILayout.Button("Load light theme"))
            {
                // Get confirmation through dialog (or not if the user doesn't want to).
                if (EditorUtility.DisplayDialog("Change to light theme?", "Are you sure you want to change the color scheme to the light theme?", "Change color scheme", "Cancel"))
                {
                    _primary = RGBA.GetNormalizedColor(_primaryLight);
                    _secondary = RGBA.GetNormalizedColor(_secondaryLight);
                    _tertiary = RGBA.GetNormalizedColor(_tertiaryLight);
                    SetColorPrefs();
                    GetColorPrefs();

                    ReloadSkins();

                    RepaintOpenWindows();
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }


        /// Draw reset button.
        private static void DrawResetButton()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Reset preferences", GUILayout.Width(120)))
            {
                ResetPrefsToDefault();
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }


        /// Reset all preferences to default.
        private static void ResetPrefsToDefault()
        {

            _showWelcome = _SHOW_WELCOME_DEFAULT;
            _primary = RGBA.GetNormalizedColor(_primaryDefault);
            _secondary = RGBA.GetNormalizedColor(_secondaryDefault);
            _tertiary = RGBA.GetNormalizedColor(_tertiaryDefault);
            _shortcut = _SHORTCUT_DEFAULT;

            SetPrefValues();
            GetAllPrefValues();
        }


        /// Set the value of all preferences.
        private static void SetPrefValues()
        {
            SetWelcome(_showWelcome);
            SetColorPrefs();
            SetShortcutPrefs();
        }


        /// Set the value of the shortcut preference.
        private static void SetShortcutPrefs()
        {
            if (_newShortcut != _shortcut && _newShortcut != null)
            {
                _shortcut = _newShortcut;
                EditorPrefs.SetString(_PREFS_ANIMATIONTESTER_SHORTCUT, _shortcut);
                var formattedShortcut = _shortcut.Replace("|", "");
                IO.OverwriteShortcut(formattedShortcut);
            }
        }


        /// Set the value of a Color preference.
        private static void SetColorPrefs()
        {
            EditorPrefs.SetString(_PREFS_ANIMATIONTESTER_COLOR_PRIMARY, RGBA.ColorToString(_primary));
            EditorPrefs.SetString(_PREFS_ANIMATIONTESTER_COLOR_SECONDARY, RGBA.ColorToString(_secondary));
            EditorPrefs.SetString(_PREFS_ANIMATIONTESTER_COLOR_TERTIARY, RGBA.ColorToString(_tertiary));
        }


        /// Get the value of a bool preference.
        private static bool GetPrefValue(string aKey, bool aDefault)
        {
            bool val;
            if (!EditorPrefs.HasKey(aKey))
            {
                EditorPrefs.SetBool(aKey, aDefault);
                val = aDefault;
            }
            else
            {
                val = EditorPrefs.GetBool(aKey, aDefault);
            }

            return val;
        }


        /// Get the value of a string preference.
        private static string GetPrefValue(string aKey, string aDefault)
        {
            string val;
            if (!EditorPrefs.HasKey(aKey))
            {
                EditorPrefs.SetString(aKey, aDefault);
                val = aDefault;
            }
            else
            {
                val = EditorPrefs.GetString(aKey, aDefault);
            }

            return val;
        }


        /// Get the value of a Color preference.
        private static Color GetPrefValue(string aKey, Color aDefault)
        {
            Color val;
            if (!EditorPrefs.HasKey(aKey))
            {
                EditorPrefs.SetString(aKey, RGBA.ColorToString(aDefault));
                val = aDefault;
            }
            else
            {
                val = RGBA.StringToColor(EditorPrefs.GetString(aKey, RGBA.ColorToString(aDefault)));
            }

            return val;
        }


        /// Get usable values from the shortcut string pref.
        private static void ParseShortcutValues()
        {
            var foundCmd = false;
            var foundAlt = false;
            var foundShift = false;

            var keys = _shortcut.Split('|');
            for (var i = 0; i < keys.Length; i++)
            {
                switch (keys[i])
                {
                    case "%":
                        foundCmd = true;
                        break;
                    case "&":
                        foundAlt = true;
                        break;
                    case "#":
                        foundShift = true;
                        break;
                    default:
                        _mainShortcutKeyIndex = System.Array.IndexOf(_shortcutKeys, keys[i]);
                        break;
                }
            }
            _modifierKeys[0] = foundCmd; // Ctrl/Cmd.
            _modifierKeys[1] = foundAlt; // Alt.
            _modifierKeys[2] = foundShift; // Shift.
        }


        /// Reload skins of open windows.
        private static void ReloadSkins()
        {
            if (WindowMain.IsOpen)
            {
                var window = EditorWindow.GetWindow(typeof(WindowMain)) as WindowMain;
                if (window != null)
                {
                    window.LoadStyles();
                }
            }

            if (WindowWelcome.IsOpen)
            {
                var window = EditorWindow.GetWindow(typeof(WindowWelcome)) as WindowWelcome;
                if (window != null)
                {
                    window.LoadStyles();
                }
            }
        }


        /// Repaint all open windows.
        private static void RepaintOpenWindows()
        {
            if (WindowMain.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowMain)).Repaint();
            }
        }

#endregion

    }
}