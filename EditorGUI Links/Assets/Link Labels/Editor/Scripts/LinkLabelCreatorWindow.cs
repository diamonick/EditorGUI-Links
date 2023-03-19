using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LinkLabels;

namespace LinkLabels
{
    public class LinkLabelCreatorWindow : EditorWindow
    {
        private static LinkLabelCreatorWindow window;    // Editor window.

        // Banner
        // Note: Make sure to import the package(s) under Assets to have the banner display properly in the editor window.
        private readonly string bannerPath = "Assets/Link Labels/Link Labels Banner.png";

        // Icon paths
        // Note: Make sure to import the package(s) under Assets to have all icons display properly in the editor window.
        private readonly string infoTooltipIconPath = "Assets/Link Labels/InfoTooltipIcon.png";
        private readonly string copyIconPath = "Assets/Link Labels/CopyIcon.png";

        private static readonly string windowTitle = "Link Label Creator";
        private static readonly string description = "Link Labels is a lightweight asset which allows developers to easily create " +
                                                     "and display custom links in editor windows.\n\n" +
                                                     "For more information on implementing Link Labels, check out the official online " +
                                                     "documentation (PDF) in the link below.";
        private static readonly string examplesInfo = "The examples below showcase a wide variety of link labels you can create.";
        private string urlText;
        private string previewText;
        private Color linkLabelColor;

        private bool underlinkLink;
        private bool displayLinkIcon;

        #region GUI Styles
        private GUIStyle defaultStyle;
        private GUIStyle infoIconStyle;
        private GUIStyle descriptionStyle;
        private GUIStyle copyButtonStyle;
        #endregion

        #region Tooltips
        private static readonly string urlTooltip = "Type a valid URL.";
        private static readonly string captionTooltip = "Type a caption. If a caption is not provided, the URL will be used " +
                                                        "as the name of the link instead.";
        private static readonly string linkColorTooltip = "Choose a custom link color.";
        private static readonly string formatOptionsTooltip = "Change link label formatting such as underlinng or adding icon(s).";
        #endregion

        /// <summary>
        /// Display the Link Labels menu item. (Tools -> Link Label Creator)
        /// </summary>
        [MenuItem("Tools/Link Label Creator", false, 10)]
        public static void DisplayWindow()
        {
            window = GetWindow<LinkLabelCreatorWindow>(windowTitle);
            //CreateInstance<LinkLabelCreatorWindow>().Show();
        }

        /// <summary>
        /// Editor GUI.
        /// </summary>
        private void OnGUI()
        {
            // Get window.
            if (window == null)
            {
                window = GetWindow<LinkLabelCreatorWindow>(windowTitle);
            }

            defaultStyle = new GUIStyle(GUI.skin.label);

            descriptionStyle = new GUIStyle(GUI.skin.label)
            {
                wordWrap = true,
            };

            infoIconStyle = new GUIStyle()
            {
                contentOffset = new Vector2(0f, 3f),
            };

            copyButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedWidth = 24,
                fixedHeight = 24
            };

            #region Banner
            // Get banner image texture.
            Texture2D banner = (Texture2D)AssetDatabase.LoadAssetAtPath(bannerPath, typeof(Texture2D));
            if (banner != null)
            {
                float bannerHeight = banner.height;                 // Banner height
                float bannerWidth = banner.width;                   // Banner width
                float aspectRatio = bannerHeight / bannerWidth;     // Aspect ratio
                Rect bannerRect = GUILayoutUtility.GetRect(bannerHeight, aspectRatio * Screen.width * 0.5f);
                EditorGUI.DrawTextureTransparent(bannerRect, banner);
            }
            #endregion

            GUILayout.Label(description, descriptionStyle);

            GUILayout.Space(4f);

            LinkLabel.Draw("https://assetstore.unity.com/", "Online Documentation (PDF)", "#00ff00");
            DrawLine(Color.gray, 1, 4f);
            GUILayout.Space(8f);

            Color defaultBGColor = GUI.backgroundColor;

            GUI.backgroundColor = defaultBGColor;

            GUILayout.BeginHorizontal();
            GUILayout.Label("URL", new GUIStyle(EditorStyles.boldLabel), GUILayout.ExpandWidth(false));
            DrawInfoTooltipIcon(urlTooltip);
            GUILayout.EndHorizontal();
            urlText = GUILayout.TextField(urlText);

            var rect = GUILayoutUtility.GetLastRect();
            rect.width = position.width;
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Text);
            GUILayout.Space(5f);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Caption", new GUIStyle(EditorStyles.boldLabel), GUILayout.ExpandWidth(false));
            DrawInfoTooltipIcon(captionTooltip);
            GUILayout.EndHorizontal();
            previewText = GUILayout.TextField(previewText);

            var captionRect = GUILayoutUtility.GetLastRect();
            captionRect.width = position.width;
            EditorGUIUtility.AddCursorRect(captionRect, MouseCursor.Text);
            GUILayout.Space(5f);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Link Color", new GUIStyle(EditorStyles.boldLabel), GUILayout.ExpandWidth(false));
            DrawInfoTooltipIcon(linkColorTooltip);
            GUILayout.EndHorizontal();
            linkLabelColor = EditorGUILayout.ColorField(linkLabelColor);
            GUILayout.Space(5f);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Format Options", new GUIStyle(EditorStyles.boldLabel), GUILayout.ExpandWidth(false));
            DrawInfoTooltipIcon(formatOptionsTooltip);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorStyles.helpBox);
            underlinkLink = GUILayout.Toggle(underlinkLink, " Underlink Link?");
            displayLinkIcon = GUILayout.Toggle(displayLinkIcon, " Display External Link icon?");
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            DrawLine(Color.gray, 1, 4f);

            GUILayout.Label("Link Label Preview:", new GUIStyle(EditorStyles.boldLabel));
            GUI.backgroundColor = Color.black * 2f;
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(EditorStyles.helpBox);
            LinkLabel.Draw(urlText, previewText, linkLabelColor, underlinkLink, displayLinkIcon);
            GUILayout.EndVertical();

            GUI.backgroundColor = Color.white;
            var copyIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(copyIconPath, typeof(Texture2D));
            if (GUILayout.Button(copyIcon, copyButtonStyle))
            {
                string methodText = GetLinkLabelMethodText();
                CopyToClipboard(methodText);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
        }

        #region Draw Method(s)
        /// <summary>
        /// Draw a line in the editor window.
        /// </summary>
        /// <param name="lineColor">Line color.</param>
        /// <param name="height">Line height.</param>
        /// <param name="spacing">Spacing.</param>
        private static void DrawLine(Color lineColor, int height, float spacing)
        {
            GUIStyle horizontalLine = new GUIStyle();
            horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
            horizontalLine.margin = new RectOffset(4, 4, height, height);
            horizontalLine.fixedHeight = height;

            GUILayout.Space(spacing);

            var c = GUI.color;
            GUI.color = lineColor;
            GUILayout.Box(GUIContent.none, horizontalLine);
            GUI.color = c;

            GUILayout.Space(spacing);
        }

        private void DrawInfoTooltipIcon(string tooltip)
        {
            var icon = (Texture2D)AssetDatabase.LoadAssetAtPath(infoTooltipIconPath, typeof(Texture2D));
            GUIContent iconContent = new GUIContent(icon, tooltip);
            EditorGUIUtility.SetIconSize(new Vector2(12f, 12f));
            GUILayout.Label(iconContent, infoIconStyle, GUILayout.ExpandWidth(false));
        }
        #endregion

        /// <summary>
        /// Copies a string to the Clipboard.
        /// </summary>
        public void CopyToClipboard(string s)
        {
            GUIUtility.systemCopyBuffer = s;

            // Display quick notification.
            window.ShowNotification(new GUIContent($"Copied!"));
        }

        private string GetLinkLabelMethodText()
        {
            string linkLabelColorString = $"new Color({linkLabelColor.r}f, {linkLabelColor.g}f, {linkLabelColor.b}f)";
            string underlineLinkString = underlinkLink.ToString().ToLower();
            string displayLinkIconString = displayLinkIcon.ToString().ToLower();
            return $"LinkLabel.Draw(\"{urlText}\", \"{previewText}\", {linkLabelColorString}, {underlineLinkString}, {displayLinkIconString});";
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

