using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LinkLabels
{
    public enum LinkFormat
    {
        None = 0,
        Underline = 1,
        Arrow = 2
    }

    public static class LinkLabel
    {
        // Default color: #4C86FC

        // Icon paths
        // Note: Make sure to import the package(s) under Assets to have all icons display properly in the editor window.
        private static readonly string externalLinkIconPath = "Assets/Link Labels/ExternalLinkIcon.png";

        public static void Draw(string url, string caption, Color urlColor, bool underlinkLink = false, bool displayIcon = false)
        {
            // GUI Style: Link.
            GUIStyle linkStyle = new GUIStyle(EditorStyles.linkLabel)
            {
                border = new RectOffset(0, 0, 0, 0),
                fontSize = 12,
                richText = true
            };

            var icon = (Texture2D)AssetDatabase.LoadAssetAtPath(externalLinkIconPath, typeof(Texture2D));
            EditorGUIUtility.SetIconSize(new Vector2(12f, 12f));
            icon = displayIcon ? icon : null;

            caption = string.IsNullOrWhiteSpace(caption) ? url : caption;
            if (displayIcon)
            {
                caption = icon != null ? $" {caption}" : $"{caption} ↗";
            }
            caption = string.Format($"<color=#{ColorUtility.ToHtmlStringRGB(urlColor)}>{caption}</color>");

            GUIContent linkContent = new GUIContent(caption, icon, "");

            // Set if link is clicked.
            GUI.contentColor = urlColor;
            bool isClicked = GUILayout.Button(linkContent, linkStyle);
            GUI.contentColor = Color.white;

            var rect = GUILayoutUtility.GetLastRect();
            float contentWidth = linkStyle.CalcSize(new GUIContent(caption)).x;
            rect.width = icon != null ? contentWidth + EditorGUIUtility.GetIconSize().x : contentWidth;
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

            // Draw a colored line underneath the link label.
            if (underlinkLink)
            {
                UnderlineLink(rect, urlColor);
            }

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

            //GUI.contentColor = defaultContentColor;
        }

        #region Overloading Method(s)
        public static void Draw(string url)
        {
            Draw(url, url, Color.green);
        }

        public static void Draw(string url, string caption)
        {
            Draw(url, caption, Color.green);
        }

        public static void Draw(string url, string caption, string hexColorCode)
        {
            Color hexColor;
            ColorUtility.TryParseHtmlString(hexColorCode, out hexColor);
            Draw(url, caption, hexColor);
        }

        public static void Draw(string url, string caption, string hexColorCode, bool underlinkLink = false, bool displayIcon = false)
        {
            Color hexColor;
            ColorUtility.TryParseHtmlString(hexColorCode, out hexColor);
            Draw(url, caption, hexColor, underlinkLink, displayIcon);
        }
        #endregion

        private static void UnderlineLink(Rect rect, Color lineColor)
        {
            Handles.BeginGUI();
            Handles.color = lineColor;
            Handles.DrawLine(new Vector3(rect.xMin, rect.yMax), new Vector3(rect.xMax, rect.yMax), 2f);
            Handles.color = Color.white;
            Handles.EndGUI();
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
    }
}
