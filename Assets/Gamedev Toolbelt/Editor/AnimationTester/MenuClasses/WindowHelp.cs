using UnityEngine;
using UnityEditor;

namespace com.immortalhydra.gdtb.animationtester
{
	public class WindowHelp : EditorWindow
	{

#region FIELDS AND PROPERTIES

		// Fields.
		private string _instructionsHeader = "How do I use AnimationTester?";
		private string _instructions1 = "1. Open the AnimationTester window (Window -> Gamedev Toolbelt -> AnimationTester).";
		private string _instructions2 = "2. Enter Play mode.";
		private string _instructions3 = "3. Select the gameobject whose animations you want to view from the first dropdown.";
		private string _instructions4 = "4. Select the animation you want to view from the second dropdown.";
		private string _instructions5 = "5. Click play, and see it played!";

        private float _usableWidth;
		private const int _OFFSET = 5;
		private GUISkin _skin;
        private GUIStyle _wordWrappedColoredLabel, _headerLabel;

		// Properties.
    	public static WindowHelp Instance { get; private set; }
    	public static bool IsOpen {
        get { return Instance != null; }
    }

#endregion

#region MONOBEHAVIOUR METHODS

		public void OnEnable()
        {
        #if UNITY_5_3_OR_NEWER || UNITY_5_1 || UNITY_5_2
            titleContent = new GUIContent("Help");
        #else
            title = "Help";
        #endif

            Instance = this;

			Preferences.GetColorPrefs();

            LoadSkin();
            LoadStyles();
        }


		private void OnGUI()
		{
            _usableWidth = position.width - _OFFSET * 2;

			DrawWindowBackground();

            var headerContent = new GUIContent(_instructionsHeader);
            var headerHeight = _headerLabel.CalcHeight(headerContent, _usableWidth);
            var headerRect = new Rect(_OFFSET * 2, _OFFSET * 2, _usableWidth - _OFFSET * 2, headerHeight);
			EditorGUI.LabelField(headerRect, headerContent, _headerLabel);

            var inst1Content = new GUIContent(_instructions1);
            var inst1Height = _wordWrappedColoredLabel.CalcHeight(inst1Content, _usableWidth);
            var inst1Rect = new Rect(_OFFSET * 2, headerRect.y + headerRect.height + _OFFSET * 2, _usableWidth - _OFFSET * 2, inst1Height);
			EditorGUI.LabelField(inst1Rect, inst1Content, _wordWrappedColoredLabel);

            var inst2Content = new GUIContent(_instructions2);
            var inst2Height = _wordWrappedColoredLabel.CalcHeight(inst2Content, _usableWidth);
            var inst2Rect = new Rect(_OFFSET * 2, inst1Rect.y + inst1Rect.height + _OFFSET * 2, _usableWidth - _OFFSET * 2, inst2Height);
			EditorGUI.LabelField(inst2Rect, inst2Content, _wordWrappedColoredLabel);

            var inst3Content = new GUIContent(_instructions3);
            var inst3Height = _wordWrappedColoredLabel.CalcHeight(inst3Content, _usableWidth);
            var inst3Rect = new Rect(_OFFSET * 2, inst2Rect.y + inst2Rect.height + _OFFSET * 2, _usableWidth - _OFFSET * 2, inst3Height);
			EditorGUI.LabelField(inst3Rect, inst3Content, _wordWrappedColoredLabel);

            var inst4Content = new GUIContent(_instructions4);
            var inst4Height = _wordWrappedColoredLabel.CalcHeight(inst4Content, _usableWidth);
            var inst4Rect = new Rect(_OFFSET * 2, inst3Rect.y + inst3Rect.height + _OFFSET * 2, _usableWidth - _OFFSET * 2, inst4Height);
			EditorGUI.LabelField(inst4Rect, inst4Content, _wordWrappedColoredLabel);

            var inst5Content = new GUIContent(_instructions5);
            var inst5Height = _wordWrappedColoredLabel.CalcHeight(inst5Content, _usableWidth);
            var inst5Rect = new Rect(_OFFSET * 2, inst4Rect.y + inst4Rect.height + _OFFSET * 2, _usableWidth - _OFFSET * 2, inst5Height);
			EditorGUI.LabelField(inst5Rect, inst5Content, _wordWrappedColoredLabel);
		}

#endregion

#region METHODS

		/// Load AnimationTester custom skin.
        public void LoadSkin()
        {
            _skin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }


        /// Load label styles.
        public void LoadStyles()
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




		[MenuItem("Window/Gamedev Toolbelt/AnimationTester/Help")]
		private static void Init()
		{
			// Get existing open window or, if none exists, make a new one.
            var window = (WindowHelp)GetWindow (typeof (WindowHelp));
            window.minSize = new Vector2(550f, 155f);
            window.Show();
		}


		/// Draw the background texture.
        private void DrawWindowBackground()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), Preferences.Color_Primary);
        }

#endregion

	}
}