using UnityEngine;
using System;
using System.Globalization;

namespace com.immortalhydra.gdtb.animationtester
{
    public class RGBA : MonoBehaviour
    {

#region METHODS
        public static string ColorToString(Color aColor)
        {
            var colorString = aColor.r.ToString(CultureInfo.InvariantCulture) + '/' +
                              aColor.g.ToString(CultureInfo.InvariantCulture) + '/' +
                              aColor.b.ToString(CultureInfo.InvariantCulture) + '/' +
                              aColor.a.ToString(CultureInfo.InvariantCulture);
            return colorString;
        }

        public static Color StringToColor(string anRGBAString)
        {
            var color = new Color();
            var values = anRGBAString.Split('/');
            color.r = Single.Parse(values[0]);
            color.g = Single.Parse(values[1]);
            color.b = Single.Parse(values[2]);
            color.a = Single.Parse(values[3]);

            return color;
        }


        // Return a color with rgba values between 0 and 1.
        public static Color GetNormalizedColor(Color aColor)
        {
            return new Color(aColor.r / 255.0f, aColor.g / 255.0f, aColor.b / 255.0f, aColor.a);
        }

#endregion

    }
}