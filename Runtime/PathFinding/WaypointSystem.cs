using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DUCK.PathFinding
{
	/// <summary>
	/// Waypoint System helps you to find the path.
	/// It stays as MonoBehaviour for now because can be easier to find in the Hierarchy.
	/// And also can be easily extended with customised / serialised settings.
	/// </summary>
	public class WaypointSystem : MonoBehaviour
	{
		/// <summary>
		/// All its children waypoints.
		/// </summary>
		public List<Waypoint> Waypoints { get; protected set; }

		protected virtual void Awake()
		{
			Waypoints = GetComponentsInChildren<Waypoint>().ToList();
		}

		/// <summary>
		/// Find a nearby Waypoint.
		/// Not recommended to use frequently -- try adding some triggers onto your Waypoints!
		/// </summary>
		/// <param name="position">The target position you trying to find the Waypoints nearby.</param>
		/// <param name="allowBlocked">Should allow blocked </param>
		/// <param name="maxRange">The maximum acceptable range. Set to 0 (default) will find the nearest.</param>
		/// <returns>A nearby Waypoint -- should never null unless no Waypoints available</returns>
		public Waypoint FindNearbyWaypoint(Vector3 position, bool allowBlocked = false, float maxRange = 0)
		{
			Waypoint nearestWaypoint = null;

			var maxRangeSqr = maxRange * maxRange;
			var nearestDistSqr = float.PositiveInfinity;
			foreach (var waypoint in Waypoints)
			{
				if (waypoint.isIgnored || !allowBlocked && waypoint.isBlocked)
				{
					continue;
				}
				var distSqr = (position - waypoint.transform.position).sqrMagnitude;
				if (distSqr < nearestDistSqr)
				{
					nearestWaypoint = waypoint;
					nearestDistSqr = distSqr;
					// Stop searching immediately once find a waypoint in range. Better performance.
					if (distSqr <= maxRangeSqr)
					{
						break;
					}
				}
			}
			return nearestWaypoint;
		}

		/// <summary>
		/// Find all Waypoints within a range.
		/// Not recommended to use frequently -- try adding some triggers onto your Waypoints!
		/// </summary>
		/// <param name="position">The target position you trying to find the Waypoints nearby.</param>
		/// <param name="range">The range.</param>
		/// <returns>List of Waypoints in the range. It may contains no waypoints if it could not find any.</returns>
		public List<Waypoint> FindWaypointsInRange(Vector3 position, float range)
		{
			var rangeSqr = range * range;
			return (from waypoint in Waypoints
					where !waypoint.isIgnored && (position - waypoint.transform.position).sqrMagnitude <= rangeSqr
					select waypoint).ToList();
		}

		/// <summary>
		/// Get a random Waypoint from the list.
		/// </summary>
		/// <returns>A random Waypoint. Will return null if no waypoint exist.</returns>
		public Waypoint GetRandomWaypoint()
		{
			return Waypoints.Count > 0 ? Waypoints[Random.Range(0, Waypoints.Count)] : null;
		}

		/// <summary>
		/// Using A* algorithm. Nothing fancy unfortunately!
		/// Heuristic search simply based on distances for now -- you can override it if you need.
		/// </summary>
		/// <param name="start">The Start Waypoint (can be set to ignored or/and blocked)</param>
		/// <param name="goal">The Goal Waypoint (must not set to ignored, but can be blocked)</param>
		/// <param name="unblocked">Set to false if you want the result contains blocked waypoints</param>
		/// <returns>The WaypointPath from Start to Goal. Returns null if no path has been found.</returns>
		public WaypointPath GetPath(Waypoint start, Waypoint goal, bool unblocked = true)
		{
			// Invalid start / goal
			if (start == null || goal == null || goal.isIgnored) return null;

			// Init for the A* search
			var frontTier = new List<Node> {new Node(start, 0)};
			var cameFrom = new Dictionary<Waypoint, Waypoint> {{start, null}};
			var costSoFar = new Dictionary<Waypoint, float> {{start, 0}};

			// If we having problems with searching time, then split this function into frames
			// It should be reasonably fast though...
			while (frontTier.Count > 0)
			{
				var current = PopHighestPriorityWaypoint(frontTier);
				// Stops searching when we hit the goal
				if (current == goal) break;
				// Return null when we find no path
				if (current == null) return null;

				foreach (var connection in current.connections)
				{
					var next = connection.Destination;
					// Ignore the connection if:
					// 1. the destination is set to ignored
					// 2. the destination is blocked when we want a unblocked path
					if (next.isIgnored || unblocked && next.isBlocked) continue;

					var cost = costSoFar[current] + connection.Distance;
					if (!costSoFar.ContainsKey(next) || cost < costSoFar[next])
					{
						var priority = cost + Heuristic(next, goal);
						frontTier.Add(new Node(next, priority));
						cameFrom[next] = current;
						costSoFar[next] = cost;
					}
				}
			}

			// from the goal to the start
			Waypoint chainNode;
			if (!cameFrom.TryGetValue(goal, out chainNode))
			{
				// less likely happen here, but -- no goal in the list means no path has been found.
				return null;
			}

			var path = new List<Waypoint> {goal};
			while (chainNode != null)
			{
				path.Add(chainNode);
				chainNode = cameFrom[chainNode];
			}
			// reverse the list --> from Start to Goal.
			path.Reverse();

			return new WaypointPath(path, costSoFar);
		}

		/// <summary>
		/// A shortcut to get a Path Tracker.
		/// </summary>
		/// <param name="start">The Start Waypoint (can be set to ignored or/and blocked)</param>
		/// <param name="goal">The Goal Waypoint (must not set to ignored, but can be blocked)</param>
		/// <param name="unblocked">Set to false if you want the result contains blocked waypoints</param>
		/// <returns>Returns null if no path has been found.</returns>
		public WaypointPathTracker GetPathTracker(Waypoint start, Waypoint goal, bool unblocked = true)
		{
			var path = GetPath(start, goal, unblocked);
			return path == null ? null : path.GenerateTracker();
		}

		/// <summary>
		/// Get the highest priority Waypoint (less costs ones) and remove it from the list.
		/// </summary>
		/// <param name="list">List of waypoints with priorities (Node)</param>
		/// <returns>The highest priority Waypoint. Returns null on fail.</returns>
		protected virtual Waypoint PopHighestPriorityWaypoint(List<Node> list)
		{
			Node target = null;
			var priority = float.PositiveInfinity;
			list.ForEach(node =>
			{
				if (node.Priority < priority)
				{
					target = node;
					priority = target.Priority;
				}
			});
			list.Remove(target);
			return target == null ? null : target.Waypoint;
		}

		/// <summary>
		/// Estimate the cost of the waypoint to the goal.
		/// For now just simply use the distance.
		/// </summary>
		/// <param name="current">The fancy Waypoint where you are currently at</param>
		/// <param name="goal">The mighty goal</param>
		/// <returns>The estimated cost</returns>
		protected virtual float Heuristic(Waypoint current, Waypoint goal)
		{
			return current.GetDistance(goal) + current.AdditionalCost;
		}

		/// <summary>
		/// Protected class Node, designed for the A* algorithm.
		/// Contains the waypoint and its priority.
		/// </summary>
		protected class Node
		{
			public Waypoint Waypoint { get; private set; }
			public float Priority { get; private set; }

			public Node(Waypoint waypoint, float priority)
			{
				Waypoint = waypoint;
				Priority = priority;
			}
		}
	}
}