using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LinkLabels
{
    public static class LinkLabel
    {
        // Default Link Label style.
        private static readonly string DefaultColor = "#4C86FC";
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
        public static void Draw(string url, GUIContent linkLabel, Color urlColor, GUIStyle linkStyle,
                                bool underlineLink = false, bool displayIcon = false)
        {
            Vector2 defaultIconSize = EditorGUIUtility.GetIconSize();
            string caption = linkLabel.text;
            string tooltip = linkLabel.tooltip;

            var icon = (Texture2D)AssetDatabase.LoadAssetAtPath(externalLinkIconPath, typeof(Texture2D));
            float iconSize = icon != null ? linkStyle.fontSize : 0f;
            EditorGUIUtility.SetIconSize(new Vector2(iconSize, iconSize));
            icon = displayIcon ? icon : null;

            caption = string.IsNullOrWhiteSpace(caption) ? url : caption;
            if (displayIcon)
            {
                caption = icon != null ? $"{caption}" : $"{caption} ↗";
            }

            caption = string.Format($"<color=#{ColorUtility.ToHtmlStringRGB(urlColor)}>{caption}</color>");

            GUIContent linkContent = new GUIContent(caption, tooltip);

            // Set if link is clicked.
            urlColor.a = 1f;
            GUIMethods.BeginGUIContentColor(urlColor);
            GUILayout.BeginHorizontal();
            GUILayout.Label(linkContent, linkStyle);
            var rect = GUILayoutUtility.GetLastRect();
            GUILayout.Label(icon, linkStyle);
            GUILayout.EndHorizontal();

            if (IsValidURL(url))
            {
                float contentWidth = linkStyle.CalcSize(new GUIContent(caption)).x;
                rect.width = icon != null ? contentWidth + EditorGUIUtility.GetIconSize().x + 8f : contentWidth;
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
            }

            // Draw a colored line underneath the link label.
            if (underlineLink)
            {
                UnderlineLink(rect, urlColor);
            }

            Color defaultGUIColor = GUI.color;
            GUI.color = Color.clear;
            bool isClicked = GUI.Button(rect, "");
            GUI.color = defaultGUIColor;

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
            EditorGUIUtility.SetIconSize(defaultIconSize);
        }

        #region Overloading Method(s)
        public static void Draw(string url)
        {
            GUIContent linkLabelContent = new GUIContent();
            Draw(url, linkLabelContent, GUIMethods.GetColorFromHexCode(DefaultColor), DefaultLinkLabelStyle);
        }

        public static void Draw(string url, string caption)
        {
            GUIContent linkLabelContent = new GUIContent(caption);
            Draw(url, linkLabelContent, GUIMethods.GetColorFromHexCode(DefaultColor), DefaultLinkLabelStyle);
        }

        public static void Draw(string url, string caption, string hexColorCode)
        {
            GUIContent linkLabelContent = new GUIContent(caption);
            Color hexColor = GUIMethods.GetColorFromHexCode(hexColorCode);
            Draw(url, linkLabelContent, hexColor, DefaultLinkLabelStyle);
        }

        public static void Draw(string url, string caption, string hexColorCode, GUIStyle linkStyle,
                                bool underlineLink = false, bool displayIcon = false)
        {
            GUIContent linkLabelContent = new GUIContent(caption);
            Color hexColor = GUIMethods.GetColorFromHexCode(hexColorCode);
            Draw(url, linkLabelContent, hexColor, linkStyle, underlineLink, displayIcon);
        }

        public static void Draw(string url, GUIContent linkLabelContent, string hexColorCode, int fontSize, CustomFontStyle fontStyle = CustomFontStyle.Normal,
                                bool underlineLink = false, bool displayIcon = false)
        {
            GUIStyle linkStyle = DefaultLinkLabelStyle;
            linkStyle.fontSize = fontSize;
            underlineLink = fontStyle.HasFlag(CustomFontStyle.Underline);
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

            Color hexColor = GUIMethods.GetColorFromHexCode(hexColorCode);
            Draw(url, linkLabelContent, hexColor, linkStyle, underlineLink, displayIcon);
        }

        public static void Draw(string url, GUIContent linkLabelContent, string hexColorCode, int fontSize, int fontStyleID = 0,
                                bool underlineLink = false, bool displayIcon = false)
        {
            CustomFontStyle fontStyle = (CustomFontStyle)fontStyleID;
            Draw(url, linkLabelContent, hexColorCode, fontSize, fontStyle, underlineLink, displayIcon);
        }
        #endregion

        #endregion

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
