using UnityEditor;
using UnityEngine;

namespace DUCK.PathFinding.Editor
{
	[CustomEditor(typeof(Waypoint))]
	[CanEditMultipleObjects]
	public class WaypointEditor : UnityEditor.Editor
	{
		private static readonly GUILayoutOption buttonWidth = GUILayout.MinWidth(80);

		private Waypoint waypoint;
		private bool awaitingSelection;
		// have to be static -- or will lost on runtime
		private static WaypointPathTracker lastPathTracker;

		private void OnEnable()
		{
			// Hard cast since we know it's 100% a Waypoint (unless Unity fails!)
			waypoint = (Waypoint)target;
			waypoint.ValidateConnectedWaypoints();
			EditorApplication.update += ForceUpdateGizmosAnimation;
		}

		private void OnDisable()
		{
			waypoint.ValidateConnectedWaypoints();
			EditorApplication.update -= ForceUpdateGizmosAnimation;
		}

		private void OnDestroy()
		{
			waypoint.ValidateOthersAfterDestroyed();
			EditorApplication.update -= ForceUpdateGizmosAnimation;
		}

		/// <summary>
		/// This function force the editor refresh all its editor functions.
		/// We use this to force update the Gizmos animation...
		/// Nice and smooth! 60fps FTW!
		/// </summary>
		private void ForceUpdateGizmosAnimation()
		{
			if (target != null)
			{
				EditorUtility.SetDirty(target);
			}
			else
			{
				EditorApplication.update -= ForceUpdateGizmosAnimation;
			}
		}

		private Waypoint CreateNewWaypoint()
		{
			var obj = new GameObject("Waypoint");
			obj.transform.SetParent(waypoint.transform.parent);
			obj.transform.position = waypoint.transform.position + Vector3.forward;
			return obj.AddComponent<Waypoint>();
		}

		private void OnOneWayConnectToSelected()
		{
			Selection.selectionChanged -= OnOneWayConnectToSelected;

			if (Selection.activeGameObject == null) return;

			var targetWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
			if (targetWaypoint == null || waypoint == null) return;

			waypoint.ConnectWaypoint(targetWaypoint);
		}

		private void OnDoubleWayConnectToSelected()
		{
			var targetWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
			if (targetWaypoint != null && waypoint != null)
			{
				waypoint.ConnectWaypoint(targetWaypoint);
				targetWaypoint.ConnectWaypoint(waypoint);
			}
			Selection.selectionChanged -= OnDoubleWayConnectToSelected;
		}

		private void OnOneWayDisconnectToSelected()
		{
			var targetWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
			if (targetWaypoint != null && waypoint != null)
			{
				waypoint.DisconnectWaypoint(targetWaypoint);
			}
			Selection.selectionChanged -= OnOneWayDisconnectToSelected;
		}

		private void OnDoubleWayDisconnectToSelected()
		{
			var targetWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
			if (targetWaypoint != null && waypoint != null)
			{
				waypoint.DisconnectWaypoint(targetWaypoint);
				targetWaypoint.DisconnectWaypoint(waypoint);
			}
			Selection.selectionChanged -= OnDoubleWayDisconnectToSelected;
		}

		public override void OnInspectorGUI()
		{
			// Awaiting the user click on another object
			if (awaitingSelection)
			{
				EditorGUILayout.HelpBox("Please select the target Waypoint...", MessageType.Info);
			}
			else
			{
				switch (Selection.gameObjects.Length)
				{
					case 1:
						DrawDefaultInspector();

						EditorGUILayout.Space();
						EditorGUILayout.PrefixLabel("One-Way");

						EditorGUILayout.BeginHorizontal();

						// Create a new one way only
						if (GUILayout.Button("Add", buttonWidth))
						{
							var newWaypoint = CreateNewWaypoint();
							waypoint.ConnectWaypoint(newWaypoint);
							Selection.activeGameObject = newWaypoint.gameObject;
						}
						// Connect to a waypoint as one way only
						if (GUILayout.Button("Connect...", buttonWidth))
						{
							Selection.selectionChanged += OnOneWayConnectToSelected;
							awaitingSelection = true;
						}
						// Disconnect to a waypoint for one way only
						if (GUILayout.Button("Disconnect...", buttonWidth))
						{
							Selection.selectionChanged += OnOneWayDisconnectToSelected;
							awaitingSelection = true;
						}

						EditorGUILayout.EndHorizontal();

						EditorGUILayout.Space();
						EditorGUILayout.PrefixLabel("Double-Way");

						EditorGUILayout.BeginHorizontal();

						// Create a new double way waypoint
						if (GUILayout.Button("Add", buttonWidth))
						{
							var newWaypoint = CreateNewWaypoint();
							waypoint.ConnectWaypoint(newWaypoint);
							newWaypoint.ConnectWaypoint(waypoint);
							Selection.activeGameObject = newWaypoint.gameObject;
						}
						// Connect to a waypoint as double way
						if (GUILayout.Button("Connect...", buttonWidth))
						{
							Selection.selectionChanged += OnDoubleWayConnectToSelected;
							awaitingSelection = true;
						}
						// Disconnect to a waypoint for double way
						if (GUILayout.Button("Disconnect...", buttonWidth))
						{
							Selection.selectionChanged += OnDoubleWayDisconnectToSelected;
							awaitingSelection = true;
						}

						EditorGUILayout.EndHorizontal();

						break;

					// Connect or disconnect two waypoints if the user selected 2 of them
					case 2:
						var waypointA = Selection.gameObjects[0].GetComponent<Waypoint>();
						var waypointB = Selection.gameObjects[1].GetComponent<Waypoint>();

						if (waypointA == null || waypointB == null) break;

						EditorGUILayout.BeginHorizontal();

						if (!waypointA.IsConnectedWith(waypointB) && !waypointB.IsConnectedWith(waypointA))
						{
							if (GUILayout.Button("Connect 1-Way", buttonWidth))
							{
								waypointA.ConnectWaypoint(waypointB);
							}
							else if (GUILayout.Button("Connect 2-Way", buttonWidth))
							{
								waypointA.ConnectWaypoint(waypointB);
								waypointB.ConnectWaypoint(waypointA);
							}
						}

						if (GUILayout.Button("Insert", buttonWidth))
						{
							var newWaypoint = CreateNewWaypoint();
							if (waypointA.IsConnectedWith(waypointB))
							{
								waypointA.ConnectWaypoint(newWaypoint);
								newWaypoint.ConnectWaypoint(waypointB);
								waypointA.DisconnectWaypoint(waypointB);
							}
							if (waypointB.IsConnectedWith(waypointA))
							{
								waypointB.ConnectWaypoint(newWaypoint);
								newWaypoint.ConnectWaypoint(waypointA);
								waypointB.DisconnectWaypoint(waypointA);
							}
							newWaypoint.transform.position = waypointA.transform.position +
								(waypointB.transform.position - waypointA.transform.position) / 2.0f;
							Selection.activeGameObject = newWaypoint.gameObject;
						}

						if (waypointA.IsConnectedWith(waypointB) || waypointB.IsConnectedWith(waypointA))
						{
							if (GUILayout.Button("Disconnect", buttonWidth))
							{
								waypointA.DisconnectWaypoint(waypointB);
								waypointB.DisconnectWaypoint(waypointA);
							}

							if (waypointA.IsConnectedWith(waypointB) && waypointB.IsConnectedWith(waypointA))
							{
								if (GUILayout.Button("To 1-Way", buttonWidth))
								{
									waypointA.ConnectWaypoint(waypointB);
									waypointB.DisconnectWaypoint(waypointA);
								}
							}
							else if (GUILayout.Button("To 2-Way", buttonWidth))
							{
								waypointA.ConnectWaypoint(waypointB);
								waypointB.ConnectWaypoint(waypointA);
							}

							if (waypointA.IsConnectedWith(waypointB) && !waypointB.IsConnectedWith(waypointA)
							    && GUILayout.Button("Inverse", buttonWidth))
							{
								waypointA.DisconnectWaypoint(waypointB);
								waypointB.ConnectWaypoint(waypointA);
							}
							else if (waypointB.IsConnectedWith(waypointA) && !waypointA.IsConnectedWith(waypointB)
							         && GUILayout.Button("Inverse", buttonWidth))
							{
								waypointA.ConnectWaypoint(waypointB);
								waypointB.DisconnectWaypoint(waypointA);
							}
						}

						EditorGUILayout.EndHorizontal();

						if (EditorApplication.isPlaying && lastPathTracker == null)
						{
							EditorGUILayout.Space();
							EditorGUILayout.PrefixLabel("Path");

							if (GUILayout.Button("Find The Path!", buttonWidth))
							{
								var waypointSystem = waypointA.GetComponentInParent<PathFinding.WaypointSystem>();
								lastPathTracker = waypointSystem.GetPathTracker(waypointA, waypointB);
								if (lastPathTracker != null)
								{
									lastPathTracker.SetGizmosDraw(true);
								}
								else
								{
									lastPathTracker = waypointSystem.GetPathTracker(waypointB, waypointA);
									if (lastPathTracker != null)
									{
										lastPathTracker.SetGizmosDraw(true);
									}
								}
							}
						}

						break;

					default:
						if (GUILayout.Button("Disconnect All", buttonWidth))
						{
							foreach (var obj in Selection.gameObjects)
							{
								var wp = obj.GetComponent<Waypoint>();
								wp.DisconnectWaypoint();
							}
						}
						break;
				}

				if (EditorApplication.isPlaying && lastPathTracker != null)
				{
					EditorGUILayout.Space();
					EditorGUILayout.PrefixLabel("Path");

					var progress = lastPathTracker.TravelledDistance / lastPathTracker.TotalDistance;
					EditorGUI.ProgressBar(GUILayoutUtility.GetRect(18, 18), progress,
						string.Format("Progress: {0:0.##}/{1:0.##} [{2:0.##}%]",
							lastPathTracker.TravelledDistance, lastPathTracker.TotalDistance, progress * 100.0f));

					EditorGUILayout.BeginHorizontal();

					if (GUILayout.Button("Clear", buttonWidth))
					{
						lastPathTracker.SetGizmosDraw(false);
						lastPathTracker = null;
					}
					else if (GUILayout.Button("Reverse", buttonWidth))
					{
						lastPathTracker.SetGizmosDraw(false);

						var waypointSystem = lastPathTracker.Start.GetComponentInParent<PathFinding.WaypointSystem>();
						lastPathTracker = waypointSystem.GetPathTracker(lastPathTracker.Goal, lastPathTracker.Start);
						if (lastPathTracker != null)
						{
							lastPathTracker.SetGizmosDraw(true);
						}
					}
					else if (GUILayout.Button("Next Point", buttonWidth))
					{
						var nextNode = lastPathTracker.GoNext();
						if (nextNode != null)
						{
							var selects = Selection.objects;
							Selection.activeGameObject = nextNode.gameObject;
							SceneView.lastActiveSceneView.FrameSelected();
							Selection.objects = selects;
						}
					}

					EditorGUILayout.EndHorizontal();
				}
			}
		}
	}
}