using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LinkLabels
{
    public static class LinkLabel
    {
        public static void Draw(string url, string caption, Color urlColor)
        {
            // GUI Style: Link.
            GUIStyle linkStyle = new GUIStyle(EditorStyles.linkLabel)
            {
                border = new RectOffset(0, 0, 0, 0),
                fontSize = 24,
                richText = true
            };

            caption = string.IsNullOrWhiteSpace(caption) ? url : caption;
            caption = string.Format($"<color=#{ColorUtility.ToHtmlStringRGB(urlColor)}>{caption}</color>");

            //Color defaultContentColor = GUI.contentColor;
            //GUI.contentColor = Color.black + (urlColor * 2f);

            GUIContent linkContent = new GUIContent(caption, "");

            // Set if link is clicked.
            bool isClicked = GUILayout.Button(linkContent, linkStyle);

            var rect = GUILayoutUtility.GetLastRect();
            rect.width = linkStyle.CalcSize(new GUIContent(caption)).x;
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

            // Draw a colored line underneath the link label.
            Handles.BeginGUI();
            Handles.color = urlColor;
            Handles.DrawLine(new Vector3(rect.xMin, rect.yMax), new Vector3(rect.xMax, rect.yMax), 2f);
            Handles.color = Color.white;
            Handles.EndGUI();

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
        #endregion

        private static void OpenURL(string url) => Application.OpenURL(url);
    }
}
