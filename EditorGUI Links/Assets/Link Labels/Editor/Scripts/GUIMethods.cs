using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LinkLabels
{
    public static class GUIMethods
    {
        public static Color defaultGUIColor;
        public static Color defaultGUIContentColor;
        public static Color defaultGUIBackgroundColor;

        #region GUI.color Method(s)
        /// <summary>
        /// Begin a GUI.color group.
        /// </summary>
        /// <param name="color">The global color to apply to all backgrounds and text inside GUI.color group.</param>
        public static void BeginGUIColor(Color color)
        {
            defaultGUIColor = GUI.color;
            GUI.color = color;
        }

        /// <summary>
        /// Begin a GUI.color group.
        /// </summary>
        /// <param name="hexColorCode">The global color (hexadecimal) to apply to all backgrounds and text inside GUI.color group.</param>
        public static void BeginGUIColor(string hexColorCode)
        {
            Color hexColor = GetColorFromHexCode(hexColorCode);
            BeginGUIColor(hexColor);
        }

        /// <summary>
        /// Close a group started with BeginGUIColor.
        /// </summary>
        public static void EndGUIColor()
        {
            GUI.color = defaultGUIColor;
        }
        #endregion

        #region GUI.contentColor Method(s)
        /// <summary>
        /// Begin a GUI.contentColor group.
        /// </summary>
        /// <param name="color">The global color to apply to all text inside GUI.contentcolor group.</param>
        public static void BeginGUIContentColor(Color color)
        {
            defaultGUIContentColor = GUI.contentColor;
            GUI.contentColor = color;
        }

        /// <summary>
        /// Begin a GUI.contentColor group.
        /// </summary>
        /// <param name="hexColorCode">The global color (hexadecimal) to apply to all text inside GUI.contentcolor group.</param>
        public static void BeginGUIContentColor(string hexColorCode)
        {
            Color hexColor = GetColorFromHexCode(hexColorCode);
            BeginGUIContentColor(hexColor);
        }

        /// <summary>
        /// Close a group started with BeginGUIContentColor.
        /// </summary>
        public static void EndGUIContentColor()
        {
            GUI.contentColor = defaultGUIContentColor;
        }
        #endregion

        #region GUI.backgroundColor Method(s)
        /// <summary>
        /// Begin a GUI.backgroundColor group.
        /// </summary>
        /// <param name="color">The global color to apply to all backgrounds inside GUI.backgroundColor group.</param>
        public static void BeginGUIBackgroundColor(Color color)
        {
            defaultGUIBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
        }

        /// <summary>
        /// Begin a GUI.backgroundColor group.
        /// </summary>
        /// <param name="hexColorCode">The global color (hexadecimal) to apply to all backgrounds inside GUI.backgroundColor group.</param>
        public static void BeginGUIBackgroundColor(string hexColorCode)
        {
            Color hexColor = GetColorFromHexCode(hexColorCode);
            BeginGUIBackgroundColor(hexColor);
        }

        /// <summary>
        /// Close a group started with BeginGUIBackgroundColor.
        /// </summary>
        public static void EndGUIBackgroundColor()
        {
            GUI.backgroundColor = defaultGUIBackgroundColor;
        }
        #endregion

        #region Miscellaneous
        /// <summary>
        /// Get color from hex string.
        /// </summary>
        /// <param name="hexColorCode">Hex color code string.</param>
        /// <returns>New color.</returns>
        public static Color GetColorFromHexCode(string hexColorCode)
        {
            Color color = Color.white;
            ColorUtility.TryParseHtmlString(hexColorCode, out color);
            return color;
        }
        #endregion
    }
}
