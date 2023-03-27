using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LinkLabels
{
    [Flags]
    public enum CustomFontStyle
    {
        Normal = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4,
        BoldAndItalic = Bold | Italic,
        BoldAndUnderline = Bold | Underline,
        ItalicAndUnderline = Italic | Underline,
        Everything = Bold | Italic | Underline
    }

    public static class LinkLabel
    {
        private static readonly string DefaultColorHexCode = "#4C86FC";
        private static Color DefaultColor { get { return GUIMethods.GetColorFromHexCode(DefaultColorHexCode); } }
        private static GUIStyle DefaultLinkLabelStyle = new GUIStyle(EditorStyles.linkLabel)
        {
            border = new RectOffset(0, 0, 0, 0),
            fontSize = 12,
            richText = true,
            wordWrap = false
        };

        // Icon paths
        // Note: Make sure to import the package(s) under Assets to have all icons display properly in the editor window.
        private static readonly string externalLinkIconPath = "Assets/Link Labels/ExternalLinkIcon.png";

        #region Static Method(s)

        #region Draw Method(s)
        /// <summary>
        /// Draw a custom link label in the editor window.
        /// </summary>
        /// <param name="url">The URL to attach to the link label.</param>
        /// <param name="linkLabelContent">Link label.</param>
        /// <param name="labelColor">The color of the link label.</param>
        /// <param name="linkStyle">The GUI style of the link label.</param>
        /// <param name="underlineLink">Boolean to toggle whether to underline the link label.</param>
        /// <param name="displayIcon">Boolean to toggle whether to display an external link icon.</param>
        public static void Draw(string url, GUIContent linkLabelContent, Color labelColor, GUIStyle linkStyle,
                                bool underlineLink, bool displayIcon)
        {
            Vector2 defaultIconSize = EditorGUIUtility.GetIconSize();          // Default icon size
            string caption = linkLabelContent.text;                            // Caption
            string tooltip = linkLabelContent.tooltip;                         // Tooltip

            linkStyle.richText = true;

            // Set icon.
            var icon = (Texture2D)AssetDatabase.LoadAssetAtPath(externalLinkIconPath, typeof(Texture2D));
            float iconSize = icon != null ? linkStyle.fontSize : 0f;
            EditorGUIUtility.SetIconSize(new Vector2(iconSize, iconSize));
            icon = displayIcon ? icon : null;

            // Format the specified caption.
            caption = string.IsNullOrWhiteSpace(caption) ? url : caption;
            if (displayIcon)
            {
                caption = icon != null ? $"{caption}" : $"{caption} ↗";
            }

            caption = string.Format($"<color=#{ColorUtility.ToHtmlStringRGB(labelColor)}>{caption}</color>");

            // Create link label.
            GUIContent linkContent = new GUIContent(caption);

            labelColor.a = 1f;
            GUIMethods.BeginGUIColor(Color.white);
            GUIMethods.BeginGUIContentColor(labelColor);
            GUILayout.BeginHorizontal();
            GUILayout.Label(linkContent, linkStyle);
            var rect = GUILayoutUtility.GetLastRect();
            GUILayout.Label(icon, linkStyle);
            GUILayout.EndHorizontal();

            // Display an invisible 2D rectangle on the link label to 
            if (IsValidURL(url))
            {
                float contentWidth = linkStyle.CalcSize(new GUIContent(caption)).x;
                rect.width = icon != null ? contentWidth + EditorGUIUtility.GetIconSize().x + 8f : contentWidth;
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
            }

            // Draw a colored line underneath the link label.
            if (underlineLink)
            {
                UnderlineLink(rect, labelColor);
            }

            // Set if link is clicked.
            GUIMethods.BeginGUIColor(Color.clear);
            bool isClicked = GUI.Button(rect, new GUIContent("", tooltip));
            GUIMethods.EndGUIColor();

            // Open the specified URL on click.
            if (isClicked)
            {
                try
                {
                    OpenURL(url);
                }
                catch
                {
                    Debug.Log("Could not open the specified URL link.");
                    EditorApplication.Beep();
                }
            }

            GUIMethods.EndGUIContentColor();
            GUIMethods.EndGUIColor();
            EditorGUIUtility.SetIconSize(defaultIconSize);
        }

        #region Overloading Method(s)
        public static void Draw(string url)
        {
            GUIContent linkLabelContent = new GUIContent();
            Draw(url, linkLabelContent, DefaultColor, DefaultLinkLabelStyle, false, false);
        }

        public static void Draw(string url, string caption)
        {
            GUIContent linkLabelContent = new GUIContent(caption);
            Draw(url, linkLabelContent, DefaultColor, DefaultLinkLabelStyle, false, false);
        }

        public static void Draw(string url, GUIContent linkLabelContent)
        {
            Draw(url, linkLabelContent, DefaultColor, DefaultLinkLabelStyle, false, false);
        }

        public static void Draw(string url, string caption, Color labelColor)
        {
            GUIContent linkLabelContent = new GUIContent(caption);
            Draw(url, linkLabelContent, labelColor, DefaultLinkLabelStyle, false, false);
        }

        public static void Draw(string url, string caption, string hexColorCode)
        {
            Color hexColor = GUIMethods.GetColorFromHexCode(hexColorCode);
            Draw(url, caption, hexColor);
        }

        public static void Draw(string url, GUIContent linkLabelContent, Color labelColor)
        {
            Draw(url, linkLabelContent, labelColor, DefaultLinkLabelStyle, false, false);
        }

        public static void Draw(string url, GUIContent linkLabelContent, string hexColorCode)
        {
            Color hexColor = GUIMethods.GetColorFromHexCode(hexColorCode);
            Draw(url, linkLabelContent, hexColor, DefaultLinkLabelStyle, false, false);
        }

        public static void Draw(string url, string caption, Color labelColor, GUIStyle linkStyle,
                                bool underlineLink, bool displayIcon)
        {
            GUIContent linkLabelContent = new GUIContent(caption);
            Draw(url, linkLabelContent, labelColor, linkStyle, underlineLink, displayIcon);
        }

        public static void Draw(string url, string caption, string hexColorCode, GUIStyle linkStyle,
                                bool underlineLink, bool displayIcon)
        {
            Color hexColor = GUIMethods.GetColorFromHexCode(hexColorCode);
            Draw(url, caption, hexColor, linkStyle, underlineLink, displayIcon);
        }

        public static void Draw(string url, string caption, string hexColorCode, int fontSize,
                                CustomFontStyle fontStyle, bool displayIcon)
        {
            GUIContent linkLabelContent = new GUIContent(caption);
            Draw(url, linkLabelContent, hexColorCode, fontSize, fontStyle, displayIcon);
        }

        public static void Draw(string url, GUIContent linkLabelContent, Color labelColor, int fontSize,
                                CustomFontStyle fontStyle, bool displayIcon)
        {
            GUIStyle linkStyle = DefaultLinkLabelStyle;                             // Default GUI Style: Link label
            linkStyle.fontSize = fontSize;                                          // Font size
            bool underlineLink = fontStyle.HasFlag(CustomFontStyle.Underline);      // Underline link

            if (fontStyle.HasFlag(CustomFontStyle.Underline))
            {
                if (fontStyle.HasFlag(CustomFontStyle.Bold) && fontStyle.HasFlag(CustomFontStyle.Italic))
                {
                    linkStyle.fontStyle = FontStyle.BoldAndItalic;
                }
                else if (fontStyle.HasFlag(CustomFontStyle.Bold))
                {
                    linkStyle.fontStyle = FontStyle.Bold;
                }
                else if (fontStyle.HasFlag(CustomFontStyle.Italic))
                {
                    linkStyle.fontStyle = FontStyle.Italic;
                }
            }
            else
            {
                linkStyle.fontStyle = (FontStyle)((int)fontStyle);
            }

            Draw(url, linkLabelContent, labelColor, linkStyle, underlineLink, displayIcon);
        }

        public static void Draw(string url, GUIContent linkLabelContent, string hexColorCode, int fontSize,
                                CustomFontStyle fontStyle, bool displayIcon)
        {
            Color hexColor = GUIMethods.GetColorFromHexCode(hexColorCode);
            Draw(url, linkLabelContent, hexColor, fontSize, fontStyle, displayIcon);
        }

        public static void Draw(string url, GUIContent linkLabelContent, string hexColorCode, int fontSize,
                                int fontStyleID, bool displayIcon)
        {
            CustomFontStyle fontStyle = (CustomFontStyle)fontStyleID;
            Draw(url, linkLabelContent, hexColorCode, fontSize, fontStyle, displayIcon);
        }
        #endregion

        #endregion

        /// <summary>
        /// Add an underline under the link label.
        /// </summary>
        /// <param name="rect">2D rectangle.</param>
        /// <param name="lineColor">Underline color.</param>
        private static void UnderlineLink(Rect rect, Color lineColor)
        {
            Handles.BeginGUI();
            Handles.color = lineColor;
            Handles.DrawLine(new Vector3(rect.xMin, rect.yMax), new Vector3(rect.xMax, rect.yMax));
            Handles.color = Color.white;
            Handles.EndGUI();
        }

        /// <summary>
        /// Open the specified URL.
        /// </summary>
        /// <param name="url">The URL to open.</param>
        private static void OpenURL(string url)
        {
            if (!IsValidURL(url))
                return;

            Application.OpenURL(url);
        }

        /// <summary>
        /// Check if the specified URL is valid.
        /// </summary>
        /// <param name="url">The URL to verify.</param>
        /// <returns>TRUE if the specified URL can be created, and the URL is not empty or comprised of just whitespace characters.
        ///          Otherwise, FALSE.</returns>
        private static bool IsValidURL(string url)
        {
            Uri uriResult;
            bool isURL = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                         && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return isURL && !string.IsNullOrWhiteSpace(url);
        }
        #endregion
    }
}
