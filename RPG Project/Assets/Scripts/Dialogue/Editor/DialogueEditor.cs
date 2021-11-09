using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue selectedDialogue;
        [NonSerialized] private GUIStyle nodeStyle;
        [NonSerialized] private GUIStyle playerNodeStyle;
        [NonSerialized] private DialogueNode draggedNode;
        [NonSerialized] private Vector2 draggingOffset;
        [NonSerialized] private DialogueNode creatingNode;
        [NonSerialized] private DialogueNode deletingNode;
        [NonSerialized] private DialogueNode linkingParentNode;
        private Vector2 scrollPosition;
        [NonSerialized] private bool draggingCanvas;
        [NonSerialized] private Vector2 draggingCanvasOffset;

        private const float canvasSize = 4000;
        private const float backgroundSize = 50;
        private Texture2D backgroundTexture;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var target = EditorUtility.InstanceIDToObject(instanceId) as Dialogue;
            if (!target) return false;

            ShowEditorWindow();
            return true;
        }

        private void OnEnable()
        {
            backgroundTexture = Resources.Load("background") as Texture2D;
            Selection.selectionChanged += OnSelectionChange;
            nodeStyle = new GUIStyle
            {
                normal =
                {
                    background = EditorGUIUtility.Load("node0") as Texture2D,
                    textColor = Color.white,
                },
                padding = new RectOffset(10, 10, 10, 10),
                border = new RectOffset(12, 12, 12, 12),
            };

            playerNodeStyle = new GUIStyle(nodeStyle)
            {
                normal =
                {
                    background = EditorGUIUtility.Load("node1") as Texture2D
                }
            };
            // {
            //     normal =
            //     {
            //         background = EditorGUIUtility.Load("node1") as Texture2D,
            //         textColor = Color.white,
            //     },
            //     padding = new RectOffset(10, 10, 10, 10),
            //     border = new RectOffset(12, 12, 12, 12),
            // };
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChange;
        }

        private void OnSelectionChange()
        {
            var newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (!selectedDialogue)
            {
                EditorGUILayout.LabelField("No dialog selected");
            }
            else
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);
                float backgroundTileRatio = canvasSize / backgroundSize;
                Rect texCoords = new Rect(0, 0, backgroundTileRatio, backgroundTileRatio);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, texCoords);

                foreach (var node in selectedDialogue.AllNodes)
                {
                    DrawConnections(node);
                }

                foreach (var node in selectedDialogue.AllNodes)
                {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();

                if (creatingNode)
                {
                    // Undo.RecordObject(selectedDialogue, "Created new DialogueNode");
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }

                if (deletingNode)
                {
                    // Undo.RecordObject(selectedDialogue, "Deleted DialogueNode");
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            Rect rect = node.Rect;
            Vector3 startPosition = new Vector3(rect.xMax, rect.center.y);

            foreach (var childNode in selectedDialogue.GetAllChildren(node))
            {
                rect = childNode.Rect;
                Vector3 endPosition = new Vector3(rect.xMin, rect.center.y);

                Vector3 tangentOffset = endPosition - startPosition;
                tangentOffset.y = 0;
                tangentOffset.x *= 0.8f;

                Vector3 startTangent = startPosition + tangentOffset;
                Vector3 endTangent = endPosition - tangentOffset;
                Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, Color.cyan, null, 4f);
            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && !draggedNode)
            {
                Vector2 clickPosition = Event.current.mousePosition + scrollPosition;
                draggedNode = GetNodeAtPoint(clickPosition);

                if (!draggedNode)
                {
                    draggingCanvas = true;
                    draggingCanvasOffset = clickPosition;
                    Selection.activeObject = selectedDialogue;
                    return;
                }

                Selection.activeObject = draggedNode;
                draggingOffset = draggedNode.Rect.position - Event.current.mousePosition;
            }
            else if (draggedNode && Event.current.type == EventType.MouseDrag)
            {
                draggedNode.SetPosition(Event.current.mousePosition + draggingOffset);
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggedNode)
            {
                draggedNode = null;
            }
            else if (draggingCanvas && Event.current.type == EventType.MouseDrag)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;
            foreach (var node in selectedDialogue.AllNodes)
            {
                if (node.Rect.Contains(point))
                {
                    foundNode = node;
                }
            }

            return foundNode;
        }

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.Rect, !node.IsPlayerSpeaking ? nodeStyle : playerNodeStyle);

            EditorGUILayout.LabelField("Text:");
            var newText = EditorGUILayout.TextField(node.Text);
            node.Text = newText;

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(" X Delete"))
            {
                deletingNode = node;
            }

            if (!linkingParentNode)
            {
                if (GUILayout.Button("link"))
                {
                    Undo.RecordObject(selectedDialogue, "Started assigning new connection in Dialogue");
                    linkingParentNode = node;
                }
            }
            else
            {
                DrawLinkButtons(node);
            }

            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button(node.IsPlayerSpeaking
                ? "Is Player Speaking"
                : "Is NPC Speaking"))
            {
                node.IsPlayerSpeaking = !node.IsPlayerSpeaking;
            }
            
            EditorGUILayout.LabelField("Trigger Enter Action:");
            newText = EditorGUILayout.TextField(node.TriggerEnterAction);
            node.TriggerEnterAction = newText;
            
            EditorGUILayout.LabelField("Trigger Exit Action:");
            newText = EditorGUILayout.TextField(node.TriggerExitAction);
            node.TriggerExitAction = newText;

            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode.name != node.name
                && !linkingParentNode.Children.Contains(node.name)
                && GUILayout.Button("Child"))
            {
                linkingParentNode.AddChild(node.name);
                linkingParentNode = null;
            }
            else if (linkingParentNode.name != node.name
                     && linkingParentNode.Children.Contains(node.name)
                     && GUILayout.Button("Unlink"))
            {
                linkingParentNode.RemoveChild(node.name);
                linkingParentNode = null;
            }
            else if (linkingParentNode.name == node.name
                     && GUILayout.Button("Cancel"))
            {
                Undo.RecordObject(selectedDialogue, "Canceling creation of new link in Dialogue");
                linkingParentNode = null;
            }
        }
    }
}