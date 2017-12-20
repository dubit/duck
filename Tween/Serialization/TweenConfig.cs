using System;
using DUCK.Serialization;
using UnityEngine;

namespace DUCK.Tween.Serialization
{
	[Serializable]
	public class TweenConfig
	{
		[SerializeField]
		private string creatorFunctionKey;
		
		[SerializeField]
		private ArgsList args = new ArgsList();

		// These properties get exposed but only for UNITY_EDITOR for custom editor access
		public string CreatorFunctionKey
		{
			get { return creatorFunctionKey; }
#if !UNITY_EDITOR
			private
#endif
			set { creatorFunctionKey = value; }
		}
		public ArgsList Args
		{
			get { return args; }
#if !UNITY_EDITOR
			private
#endif
			set { args = value; }
		}
	}
}