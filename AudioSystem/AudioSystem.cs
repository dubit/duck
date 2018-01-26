using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace DUCK.AudioSystem
{
	/// <summary>
	/// A simple and lightweight Audio System
	/// Designed for 2D sounds playback
	/// Must be placed in the scene to make it works (Unity style Singleton)
	/// </summary>
	[DisallowMultipleComponent]
	public sealed class AudioSystem : MonoBehaviour
	{
		/// <summary>
		/// For internal audio volume update
		/// </summary>
		private class AudioFader
		{
			public AudioSource Channel { get { return channel; } }

			private readonly AudioSource channel;
			private readonly float from;
			private readonly float to;
			private readonly float minRangeSqr;
			private readonly float maxRangeSqr;
			private readonly float duration = 1.0f;
			private readonly Action onComplete;

			private float timer;

			/// <summary>
			/// Create an Audio Fader and update the volume based on lerp delta.
			/// </summary>
			/// <param name="channel">The target channel</param>
			/// <param name="minVolume">Minimum volume at maximum range</param>
			/// <param name="maxVolume">Maximum volume at minimum range</param>
			/// <param name="minRange">Minimum range for maximum volume</param>
			/// <param name="maxRange">Maximum range for minimum volume</param>
			public AudioFader(AudioSource channel, float minVolume, float maxVolume, float minRange, float maxRange)
			{
				this.channel = channel;

				from = minVolume;
				to = maxVolume;
				minRangeSqr = minRange * minRange;
				maxRangeSqr = maxRange * maxRange;
			}

			/// <summary>
			/// Create an Audio Fader that update the volume based on elapsed time (delta time).
			/// </summary>
			/// <param name="channel">The target channel</param>
			/// <param name="from">From volume</param>
			/// <param name="to">To volume</param>
			/// <param name="duration">Duration of the fade</param>
			/// <param name="onComplete">On Complete callback</param>
			public AudioFader(AudioSource channel, float from, float to, float duration, Action onComplete)
			{
				this.channel = channel;
				this.from = from;
				this.to = to;
				this.duration = duration;
				this.onComplete = onComplete;
			}

			/// <summary>
			/// Update the volume based on the time elapsed
			/// </summary>
			/// <param name="dt">Delta Time</param>
			/// <returns>Return false if it is no longer valid</returns>
			public bool Update(float dt)
			{
				if (timer >= duration || channel == null) return false;

				timer += dt;
				channel.volume = Mathf.Lerp(from, to, timer / duration);
				if (timer >= duration)
				{
					if (onComplete != null)
					{
						onComplete.Invoke();
					}
					return false;
				}

				return true;
			}

			/// <summary>
			/// Update the volume based on the distance to the listener
			/// </summary>
			/// <param name="listenerPos">The audio listener position</param>
			/// <returns>Return false if it is no longer valid</returns>
			public bool Update(Vector3 listenerPos)
			{
				if (channel == null || !channel.isPlaying) return false;

				var t = Mathf.InverseLerp(maxRangeSqr, minRangeSqr, (channel.transform.position - listenerPos).sqrMagnitude);
				channel.volume = Mathf.Lerp(from, to, t);
				return true;
			}
		}

		/// <summary>
		/// An Unity style "singleton". It needs manually instantiated (or serialised) and attached to a GameObject.
		/// Remember -- it can be null.
		/// </summary>
		public static AudioSystem Instance { get; private set; }

		[SerializeField]
		private AudioMixerGroup defaultMixerGroup;

		[SerializeField]
		private AudioListener audioListener;
		public AudioListener AudioListener { get { return audioListener; } }

		private List<AudioSource> channels;
		private readonly List<AudioSource> clonedChannels = new List<AudioSource>();
		private readonly List<AudioFader> audioFaders = new List<AudioFader>();
		private readonly List<AudioFader> trackedChannels = new List<AudioFader>();
		private readonly UnityObjectDictionary<AudioSource, Action<AudioSource>> callbacks = new UnityObjectDictionary<AudioSource, Action<AudioSource>>();

		private void Awake()
		{
			if (Instance != null)
			{
				throw new Exception("You should only have 1 AudioSystem in your game! " + name);
			}
			Instance = this;
			if (transform.parent == null)
			{
				DontDestroyOnLoad(this);
			}

			// Reset transform
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
			transform.localScale = Vector3.one;

			// Setup new channel pool
			var config = AudioSettings.GetConfiguration();
			channels = new List<AudioSource>(config.numRealVoices);
			for (var i = 0; i < channels.Capacity; ++i)
			{
				channels.Add(CreateChannel(i));
			}
		}

		/// <summary>
		/// Set the audio listener (for simple 2D sound)
		/// </summary>
		/// <param name="audioListener">The target audio listener</param>
		public void SetAudioListener(AudioListener audioListener)
		{
			this.audioListener = audioListener;
		}

		/// <summary>
		/// Play an audio clip by using a channel from the pool
		/// </summary>
		/// <param name="audioClip">The audio clip you want to play</param>
		/// <param name="volume">The volume scale for the playback</param>
		/// <param name="loop">Loop the clip</param>
		/// <param name="onComplete">The callback when the playback finished</param>
		/// <returns>The channel (AudioSource) which the Audio System are using</returns>
		public AudioSource Play(AudioClip audioClip, float volume = 1.0f, bool loop = false, Action<AudioSource> onComplete = null)
		{
			var channel = GetFreeChannel();
			channel.clip = audioClip;
			channel.loop = loop;
			channel.volume = volume;

			return Play(channel, onComplete);
		}

		/// <summary>
		/// Play an audio clip by cloning an Audio Source based on the template
		/// This will give you rich audio playback settings by using a serialised Audio Source object
		/// Beware: onComplete will not work on looped template audio source!
		/// </summary>
		/// <param name="audioClip">The audio clip you want to play</param>
		/// <param name="templateAudioSource">The template audio source</param>
		/// <param name="volume">The volume scale for the playback</param>
		/// <param name="onComplete">The callback when the playback finished</param>
		/// <returns>The cloned Audio Source which the Audio System are using</returns>
		public AudioSource Play(AudioClip audioClip, AudioSource templateAudioSource, float volume = 1.0f, Action<AudioSource> onComplete = null)
		{
			// Clone a new channel
			var channel = Instantiate(templateAudioSource, transform);
			clonedChannels.Add(channel);

			channel.clip = audioClip;
			channel.volume = volume;
			if (channel.outputAudioMixerGroup == null)
			{
				channel.outputAudioMixerGroup = defaultMixerGroup;
			}

			// Destroy the cloned channel after the playback
			return Play(channel, callbackChannel =>
			{
				if (onComplete != null)
				{
					onComplete.Invoke(callbackChannel);
				}
				DestroyClonedChannel(callbackChannel);
			});
		}

		/// <summary>
		/// Play a channel and add a callback for it.
		/// </summary>
		/// <param name="channel">Any audio source</param>
		/// <param name="onComplete">The callback when the playback finished</param>
		/// <returns>The audio source</returns>
		public AudioSource Play(AudioSource channel, Action<AudioSource> onComplete = null)
		{
			channel.Play();
			callbacks.Add(channel, onComplete);

			return channel;
		}

		/// <summary>
		/// Stop a sound. Can also remove its callback.
		/// This will destory the cloned Audio Source (created in Play(clip, template))
		/// </summary>
		/// <param name="channel">The target channel</param>
		/// <param name="removeOnCompleteCallback">If true then it will remove the onComplete callback</param>
		/// <returns>False if the channel is invalid (not from this Audio System)</returns>
		public void Stop(AudioSource channel, bool removeOnCompleteCallback = false)
		{
			// Stop it first
			if (channel != null)
			{
				channel.Stop();
				channel.time = 0f;
			}

			// Try to remove from the callback list (null is acceptable)
			if (removeOnCompleteCallback)
			{
				callbacks.Remove(channel);
			}

			// Try to destroy the channel if it was a clone from a template (null is acceptable)
			DestroyClonedChannel(channel);
		}

		/// <summary>
		/// Track a channel and update its volume based on the distance to the audio listener
		/// </summary>
		/// <param name="channel">The target channel</param>
		/// <param name="minVolume">Minimum volume at maximum range</param>
		/// <param name="maxVolume">Maximum volume at minimum range</param>
		/// <param name="minRange">Minimum range for maximum volume</param>
		/// <param name="maxRange">Maximum range for minimum volume</param>
		public void TrackChannelPosition(AudioSource channel, float minVolume, float maxVolume, float minRange, float maxRange)
		{
			if (!trackedChannels.Exists(fader => fader.Channel == channel))
			{
				trackedChannels.Add(new AudioFader(channel, minVolume, maxVolume, minRange, maxRange));
			}
		}

		/// <summary>
		/// Fade an AudioSource volume from current value to target value.
		/// Not necessarily to be the channels only from the Audio System.
		/// </summary>
		/// <param name="channel">Target channel (audio source)</param>
		/// <param name="to">To volume</param>
		/// <param name="duration">Duration of the fading</param>
		/// <param name="onComplete">Callback after the fading completed</param>
		public void FadeTo(AudioSource channel, float to, float duration = 1.0f, Action onComplete = null)
		{
			if (channel == null) return;

			Fade(channel, channel.volume, to, duration, onComplete);
		}

		/// <summary>
		/// Fade an AudioSource volume from a value to another value.
		/// Not necessarily to be the channels only from the Audio System.
		/// </summary>
		/// <param name="channel">Target channel (audio source)</param>
		/// <param name="from">From volume</param>
		/// <param name="to">To volume</param>
		/// <param name="duration">Duration of the fading</param>
		/// <param name="onComplete">Callback after the fading completed</param>
		public void Fade(AudioSource channel, float from, float to, float duration = 1.0f, Action onComplete = null)
		{
			if (channel == null) return;

			audioFaders.Add(new AudioFader(channel, from, to, duration, onComplete));
		}

		/// <summary>
		/// Get a free channel from the pool.
		/// </summary>
		/// <param name="guaranteed">Guaranteed to get one channel even if all channels are occupied.</param>
		/// <returns>The channel. Can be null if it's not guaranteed.</returns>
		public AudioSource GetFreeChannel(bool guaranteed = true)
		{
			ValidateChannels();

			var channel = channels.FirstOrDefault(source => !source.isPlaying);
			if (channel == null)
			{
				Debug.LogWarning("Too many sounds on going -- no available free audio channels.");
				if (guaranteed)
				{
					channel = GetLowestPriorityChannel();
				}
			}
			// There is a chance that the Update() has not yet invoked the callback in a single frame
			else
			{
				var callback = callbacks.Get(channel);
				if (callback != null)
				{
					callback.Invoke(channel);
				}
			}

			return channel;
		}

		/// <summary>
		/// Reset the channel to the default setting
		/// Only works on the children channels
		/// </summary>
		/// <param name="channel">The target channel</param>
		public void ResetChannel(AudioSource channel)
		{
			if (channel == null || !channels.Contains(channel)) return;

			channel.transform.position = Vector3.zero;
			channel.transform.rotation = Quaternion.identity;
			channel.transform.localScale = Vector3.one;
			channel.transform.SetParent(transform);

			channel.volume = 1.0f;
			channel.loop = false;
			channel.pitch = 1.0f;
			channel.spatialBlend = 0f;
		}

		/// <summary>
		/// Guaranteed to get a sound channel even they all occupied.
		/// Perfer lowest priority and non-looped.
		/// Will terminate the audio and do the onComplete callback (if it has one).
		/// </summary>
		/// <returns>A low priority channel.</returns>
		private AudioSource GetLowestPriorityChannel()
		{
			// Find the last channel (most likely the unimprotant one -- PURE GUESS!)
			var lowestPriorityChannel = channels.Last();

			foreach (var channel in channels)
			{
				// If channel is not looped and has lower priority
				if (!channel.loop && channel.priority > lowestPriorityChannel.priority)
				{
					lowestPriorityChannel = channel;
				}
			}

			// Stop (release and callback) the channel
			Stop(lowestPriorityChannel);

			return lowestPriorityChannel;
		}

		private AudioSource CreateChannel(int number)
		{
			var channel = new GameObject("Channel " + number).AddComponent<AudioSource>();
			channel.outputAudioMixerGroup = defaultMixerGroup;
			channel.transform.SetParent(transform);
			channel.playOnAwake = false;

			return channel;
		}

		private void DestroyClonedChannel(AudioSource channel)
		{
			if (clonedChannels.Remove(channel))
			{
				Destroy(channel.gameObject);
			}
		}

		/// <summary>
		/// The user may accidentally deleted our channels
		/// </summary>
		private void ValidateChannels()
		{
			for (var i = 0; i < channels.Count; ++i)
			{
				var channel = channels[i];
				// ReSharper disable once ConvertIfStatementToNullCoalescingExpression
				// Do not use ?? here -- thanks for Unity
				if (channel == null)
				{
					channels[i] = CreateChannel(i);
				}
				else if (!channel.enabled)
				{
					channel.enabled = true;
				}
			}
		}

		private void Update()
		{
			// Update the fading effects
			for (var i = audioFaders.Count - 1; i >= 0; --i)
			{
				// If the result of the update no longer valid then destroy the fader
				if (!audioFaders[i].Update(Time.deltaTime))
				{
					audioFaders.RemoveAt(i);
				}
			}

			// Update the Simple 2D spatial effects
			for (var i = trackedChannels.Count - 1; i >= 0; --i)
			{
				// If the result of the update no longer valid then destroy the fader
				if (!trackedChannels[i].Update(audioListener.transform.position))
				{
					trackedChannels.RemoveAt(i);
				}
			}

			ValidateChannels();

			// Only do the audio.isPlaying check if the application has the focus or allowed to be running in the background
			if (Application.isFocused || Application.runInBackground)
			{
				// Checking callbacks
				callbacks.GetAllAndRemove((channel, callback) => callback(channel), channel => !channel.isPlaying);

				// Cleaning up the clones
				for (var i = clonedChannels.Count - 1; i >= 0; --i)
				{
					var clone = clonedChannels[i];
					if (clone == null)
					{
						clonedChannels.RemoveAt(i);
					}
					else if (!clone.isPlaying)
					{
						clonedChannels.RemoveAt(i);
						Destroy(clone.gameObject);
					}
				}
			}
		}

		private void OnDestroy()
		{
			Instance = null;
		}
	}
}