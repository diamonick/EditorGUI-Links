using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LinkLabels;

namespace LinkLabels
{
    [Flags]
    public enum CustomFontStyle
    {
        Normal = 0,
        Bold = 1,
        Italic = 2,
        Underline = 3,
        BoldandItalic = 4
    }

    public enum LetterCase
    {
        None = 0,
        Upper = 1,
        Lower = 2
    }

    public class LinkLabelCreatorWindow : EditorWindow
    {
        private static LinkLabelCreatorWindow window;    // Editor window.
        private static readonly string DefaultColor = "#4C86FC";

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

        #region Format Options
        private Color linkLabelColor;
        private bool IsDefaultColor
        {
            get
            {
                return linkLabelColor == GetColorFromHexCode(DefaultColor);
            }
        }
        private CustomFontStyle fontStyle = CustomFontStyle.Normal;
        private int fontStyleID { get { return (int)fontStyle; } }
        string[] fontStyleStrings = { "Normal", "Bold", "Italic", "Underline" };
        private LetterCase letterCase = LetterCase.None;
        string[] letterCaseStrings = { "None", "Uppercase", "Lowercase" };
        private int fontSize = 12;
        private bool underlineLink = false;
        private bool displayLinkIcon = false;
        #endregion

        #region GUI Styles
        private GUIStyle infoIconStyle;
        private GUIStyle descriptionStyle;
        private GUIStyle copyButtonStyle;
        private GUIStyle incrementButtonStyle;
        private GUIStyle customLinkLabelStyle;
        #endregion

        #region Tooltips
        private static readonly string urlTooltip = "Type a valid URL.";
        private static readonly string captionTooltip = "Type a caption. If a caption is not provided, the URL will be used " +
                                                        "as the name of the link instead.";
        private static readonly string linkColorTooltip = "Choose a custom link color.";
        private static readonly string formatOptionsTooltip = "Change link label formatting such as underlinng or adding icon(s).";
        private static readonly string fontStyleTooltip = "Type a valid URL.";
        private static readonly string fontSizeTooltip = "Type a valid URL.";
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

            descriptionStyle = new GUIStyle(GUI.skin.label)
            {
                wordWrap = true,
            };

            infoIconStyle = new GUIStyle()
            {
                contentOffset = new Vector2(0f, 4.5f),
                stretchWidth = false
            };

            copyButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedWidth = 24,
                fixedHeight = 24,
            };

            incrementButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedWidth = 24,
            };

            customLinkLabelStyle = new GUIStyle(EditorStyles.linkLabel)
            {
                fontSize = this.fontSize,
                border = new RectOffset(0, 0, 0, 0),
                richText = true,
                wordWrap = false
            };

            switch (fontStyle)
            {
                case CustomFontStyle.Underline:
                    underlineLink = true;
                    break;
                default:
                    underlineLink = false;
                    customLinkLabelStyle.fontStyle = (FontStyle)fontStyleID;
                    break;
            }

            // Set minimum & maximum window size (Docked / Windowed).
            if (window.docked)
            {
                window.minSize = new Vector2(360f, 312f);
                window.maxSize = new Vector2(1920f, 1080f);
            }
            else
            {
                window.minSize = new Vector2(360f, 312f);
                window.maxSize = new Vector2(1920f, 1080f);
            }

            // Calculate field width.
            float fieldWidth = position.width / 2f;

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
            urlText = EditorGUILayout.TextField(urlText);

            GUILayout.Space(5f);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Caption", new GUIStyle(EditorStyles.boldLabel), GUILayout.ExpandWidth(false));
            DrawInfoTooltipIcon(captionTooltip);
            GUILayout.EndHorizontal();
            previewText = EditorGUILayout.TextField(previewText);

            GUILayout.Space(5f);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Format Options", new GUIStyle(EditorStyles.boldLabel), GUILayout.ExpandWidth(false));
            DrawInfoTooltipIcon(formatOptionsTooltip);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal();
            DrawBulletPoint("#61ffda");
            GUILayout.Label("Font Style", GUILayout.ExpandWidth(false));
            DrawInfoTooltipIcon(fontStyleTooltip, true);
            fontStyle = (CustomFontStyle)EditorGUILayout.Popup((int)fontStyle, fontStyleStrings, GUILayout.Width(fieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.Space(5f);

            GUILayout.BeginHorizontal();
            DrawBulletPoint("#ffbd61");
            GUILayout.Label("Letter Case", GUILayout.ExpandWidth(false));
            DrawInfoTooltipIcon(fontStyleTooltip, true);
            letterCase = (LetterCase)EditorGUILayout.Popup((int)letterCase, letterCaseStrings, GUILayout.Width(fieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.Space(5f);

            GUILayout.BeginHorizontal();
            DrawBulletPoint("#61ffda");
            GUILayout.Label("Font Size", GUILayout.ExpandWidth(false));
            DrawInfoTooltipIcon(fontSizeTooltip, true);
            if (GUILayout.Button("-", incrementButtonStyle))
            {
                fontSize = Mathf.Clamp(fontSize - 1, 1, short.MaxValue);
            }
            fontSize = EditorGUILayout.IntField(Mathf.Clamp(fontSize, 1, short.MaxValue), GUILayout.ExpandWidth(false), GUILayout.Width(fieldWidth - 54));
            if (GUILayout.Button("+", incrementButtonStyle))
            {
                fontSize = Mathf.Clamp(fontSize + 1, 1, short.MaxValue);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5f);

            GUILayout.BeginHorizontal();
            DrawBulletPoint("#ffbd61");
            GUILayout.Label("Link Color", GUILayout.ExpandWidth(false));
            DrawInfoTooltipIcon(linkColorTooltip, true);
            linkLabelColor = EditorGUILayout.ColorField(linkLabelColor, GUILayout.ExpandWidth(true), GUILayout.Width(fieldWidth - 26f));
            GUI.enabled = !IsDefaultColor;
            GUI.backgroundColor = IsDefaultColor ? Color.white : GetColorFromHexCode("#00ff00") * 2f;
            if (GUILayout.Button("R", incrementButtonStyle))
            {
                ResetLinkLabelColor();
            }
            GUI.backgroundColor = Color.white;
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.Space(5f);

            GUILayout.BeginHorizontal();
            DrawBulletPoint("#61ffda");
            displayLinkIcon = EditorGUILayout.Toggle("Display External Link icon?", displayLinkIcon);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            DrawLine(Color.gray, 1, 4f);

            GUILayout.Label("Link Label Preview:", new GUIStyle(EditorStyles.boldLabel));
            GUI.backgroundColor = Color.black * 2f;
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(EditorStyles.helpBox);
            LinkLabel.Draw(urlText, previewText, linkLabelColor, customLinkLabelStyle, underlineLink, displayLinkIcon, letterCase);
            GUILayout.EndVertical();

            GUI.backgroundColor = Color.white;
            var copyIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(copyIconPath, typeof(Texture2D));
            EditorGUIUtility.SetIconSize(new Vector2(12f, 12f));
            if (GUILayout.Button(copyIcon, copyButtonStyle))
            {
                string methodText = ParseLinkLabelMethod();
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

        private void DrawInfoTooltipIcon(string tooltip, bool expandWidth = false)
        {
            var icon = (Texture2D)AssetDatabase.LoadAssetAtPath(infoTooltipIconPath, typeof(Texture2D));
            GUIContent iconContent = new GUIContent(icon, tooltip);
            EditorGUIUtility.SetIconSize(new Vector2(10f, 10f));
            GUILayout.Label(iconContent, infoIconStyle);
            GUILayout.Label("", GUILayout.ExpandWidth(expandWidth));
        }

        /// <summary>
        /// Draw bullet point: "•"
        /// </summary>
        /// <param name="bulletPointColor">Bullet point color string (Hexadecimal).</param>
        /// <param name="indents">Indention level. Default: 0</param>
        protected static void DrawBulletPoint(string bulletPointColor, int indents = 0)
        {
            // GUI Style: Bullet Point
            GUIStyle bulletPointStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                stretchWidth = true,
                fixedWidth = 12 + (24 * indents),
                contentOffset = new Vector2(24 * indents, 0f)
            };

            // Draw bullet point w/ the specified color.
            GUI.contentColor = GetColorFromHexCode(bulletPointColor);
            GUILayout.Label("•", bulletPointStyle);
            GUI.contentColor = Color.white;
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

        private string ParseLinkLabelMethod()
        {
            string linkLabelColorString = $"new Color({linkLabelColor.r}f, {linkLabelColor.g}f, {linkLabelColor.b}f)";
            string underlineLinkString = underlineLink.ToString().ToLower();
            string displayLinkIconString = displayLinkIcon.ToString().ToLower();

            string guiStyleString = "GUIStyle newStyle = new GUIStyle(EditorStyles.linkLabel)" +
                                    "{" +
                                    "}";

            return $"LinkLabel.Draw(\"{urlText}\", \"{previewText}\", {linkLabelColorString}, {8}," +
                   $"{underlineLinkString}, {displayLinkIconString}, LetterCase.{letterCase});";
        }

        private void ResetLinkLabelColor()
        {
            linkLabelColor = GetColorFromHexCode(DefaultColor);
        }

        /// <summary>
        /// Get color from hex string.
        /// </summary>
        /// <param name="hexColorCode">Hex color code string.</param>
        /// <returns>New color.</returns>
        private static Color GetColorFromHexCode(string hexColorCode)
        {
            Color color = Color.white;
            ColorUtility.TryParseHtmlString(hexColorCode, out color);
            return color;
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

