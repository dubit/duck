using System.Collections.Generic;

namespace DUCK.PathFinding
{
	/// <summary>
	/// This wrapper holds the result of the path finding.
	/// You can also generate helpers right from it.
	/// </summary>
	public class WaypointPath
	{
		/// <summary>
		/// The actual path -- the path finding result
		/// Order: Start --> Goal
		/// </summary>
		public List<Waypoint> Waypoints { get; private set; }

		/// <summary>
		/// The distance from each waypoint to the Start point.
		/// So it starts from 0, 
		/// </summary>
		public List<float> CostSoFar { get; private set; }

		/// <summary>
		/// Create a Waypoint Path. Requires a list of waypoints.
		/// The costs of the path will generated based on the reference or calculation.
		/// </summary>
		/// <param name="waypoints"></param>
		/// <param name="costs"></param>
		public WaypointPath(List<Waypoint> waypoints, IDictionary<Waypoint, float> costs = null)
		{
			Waypoints = waypoints;
			CostSoFar = new List<float>();
			if (costs != null)
			{
				foreach (var waypoint in waypoints)
				{
					CostSoFar.Add(costs[waypoint]);
				}
			}
			else
			{
				var totalDistance = 0f;
				CostSoFar.Add(totalDistance);
				for (var i = 1; i < waypoints.Count; ++i)
				{
					var distance = waypoints[i - 1].GetDistance(waypoints[i]);
					CostSoFar.Add(totalDistance + distance);
					totalDistance += distance;
				}
			}
		}

		/// <summary>
		/// Check if the path contains any blocked waypoints.
		/// </summary>
		/// <returns>True if it contains.</returns>
		public bool ContainsBlockedWaypoint()
		{
			return Waypoints.Exists(waypoint => waypoint.isBlocked);
		}

		/// <summary>
		/// Generate a tracker based on this path.
		/// </summary>
		/// <returns>The Tracker</returns>
		public WaypointPathTracker GenerateTracker()
		{
			return new WaypointPathTracker(this);
		}
	}
}