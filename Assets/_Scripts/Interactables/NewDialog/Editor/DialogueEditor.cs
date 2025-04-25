using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using Dialogue = NewDialogue.Dialogue;
using Unity.Mathematics;
using UnityEngine.UI.Extensions;

namespace NewDialogue
{
    public class DialogueEditor : EditorWindow
    {
        private GUIStyle speechStyle;
        private Vector2 handleSize = new Vector2(20, 20);

        private Rect editorSize = new Rect();
        private Vector2 scrollPosition;

        private Dialogue currentDialogue = null;

        // Dragging data
        private Speech draggedSpeech = null;
        private Vector2 dragOffset = Vector2Int.zero;
        private Speech linkingSpeech = null;

        // Add and remove requests
        private Speech childSpeechRequested;
        private Speech removeSpeechRequested;


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
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                GUILayoutUtility.GetRect(editorSize.size.x, editorSize.size.y);

                ManageEvents();
                DisplaySpeeches();

                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.LabelField("Select a dialogue");
            }

        }

        private void ManageEvents()
        {
            if (linkingSpeech != null)
            {
                DrawBezier(GetHandleRect(linkingSpeech).center, Event.current.mousePosition);
                Repaint();
            }

            if (Event.current.type == EventType.MouseDown)
            {
                draggedSpeech = GetNodeAtPosition(Event.current.mousePosition);
                if (draggedSpeech != null)
                {
                    dragOffset = draggedSpeech.editorPosition.position - Event.current.mousePosition;
                }
                else
                {
                    linkingSpeech = GetHandleAtPosition(Event.current.mousePosition);
                }
            }
            else if (Event.current.type == EventType.MouseDrag)
            {
                if (draggedSpeech != null)
                {
                    Undo.RecordObject(currentDialogue, "Moved dialogue speech");
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
                else if (linkingSpeech != null)
                {
                    Speech childSpeech = GetNodeAtPosition(Event.current.mousePosition);
                    if (childSpeech != null)
                    {
                        Undo.RecordObject(currentDialogue, "Dialogue reparent");
                        currentDialogue.ReparentSpeech(linkingSpeech, childSpeech);
                    }
                    Repaint();
                    linkingSpeech = null;
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

        private Speech GetHandleAtPosition(Vector2 positioin)
        {
            Speech containedSpeech = null;
            foreach (Speech speech in currentDialogue.GetSpeeches())
            {
                if (GetHandleRect(speech).Contains(positioin))
                    containedSpeech = speech;
            }
            return containedSpeech;
        }

        private void DisplaySpeeches()
        {
            childSpeechRequested = null;
            removeSpeechRequested = null;

            foreach (Speech speech in currentDialogue.GetSpeeches())
                DrawSpeechRect(speech);

            CreateSpeech();
            RemoveSpeech();
        }

        private void DrawSpeechRect(Speech speech)
        {
            editorSize.xMin = Mathf.Min(speech.editorPosition.xMin, editorSize.xMin);
            editorSize.yMin = Mathf.Min(speech.editorPosition.yMin, editorSize.yMin);
            editorSize.xMax = Mathf.Max(speech.editorPosition.xMax, editorSize.xMax);
            editorSize.yMax = Mathf.Max(speech.editorPosition.yMax, editorSize.yMax);

            GUILayout.BeginArea(speech.editorPosition, speechStyle);

            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Speaker:", GUILayout.MaxWidth(60f));
            int editingID = EditorGUILayout.IntField(speech.speakerID, GUILayout.MaxWidth(30f));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(currentDialogue, "Edit speech speaker");
                speech.speakerID = editingID;
            }
            GUILayout.EndHorizontal();

            string editingText = EditorGUILayout.TextArea(speech.text);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(currentDialogue, "Edit dialogue text");
                speech.text = editingText;
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add"))
                childSpeechRequested = speech;

            if (GUILayout.Button("Remove"))
                removeSpeechRequested = speech;

            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            Rect handleRect = GetHandleRect(speech);
            GUI.DrawTexture(handleRect, EditorGUIUtility.Load("node1") as Texture2D, ScaleMode.StretchToFill);
            if (GUI.Button(new Rect(new Vector2(speech.editorPosition.xMin, speech.editorPosition.center.y) - handleSize / 2, handleSize), "x"))
            {
                Undo.RecordObject(currentDialogue, "Dialogue unlink");
                currentDialogue.ReparentSpeech(null, speech);
            }

            DrawChildConnections(speech);
        }

        private Rect GetHandleRect(Speech speech)
        {
            return new Rect(new Vector2(speech.editorPosition.xMax, speech.editorPosition.center.y) - handleSize / 2, handleSize);
        }

        private void RemoveSpeech()
        {
            if (currentDialogue != null && removeSpeechRequested != null)
            {
                Undo.RecordObject(currentDialogue, "Removed dialogue speech");
                currentDialogue.DeleteSpeech(removeSpeechRequested);
                removeSpeechRequested = null;
            }
        }

        private void CreateSpeech()
        {
            if (currentDialogue != null && childSpeechRequested != null)
            {
                Undo.RecordObject(currentDialogue, "Added dialogue speech");
                currentDialogue.CreateChildOfSpeech(childSpeechRequested);
                childSpeechRequested = null;
            }
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

                DrawBezier(startPosition, endPosition);
            }
        }

        private static void DrawBezier(Vector2 startPosition, Vector2 endPosition)
        {
            Vector2 lineHelper = new Vector2(Mathf.Clamp(endPosition.x - startPosition.x, -20f, 20f), 0f);
            Vector2 handle1 = startPosition + lineHelper;
            Vector2 handle2 = endPosition - lineHelper;
            Handles.DrawBezier(startPosition, endPosition, handle1, handle2, Color.black, null, 6);
            Handles.DrawBezier(startPosition, endPosition, handle1, handle2, Color.white, null, 2);
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