using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DUCK.AudioSystem
{
	[CreateAssetMenu(menuName = "Sequence Audio Config (Fancy™)", order = 220)] // Right after Audio Mixer
	public class AudioConfigSequence : AbstractAudioConfig
	{
		public enum AudioClipIndexType
		{
			Random,
			Constant,
			Parameter
		}

		private AudioConfig[] compositeAudioConfigs;
		public AudioConfig[] AudioConfigs
		{
			get
			{
				if (compositeAudioConfigs == null)
				{
					compositeAudioConfigs = entries.Select(e => e.AudioConfig).ToArray();
				}

				return compositeAudioConfigs;
			}
		}

		public int Count
		{
			get
			{
				return AudioConfigs.Length;
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

			[SerializeField]
			private float preClipDelay;
			public float ClipDelay { get { return preClipDelay; } }

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

		[SerializeField]
		private SequenceAudioConfigEntry[] entries;

		private Dictionary<string, int> clipIndexParameters;

		private void OnEnable()
		{
			compositeAudioConfigs = null;
		}

		public void SetParameter(string key, int value)
		{
			if (string.IsNullOrEmpty(key)) return;

			if (clipIndexParameters == null)
			{
				clipIndexParameters = new Dictionary<string, int>();
			}

			clipIndexParameters[key] = value;
		}

		public int GetClipIndex(int audioConfigIndex)
		{
			var entry = GetEntry(audioConfigIndex);
			if (entry != null)
			{
				return entry.GetClipIndex(clipIndexParameters);
			}

			return AudioConfig.RANDOM_CLIP;
		}

		public float GetClipDelay(int audioConfigIndex)
		{
			var entry = GetEntry(audioConfigIndex);
			if (entry != null)
			{
				return entry.ClipDelay;
			}

			return 0f;
		}

		private SequenceAudioConfigEntry GetEntry(int audioConfigIndex)
		{
			if (audioConfigIndex >= 0 && audioConfigIndex < entries.Length)
			{
				return entries[audioConfigIndex];
			}

			return null;
		}
	}
}