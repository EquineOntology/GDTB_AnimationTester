namespace com.immortalhydra.gdtb.animationtester
{
    public static class Constants
    {
        public const string FILE_GUISKIN = "GUI/animationtester_skin";

        public const float LINE_HEIGHT = 16.5f;

		public const string TEX_PLAY_DARK = "GUI/gdtb_PLAY_dark";
		public const string TEX_COMPLETE_DARK = "GUI/gdtb_COMPLETE_dark";
		public const string TEX_SETTINGS_DARK = "GUI/gdtb_SETTINGS_dark";

		public const string TEX_PLAY_LIGHT = "GUI/gdtb_PLAY_light";
		public const string TEX_COMPLETE_LIGHT = "GUI/gdtb_COMPLETE_light";
		public const string TEX_SETTINGS_LIGHT = "GUI/gdtb_SETTINGS_light";

        public const int ICON_SIZE = 20;
        public const int BUTTON_TEXTURE_SIZE = 16;
        public const int BUTTON_BORDER_THICKNESS = 1;

        public const string ANIMATABLES_LIST = "Select gameobject: ";
        public const string ANIMATABLE_CLIPS_LIST = "Select clip: ";
        public const string UPDATE_ANIMATABLES_LIST = "Refresh list";
        public const string UPDATE_CLIP_NAMES_LIST = "Refresh list";
        public const string PLAYBUTTON_TEXT = "Play";

        public const int LABEL_WIDTH = 150;
        public const int POPUP_WIDTH = 150;
        public const int BUTTON_WIDTH = 100;

        public const string ERROR_NO_ANIMATABLES = "There are no gameobjects with an Animator component in the scene.";
        public const string ERROR_ANIMATABLE_NOT_FOUND = "The selected gameobject was not found.\nDid you remove the object while the window was open? If so, please click on \"Refresh list\" and try again.";
        public const string ERROR_MUST_BE_IN_PLAY_MODE = "To play an animation you must be in Play mode.";
    }
}