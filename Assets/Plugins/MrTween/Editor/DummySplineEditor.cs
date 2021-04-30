#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


// TODO: really should add settings for free, mirrored and aligned handles


namespace Mara.MrTween {
    [CustomEditor(typeof(DummySpline))]
    public class DummySplineEditor : Editor {
        private DummySpline _target;
        private GUIStyle _labelStyle;
        private GUIStyle _indexStyle;

        private float _snapDistance = 5f;
        private bool _showNodeDetails;
        private int _selectedNodeIndex = -1;

        private bool _saveAsScriptableObject = true;
        private bool _loadFromScriptableObject = true;

        #region Monobehaviour and Editor

        void OnEnable() {
            // setup the font for the 'begin' 'end' text
            _labelStyle = new GUIStyle();
            _labelStyle.fontStyle = FontStyle.Bold;
            _labelStyle.normal.textColor = Color.white;
            _labelStyle.fontSize = 16;

            _indexStyle = new GUIStyle();
            _indexStyle.fontStyle = FontStyle.Bold;
            _indexStyle.normal.textColor = Color.white;
            _indexStyle.fontSize = 12;

            _target = (DummySpline)target;
        }

        void OnDisable() {
            _target = null;
            _labelStyle = null;
            _indexStyle = null;
        }

        public override void OnInspectorGUI() {
            showEditModeToggle();

            if (!_target.IsInEditMode) {
                DrawInstructions();
                return;
            }

            if (_target.UseBezier && _target.Nodes.Count < 4)
                _target.UseBezier = false;

            // what kind of handles shall we use?
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Use Standard Handles");
            _target.UseStandardHandles = EditorGUILayout.Toggle(_target.UseStandardHandles);
            EditorGUILayout.EndHorizontal();

            // path name:
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Route Name");
            _target.PathName = EditorGUILayout.TextField(_target.PathName);
            EditorGUILayout.EndHorizontal();

            if (_target.PathName == string.Empty)
                _target.PathName = "route" + Random.Range(1, 100000);

            // path color:
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Route Color");
            _target.PathColor = EditorGUILayout.ColorField(_target.PathColor);
            EditorGUILayout.EndHorizontal();

            // force bezier:
            GUI.enabled = !_target.ForceStraightLinePath;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Use Bezier Path");
            EditorGUI.BeginChangeCheck();
            _target.UseBezier = EditorGUILayout.Toggle(_target.UseBezier);
            if (EditorGUI.EndChangeCheck() && _target.UseBezier) {
                // validate
                // make sure we have enough nodes to use a bezier.  nodeCount - 1 must be divisible by 3
                if (_target.Nodes.Count < 4) {
                    Debug.LogError("there must be at least 4 nodes to use a bezier");
                    _target.UseBezier = false;
                } else {
                    var excessNodes = (_target.Nodes.Count - 1) % 3;
                    if (excessNodes > 0) {
                        _target.Nodes.RemoveRange(_target.Nodes.Count - excessNodes, excessNodes);
                        Debug.LogWarning("trimming " + excessNodes + " from the node list to make a proper bezier spline");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

            // force straight lines:
            GUI.enabled = !_target.UseBezier;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Force Straight Line Path");
            _target.ForceStraightLinePath = EditorGUILayout.Toggle(_target.ForceStraightLinePath);
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

            // close path. only relevant for node counts greater than 5
            if (_target.Nodes.Count < 5)
                GUI.enabled = false;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Close Path");
            EditorGUI.BeginChangeCheck();
            _target.ClosePath = EditorGUILayout.Toggle(_target.ClosePath);
            if (EditorGUI.EndChangeCheck()) {
                if (_target.ClosePath)
                    closeRoute();
            }
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

            // resolution
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Editor Drawing Resolution");
            _target.PathResolution = EditorGUILayout.IntSlider(_target.PathResolution, 2, 100);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            // insert node - we need 3 or more nodes for insert to make sense
            if (_target.Nodes.Count > 2) {
                EditorGUILayout.BeginHorizontal();

                GUI.enabled = _selectedNodeIndex != 0 && _selectedNodeIndex != -1;
                if (GUILayout.Button("Insert Node Before Selected"))
                    insertNodeAtIndex(_selectedNodeIndex, false);

                GUI.enabled = _selectedNodeIndex != _target.Nodes.Count - 1 && _selectedNodeIndex != -1;
                if (GUILayout.Button("Insert Node After Selected"))
                    insertNodeAtIndex(_selectedNodeIndex, true);

                GUI.enabled = true;

                EditorGUILayout.EndHorizontal();
            }

            // shift the start point to the origin
            if (GUILayout.Button("Shift Path to Start at Origin")) {
                Undo.RecordObject(_target, "Path Vector Changed");

                var offset = Vector3.zero;

                // see what kind of path we are. the simplest case is just a straight line
                var path = new Spline(_target.Nodes, _target.UseBezier, _target.ForceStraightLinePath);
                if (path.SplineType == SplineType.StraightLine || path.SplineType == SplineType.Bezier || _target.Nodes.Count < 5)
                    offset = Vector3.zero - _target.Nodes[0];
                else
                    offset = Vector3.zero - _target.Nodes[1];

                for (var i = 0; i < _target.Nodes.Count; i++)
                    _target.Nodes[i] += offset;

                GUI.changed = true;
            }

            // reverse
            if (GUILayout.Button("Reverse Path")) {
                Undo.RecordObject(_target, "Path Vector Changed");
                _target.Nodes.Reverse();
                GUI.changed = true;
            }

            // shifters. thse only make sense for straight line and catmull rom
            if (_target.ForceStraightLinePath || (_target.Nodes.Count > 4 && !_target.UseBezier)) {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Shift Nodes Left")) {
                    Undo.RecordObject(_target, "Path Vector Changed");

                    var firstItem = _target.Nodes[0];
                    _target.Nodes.RemoveAt(0);
                    _target.Nodes.Add(firstItem);

                    GUI.changed = true;
                }

                if (GUILayout.Button("Shift Nodes Right")) {
                    Undo.RecordObject(_target, "Path Vector Changed");

                    var lastItem = _target.Nodes[_target.Nodes.Count - 1];
                    _target.Nodes.RemoveAt(_target.Nodes.Count - 1);
                    _target.Nodes.Insert(0, lastItem);

                    GUI.changed = true;
                }

                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Clear Path")) {
                Undo.RecordObject(_target, "Path Vector Changed");
                _target.Nodes.Clear();
                _target.Nodes.Add(_target.transform.position);
                _target.Nodes.Add(_target.transform.position + new Vector3(5f, 5f));

                GUI.changed = true;
            }

            if (GUILayout.Button("Move z-axis Values to 0")) {
                Undo.RecordObject(_target, "Path Vector Changed");

                for (var i = 0; i < _target.Nodes.Count; i++)
                    _target.Nodes[i] = new Vector3(_target.Nodes[i].x, _target.Nodes[i].y, 0f);

                GUI.changed = true;
            }

            // persist to disk
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Save to/Read from Disk");

            // Should we save it as a ScriptableObject instead?
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Save As Scriptable Object");
            _saveAsScriptableObject = EditorGUILayout.Toggle(_saveAsScriptableObject);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Serialize and Save Path");
            if (GUILayout.Button("Save")) {
                if (_saveAsScriptableObject) {
                    if (_saveAsScriptableObject) {
                        var folderPath = EditorUtility.OpenFolderPanel("Save Folder", Application.dataPath, "");
                        if (folderPath != string.Empty) {
                            saveAsScriptableObject(folderPath.Replace(Application.dataPath, "Assets") + "/" + _target.PathName + ".asset");
                        }
                    }
                } else {
                    var path = EditorUtility.SaveFilePanel("Save path", Application.dataPath + "/StreamingAssets", _target.PathName + ".asset", "asset");
                    if (path != string.Empty) {
                        persistRouteToDisk(path);

                        // fetch the filename and set it as the routeName
                        _target.PathName = Path.GetFileName(path).Replace(".asset", string.Empty);
                    }
                }

                GUI.changed = true;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Load From Scriptable Object");
            _loadFromScriptableObject = EditorGUILayout.Toggle(_loadFromScriptableObject);
            EditorGUILayout.EndHorizontal();

            // load from disk
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Load saved path");
            if (GUILayout.Button("Load")) {
                if (_loadFromScriptableObject) {
                    var path = EditorUtility.OpenFilePanel("Choose path to load", Path.Combine(Application.dataPath, "Asset"), "asset");
                    if (path != string.Empty) {
                        MrTweenSplineSettings asset = AssetDatabase.LoadAssetAtPath(path.Replace(Application.dataPath, "Assets"), typeof(MrTweenSplineSettings)) as MrTweenSplineSettings;
                        if (asset != null) {
                            _target.Nodes = asset.Nodes;
                            _target.PathName = Path.GetFileName(path).Replace(".asset", string.Empty);
                        } else {
                            EditorUtility.DisplayDialog("Incorrect file selected", "File selected does not contain a valid path",
                                "Close");
                        }
                    }
                } else {
                    var path = EditorUtility.OpenFilePanel("Choose path to load",
                        Path.Combine(Application.dataPath, "StreamingAssets"), "asset");
                    if (path != string.Empty) {
                        if (!File.Exists(path)) {
                            EditorUtility.DisplayDialog("File does not exist", "Path couldn't find the file you specified",
                                "Close");
                        } else {
                            _target.Nodes = SplineAssetUtils.BytesToVector3List(File.ReadAllBytes(path));
                            _target.PathName = Path.GetFileName(path).Replace(".asset", string.Empty);
                        }
                    }
                    GUI.changed = true;
                }
            }
            EditorGUILayout.EndHorizontal();

            // node display
            EditorGUILayout.Space();
            _showNodeDetails = EditorGUILayout.Foldout(_showNodeDetails, "Show Node Values");
            if (_showNodeDetails) {
                EditorGUI.indentLevel++;
                for (int i = 0; i < _target.Nodes.Count; i++)
                    _target.Nodes[i] = EditorGUILayout.Vector3Field("Node " + (i + 1), _target.Nodes[i]);
                EditorGUI.indentLevel--;
            }

            DrawInstructions();

            // update and redraw:
            if (GUI.changed) {
                EditorUtility.SetDirty(_target);
                Repaint();
            }
        }


        void DrawInstructions() {
            // instructions
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Press the 'Enter Edit Mode' button to lock the scene view and begin editing\n\n" +
                "While dragging a node, hold down Ctrl and slowly move the cursor to snap to a nearby point\n\n" +
                "Click the 'Close Path' button to add a new node that will close out the current path.\n\n" +
                "Hold Command while dragging a node to snap in 5 point increments\n\n" +
                "Double click to add a new node at the end of the path\n\n" +
                "Hold down alt while double clicking to prepend the new node at the front of the route\n\n" +
                "Press delete or backspace to delete the selected node\n\n" +
                "When preparing relative tweens, click the 'Shift Path to Start at Origin' button. This will let you're spline" +
                "tween start at the exact position of the object being tweened.\n\n" +
                "NOTE: make sure you have the pan tool selected while editing paths! You can hold alt and click-drag" +
                "to pan the view", MessageType.None);
        }


        void OnSceneGUI() {
            if (!_target.gameObject.activeSelf)
                return;

            if (_target.IsInEditMode) {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            } else {
                drawRoute();
                return;
            }

            // handle current selection and node addition via double click or ctrl click
            if (Event.current.type == EventType.MouseDown) {
                var nearestIndex = getNearestNodeForMousePosition(Event.current.mousePosition);
                _selectedNodeIndex = nearestIndex;
                Repaint();

                // double click to add
                if (Event.current.clickCount > 1) {
                    var translatedPoint = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition)
                            .GetPoint((_target.transform.position - Camera.current.transform.position).magnitude);

                    Undo.RecordObject(_target, "Path Node Added");

                    // if alt is down then prepend the node to the beginning
                    if (Event.current.alt)
                        prependNode(translatedPoint);
                    else
                        appendNode(translatedPoint);
                }
            }


            if (_selectedNodeIndex >= 0) {
                // shall we delete the selected node?
                if (Event.current.keyCode == KeyCode.Delete || Event.current.keyCode == KeyCode.Backspace) {
                    if (_target.Nodes.Count > 2) {
                        Undo.RecordObject(_target, "Path Node Deleted");
                        Event.current.Use();
                        removeNodeAtIndex(_selectedNodeIndex);
                        _selectedNodeIndex = -1;
                        Repaint();
                        Event.current.Use();
                    }
                }
            }

            var isBezierControlPoint = _selectedNodeIndex % 3 != 0;
            if (_target.Nodes.Count > 1) {
                // allow path adjustment undo
                Undo.RecordObject(_target, "Path Vector Changed");

                // path begin and end labels or just one if the path is closed
                if (Vector3.Distance(_target.Nodes[0], _target.Nodes[_target.Nodes.Count - 1]) == 0) {
                    Handles.Label(_target.Nodes[0], "  Begin and End", _labelStyle);
                } else {
                    Handles.Label(_target.Nodes[0], "  Begin", _labelStyle);
                    Handles.Label(_target.Nodes[_target.Nodes.Count - 1], "  End", _labelStyle);
                }

                // draw the handles, arrows and lines
                drawRoute();


                //				var distanceToTarget = Vector3.Distance( SceneView.lastActiveSceneView.camera.transform.position, _target.transform.position );
                //				distanceToTarget = Mathf.Abs( distanceToTarget );
                //				var handleSize = Mathf.Ceil( distanceToTarget / 75 );

                // how big shall we draw the handles?
                var handleSize = HandleUtility.GetHandleSize(_target.transform.position) * 0.2f;

                for (var i = 0; i < _target.Nodes.Count; i++) {
                    Handles.color = _target.PathColor;

                    // dont label the first and last nodes or ctrl handles on a bezier
                    if (i > 0 && i < _target.Nodes.Count - 1)
                        if (!(_target.IsMultiPointBezierSpline && i % 3 != 0))
                            Handles.Label(_target.Nodes[i] + new Vector3(1f, 0.0f), i.ToString(), _indexStyle);

                    Handles.color = Color.white;
                    if (_target.UseStandardHandles) {
                        EditorGUI.BeginChangeCheck();
                        var newNodePosition = Handles.PositionHandle(_target.Nodes[i], Quaternion.identity);
                        if (EditorGUI.EndChangeCheck())
                            handleNodeMove(i, newNodePosition);
                    } else {
                        // dont snap bezier handles
                        var snapper = isBezierControlPoint && _target.IsMultiPointBezierSpline ? Vector3.zero : new Vector3(5f, 5f, 5f);
                        EditorGUI.BeginChangeCheck();

                        var newNodePosition = Handles.FreeMoveHandle(_target.Nodes[i],
                                                Quaternion.identity,
                                                handleSize,
                                                snapper,
                                                Handles.SphereHandleCap);

                        if (EditorGUI.EndChangeCheck())
                            handleNodeMove(i, newNodePosition);
                    }


                    // should we snap?  we need at least 4 nodes because we dont snap to the previous and next nodes
                    if (Event.current.control && _target.Nodes.Count > 3 && !(isBezierControlPoint && _target.IsMultiPointBezierSpline)) {
                        // dont even bother checking for snapping to the previous/next nodes and we can disregard all ctrl points for beziers
                        var excludedNodes = new List<int>();
                        excludedNodes.Add(_selectedNodeIndex);

                        if (_target.IsMultiPointBezierSpline) {
                            for (var nodeIndex = 0; nodeIndex < _target.Nodes.Count; nodeIndex++) {
                                if (nodeIndex != _selectedNodeIndex && nodeIndex % 3 != 0 || Mathf.Abs(_selectedNodeIndex - nodeIndex) < 5)
                                    excludedNodes.Add(nodeIndex);
                            }
                        } else {
                            excludedNodes.Add(_selectedNodeIndex - 1);
                            excludedNodes.Add(_selectedNodeIndex + 1);
                        }

                        var nearestIndex = getNearestNode(_target.Nodes[_selectedNodeIndex], excludedNodes.ToArray());
                        var nearest = _target.Nodes[nearestIndex];
                        var distanceToNearestNode = Vector3.Distance(nearest, _target.Nodes[_selectedNodeIndex]);

                        // is it close enough to snap?
                        if (distanceToNearestNode <= _snapDistance * 0.5f) {
                            GUI.changed = true;
                            handleNodeMove(_selectedNodeIndex, nearest);
                        } else if (distanceToNearestNode <= _snapDistance * 25f && !excludedNodes.Contains(i)) {
                            // show which nodes are getting close enough to snap to
                            var color = Color.red;
                            color.a = 0.3f;
                            Handles.color = color;
                            Handles.SphereHandleCap(0, _target.Nodes[i], Quaternion.identity, 1, EventType.Repaint);
                            Handles.color = Color.white;
                        }
                    }
                } // end for


                if (GUI.changed) {
                    Repaint();
                    EditorUtility.SetDirty(_target);
                }
            } // end if
        }


        void showEditModeToggle() {
            var originalColor = GUI.color;
            var text = _target.IsInEditMode ? "Exit Edit Mode" : "Enter Edit Mode";
            GUI.color = _target.IsInEditMode ? Color.green : GUI.color;

            if (GUILayout.Button(text))
                _target.IsInEditMode = !_target.IsInEditMode;

            SceneView.RepaintAll();

            GUI.color = originalColor;
        }

        #endregion


        #region Private methods

        private void handleNodeMove(int index, Vector3 pos) {
            // non-beziers are just a simple set
            if (!_target.IsMultiPointBezierSpline) {
                _target.Nodes[index] = pos;

                // handle closed paths. we only care about straight line and catmull rom here. bezier is handled below
                if (_target.ClosePath) {
                    if (_target.ForceStraightLinePath) {
                        if (index == 0)
                            _target.Nodes[_target.Nodes.Count - 1] = pos;
                        else if (index == _target.Nodes.Count - 1)
                            _target.Nodes[0] = pos;
                    } else // catmull rom keeps the second and second from last nodes in check
                      {
                        if (index == 1)
                            _target.Nodes[_target.Nodes.Count - 2] = pos;
                        else if (index == _target.Nodes.Count - 2)
                            _target.Nodes[1] = pos;
                    }
                }
                return;
            }

            // beziers. let the fun begin. ctrl points work differently than nodes
            var deltaMove = pos - _target.Nodes[index];
            if (index % 3 == 0) {
                _target.Nodes[index] = pos;

                // special cases are start and end nodes. we need to move their single ctrl point
                if (index > 0)
                    _target.Nodes[index - 1] += deltaMove;

                if (index < _target.Nodes.Count - 2)
                    _target.Nodes[index + 1] += deltaMove;

                // handle closed paths. we need to keep our first/last points in sync
                if (_target.ClosePath && index == 0) {
                    _target.Nodes[_target.Nodes.Count - 2] += deltaMove;
                    _target.Nodes[_target.Nodes.Count - 1] = pos;
                } else if (_target.ClosePath && index == _target.Nodes.Count - 1) {
                    _target.Nodes[0] = pos;
                    _target.Nodes[1] += deltaMove;
                }
            } else {
                // special case for first and last ctrl points. they be free unless we are a closed path
                if (!_target.ClosePath && (index == 1 || index == _target.Nodes.Count - 2)) {
                    _target.Nodes[index] = pos;
                } else {
                    _target.Nodes[index] = pos;
                    var modeIndex = (index + 1) / 3;

                    var middleIndex = modeIndex * 3;
                    int fixedIndex, enforcedIndex;
                    if (index <= middleIndex) {
                        fixedIndex = middleIndex - 1;
                        enforcedIndex = middleIndex + 1;
                    } else {
                        fixedIndex = middleIndex + 1;
                        enforcedIndex = middleIndex - 1;
                    }

                    // wrap enforcedIndex in case we are messing with the first/last ctrl point of a closed path
                    enforcedIndex = (int)Mathf.Repeat(enforcedIndex, _target.Nodes.Count - 1);

                    var middle = _target.Nodes[middleIndex];
                    var enforcedTangent = middle - _target.Nodes[fixedIndex];
                    _target.Nodes[enforcedIndex] = middle + enforcedTangent;
                }
            }
        }


        private void appendNode(Vector3 node) {
            if (_target.IsMultiPointBezierSpline) {
                var lastNodeCtrlPoint = _target.Nodes[_target.Nodes.Count - 2];
                var lastNode = _target.Nodes[_target.Nodes.Count - 1];

                _target.Nodes.Add(lastNode - (lastNodeCtrlPoint - lastNode)); // 2nd control point for the last node
                _target.Nodes.Add(node + (lastNodeCtrlPoint - lastNode)); // control point for new node
                _target.Nodes.Add(node); // new node
            } else {
                _target.Nodes.Add(node);
            }

            GUI.changed = true;
        }


        private void prependNode(Vector3 node) {
            if (_target.IsMultiPointBezierSpline) {
                var firstNodeCtrlPoint = _target.Nodes[1];
                var firstNode = _target.Nodes[0];

                _target.Nodes.Insert(0, firstNode - (firstNodeCtrlPoint - firstNode)); // 2nd control point for the first node
                _target.Nodes.Insert(0, node + (firstNodeCtrlPoint - firstNode)); // control point for new node
                _target.Nodes.Insert(0, node); // new node
            } else {
                _target.Nodes.Insert(0, node);
            }
        }


        private void removeNodeAtIndex(int index) {
            if (index >= _target.Nodes.Count || index < 0)
                return;

            if (_target.IsMultiPointBezierSpline) {
                if (index % 3 == 0) {
                    // we want to remove the node before through the node after index but we need to be careful for index = 0 && count - 1
                    if (index == 0)
                        _target.Nodes.RemoveRange(index, 3);
                    else if (index == _target.Nodes.Count - 1)
                        _target.Nodes.RemoveRange(index - 2, 3);
                    else
                        _target.Nodes.RemoveRange(index - 1, 3);
                } else {
                    Debug.LogError("Sorry. You cannot remove control points of a bezier curve");
                }
            } else {
                _target.Nodes.RemoveAt(index);
            }

            if (_target.Nodes.Count < 2) {
                _target.Nodes.Clear();
                _target.Nodes.Add(_target.transform.position);
                _target.Nodes.Add(_target.transform.position + new Vector3(5f, 5f));
            }

            GUI.changed = true;
        }


        private void insertNodeAtIndex(int index, bool isAfter) {
            Undo.RecordObject(_target, "Insert Node");

            if (_target.IsMultiPointBezierSpline) {
                if (index % 3 != 0) {
                    Debug.LogError("you cant insert a node before or after a bezier control point");
                    return;
                }

                var firstCtrlPointIndex = isAfter ? index : index - 3;
                var secondCtrlPointIndex = isAfter ? index + 3 : index;
                var insertIndex = isAfter ? index + 2 : index - 1;

                var ctrlPointOffsetIndex = isAfter ? index + 1 : index - 1;
                var ctrlPointOffset = _target.Nodes[ctrlPointOffsetIndex] - _target.Nodes[index];

                var nodeLocation = Vector3.Lerp(_target.Nodes[firstCtrlPointIndex], _target.Nodes[secondCtrlPointIndex], 0.5f);

                _target.Nodes.Insert(insertIndex, nodeLocation - ctrlPointOffset); // 1st control point for new node
                _target.Nodes.Insert(insertIndex, nodeLocation); // new node
                _target.Nodes.Insert(insertIndex, nodeLocation + ctrlPointOffset); // 2nd control point for new node
            } else {
                var firstIndex = isAfter ? index : index - 1;
                var secondIndex = isAfter ? index + 1 : index;
                var insertIndex = isAfter ? index + 1 : index;
                insertNodeAtIndex(Vector3.Lerp(_target.Nodes[firstIndex], _target.Nodes[secondIndex], 0.5f), insertIndex);
            }
        }


        // this is only called from insertNodeBetweenIndices so we dont do anything special for beziers
        private void insertNodeAtIndex(Vector3 node, int index) {
            // validate the index
            if (index >= 0 && index < _target.Nodes.Count) {
                _target.Nodes.Insert(index, node);
                GUI.changed = true;
            }
        }


        private void drawArrowBetweenPoints(Vector3 point1, Vector3 point2) {
            // no need to draw arrows for tiny segments
            var distance = Vector3.Distance(point1, point2);
            if (distance < 10)
                return;

            // we dont want to be exactly in the middle so we offset the length of the arrow
            var lerpModifier = (distance * 0.5f - 25) / distance;

            Handles.color = _target.PathColor;

            // get the midpoint between the 2 points
            var dir = Vector3.Lerp(point1, point2, lerpModifier);
            var quat = Quaternion.LookRotation(point2 - point1);
            Handles.ArrowHandleCap(0, dir, quat, 5, EventType.Repaint);
            //Handles.ArrowCap( 0, dir, quat, 5 );

            Handles.color = Color.white;
        }


        private int getNearestNode(Vector3 pos, params int[] excludeNodes) {
            var excludeNodesList = new System.Collections.Generic.List<int>(excludeNodes);
            var bestDistance = float.MaxValue;
            var index = -1;

            var distance = float.MaxValue;
            for (var i = _target.Nodes.Count - 1; i >= 0; i--) {
                if (excludeNodesList.Contains(i))
                    continue;

                distance = Vector3.Distance(pos, _target.Nodes[i]);
                if (distance < bestDistance) {
                    bestDistance = distance;
                    index = i;
                }
            }
            return index;
        }


        private int getNearestNodeForMousePosition(Vector3 mousePos) {
            var bestDistance = float.MaxValue;
            var index = -1;

            var distance = float.MaxValue;
            for (var i = _target.Nodes.Count - 1; i >= 0; i--) {
                var nodeToGui = HandleUtility.WorldToGUIPoint(_target.Nodes[i]);
                distance = Vector2.Distance(nodeToGui, mousePos);

                if (distance < bestDistance) {
                    bestDistance = distance;
                    index = i;
                }
            }

            // make sure we are close enough to a node
            if (bestDistance < 10f)
                return index;
            return -1;
        }


        private void closeRoute() {
            // we will use the GoSpline class to handle the dirtywork of closing the path
            var path = new Spline(_target.Nodes, _target.UseBezier, _target.ForceStraightLinePath);
            path.ClosePath();

            _target.Nodes = path.Nodes;

            GUI.changed = true;
        }

        private void saveAsScriptableObject(string path) {
            MrTweenSplineSettings.CreateAsset(path, _target.Nodes);
        }

        private void persistRouteToDisk(string path) {
            var bytes = new List<byte>();

            for (int k = 0; k < _target.Nodes.Count; ++k) {
                Vector3 vec = _target.Nodes[k];
                bytes.AddRange(System.BitConverter.GetBytes(vec.x));
                bytes.AddRange(System.BitConverter.GetBytes(vec.y));
                bytes.AddRange(System.BitConverter.GetBytes(vec.z));
            }

            File.WriteAllBytes(path, bytes.ToArray());
        }


        private void drawRoute() {
            // if we are forcing straight lines just use this setup
            if (_target.ForceStraightLinePath) {
                // draw just the route here and optional arrows
                for (var i = 0; i < _target.Nodes.Count; i++) {
                    Handles.color = _target.PathColor;
                    if (i < _target.Nodes.Count - 1) {
                        Handles.DrawLine(_target.Nodes[i], _target.Nodes[i + 1]);
                        drawArrowBetweenPoints(_target.Nodes[i], _target.Nodes[i + 1]);
                    }
                }
            }
        }

        #endregion

    }
}
#endif