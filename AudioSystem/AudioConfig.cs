using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DUCK.AudioSystem
{
	[CreateAssetMenu(menuName = "Audio Config (Fancy™)", order = 220)] // Right after Audio Mixer
	public class AudioConfig : ScriptableObject
	{
		public enum SpatialModes
		{
			Pure2D,
			Simple2D,
			Simple3D
		}

		public enum AudioPlayModes
		{
			Normal,
			Override,
			OneAtATime
		}

		[Tooltip("A list of audio clips. Can play a random one if it has more than 1 clips.")]
		[SerializeField]
		private List<AudioClip> audioClips = new List<AudioClip>();
		public List<AudioClip> AudioClips { get { return audioClips; } }

		[Tooltip("Will override the volume of the Template Audio Source (if presented)")]
		[SerializeField]
		private float volumeScale = 1.0f;
		public float VolumeScale { get { return volumeScale; } }

		[SerializeField]
		private AudioPlayModes playMode;
		public AudioPlayModes PlayMode { get { return playMode; } }

		[Tooltip("Play the audio clip based on this Audio Source's setting")]
		[SerializeField]
		private AudioSource templateAudioSource;
		public AudioSource TemplateAudioSource { get { return templateAudioSource; } }

		[SerializeField]
		private bool loop;
		public bool Loop { get { return loop; } }

		[SerializeField]
		private int priority = 128;
		public int Priority { get { return priority; } }

		[SerializeField]
		private SpatialModes spatialMode;
		public SpatialModes SpatialMode { get { return spatialMode; } }

		[SerializeField]
		private float minDistance = 1.0f;
		public float MinDistance { get { return minDistance; } }

		[SerializeField]
		private float maxDistance = 100.0f;
		public float MaxDistance { get { return maxDistance; } }

		/// <summary>
		/// When an audio clip played, the channel it uses will be stored into this list.
		/// Should be auto managed by the Audio System Extension.
		/// </summary>
		private readonly List<AudioSource> occupiedChannels = new List<AudioSource>();

		/// <summary>
		/// The callbacks on top of the Audio System callbacks.
		/// It's required for the Audio System Extension (for auto referencing the Occupied Channels).
		/// Auto managed by the Audio System Extension.
		/// </summary>
		public UnityObjectDictionary<AudioSource, Action> Callbacks { get { return callbacks; } }
		private readonly UnityObjectDictionary<AudioSource, Action> callbacks = new UnityObjectDictionary<AudioSource, Action>();

		/// <summary>
		/// The current channel.
		/// Will return the last channel if it has multiple; Return null if it has nothing.
		/// </summary>
		public AudioSource CurrentChannel { get { return occupiedChannels.LastOrDefault(); }}

		private void Awake()
		{
			// Tiny helper for lazy developers
			// In addition less crashes!
			audioClips.RemoveAll(clip => clip == null);
		}

		/// <summary>
		/// Get a specific Audio Clip from the list, or a random one if the index is invalid or unspecified.
		/// </summary>
		/// <param name="index">The index of the selected audio clip, if available.</param>
		/// <returns>A random audio clip from the list</returns>
		public AudioClip GetAudioClip(int index = -1)
		{
			var count = audioClips.Count;
			if (count == 0 || index < 0 || index >= count)
			{
				return GetRandomAudioClip();
			}

			return audioClips[index];
		}

		/// <summary>
		/// Get a random Audio Clip from the list.
		/// </summary>
		/// <returns>A random audio clip from the list</returns>
		public AudioClip GetRandomAudioClip()
		{
			var count = audioClips.Count;
			if (count == 0)
			{
				throw new Exception(name + ": has no audio clips!");
			}

			return count == 1 ? audioClips[0] : audioClips[Random.Range(0, count)];
		}

		/// <summary>
		/// Add the channel to the occupied channel
		/// </summary>
		/// <param name="channel">The target channel</param>
		public void AddOccupiedChannel(AudioSource channel)
		{
			if (!occupiedChannels.Contains(channel))
			{
				occupiedChannels.Add(channel);
			}
		}

		/// <summary>
		/// Remove the channel from the occupied channel
		/// </summary>
		/// <param name="channel">The target channel</param>
		/// <returns>True if success</returns>
		public bool RemoveOccupiedChannel(AudioSource channel)
		{
			return occupiedChannels.Remove(channel);
		}

		/// <summary>
		/// Validate the channels, remove the invalid ones. Then return the all valid channels.
		/// </summary>
		public List<AudioSource> GetChannels()
		{
			for (var i = occupiedChannels.Count - 1; i >=0; --i)
			{
				if (occupiedChannels[i] == null)
				{
					occupiedChannels.RemoveAt(i);
				}
			}
			return occupiedChannels;
		}

		/// <summary>
		/// Validate the channels, remove the invalid ones, then get the channel based on its parent transform.
		/// If the parent is null then return everything.
		/// </summary>
		/// <param name="parent">The target parent</param>
		/// <returns>All channels which under the parent transform.</returns>
		public List<AudioSource> GetChannels(Transform parent)
		{
			if (parent == null) return GetChannels();

			var result = new List<AudioSource>();
			for (var i = occupiedChannels.Count - 1; i >=0; --i)
			{
				var channel = occupiedChannels[i];
				if (channel == null)
				{
					occupiedChannels.RemoveAt(i);
				}
				else if (channel.transform.parent == parent)
				{
					result.Add(channel);
				}
			}
			return result;
		}
	}
}