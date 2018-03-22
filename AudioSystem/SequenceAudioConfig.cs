using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DUCK.AudioSystem
{
	[CreateAssetMenu(menuName = "Sequence Audio Config (Fancy™)", order = 220)] // Right after Audio Mixer
	public class SequenceAudioConfig : AbstractAudioConfig
	{
		public enum AudioClipIndexType
		{
			Random,
			Constant,
			Parameter
		}

		public override AudioConfig[] AudioConfigs
		{
			get
			{
				return entries.Select(e => e.AudioConfig).ToArray();
			}
		}

		public override string AlternativeText
		{
			get
			{
				return String.Join(" ", entries.Select(e => e.AudioConfig.AlternativeText).ToArray());
			}
		}

		[Serializable]
		public class SequenceAudioConfigEntry
		{
			[SerializeField]
			private AudioConfig audioConfig;

			[SerializeField]
			private AudioClipIndexType clipMode;

			[SerializeField]
			private int clipIndexConstant;

			[SerializeField]
			private string clipIndexParameter;

			public AudioConfig AudioConfig { get { return audioConfig; } }

			public int GetClipIndex(IDictionary<string, int> parameters = null)
			{
				switch (clipMode)
				{
					case AudioClipIndexType.Constant:
						return clipIndexConstant;

					case AudioClipIndexType.Parameter:
						if (parameters != null && parameters.ContainsKey(clipIndexParameter))
						{
							return parameters[clipIndexParameter];
						}
						else goto default;

					default:
						return AudioConfig.RANDOM_CLIP;
				}
			}
		}

		public float DelayBetweenClips { get { return delay; } }

		[SerializeField]
		private SequenceAudioConfigEntry[] entries;

		[SerializeField]
		private float delay;

		private Dictionary<string, int> clipIndexParameters;

		public void SetParameter(string key, int value)
		{
			if (string.IsNullOrEmpty(key)) return;

			if (clipIndexParameters == null)
			{
				clipIndexParameters = new Dictionary<string, int>();
			}

			clipIndexParameters[key] = value;
		}

		public int GetClipIndex(AudioConfig audioConfig)
		{
			var entry = entries.FirstOrDefault(e => e.AudioConfig == audioConfig);
			if (entry != null)
			{
				return entry.GetClipIndex(clipIndexParameters);
			}

			return AudioConfig.RANDOM_CLIP;
		}
	}
}