using UnityEditor;
using UnityEngine;

namespace com.immortalhydra.gdtb.animationtester
{
    public static class DrawingUtils
    {
        public static Texture2D Texture_Add = Resources.Load(Constants.TEX_ADD_DARK, typeof(Texture2D)) as Texture2D;
        public static Texture2D Texture_Settings = Resources.Load(Constants.TEX_SETTINGS_DARK, typeof(Texture2D)) as Texture2D;
        public static Texture2D Texture_Complete = Resources.Load(Constants.TEX_COMPLETE_DARK, typeof(Texture2D)) as Texture2D;

        public static IconStyle CurrentIconStyle = IconStyle.LIGHT;


        /// Draw the button, based on the type, not pressed.
        public static void DrawButton(Rect aRect, ButtonsDisplayFormat aButtonType, Texture2D aTexture, string aText, GUIStyle aStyle)
        {
            if(aButtonType == ButtonsDisplayFormat.COOL_ICONS)
            {
                DrawIconButton(aRect, aTexture);
            }
            else
            {
                DrawTextButton(aRect, aText, aStyle);
            }
        }


        /// Draw "fake" texture button.
        public static void DrawIconButton(Rect aRect, Texture2D aTexture)
        {
            EditorGUI.DrawRect(aRect, Preferences.Color_Secondary);
            var bgRect = new Rect(aRect.x + Constants.BUTTON_BORDER_THICKNESS, aRect.y + Constants.BUTTON_BORDER_THICKNESS, aRect.width - Constants.BUTTON_BORDER_THICKNESS * 2, aRect.height - Constants.BUTTON_BORDER_THICKNESS * 2);
            EditorGUI.DrawRect(bgRect, Preferences.Color_Primary);
            GUI.DrawTexture(new Rect(aRect.x + 2, aRect.y + 2, Constants.BUTTON_TEXTURE_SIZE, Constants.BUTTON_TEXTURE_SIZE), aTexture);
        }


        /// Draw "fake" text button based on preferences.
        public static void DrawTextButton(Rect aRect, string aText, GUIStyle aStyle)
        {
            EditorGUI.DrawRect(aRect, Preferences.Color_Secondary);
            var bgRect = new Rect(aRect.x + Constants.BUTTON_BORDER_THICKNESS, aRect.y + Constants.BUTTON_BORDER_THICKNESS, aRect.width - Constants.BUTTON_BORDER_THICKNESS * 2, aRect.height - Constants.BUTTON_BORDER_THICKNESS * 2);
            EditorGUI.DrawRect(bgRect, Preferences.Color_Primary);
            GUI.Label(new Rect(bgRect.x, bgRect.y - 1, bgRect.width, bgRect.height), aText, aStyle);
            aStyle.normal.textColor = Preferences.Color_Tertiary;
        }


        /// Draw "fake" text button in the pressed state.
        public static void DrawPressedTextButton(Rect aRect, string aText, GUIStyle aStyle)
        {
            EditorGUI.DrawRect(aRect, Preferences.Color_Secondary);
            aStyle.normal.textColor = Preferences.Color_Primary;
            GUI.Label(new Rect(aRect.x, aRect.y - 1, aRect.width, aRect.height), aText, aStyle);
        }


        /// Draw custom selectionGrid.
        public static void DrawSelectionGrid(Rect aRect, string[] anElementArray, int aSelectedIndex, float anHorizontalSize, float aSpace, GUIStyle aNormalStyle, GUIStyle aPressedStyle)
        {
            var x = aRect.x;
            for (var i = 0; i < anElementArray.Length; i++)
            {
                var gridRect = aRect;
                x = i == 0 ? x : x + anHorizontalSize + aSpace; // Add parameters to horizontal pos., but not for the first rect.
                gridRect.x = x;
                gridRect.width = anHorizontalSize;

                if (i != aSelectedIndex)
                {
                    DrawTextButton(gridRect, anElementArray[i], aNormalStyle);
                }
                else
                {
                    DrawPressedTextButton(gridRect, anElementArray[i], aPressedStyle);
                }
            }
        }


        /// Change button textures when they're changed in preferences.
        public static void LoadTextures(IconStyle aStyle)
        {
            Texture_Add = null;
            Texture_Settings = null;
            Texture_Complete = null;

            if (aStyle == IconStyle.DARK)
            {
                Texture_Add = Resources.Load(Constants.TEX_ADD_DARK, typeof(Texture2D)) as Texture2D;
                Texture_Settings = Resources.Load(Constants.TEX_SETTINGS_DARK, typeof(Texture2D)) as Texture2D;
                Texture_Complete = Resources.Load(Constants.TEX_COMPLETE_DARK, typeof(Texture2D)) as Texture2D;
                CurrentIconStyle = IconStyle.DARK;
            }
            else
            {
                Texture_Add = Resources.Load(Constants.TEX_ADD_LIGHT, typeof(Texture2D)) as Texture2D;
                Texture_Settings = Resources.Load(Constants.TEX_SETTINGS_LIGHT, typeof(Texture2D)) as Texture2D;
                Texture_Complete = Resources.Load(Constants.TEX_COMPLETE_LIGHT, typeof(Texture2D)) as Texture2D;
                CurrentIconStyle = IconStyle.LIGHT;
            }
        }
    }
}