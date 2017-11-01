using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DUCK.PathFinding
{
	[DisallowMultipleComponent]
	public class Waypoint : MonoBehaviour
	{
		/// <summary>
		/// Cached data for faster path finding
		/// </summary>
		public readonly List<WaypointConnection> connections = new List<WaypointConnection>();

		[SerializeField]
		[Tooltip("It is not recommended to modify this list manually, unless you know what's going on.")]
		private List<Waypoint> connectedWaypoints = new List<Waypoint>();

		[SerializeField]
		[Tooltip("Is the path finding system ignoring this Waypoint or not?")]
		public bool isIgnored;

		[SerializeField]
		[Tooltip("Is this waypoint blocked or not?")]
		public bool isBlocked;

		[SerializeField]
		[Tooltip("Is this waypoint attached to a dynamic object?")]
		private bool isDynamic;
		public bool IsDynamic
		{
			get { return isDynamic; }
			set
			{
				isDynamic = value;
				gameObject.isStatic = !isDynamic;
				if (isDynamic)
				{
					cachedDistance.Clear();
				}
			}
		}

		/// <summary>
		/// Additional Cost will add up to the distance to goal when path finding doing Heuristic search.
		/// This can be very useful to mark a waypoint expensive than the others and reroute the path.
		/// </summary>
		public float AdditionalCost { get; set; }

		private readonly Dictionary<Waypoint, float> cachedDistance = new Dictionary<Waypoint, float>();

#if UNITY_EDITOR
		// Those vars for Gizmos only
		private Waypoint nextWaypointOnPath;
		private float gizmosLerpT = 0.5f;
		private bool isCurrentPathTarget;
#endif

		protected void Awake()
		{
			ValidateConnectedWaypoints();
			RefreshConnections();
			if (!isDynamic)
			{
				gameObject.isStatic = true;
			}
		}

		private void RefreshConnections()
		{
			connections.Clear();
			foreach (var waypoint in connectedWaypoints)
			{
				connections.Add(new WaypointConnection(this, waypoint));
			}
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (connectedWaypoints.Count > 0)
			{
				// Draw the sphere / cube
				if (isBlocked)
				{
					Gizmos.color = isCurrentPathTarget ? WaypointGizmoStyle.ColorCurrentNode : WaypointGizmoStyle.CubeColor;
					if (isIgnored)
					{
						Gizmos.DrawWireCube(transform.position, WaypointGizmoStyle.wireCubeSize);
						Gizmos.color = isCurrentPathTarget ? WaypointGizmoStyle.ColorCurrentNode : WaypointGizmoStyle.CubeIgnoredColor;
					}
					Gizmos.DrawCube(transform.position,
						isCurrentPathTarget ? WaypointGizmoStyle.cubeSizeCurrentNode : WaypointGizmoStyle.cubeSize);
				}
				else
				{
					Gizmos.color = isCurrentPathTarget ? WaypointGizmoStyle.ColorCurrentNode : WaypointGizmoStyle.SphereColor;
					if (isIgnored)
					{
						Gizmos.DrawWireSphere(transform.position, WaypointGizmoStyle.wireSphereRadius);
						Gizmos.color = isCurrentPathTarget ? WaypointGizmoStyle.ColorCurrentNode : WaypointGizmoStyle.SphereIgnoredColor;
					}
					Gizmos.DrawSphere(transform.position,
						isCurrentPathTarget ? WaypointGizmoStyle.sphereRadiusCurrentNode : WaypointGizmoStyle.nodeRadius);
				}

				if (isDynamic)
				{
					Gizmos.color = WaypointGizmoStyle.DynamicNodeColor;
					Gizmos.DrawSphere(transform.position, WaypointGizmoStyle.dynamicNodeRadius);
				}

				// Draw the lines and arrows
				foreach (var waypoint in connectedWaypoints.Where(waypoint => waypoint != null))
				{
					var lineNormal = (waypoint.transform.position - transform.position).normalized;
					var offset = waypoint.IsConnectedWith(this)
						? Quaternion.AngleAxis(90.0f, Vector3.up) * lineNormal * WaypointGizmoStyle.doubleLineWidth
						: Vector3.zero;

					var lineFrom = transform.position + offset;
					var lineTo = waypoint.transform.position + offset;

					var arrowAxis = Vector3.Cross(lineFrom, lineTo);
					var arrowDiff = lineTo - lineFrom;
					var arrowLength = arrowDiff.normalized * WaypointGizmoStyle.arrowSize;
					var arrowFrom = lineFrom + arrowDiff * Mathf.Lerp(0.35f, 0.65f, gizmosLerpT);
					var arrowToLeft = arrowFrom + Quaternion.AngleAxis(Mathf.Lerp(160f, 180f, gizmosLerpT), arrowAxis) * arrowLength;
					var arrowToRight = arrowFrom + Quaternion.AngleAxis(Mathf.Lerp(200f, 180f, gizmosLerpT), arrowAxis) * arrowLength;

					// The line
					Gizmos.color = waypoint.isBlocked
						? WaypointGizmoStyle.LineBlockedColor
						: waypoint == nextWaypointOnPath
							? WaypointGizmoStyle.PathColor
							: WaypointGizmoStyle.LineColor;
					Gizmos.DrawLine(lineFrom, lineTo);
					// The arrow
					Gizmos.color = WaypointGizmoStyle.ArrowColor;
					Gizmos.DrawLine(arrowFrom, arrowToLeft);
					Gizmos.DrawLine(arrowFrom, arrowToRight);
					Gizmos.DrawLine(arrowToLeft, arrowToRight);
				}
			}
			else // a.k.a DEAD END
			{
				Gizmos.color = WaypointGizmoStyle.ColorFaulty;
				Gizmos.DrawSphere(transform.position, WaypointGizmoStyle.nodeRadius);
			}
		}

		private void OnDrawGizmosSelected()
		{
			// This will also animate the arrows draw in the OnDrawGizmos
			gizmosLerpT = Mathf.PingPong(Time.realtimeSinceStartup, WaypointGizmoStyle.glowingSpeed) / WaypointGizmoStyle.glowingSpeed;
			gizmosLerpT = Mathf.SmoothStep(0, 1.0f, gizmosLerpT);
			// Draw the animated sphere / cube
			Gizmos.color = Color.Lerp(WaypointGizmoStyle.ColorGlowFrom, WaypointGizmoStyle.ColorGlowTo, gizmosLerpT);
			if (isBlocked)
			{
				Gizmos.DrawCube(transform.position,
					Vector3.Lerp(WaypointGizmoStyle.cubeSizeFrom, WaypointGizmoStyle.cubeSizeTo, gizmosLerpT));
			}
			else
			{
				Gizmos.DrawSphere(transform.position,
					Mathf.Lerp(WaypointGizmoStyle.radiusGrowFrom, WaypointGizmoStyle.radiusGrowTo, gizmosLerpT));
			}
		}

		public void SetGizmosNextPathNode(Waypoint targetWaypoint)
		{
			nextWaypointOnPath = targetWaypoint;
			EditorUtility.SetDirty(this);
		}

		public void SetGizmosAsCurrentNode(bool isCurrent)
		{
			isCurrentPathTarget = isCurrent;
			EditorUtility.SetDirty(this);
		}
#endif

		public void ValidateConnectedWaypoints()
		{
			connectedWaypoints.RemoveAll(waypoint => waypoint == null);
		}

		public void ValidateOthersAfterDestroyed()
		{
			foreach (var waypoint in connectedWaypoints.Where(waypoint => waypoint != null))
			{
				waypoint.ValidateConnectedWaypoints();
			}
		}

		public bool IsConnectedWith(Waypoint targetWaypoint)
		{
			return connectedWaypoints.Contains(targetWaypoint);
		}

		public void ConnectWaypoint(Waypoint targetWaypoint)
		{
			if (connectedWaypoints.Contains(targetWaypoint)) return;

			connectedWaypoints.Add(targetWaypoint);
			RefreshConnections();
		}

		public void DisconnectWaypoint()
		{
			connectedWaypoints.Clear();
			RefreshConnections();
		}

		public void DisconnectWaypoint(Waypoint targetWaypoint)
		{
			if (connectedWaypoints.Count > 0 && !connectedWaypoints.Contains(targetWaypoint)) return;

			connectedWaypoints.Remove(targetWaypoint);
			RefreshConnections();
		}

		/// <summary>
		/// Get distance to the target waypoint.
		/// The result will be cached if both side are static waypoint.
		/// </summary>
		/// <param name="targetWaypoint">The target Waypoint</param>
		/// <returns>Distance between the two waypoints</returns>
		public float GetDistance(Waypoint targetWaypoint)
		{
			if (isDynamic || targetWaypoint.isDynamic)
			{
				return Vector3.Distance(transform.position, targetWaypoint.transform.position);
			}

			float result;
			if (!cachedDistance.TryGetValue(targetWaypoint, out result))
			{
				result = Vector3.Distance(transform.position, targetWaypoint.transform.position);
				cachedDistance[targetWaypoint] = result;
				targetWaypoint.cachedDistance[this] = result;
			}
			return result;
		}

		/// <summary>
		/// Get the cached connection which connected to the target Waypoint.
		/// </summary>
		/// <param name="targetWaypoint"></param>
		/// <returns>Null if failed to find the connection</returns>
		public WaypointConnection GetConnection(Waypoint targetWaypoint)
		{
			return connections.FirstOrDefault(connection => connection.Destination == targetWaypoint);
		}

		protected void OnDestroy()
		{
			foreach (var waypoint in connectedWaypoints.Where(waypoint => waypoint != null))
			{
				waypoint.DisconnectWaypoint(this);
			}
		}
	}
}