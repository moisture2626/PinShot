using UnityEngine;

namespace PinShot.Extensions {
    public static class StringExtensions {
        public static string SetColor(this string str, Color color) {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{str}</color>";
        }
    }
}
