namespace DUCK.PathFinding
{
	/// <summary>
	/// Waypoint connection is the connection between two waypoints.
	/// With a cached distance information to help the path finding.
	/// </summary>
	public class WaypointConnection
	{
		public Waypoint Origin { get; private set; }
		public Waypoint Destination { get; private set; }
		public float Distance { get { return Origin.GetDistance(Destination); } }

		public WaypointConnection(Waypoint origin, Waypoint destination)
		{
			Origin = origin;
			Destination = destination;
		}
	}
}