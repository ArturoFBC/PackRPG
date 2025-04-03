using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using Dialogue = NewDialogue.Dialogue;

namespace NewDialogue
{
    public class DialogueEditor : EditorWindow
    {
        private GUIStyle speechStyle;

        private Dialogue currentDialogue = null;

        private Speech draggedSpeech = null;
        private Vector2 dragOffset = Vector2Int.zero;

        private Speech childSpeechRequested;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAssetAttribute(1)]
        public static bool OnOpenDialog(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue == null)
                return false;

            return true;
        }

        private void OnGUI()
        {
            if (currentDialogue != null)
            {
                ManageEvents();
                DisplaySpeeches();
            }
            else
            {
                EditorGUILayout.LabelField("Select a dialogue.");
            }

        }

        private void ManageEvents()
        {
            if (Event.current.type == EventType.MouseDown)
            {
                if (draggedSpeech == null)
                {
                    Undo.RecordObject(currentDialogue, "Moved dialogue speech");
                    draggedSpeech = GetNodeAtPosition(Event.current.mousePosition);
                    if (draggedSpeech != null)
                        dragOffset = draggedSpeech.editorPosition.position - Event.current.mousePosition;
                }
            }
            else if (Event.current.type == EventType.MouseDrag)
            {
                if (draggedSpeech != null)
                {
                    draggedSpeech.editorPosition.position = Event.current.mousePosition + dragOffset;
                    Repaint();
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                if (draggedSpeech != null)
                {
                    draggedSpeech = null;
                }
            }
        }

        private Speech GetNodeAtPosition(Vector2 positioin)
        {
            Speech containedSpeech = null;
            foreach (Speech speech in currentDialogue.GetSpeeches())
            {
                if (speech.editorPosition.Contains(positioin))
                    containedSpeech = speech;
            }
            return containedSpeech;
        }

        private void DisplaySpeeches()
        {
            foreach (Speech speech in currentDialogue.GetSpeeches())
            {
                GUILayout.BeginArea(speech.editorPosition, speechStyle);
                EditorGUI.BeginChangeCheck();

                string editingText = EditorGUILayout.TextField(speech.text);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(currentDialogue, "Edit dialogue text");
                    speech.text = editingText;
                }

                if( GUILayout.Button("Add"))
                {
                    FlagCreateSpeech(speech);
                }

                GUILayout.EndArea();

                DrawChildConnections(speech);
            }

            CreateSpeech();
        }

        private void CreateSpeech()
        {
            if (currentDialogue != null && childSpeechRequested != null)
            {
                currentDialogue.CreateChildOfSpeech(childSpeechRequested);
                childSpeechRequested = null;
            }
        }

        private void FlagCreateSpeech(Speech speech)
        {
            childSpeechRequested = speech;
        }

        private void DrawChildConnections(Speech speech)
        {
            Vector2 startPosition = new Vector2(
                speech.editorPosition.xMax,
                speech.editorPosition.center.y);
            foreach (Speech childSpeech in currentDialogue.GetChildrenOfSpeech(speech))
            {
                Vector2 endPosition = new Vector2(
                    childSpeech.editorPosition.xMin,
                    childSpeech.editorPosition.center.y);

                Vector2 lineHelper = new Vector2( Mathf.Clamp( endPosition.x - startPosition.x, -20f, 20f), 0f);
                Vector2 handle1 = startPosition + lineHelper;
                Vector2 handle2 = endPosition - lineHelper;
                Handles.DrawBezier(startPosition, endPosition, handle1, handle2, Color.gray, null, 2);
            }
        }

        private void Awake()
        {
            speechStyle = new GUIStyle();
            Texture2D texture = EditorGUIUtility.Load("node0") as Texture2D;
            speechStyle.normal.background = texture;
            speechStyle.padding = new RectOffset(10, 10, 20, 10);
            speechStyle.border = new RectOffset(12, 12, 22, 12);
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            Dialogue dialogue = Selection.activeObject as Dialogue;

            if (dialogue != null)
            {
                currentDialogue = dialogue;
                Repaint();
            }


        }
    }
}