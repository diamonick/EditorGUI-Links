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

        private static readonly string windowTitle = "Link Label Creator";
        private static readonly string description = "Link Labels is a lightweight asset which allows developers to easily create " +
                                                     "and display custom links in editor windows.\n\n" +
                                                     "For more information on implementing Link Labels, check out the official online " +
                                                     "documentation (PDF) in the link below.";
        private static readonly string examplesInfo = "The examples below showcase a wide variety of link labels you can create.";
        private string urlText;
        private string previewText;
        private Color linkLabelColor;

        #region GUI Styles
        private GUIStyle defaultStyle;
        private GUIStyle infoIconStyle;
        private GUIStyle descriptionStyle;
        #endregion

        #region Tooltips
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
            DrawInfoTooltipIcon("Type a valid URL.");
            GUILayout.EndHorizontal();
            urlText = GUILayout.TextField(urlText);

            var rect = GUILayoutUtility.GetLastRect();
            rect.width = position.width;
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Text);
            GUILayout.Space(5f);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Caption", new GUIStyle(EditorStyles.boldLabel), GUILayout.ExpandWidth(false));
            DrawInfoTooltipIcon("Type a caption.");
            GUILayout.EndHorizontal();
            previewText = GUILayout.TextField(previewText);

            var captionRect = GUILayoutUtility.GetLastRect();
            captionRect.width = position.width;
            EditorGUIUtility.AddCursorRect(captionRect, MouseCursor.Text);
            GUILayout.Space(5f);

            GUILayout.Label("Select a custom link color:", new GUIStyle(EditorStyles.boldLabel));
            linkLabelColor = EditorGUILayout.ColorField(linkLabelColor);

            GUILayout.FlexibleSpace();
            DrawLine(Color.gray, 1, 4f);

            GUILayout.Label("Link Label Preview:", new GUIStyle(EditorStyles.boldLabel));
            GUI.backgroundColor = Color.black * 2f;
            GUILayout.BeginVertical(EditorStyles.helpBox);
            LinkLabel.Draw(urlText, previewText, linkLabelColor);
            GUILayout.EndVertical();

        }

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

            var rect = GUILayoutUtility.GetLastRect();
            rect.width = infoIconStyle.CalcSize(new GUIContent(icon)).x;
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
        }

        /// <summary>
        /// Draw box w/ multiple selectable options.
        /// </summary>
        /// <param name="addParenthesesOption">Boolean parameter to add parentheses ().</param>
        /// <param name="addBracketsOption">Boolean parameter to add brackets [].</param>
        /// <param name="addBracesOption">Boolean parameter to add braces {}.</param>
        /// <param name="addUnderscoreOption">Boolean parameter to add underscore _.</param>
        /// <param name="addHyphenOption">Boolean parameter to add hyphen -.</param>
        private void DrawFormatBox(ref bool addParenthesesOption, ref bool addBracketsOption, ref bool addBracesOption,
                                         ref bool addUnderscoreOption, ref bool addHyphenOption)
        {
            GUILayout.Label("Format number with:");
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            addParenthesesOption = GUILayout.Toggle(addParenthesesOption, " Parentheses \"()\"", EditorStyles.radioButton);
            if (addParenthesesOption)
            {
                addBracketsOption = false;
                addBracesOption = false;
                addUnderscoreOption = false;
                addHyphenOption = false;
            }
            addBracketsOption = GUILayout.Toggle(addBracketsOption, " Brackets \"[]\"", EditorStyles.radioButton);
            if (addBracketsOption)
            {
                addParenthesesOption = false;
                addBracesOption = false;
                addUnderscoreOption = false;
                addHyphenOption = false;
            }
            addBracesOption = GUILayout.Toggle(addBracesOption, " Braces \"{}\"", EditorStyles.radioButton);
            if (addBracesOption)
            {
                addParenthesesOption = false;
                addBracketsOption = false;
                addUnderscoreOption = false;
                addHyphenOption = false;
            }
            addUnderscoreOption = GUILayout.Toggle(addUnderscoreOption, " Underscore \"_\"", EditorStyles.radioButton);
            if (addUnderscoreOption)
            {
                addParenthesesOption = false;
                addBracketsOption = false;
                addBracesOption = false;
                addHyphenOption = false;
            }
            addHyphenOption = GUILayout.Toggle(addHyphenOption, " Hyphen \"-\"", EditorStyles.radioButton);
            if (addHyphenOption)
            {
                addParenthesesOption = false;
                addBracketsOption = false;
                addBracesOption = false;
                addUnderscoreOption = false;
            }
            EditorGUILayout.EndHorizontal();
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

