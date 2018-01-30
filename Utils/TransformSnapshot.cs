using UnityEngine;

namespace DUCK.Utils
{
	public class TransformSnapshot 
	{
		public Transform Parent { get; private set; }
		public Transform Transform { get; private set; }
		
		// Positions
		public Vector3 WorldPosition { get; private set; }
		public Vector3 LocalPosition { get; private set; }
		
		// Rotations
		public Quaternion WorldRotation { get; private set; }
		public Vector3 WorldEulerAngles { get; private set; }
		public Quaternion LocalRotation { get; private set; }
		public Vector3 LocalEulerAngles { get; private set; }
		
		// Direction
		public Vector3 Forward { get; private set; }
		public Vector3 Up { get; private set; }
		public Vector3 Right { get; private set; }
		
		// Scale
		public Vector3 WorldScale { get; private set; }
		public Vector3 LocalScale { get; private set; }
		
		public TransformSnapshot(Transform transform)
		{
			Parent = transform.parent;
			Transform = transform;

			// Position
			WorldPosition = transform.position;
			LocalPosition = transform.localPosition;

			// Rotation
			WorldRotation = transform.rotation;
			WorldEulerAngles = transform.eulerAngles;
			LocalRotation = transform.localRotation;
			LocalEulerAngles = transform.localEulerAngles;

			// Direction
			Forward = transform.forward;
			Up = transform.up;
			Right = transform.right;

			// Scale
			WorldScale = transform.lossyScale;
			LocalScale = transform.localScale;
		}
	}
}
