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
        Underline = 4,
    }

    public enum LetterCase
    {
        None = 0,
        Upper = 1,
        Lower = 2
    }

    public class LinkLabelCreatorWindow : EditorWindow
    {
        #region Constants
        private const int MIN_FONT_SIZE = 1;
        private const int MAX_FONT_SIZE = 100;
        #endregion

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
                                                     "Feel free to use the Link Label Creator to quickly create custom link labels, " +
                                                     "and copy/paste them directly into your preferred code editor/IDE.\n\n" +
                                                     "For more information on implementing Link Labels, check out the official online " +
                                                     "documentation (PDF) in the link below.";
        private static readonly string invalidURLWarning = "Invalid URL address. Please provide a valid URL in the text field " +
                                                           "above to open the specified URL.";

        private string urlText = "https://docs.unity3d.com/Manual/index.html";
        private string previewText = "Unity User Manual";

        #region Format Options
        private bool IsDefaultColor
        {
            get
            {
                return linkLabelColor == GUIMethods.GetColorFromHexCode(DefaultColor);
            }
        }
        private CustomFontStyle fontStyle;
        private int fontStyleID { get { return (int)fontStyle; } }
        string[] fontStyleStrings = { "Normal", "Bold", "Italic", "Bold and Italic", "Underline" };
        private LetterCase letterCase = LetterCase.None;
        string[] letterCaseStrings = { "None", "Uppercase", "Lowercase" };
        private int fontSize = 12;
        private Color linkLabelColor = new Color(76f / 255f, 134f / 255f, 252f / 255f, 1f);
        private bool underlineLink = false;
        private bool displayLinkIcon = false;
        #endregion

        #region GUI Styles
        private GUIStyle infoIconStyle;
        private GUIStyle descriptionStyle;
        private GUIStyle formatOptionStyle;
        private GUIStyle incrementButtonStyle;
        private GUIStyle customLinkLabelStyle;
        private GUIStyle copyButtonStyle;
        #endregion

        #region Tooltips
        private static readonly string urlTooltip = "Type a valid URL.";
        private static readonly string captionTooltip = "Type a caption. If a caption is not provided, the URL will be used " +
                                                        "as the name of the link instead.";
        private static readonly string formatOptionsTooltip = "Change link label formatting such as font style, size, and more.\n\n" +
                                                              "• Font Style: Set the font style(s) of the link label.\n" +
                                                              "• Letter Case: Set the letter case of the link label.\n" +
                                                              "• Font Size: Set the font size of the link label.\n" +
                                                              "• Link Color: Set the color of the link label.\n" +
                                                              "• Display Icon?: Toggle whether to display an external link icon after " +
                                                              "the link label.\n";
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
                stretchWidth = false
            };

            copyButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = 48,
                fontSize = 20,
                fontStyle = FontStyle.Bold
            };

            formatOptionStyle = new GUIStyle(EditorStyles.label)
            {
                stretchWidth = true
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

            underlineLink = fontStyle.HasFlag(CustomFontStyle.Underline);
            if (fontStyle.HasFlag(CustomFontStyle.Underline))
            {
                if (fontStyle.HasFlag(CustomFontStyle.Bold) && fontStyle.HasFlag(CustomFontStyle.Italic))
                {
                    customLinkLabelStyle.fontStyle = FontStyle.BoldAndItalic;
                }
                else if (fontStyle.HasFlag(CustomFontStyle.Bold))
                {
                    customLinkLabelStyle.fontStyle = FontStyle.Bold;
                }
                else if (fontStyle.HasFlag(CustomFontStyle.Italic))
                {
                    customLinkLabelStyle.fontStyle = FontStyle.Italic;
                }
            }
            else
            {
                customLinkLabelStyle.fontStyle = (FontStyle)((int)fontStyle);
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
            float fieldWidth = (position.width / 2f) + 29f;

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

            LinkLabel.Draw("https://assetstore.unity.com/", "Online Documentation (PDF)", "#00FFE1", 12, 1, 0, false, true);
            DrawLine(Color.gray, 1, 4f);
            GUILayout.Space(8f);

            Color defaultBGColor = GUI.backgroundColor;

            GUI.backgroundColor = defaultBGColor;

            GUILayout.BeginHorizontal();
            GUILayout.Label("URL", new GUIStyle(EditorStyles.boldLabel), GUILayout.ExpandWidth(false));
            DrawInfoTooltipIcon(urlTooltip);
            GUILayout.EndHorizontal();
            urlText = EditorGUILayout.TextField(urlText);

            // Display a warning block if the URL is invalid.
            if (!IsValidURL(urlText))
            {
                GUIMethods.BeginGUIBackgroundColor(GUIMethods.GetColorFromHexCode("#ffa200") * 1.5f);
                GUIMethods.BeginGUIContentColor(GUIMethods.GetColorFromHexCode("#ffd000") * 1.5f);
                EditorGUILayout.HelpBox(invalidURLWarning, MessageType.Warning);
                GUIMethods.EndGUIContentColor();
                GUIMethods.EndGUIBackgroundColor();
            }

            GUILayout.Space(5f);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Caption", new GUIStyle(EditorStyles.boldLabel), GUILayout.ExpandWidth(false));
            DrawInfoTooltipIcon(captionTooltip);
            GUILayout.EndHorizontal();
            previewText = EditorGUILayout.TextField(previewText);

            GUILayout.Space(5f);

            #region Format Options
            GUILayout.BeginHorizontal();
            GUILayout.Label("Format Options", new GUIStyle(EditorStyles.boldLabel), GUILayout.ExpandWidth(false));
            DrawInfoTooltipIcon(formatOptionsTooltip);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorStyles.helpBox);

            #region Font Style
            GUILayout.BeginHorizontal();
            DrawBulletPoint("#61ffda");
            GUILayout.Label("Font Style", formatOptionStyle);
            GUIMethods.BeginGUIBackgroundColor(GUIMethods.GetColorFromHexCode("#2bbcff") * 2.5f);
            fontStyle = (CustomFontStyle)EditorGUILayout.EnumFlagsField(fontStyle, GUILayout.Width(fieldWidth));
            GUIMethods.EndGUIBackgroundColor();
            GUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(5f);

            #region Letter Case
            GUILayout.BeginHorizontal();
            DrawBulletPoint("#ffbd61");
            GUILayout.Label("Letter Case", formatOptionStyle);
            GUIMethods.BeginGUIBackgroundColor(GUIMethods.GetColorFromHexCode("#ff9400") * 2.5f);
            letterCase = (LetterCase)EditorGUILayout.Popup((int)letterCase, letterCaseStrings, GUILayout.Width(fieldWidth));
            GUIMethods.EndGUIBackgroundColor();
            GUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(5f);

            #region Font Size
            GUILayout.BeginHorizontal();
            DrawBulletPoint("#61ffda");
            GUILayout.Label("Font Size", formatOptionStyle);
            GUI.enabled = fontSize > MIN_FONT_SIZE;
            Color minusButtonColor = fontSize > MIN_FONT_SIZE ? GUIMethods.GetColorFromHexCode("#00f088") * 2f : Color.white;
            GUIMethods.BeginGUIBackgroundColor(minusButtonColor);
            if (GUILayout.Button("-", incrementButtonStyle))
            {
                fontSize = Mathf.Clamp(fontSize - 1, MIN_FONT_SIZE, MAX_FONT_SIZE);
            }
            GUIMethods.EndGUIBackgroundColor();
            GUI.enabled = true;
            fontSize = EditorGUILayout.IntField(Mathf.Clamp(fontSize, MIN_FONT_SIZE, MAX_FONT_SIZE), GUILayout.ExpandWidth(false), GUILayout.Width(fieldWidth - 54));
            GUI.enabled = fontSize < MAX_FONT_SIZE;
            Color plusButtonColor = fontSize < MAX_FONT_SIZE ? GUIMethods.GetColorFromHexCode("#00f088") * 2f : Color.white;
            GUIMethods.BeginGUIBackgroundColor(plusButtonColor);
            if (GUILayout.Button("+", incrementButtonStyle))
            {
                fontSize = Mathf.Clamp(fontSize + 1, MIN_FONT_SIZE, MAX_FONT_SIZE);
            }
            GUIMethods.EndGUIBackgroundColor();
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(5f);

            #region Link Color
            GUILayout.BeginHorizontal();
            DrawBulletPoint("#ffbd61");
            GUILayout.Label("Link Color", formatOptionStyle);
            linkLabelColor = EditorGUILayout.ColorField(linkLabelColor, GUILayout.ExpandWidth(true), GUILayout.Width(fieldWidth - 27f));
            GUI.enabled = !IsDefaultColor;
            GUI.backgroundColor = IsDefaultColor ? Color.white : GUIMethods.GetColorFromHexCode("#00f088") * 2f;
            if (GUILayout.Button("R", incrementButtonStyle))
            {
                ResetLinkLabelColor();
            }
            GUI.backgroundColor = Color.white;
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(5f);

            #region Display Icon?
            GUILayout.BeginHorizontal();
            DrawBulletPoint("#61ffda");
            GUILayout.Label("Display Icon?", formatOptionStyle);
            displayLinkIcon = EditorGUILayout.Toggle(displayLinkIcon);
            GUILayout.EndHorizontal();
            #endregion

            GUILayout.EndVertical();
            #endregion

            GUILayout.FlexibleSpace();
            DrawLine(Color.gray, 1, 4f);

            GUILayout.Label("Link Label Preview:", new GUIStyle(EditorStyles.boldLabel));
            GUI.backgroundColor = Color.black * 2f;
            GUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.ExpandWidth(true));
            LinkLabel.Draw(urlText, previewText, linkLabelColor, customLinkLabelStyle, letterCase, underlineLink, displayLinkIcon);
            GUILayout.EndHorizontal();

            GUILayout.Space(2f);

            GUI.backgroundColor = Color.white;
            var copyIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(copyIconPath, typeof(Texture2D));
            EditorGUIUtility.SetIconSize(new Vector2(18f, 18f));
            GUI.backgroundColor = GUIMethods.GetColorFromHexCode("#0062ff") * 4f;
            GUIContent copyButtonContent = new GUIContent(" Copy", copyIcon);
            if (GUILayout.Button(copyButtonContent, copyButtonStyle))
            {
                string methodText = ParseLinkLabelMethod();
                CopyToClipboard(methodText);
            }
            GUI.backgroundColor = Color.white;

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
            Vector2 defaultIconSize = EditorGUIUtility.GetIconSize();

            GUILayout.BeginVertical();
            GUILayout.Space(4.5f);
            var icon = (Texture2D)AssetDatabase.LoadAssetAtPath(infoTooltipIconPath, typeof(Texture2D));
            EditorGUIUtility.SetIconSize(new Vector2(10f, 10f));
            GUIContent iconContent = new GUIContent(icon, tooltip);
            GUILayout.Label(iconContent, infoIconStyle);
            GUILayout.EndVertical();
            GUILayout.Label("", GUILayout.ExpandWidth(expandWidth));

            EditorGUIUtility.SetIconSize(defaultIconSize);
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
            GUI.contentColor = GUIMethods.GetColorFromHexCode(bulletPointColor);
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
            window.ShowNotification(new GUIContent($"Link Label\n\nCopied!"));
        }

        private string ParseLinkLabelMethod()
        {
            string linkLabelColorHexCode = ColorUtility.ToHtmlStringRGB(linkLabelColor);
            string underlineLinkString = underlineLink.ToString().ToLower();
            string displayLinkIconString = displayLinkIcon.ToString().ToLower();

            string guiStyleString = "GUIStyle newStyle = new GUIStyle(EditorStyles.linkLabel)" +
                                    "{" +
                                    "}";

            return $"LinkLabel.Draw(\"{urlText}\", \"{previewText}\", \"#{linkLabelColorHexCode}\", {fontSize}, " +
                   $"{(int)fontStyle}, {(int)letterCase}, {underlineLinkString}, {displayLinkIconString});";
        }

        private void ResetLinkLabelColor()
        {
            linkLabelColor = GUIMethods.GetColorFromHexCode(DefaultColor);
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

