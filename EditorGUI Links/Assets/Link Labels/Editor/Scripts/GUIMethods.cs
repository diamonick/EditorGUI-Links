using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LinkLabels
{
    public static class GUIMethods
    {
        public static List<Color> savedGUIColors;
        public static List<Color> savedGUIContentColors;
        public static List<Color> savedGUIBackgroundColors;
        private static int GUIColorCount { get { return savedGUIColors != null ? savedGUIColors.Count : 0; } }
        private static int GUIContentColorCount { get { return savedGUIContentColors != null ? savedGUIContentColors.Count : 0; } }
        private static int GUIBackgroundColorCount { get { return savedGUIBackgroundColors != null ? savedGUIBackgroundColors.Count : 0; } }

        #region GUI.color Method(s)
        /// <summary>
        /// Begin a GUI.color group.
        /// </summary>
        /// <param name="color">The global color to apply to all backgrounds and text inside GUI.color group.</param>
        public static void BeginGUIColor(Color color)
        {
            // Initialize default GUI color list if list is null.
            if (savedGUIColors == null)
            {
                savedGUIColors = new List<Color>();
            }

            Color defaultGUIColor = GUI.color;
            GUI.color = color;

            // Add GUI color to list.
            if (!savedGUIColors.Contains(defaultGUIColor))
            {
                savedGUIColors.Add(defaultGUIColor);
            }
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
            GUI.color = GetLastGUIColor();

            // Remove GUI color from list.
            savedGUIColors.Remove(GetLastGUIColor());
        }

        /// <summary>
        /// Get the last GUI color from the list.
        /// </summary>
        /// <returns>The last GUI color from the list.</returns>
        private static Color GetLastGUIColor()
        {
            if (savedGUIColors == null)
                return Color.white;
            if (GUIColorCount == 0)
                return Color.white;

            return savedGUIColors[GUIColorCount - 1];
        }
        #endregion

        #region GUI.contentColor Method(s)
        /// <summary>
        /// Begin a GUI.contentColor group.
        /// </summary>
        /// <param name="color">The global color to apply to all text inside GUI.contentcolor group.</param>
        public static void BeginGUIContentColor(Color color)
        {
            // Initialize default GUI content color list if list is null.
            if (savedGUIContentColors == null)
            {
                savedGUIContentColors = new List<Color>();
            }

            Color defaultGUIContentColor = GUI.contentColor;
            GUI.contentColor = color;

            // Add GUI content color to list.
            if (!savedGUIContentColors.Contains(defaultGUIContentColor))
            {
                savedGUIContentColors.Add(defaultGUIContentColor);
            }
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
            GUI.contentColor = GetLastGUIContentColor();

            // Remove GUI content color from list.
            savedGUIContentColors.Remove(GetLastGUIContentColor());
        }

        /// <summary>
        /// Get the last GUI content color from the list.
        /// </summary>
        /// <returns>The last GUI content color from the list.</returns>
        private static Color GetLastGUIContentColor()
        {
            if (savedGUIContentColors == null)
                return Color.white;
            if (GUIContentColorCount == 0)
                return Color.white;

            return savedGUIContentColors[GUIContentColorCount - 1];
        }
        #endregion

        #region GUI.backgroundColor Method(s)
        /// <summary>
        /// Begin a GUI.backgroundColor group.
        /// </summary>
        /// <param name="color">The global color to apply to all backgrounds inside GUI.backgroundColor group.</param>
        public static void BeginGUIBackgroundColor(Color color)
        {
            // Initialize default GUI background color list if list is null.
            if (savedGUIBackgroundColors == null)
            {
                savedGUIBackgroundColors = new List<Color>();
            }

            Color defaultGUIBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = color;

            // Add GUI background color to list.
            if (!savedGUIBackgroundColors.Contains(defaultGUIBackgroundColor))
            {
                savedGUIBackgroundColors.Add(defaultGUIBackgroundColor);
            }
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
            GUI.backgroundColor = GetLastGUIBackgroundColor();

            // Remove GUI background color from list.
            savedGUIBackgroundColors.Remove(GetLastGUIBackgroundColor());
        }

        /// <summary>
        /// Get the last GUI background color from the list.
        /// </summary>
        /// <returns>The last GUI background color from the list.</returns>
        private static Color GetLastGUIBackgroundColor()
        {
            if (savedGUIBackgroundColors == null)
                return Color.white;
            if (GUIBackgroundColorCount == 0)
                return Color.white;

            return savedGUIBackgroundColors[GUIBackgroundColorCount - 1];
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
