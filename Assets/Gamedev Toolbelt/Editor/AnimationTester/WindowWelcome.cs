using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace com.immortalhydra.gdtb.animationtester
{
    public class WindowWelcome : EditorWindow
    {
        public static WindowWelcome Instance { get; private set; }
        public static bool IsOpen {
            get { return Instance != null; }
        }
        private GUISkin _skin;
        private GUIStyle _wordWrappedColoredLabel, _headerLabel;
        private int _offset = 5;
        private bool _welcomeValue;
        private float _usableWidth = 0;


        public static void Init()
        {
            // Get existing open window or if none, make a new one.
            var window = (WindowWelcome)EditorWindow.GetWindow(typeof(WindowWelcome));
            window.SetMinSize();
            window.LoadSkin();
            window.Show();
        }


        public void OnEnable()
        {
            #if UNITY_5_3_OR_NEWER || UNITY_5_1 || UNITY_5_2
                titleContent = new GUIContent("Hello!");
            #else
                title = "Hello!";
            #endif

            Instance = this;

            LoadSkin();
            LoadStyle();

            _welcomeValue = Preferences.ShowWelcome;
        }


        /// Called when the window is closed.
        private void OnDestroy()
        {
            Resources.UnloadUnusedAssets();
        }


        private void OnGUI()
        {
            _usableWidth = position.width - _offset * 2;
            GUI.skin = _skin;

            DrawWindowBackground();
            var label1Content = new GUIContent("Hello! Using AnimationTester is a snap.");
            var label1Height = _wordWrappedColoredLabel.CalcHeight(label1Content, _usableWidth);
            var label1Rect = new Rect(_offset * 2, _offset * 2, _usableWidth - _offset * 2, label1Height);
            EditorGUI.LabelField(label1Rect, label1Content, _headerLabel);

            var label2Content = new GUIContent("1. Select the gameobject whose animations you want to test from the first dropdown.");
            var label2Height = _wordWrappedColoredLabel.CalcHeight(label2Content, _usableWidth);
            var label2Rect = new Rect(_offset * 2, _offset * 2 + 20, _usableWidth - _offset * 2, label2Height);
            EditorGUI.LabelField(label2Rect, label2Content, _wordWrappedColoredLabel);

            var label3Content = new GUIContent("2. Choose the animation you want to test from the second dropdown.");
            var label3Height = _wordWrappedColoredLabel.CalcHeight(label3Content, _usableWidth);
            var label3Rect = new Rect(_offset * 2, _offset * 2 + 60, _usableWidth - _offset * 2, label3Height);
            EditorGUI.LabelField(label3Rect, label3Content, _wordWrappedColoredLabel);

            var label4Content = new GUIContent("3. Enter Play mode in Unity, then press Play!");
            var label4Rect = new Rect(_offset * 2, _offset * 2 + 100, _usableWidth - _offset * 2, 0);;
            label4Rect.height = _wordWrappedColoredLabel.CalcHeight(label4Content, _usableWidth);
            EditorGUI.LabelField(label4Rect, label4Content, _wordWrappedColoredLabel);

            var label5Content = new GUIContent("There are many settings you can change, a new section has been added to the Preferences window.");
            var label5Rect = new Rect(_offset * 2, _offset * 2 + 140, _usableWidth - _offset * 2, 0);;
            label5Rect.height = _wordWrappedColoredLabel.CalcHeight(label5Content, _usableWidth);
            EditorGUI.LabelField(label5Rect, label5Content, _wordWrappedColoredLabel);

            var reviewContent = new GUIContent("If you like the extension, please leave a review!\nYou can do so by clicking this sentence, a browser window will be opened.");
            var reviewRect = new Rect(_offset * 2, _offset * 2 + 180, _usableWidth - _offset * 2, 0);;
            reviewRect.height = _headerLabel.CalcHeight(reviewContent, _usableWidth);
            EditorGUIUtility.AddCursorRect(reviewRect, MouseCursor.Link);
            EditorGUI.LabelField(reviewRect, reviewContent, _headerLabel);
            if (Event.current.type == EventType.MouseUp && reviewRect.Contains(Event.current.mousePosition))
            {
                UnityEngine.Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/70010");
            }

            DrawToggle();
        }


        /// Draw the background texture.
        private void DrawWindowBackground()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), Preferences.Color_Primary);
        }


        private void DrawToggle()
        {
            var rect = new Rect(_offset * 2, position.height - 20 - _offset, position.width, 20);
            _welcomeValue = EditorGUI.ToggleLeft(rect, " Show this window every time AnimationTester is opened", _welcomeValue, _wordWrappedColoredLabel);
            if (_welcomeValue != Preferences.ShowWelcome)
            {
                Preferences.SetWelcome(_welcomeValue);
            }
        }


        /// Load AnimationTester custom skin.
        public void LoadSkin()
        {
            _skin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }


        /// Load label styles.
        public void LoadStyle()
        {
            _wordWrappedColoredLabel = _skin.GetStyle("GDTB_AnimationTester_wordWrappedColoredLabel");
            _wordWrappedColoredLabel.active.textColor = Preferences.Color_Tertiary;
            _wordWrappedColoredLabel.normal.textColor = Preferences.Color_Tertiary;
            _wordWrappedColoredLabel.wordWrap = true;
            _wordWrappedColoredLabel.fontStyle = FontStyle.Normal;

            _headerLabel = _skin.GetStyle("GDTB_AnimationTester_header");
            _headerLabel.active.textColor = Preferences.Color_Secondary;
            _headerLabel.normal.textColor = Preferences.Color_Secondary;
            _headerLabel.fontStyle = FontStyle.Bold;
        }


        /// Set the minSize of the window based on preferences.
        public void SetMinSize()
        {
            var window = GetWindow(typeof(WindowWelcome)) as WindowWelcome;
            window.minSize = new Vector2(360f, 300f);
        }
    }
}
