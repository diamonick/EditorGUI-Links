﻿using System;
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
        public static void Draw(string url, string caption, Color urlColor, GUIStyle linkStyle,
                                LetterCase letterCase = LetterCase.None, bool underlineLink = false, bool displayIcon = false)
        {
            Vector2 defaultIconSize = EditorGUIUtility.GetIconSize();

            var icon = (Texture2D)AssetDatabase.LoadAssetAtPath(externalLinkIconPath, typeof(Texture2D));
            float iconSize = icon != null ? linkStyle.fontSize : 0f;
            EditorGUIUtility.SetIconSize(new Vector2(iconSize, iconSize));
            icon = displayIcon ? icon : null;

            caption = string.IsNullOrWhiteSpace(caption) ? url : caption;
            if (displayIcon)
            {
                caption = icon != null ? $"{caption}" : $"{caption} ↗";
            }

            FormatLinkLabelByCase(ref caption, letterCase);
            caption = string.Format($"<color=#{ColorUtility.ToHtmlStringRGB(urlColor)}>{caption}</color>");

            GUIContent linkContent = new GUIContent(caption);

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
            Draw(url, url, GUIMethods.GetColorFromHexCode(DefaultColor), DefaultLinkLabelStyle);
        }

        public static void Draw(string url, string caption)
        {
            Draw(url, caption, GUIMethods.GetColorFromHexCode(DefaultColor), DefaultLinkLabelStyle);
        }

        public static void Draw(string url, string caption, string hexColorCode)
        {
            Color hexColor = GUIMethods.GetColorFromHexCode(hexColorCode);
            Draw(url, caption, hexColor, DefaultLinkLabelStyle);
        }

        public static void Draw(string url, string caption, string hexColorCode, GUIStyle linkStyle,
                                LetterCase letterCase = LetterCase.None, bool underlineLink = false, bool displayIcon = false)
        {
            Color hexColor = GUIMethods.GetColorFromHexCode(hexColorCode);
            Draw(url, caption, hexColor, linkStyle, letterCase, underlineLink, displayIcon);
        }

        public static void Draw(string url, string caption, string hexColorCode, int fontSize, CustomFontStyle fontStyle = CustomFontStyle.Normal,
                                LetterCase letterCase = LetterCase.None, bool underlineLink = false, bool displayIcon = false)
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
            Draw(url, caption, hexColor, linkStyle, letterCase, underlineLink, displayIcon);
        }

        public static void Draw(string url, string caption, string hexColorCode, int fontSize, int fontStyleID = 0,
                                int letterCaseID = 0, bool underlineLink = false, bool displayIcon = false)
        {
            CustomFontStyle fontStyle = (CustomFontStyle)fontStyleID;
            LetterCase letterCase = (LetterCase)letterCaseID;

            Draw(url, caption, hexColorCode, fontSize, fontStyle, letterCase, underlineLink, displayIcon);
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

        private static void FormatLinkLabelByCase(ref string linkLabel, LetterCase lc)
        {
            switch (lc)
            {
                case LetterCase.Upper:
                    linkLabel = linkLabel.ToUpper();
                    break;
                case LetterCase.Lower:
                    linkLabel = linkLabel.ToLower();
                    break;
            }
        }

        private static void OpenURL(string url)
        {
            if (!IsValidURL(url))
                return;

            Application.OpenURL(url);
        }

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
