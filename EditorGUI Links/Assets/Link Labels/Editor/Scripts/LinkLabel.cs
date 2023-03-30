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
        Underline = 4
    }

    public static class LinkLabel
    {
        private static readonly string DefaultColorHexCode = "#4C86FC";
        private static Color DefaultColor { get { return GUIMethods.GetColorFromHexCode(DefaultColorHexCode); } }
        private static GUIStyle DefaultLinkLabelStyle;

        #region Static Method(s)

        #region Draw Method(s)
        /// <summary>
        /// Draw a custom link label in the editor window.
        /// </summary>
        /// <param name="url">The URL to attach to the link label.</param>
        /// <param name="linkLabelContent">Link label.</param>
        /// <param name="labelColor">The color of the link label.</param>
        /// <param name="linkStyle">The GUI style of the link label.</param>
        /// <param name="underlineLink">When enabled, it adds a horizontal line under the link label.</param>
        /// <param name="displayIcon">When enabled, it displays an external link ↗ icon to the right of the link label.</param>
        public static void Draw(string url, GUIContent linkLabelContent, Color labelColor, GUIStyle linkStyle,
                                bool underlineLink, bool displayIcon)
        {
            string caption = linkLabelContent.text;                            // Caption
            string tooltip = linkLabelContent.tooltip;                         // Tooltip

            // Set GUI style's rich-text to true.
            linkStyle.richText = true;

            // Format the specified caption.
            caption = string.IsNullOrWhiteSpace(caption) ? url : caption;
            string uneditedCaption = caption;

            if (displayIcon)
            {
                caption = $"{caption} ↗";
            }

            caption = string.Format($"<color=#{ColorUtility.ToHtmlStringRGB(labelColor)}>{caption}</color>");

            // Create link label.
            GUIContent linkContent = new GUIContent(caption);
            // Set link label's color as opaque.
            labelColor.a = 1f;

            GUIMethods.BeginGUIColor(Color.white);
            GUIMethods.BeginGUIContentColor(labelColor);
            GUILayout.Label(linkContent, linkStyle);

            var fullRect = GUILayoutUtility.GetLastRect();
            var rect = GUILayoutUtility.GetLastRect();

            // Display an invisible 2D rectangle on the link label to 
            if (IsValidURL(url))
            {
                float fullContentWidth = linkStyle.CalcSize(linkContent).x;
                float contentWidth = linkStyle.CalcSize(new GUIContent(uneditedCaption)).x;
                fullRect.width = fullContentWidth;
                rect.width = contentWidth;
                EditorGUIUtility.AddCursorRect(fullRect, MouseCursor.Link);
            }

            // Draw a colored line underneath the link label.
            if (underlineLink)
            {
                UnderlineLink(rect, labelColor);
            }

            // Set if link is clicked.
            GUIMethods.BeginGUIColor(Color.clear);
            bool isClicked = GUI.Button(fullRect, new GUIContent("", tooltip));
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
        }

        #region Overloaded Method(s)
        public static void Draw(string url)
        {
            DefaultLinkLabelStyleInit();
            GUIContent linkLabelContent = new GUIContent();
            Draw(url, linkLabelContent, DefaultColor, DefaultLinkLabelStyle, false, false);
        }

        public static void Draw(string url, string caption)
        {
            DefaultLinkLabelStyleInit();
            GUIContent linkLabelContent = new GUIContent(caption);
            Draw(url, linkLabelContent, DefaultColor, DefaultLinkLabelStyle, false, false);
        }

        public static void Draw(string url, GUIContent linkLabelContent)
        {
            DefaultLinkLabelStyleInit();
            Draw(url, linkLabelContent, DefaultColor, DefaultLinkLabelStyle, false, false);
        }

        public static void Draw(string url, string caption, Color labelColor)
        {
            DefaultLinkLabelStyleInit();
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
            DefaultLinkLabelStyleInit();
            Draw(url, linkLabelContent, labelColor, DefaultLinkLabelStyle, false, false);
        }

        public static void Draw(string url, GUIContent linkLabelContent, string hexColorCode)
        {
            DefaultLinkLabelStyleInit();
            Color hexColor = GUIMethods.GetColorFromHexCode(hexColorCode);
            Draw(url, linkLabelContent, hexColor, DefaultLinkLabelStyle, false, false);
        }

        public static void Draw(string url, GUIContent linkLabelContent, Color labelColor, int fontSize)
        {
            DefaultLinkLabelStyleInit();
            DefaultLinkLabelStyle.fontSize = fontSize;
            Draw(url, linkLabelContent, labelColor, DefaultLinkLabelStyle, false, false);
        }

        public static void Draw(string url, GUIContent linkLabelContent, string hexColorCode, int fontSize)
        {
            Color hexColor = GUIMethods.GetColorFromHexCode(hexColorCode);
            Draw(url, linkLabelContent, hexColor, fontSize);
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
            DefaultLinkLabelStyleInit();
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
                else
                {
                    linkStyle.fontStyle = FontStyle.Normal;
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
        /// Initialize default link label style.
        /// </summary>
        private static void DefaultLinkLabelStyleInit()
        {
            DefaultLinkLabelStyle = new GUIStyle(EditorStyles.linkLabel)
            {
                fontStyle = FontStyle.Normal,
                border = new RectOffset(0, 0, 0, 0),
                fontSize = 12,
                richText = true,
                wordWrap = false,
                clipping = TextClipping.Clip
            };
        }

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
