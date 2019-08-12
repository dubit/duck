using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DUCK.PathFinding.Editor
{
	public class WaypointTests
	{
		private WaypointSystem waypointSystem;
		private List<Waypoint> waypoints;
		private Scene editorScene;

		[OneTimeSetUp]
		public void SetUpTest()
		{
			editorScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
		}

		private Waypoint CreateWaypoint(Vector2 position)
		{
			var waypoint = new GameObject();
			EditorSceneManager.MoveGameObjectToScene(waypoint, editorScene);
			waypoint.transform.position = position;
			var waypointComponent = waypoint.AddComponent<Waypoint>();
			waypoints.Add(waypointComponent);
			return waypointComponent;
		}

		[SetUp]
		public void SetUp()
		{
			waypoints = new List<Waypoint>();
			var systemObj = new GameObject();
			waypointSystem = systemObj.AddComponent<WaypointSystem>();
			EditorSceneManager.MoveGameObjectToScene(systemObj, editorScene);
		}

		[Test]
		public void Expect_Path_ToBeFound()
		{
			var waypoint1 = CreateWaypoint(Vector2.zero);
			var waypoint2 = CreateWaypoint(Vector2.one);
			var waypoint3 = CreateWaypoint(Vector2.one + Vector2.one);

			waypoint1.ConnectWaypoint(waypoint2);
			waypoint2.ConnectWaypoint(waypoint3);

			var path = waypointSystem.GetPath(waypoint1, waypoint3);

			Assert.IsNotNull(path);
			Assert.IsTrue(path.Waypoints.Contains(waypoint1));
			Assert.IsTrue(path.Waypoints.Contains(waypoint2));
			Assert.IsTrue(path.Waypoints.Contains(waypoint3));
		}

		[Test]
		public void Expect_Waypoint_ToHaveConnection()
		{
			var waypoint1 = CreateWaypoint(Vector2.zero);
			var waypoint2 = CreateWaypoint(Vector2.one);

			waypoint1.ConnectWaypoint(waypoint2);
			waypoint2.ConnectWaypoint(waypoint1);
			
			Assert.IsTrue(waypoint1.IsConnectedWith(waypoint2));
			Assert.IsTrue(waypoint2.IsConnectedWith(waypoint1));
		}

		[Test]
		public void Expect_Path_ToBeBlocked()
		{
			var waypoint1 = CreateWaypoint(Vector2.zero);
			var waypoint2 = CreateWaypoint(Vector2.one);
			var waypoint3 = CreateWaypoint(Vector2.one + Vector2.one);

			waypoint1.ConnectWaypoint(waypoint2);
			waypoint2.ConnectWaypoint(waypoint3);

			waypoint2.isBlocked = true;

			var path = waypointSystem.GetPath(waypoint1, waypoint3);

			Assert.IsNull(path);
		}

		[Test]
		public void Expect_ShortestPath_ToBeFound()
		{
			var waypoint1 = CreateWaypoint(Vector2.zero);
			var waypoint2 = CreateWaypoint(Vector2.one);
			var waypoint3 = CreateWaypoint(Vector2.one * 2);
			var waypoint4 = CreateWaypoint(Vector2.one + Vector2.down);
			var waypoint5 = CreateWaypoint(Vector2.one * 3);
			var waypoint6 = CreateWaypoint(Vector2.one * 4);

			//1 -> 2 -> 3-> 4-> goal (long way)
			waypoint1.ConnectWaypoint(waypoint2);
			waypoint2.ConnectWaypoint(waypoint3);
			waypoint3.ConnectWaypoint(waypoint4);
			waypoint4.ConnectWaypoint(waypoint6);

			//1 -> 2 -> 5 -> goal (short way)
			waypoint2.ConnectWaypoint(waypoint5);
			waypoint5.ConnectWaypoint(waypoint6);

			var path = waypointSystem.GetPath(waypoint1, waypoint6);

			Assert.IsNotNull(path);

			Assert.IsTrue(path.Waypoints.Contains(waypoint1));
			Assert.IsTrue(path.Waypoints.Contains(waypoint2));
			Assert.IsTrue(path.Waypoints.Contains(waypoint5));
			Assert.IsTrue(path.Waypoints.Contains(waypoint6));
			
			Assert.IsFalse(path.Waypoints.Contains(waypoint3));
			Assert.IsFalse(path.Waypoints.Contains(waypoint4));
		}

		[Test]
		public void Expect_PathTracker_ToStartOnCorrectWaypoint()
		{
			var waypoint1 = CreateWaypoint(Vector2.zero);
			var waypoint2 = CreateWaypoint(Vector2.one);
			var waypoint3 = CreateWaypoint(Vector2.one + Vector2.one);

			waypoint1.ConnectWaypoint(waypoint2);
			waypoint2.ConnectWaypoint(waypoint3);

			var path = waypointSystem.GetPath(waypoint1, waypoint3);
			var pathTracker = new WaypointPathTracker(path);
			
			Assert.IsTrue(pathTracker.Current == waypoint1);
		}

		[Test]
		public void Expect_PathTracker_ToFollowCorrectPath()
		{
			var waypoint1 = CreateWaypoint(Vector2.zero);
			var waypoint2 = CreateWaypoint(Vector2.one);
			var waypoint3 = CreateWaypoint(Vector2.one + Vector2.one);

			waypoint1.ConnectWaypoint(waypoint2);
			waypoint2.ConnectWaypoint(waypoint3);

			var path = waypointSystem.GetPath(waypoint1, waypoint3);
			var pathTracker = new WaypointPathTracker(path);

			Assert.IsTrue(pathTracker.Current == waypoint1);

			pathTracker.GoNext();

			Assert.IsTrue(pathTracker.Current == waypoint2);

			pathTracker.GoNext();

			Assert.IsTrue(pathTracker.Current == waypoint3);
			Assert.IsTrue(pathTracker.IsFinished());
		}

		[Test]
		public void Expect_PathTracker_GoalToBeCorrectWaypoint()
		{
			var waypoint1 = CreateWaypoint(Vector2.zero);
			var waypoint2 = CreateWaypoint(Vector2.one);
			var waypoint3 = CreateWaypoint(Vector2.one + Vector2.one);

			waypoint1.ConnectWaypoint(waypoint2);
			waypoint2.ConnectWaypoint(waypoint3);

			var path = waypointSystem.GetPath(waypoint1, waypoint3);
			var pathTracker = new WaypointPathTracker(path);

			Assert.IsTrue(pathTracker.Goal == waypoint3);
		}

		[Test]
		public void Expect_FinishedPathTracker_NextWaypoint_ToBeNull()
		{
			var waypoint1 = CreateWaypoint(Vector2.zero);
			var waypoint2 = CreateWaypoint(Vector2.one);
			var waypoint3 = CreateWaypoint(Vector2.one + Vector2.one);

			waypoint1.ConnectWaypoint(waypoint2);
			waypoint2.ConnectWaypoint(waypoint3);

			var path = waypointSystem.GetPath(waypoint1, waypoint3);
			var pathTracker = new WaypointPathTracker(path);

			pathTracker.GoNext();
			pathTracker.GoNext();

			Assert.IsTrue(pathTracker.IsFinished());
			Assert.IsNull(pathTracker.GoNext());
		}

		[Test]
		public void Expect_FinishedPathTracker_PreviousWaypoint_ToBeNull()
		{
			var waypoint1 = CreateWaypoint(Vector2.zero);
			var waypoint2 = CreateWaypoint(Vector2.one);
			var waypoint3 = CreateWaypoint(Vector2.one + Vector2.one);

			waypoint1.ConnectWaypoint(waypoint2);
			waypoint2.ConnectWaypoint(waypoint3);

			var path = waypointSystem.GetPath(waypoint1, waypoint3);
			var pathTracker = new WaypointPathTracker(path);

			Assert.IsFalse(pathTracker.IsFinished());
			Assert.IsNull(pathTracker.GoPrevious());
		}

		[Test]
		public void Expect_PathTracker_ToGoBackwards()
		{
			var waypoint1 = CreateWaypoint(Vector2.zero);
			var waypoint2 = CreateWaypoint(Vector2.one);
			var waypoint3 = CreateWaypoint(Vector2.one + Vector2.one);

			waypoint1.ConnectWaypoint(waypoint2);
			waypoint2.ConnectWaypoint(waypoint3);

			var path = waypointSystem.GetPath(waypoint1, waypoint3);
			var pathTracker = new WaypointPathTracker(path);

			pathTracker.GoNext();
			pathTracker.GoPrevious();

			Assert.IsFalse(pathTracker.IsFinished());
			Assert.IsTrue(pathTracker.Current == waypoint1);
		}

		[Test]
		public void Expect_PathTracker_TravelDistanceToBeCorrect()
		{
			var waypoint1 = CreateWaypoint(Vector2.zero);
			var waypoint2 = CreateWaypoint(Vector2.one);
			var waypoint3 = CreateWaypoint(Vector2.one + Vector2.one);

			waypoint1.ConnectWaypoint(waypoint2);
			waypoint2.ConnectWaypoint(waypoint3);

			var path = waypointSystem.GetPath(waypoint1, waypoint3);
			var pathTracker = new WaypointPathTracker(path);

			Assert.IsTrue(pathTracker.TravelledDistance == 0);

			pathTracker.GoNext();
			pathTracker.GoNext();

			Assert.IsTrue(pathTracker.IsFinished());
			Assert.IsTrue(pathTracker.TravelledDistance == pathTracker.TotalDistance);
		}

		[TearDown]
		public void TearDown()
		{
			foreach(var waypoint in waypoints)
			{
				GameObject.DestroyImmediate(waypoint.gameObject);
			}

			waypoints.Clear();

			GameObject.DestroyImmediate(waypointSystem.gameObject);
			waypointSystem = null;
		}

		[OneTimeTearDown]
		public void TearDownTest()
		{
			EditorSceneManager.CloseScene(editorScene, true);
		}
	}
}
