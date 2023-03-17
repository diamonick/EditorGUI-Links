using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LinkLabels;

namespace LinkLabels
{
    public class EditorGUILinksDemoWindow : EditorWindow
    {
        private static EditorGUILinksDemoWindow window;    // Editor window
        private static readonly string windowTitle = "EditorGUI Links (DEMO)";
        private bool keyFoldout = true;

        #region Tooltips
        #endregion

        /// <summary>
        /// Display the EditorGUI Links menu item. (Tools -> Massive Unicode Symbol Table)
        /// </summary>
        [MenuItem("Tools/EditorGUI Links (DEMO)", false, 10)]
        public static void DisplayWindow()
        {
            window = GetWindow<EditorGUILinksDemoWindow>(windowTitle);
            //CreateInstance<MUSTEditor>().Show();
        }

        /// <summary>
        /// Editor GUI.
        /// </summary>
        private void OnGUI()
        {
            // Get window.
            if (window == null)
            {
                window = GetWindow<EditorGUILinksDemoWindow>(windowTitle);
            }

            LinkLabel.Draw("https://assetstore.unity.com/", "Unity Asset Store", "#00ff00");
        }
    }
}

