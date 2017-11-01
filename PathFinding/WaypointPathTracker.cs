using System.Collections.Generic;
using System.Linq;

namespace DUCK.PathFinding
{
	/// <summary>
	/// This wrapper holds the result of the path finding.
	/// It also gives you some helper functions
	/// </summary>
	public class WaypointPathTracker
	{
		/// <summary>
		/// The path being tracked by this
		/// </summary>
		public WaypointPath Path { get; private set; }

		/// <summary>
		/// The Current Waypoint
		/// </summary>
		public Waypoint Current { get; private set; }

		/// <summary>
		/// The Start Waypoint
		/// </summary>
		public Waypoint Start { get; private set; }

		/// <summary>
		/// The Goal Waypoint
		/// </summary>
		public Waypoint Goal { get; private set; }

		/// <summary>
		/// The combined distances (total cost)
		/// </summary>
		public float TotalDistance { get; private set; }

		/// <summary>
		/// Gives you the distance so far (the current cost from start to current)
		/// </summary>
		public float TravelledDistance { get; private set; }

		// Shortcuts
		private readonly List<Waypoint> waypoints;

		private int currentIndex;
		private bool isGizmosDrawing;

		public WaypointPathTracker(WaypointPath path)
		{
			Path = path;
			waypoints = path.Waypoints;

			Current = Start = waypoints[currentIndex = 0];
			Goal = waypoints.Last();
			TravelledDistance = 0;
			TotalDistance = Path.CostSoFar.Last();
		}

		private Waypoint RefreshCurrentWaypoint()
		{
#if UNITY_EDITOR
			Current.SetGizmosAsCurrentNode(false);
#endif
			Current = waypoints[currentIndex];
#if UNITY_EDITOR
			Current.SetGizmosAsCurrentNode(isGizmosDrawing);
#endif
			TravelledDistance = Path.CostSoFar[currentIndex];
			return Current;
		}

		/// <summary>
		/// Go the next Waypoint (of current).
		/// </summary>
		/// <returns>Null if failed / finished.</returns>
		public Waypoint GoNext()
		{
			if (++currentIndex < waypoints.Count)
			{
				return RefreshCurrentWaypoint();
			}
			// Clamp the index
			currentIndex = waypoints.Count - 1;
			return null;
		}

		/// <summary>
		/// Go the previous Waypoint (of current).
		/// </summary>
		/// <returns>Null if failed / finished.</returns>
		public Waypoint GoPrevious()
		{
			if (--currentIndex >= 0)
			{
				return RefreshCurrentWaypoint();
			}
			// Clamp the index
			currentIndex = 0;
			return null;
		}

		/// <summary>
		/// Check if the path is finished it's traversal.
		/// </summary>
		/// <returns>True if finished.</returns>
		public bool IsFinished()
		{
			return currentIndex == waypoints.Count - 1;
		}

		/// <summary>
		/// Check if the path contains any blocked waypoints.
		/// </summary>
		/// <returns>True if it contains.</returns>
		public bool ContainsBlockedWaypoint()
		{
			return Path.ContainsBlockedWaypoint();
		}

		/// <summary>
		/// Get the next blocked waypoint.
		/// </summary>
		/// <returns>Null if no blocked waypoint on the way.</returns>
		public Waypoint GetNextBlockedWaypoint()
		{
			for (var i = currentIndex + 1; i < waypoints.Count; i++)
			{
				if (waypoints[i].isBlocked)
				{
					return waypoints[i];
				}
			}
			return null;
		}

#if UNITY_EDITOR
		/// <summary>
		/// Draw current path as Gizmos
		/// </summary>
		public void SetGizmosDraw(bool isDrawing)
		{
			isGizmosDrawing = isDrawing;

			for (var i = 0; i < waypoints.Count - 1; ++i)
			{
				var waypoint = waypoints[i];
				waypoint.SetGizmosNextPathNode(isGizmosDrawing ? waypoints[i + 1] : null);
			}
			Current.SetGizmosAsCurrentNode(isGizmosDrawing);
		}
#endif
	}
}