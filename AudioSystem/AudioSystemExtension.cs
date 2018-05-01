using DUCK.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DUCK.AudioSystem
{
	/// <summary>
	/// Helpers for using the Audio System
	/// You can just do clip.Play(), config.Play(), config.FadeInAndPlay(), etc.
	/// So Fancy!
	/// </summary>
	public static class AudioSystemExtension
	{
		/// <summary>
		/// Play an audio clip
		/// </summary>
		/// <param name="audioClip">The target audio clip</param>
		/// <param name="volume">The volume scale for the channel</param>
		/// <param name="loop">Loop the clip</param>
		/// <param name="onComplete">The callback when the playback finished</param>
		/// <returns>The channel (AudioSource) which the Audio System are using</returns>
		public static AudioSource Play(this AudioClip audioClip, float volume = 1.0f, bool loop = false, Action<AudioSource> onComplete = null)
		{
			return AudioSystem.Instance.Play(audioClip, volume, loop, onComplete);
		}

		/// <summary>
		/// Play an audio source and attach a callback onto it.
		/// </summary>
		/// <param name="audioSource">The target audio source</param>
		/// <param name="onComplete">The callback when the playback finished</param>
		/// <returns>The audio source</returns>
		public static AudioSource Play(this AudioSource audioSource, Action<AudioSource> onComplete)
		{
			return AudioSystem.Instance.Play(audioSource, onComplete);
		}

		/// <summary>
		/// Play an audio clip by using an Audio Config
		/// </summary>
		/// <param name="audioConfig">The target audio config</param>
		/// <param name="onComplete">The callback when the playback finished</param>
		/// <returns>The channel (AudioSource) which the Audio System are using</returns>
		public static AudioSource Play(this AudioConfig audioConfig, Action onComplete = null)
		{
			return audioConfig.Play(audioConfig.VolumeScale, null, onComplete: onComplete);
		}

		/// <summary>
		/// Play an audio clip by using an Audio Config
		/// </summary>
		/// <param name="audioConfig">The target audio config</param>
		/// <param name="parent">The source of the audio</param>
		/// <param name="onComplete">The callback when the playback finished</param>
		/// <returns>The channel (AudioSource) which the Audio System are using</returns>
		public static AudioSource Play(this AudioConfig audioConfig, Transform parent, int clipIndex = -1, Action onComplete = null)
		{
			return audioConfig.Play(audioConfig.VolumeScale, parent, clipIndex, onComplete);
		}

		/// <summary>
		/// Play an audio clip by using an Audio Config but override the volume scale
		/// </summary>
		/// <param name="audioConfig">The target audio config</param>
		/// <param name="volume">The volume scale for the channel (will override the config one)</param>
		/// <param name="parent">The source of the audio</param>
		/// <param name="onComplete">The callback when the playback finished</param>
		/// <returns>The channel (AudioSource) which the Audio System are using</returns>
		public static AudioSource Play(this AudioConfig audioConfig, float volume, Transform parent = null, int clipIndex = -1, Action onComplete = null)
		{
			// Stop the previous playbacks if necessary
			if (audioConfig.CurrentChannel != null)
			{
				switch (audioConfig.PlayMode)
				{
					case AudioConfig.AudioPlayModes.Override:
					{
						// Stop the current one
						audioConfig.Stop();
						break;
					}
					case AudioConfig.AudioPlayModes.OneAtATime:
					{
						// Return the existing one
						return audioConfig.CurrentChannel;
					}
					case AudioConfig.AudioPlayModes.Normal:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			// Play and get the channel. Matching the channel attributes to the config.
			AudioSource targetChannel;
			if (audioConfig.TemplateAudioSource != null)
			{
				targetChannel = AudioSystem.Instance.Play(audioConfig.GetAudioClip(clipIndex),
					audioConfig.TemplateAudioSource, volume, channel => HandleOnComplete(channel, audioConfig));
			}
			else
			{
				targetChannel = AudioSystem.Instance.GetFreeChannel();
				targetChannel.clip = audioConfig.GetAudioClip(clipIndex);
				targetChannel.volume = volume;
				targetChannel.loop = audioConfig.Loop;

				// Override the settings
				targetChannel.priority = audioConfig.Priority;
				switch (audioConfig.SpatialMode)
				{
					case AudioConfig.SpatialModes.Pure2D:
						break;
					case AudioConfig.SpatialModes.Simple2D:
						if (parent == null)
						{
							Debug.LogWarning(audioConfig.name + ": should attach to a transform for Simple2D spatial playback!");
						}
						AudioSystem.Instance.TrackChannelPosition(targetChannel, 0f, audioConfig.VolumeScale,
							audioConfig.MinDistance, audioConfig.MaxDistance);
						break;
					case AudioConfig.SpatialModes.Simple3D:
						if (parent == null)
						{
							Debug.LogWarning(audioConfig.name + ": should attach to a transform for Simple3D spatial playback!");
						}
						targetChannel.minDistance = audioConfig.MinDistance;
						targetChannel.maxDistance = audioConfig.MaxDistance;
						targetChannel.spatialBlend = 1.0f;
						targetChannel.rolloffMode = AudioRolloffMode.Linear;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				// Play and add the callback
				AudioSystem.Instance.Play(targetChannel, channel => HandleOnComplete(channel, audioConfig));
			}

			// Set the source of the audio
			if (parent != null)
			{
				targetChannel.transform.SetParent(parent, false);
			}

			// Add the reference
			audioConfig.AddOccupiedChannel(targetChannel);

			// Add the callback to the dictionary
			audioConfig.Callbacks.Add(targetChannel, onComplete);

			return targetChannel;
		}

		/// <summary>
		/// Play a sequence of audio clips with optional delay between them
		/// </summary>
		/// <param name="compositeAudioConfig">Enumerable collection of AudioConfigs</param>
		/// <param name="volume">The volume scale for the channel (will override the config one)</param>
		/// <param name="parent">The source of the audio</param>
		/// <param name="getClipId">Function for deciding what clip sub-index to play for each config in the sequence</param>
		/// <param name="getClipId">Function for deciding delay to play for each config in the sequence with</param>
		/// <param name="onComplete">The callback when the playback of all configs finished</param>
		/// <param name="onEachConfigPlayed">The callback when the playback of each config starts</param>
		/// <param name="onTimerStarted">The callback called when a delay timer is created</param>
		/// <returns>The channel (AudioSource) which the Audio System is using for the first clip in the sequence</returns>
		public static AudioConfig PlaySequence(this AudioConfig[] audioConfigs, float volume = 1f, Transform parent = null, 
			Func<int, int> getClipId = null, Func<int, float> getClipDelay = null, 
			Action onComplete = null, Action<AudioConfig, int> onEachConfigPlayed = null, Action<Timer> onTimerStarted = null)
		{
			return PlaySequenceAtIndex(audioConfigs, 0, volume, parent, getClipId, getClipDelay, 
				onComplete, onEachConfigPlayed, onTimerStarted);
		}

		public static AudioConfig PlaySequence(this AudioConfigSequence compositeConfig, float volume = 1f, Transform parent = null, 
			Action onComplete = null, Action<AudioConfig, int> onEachConfigPlayed = null, Action<Timer> onTimerStarted = null)
		{
			return PlaySequence(compositeConfig.AudioConfigs, volume, parent, compositeConfig.GetClipIndex, compositeConfig.GetClipDelay, 
				onComplete, onEachConfigPlayed, onTimerStarted);
		}

		public static AudioConfig PlaySequence(this AudioConfigSequence compositeConfig, Action onComplete = null)
		{
			return PlaySequence(compositeConfig, onComplete: onComplete);
		}

		private static AudioConfig PlaySequenceAtIndex(AudioConfig[] configs, int index, float volume, Transform parent = null, 
			Func<int, int> getClipId = null, Func<int, float> getClipDelay = null, 
			Action onComplete = null, Action<AudioConfig, int> onEachConfigPlayed = null, Action<Timer> onTimerStarted = null)
		{
			Func<AudioConfig> playThisConfig = () =>
			{
				if (index >= configs.Length)
				{
					onComplete.SafeInvoke();
					return null;
				}

				var thisConfig = configs[index];

				onEachConfigPlayed.SafeInvoke(thisConfig, index);

				thisConfig.Play(volume, parent, (getClipId != null) ? getClipId(index) : AudioConfig.RANDOM_CLIP, () =>
				{
					PlaySequenceAtIndex(configs, index + 1, volume, parent, getClipId, getClipDelay, onComplete, onEachConfigPlayed);
				});

				return thisConfig;
			};

			var delay = (getClipDelay != null) ? getClipDelay(index) : 0f;

			if (index == 0 || delay <= 0f)
			{
				return playThisConfig();
			}
			else
			{
				var timer = Timer.SetTimeout(delay, () => playThisConfig());
				onTimerStarted.SafeInvoke(timer);
				return null;
			}
		}

		private static void HandleOnComplete(AudioSource channel, AudioConfig audioConfig)
		{
			// Remove the reference
			audioConfig.RemoveOccupiedChannel(channel);

			// Check the callbacks
			var callback = audioConfig.Callbacks.GetAndRemove(channel);
			if (callback != null)
			{
				callback.Invoke();
			}

			// Reset the channel
			AudioSystem.Instance.ResetChannel(channel);
		}

		/// <summary>
		/// Stop the audio and can also remove the callback if it has one.
		/// </summary>
		/// <param name="channel">The target channel</param>
		/// <param name="removeOnCompleteCallback">Remove the callback</param>
		public static void Stop(this AudioSource channel, bool removeOnCompleteCallback)
		{
			AudioSystem.Instance.Stop(channel, removeOnCompleteCallback);
		}

		/// <summary>
		/// Stop the audio
		/// </summary>
		/// <param name="audioConfig">The target audio config</param>
		/// <param name="parent">The target parent transform</param>
		/// <param name="removeOnCompleteCallback">Remove the callback</param>
		/// <returns>False if the channel is invalid (not from this Audio System) or already stopped</returns>
		public static void Stop(this AudioConfig audioConfig, Transform parent = null, bool removeOnCompleteCallback = false)
		{
			var channels = audioConfig.GetChannels(parent);
			// Use for loop here because the collection will be modified during the iteration
			for (var i = channels.Count - 1; i >= 0; --i)
			{
				var channel = channels[i];
				AudioSystem.Instance.Stop(channel, true);
				audioConfig.RemoveOccupiedChannel(channel);

				var callback = audioConfig.Callbacks.GetAndRemove(channel);
				if (callback != null && !removeOnCompleteCallback)
				{
					callback.Invoke();
				}
			}
		}

		/// <summary>
		/// Fade in and play the audio (from an audio config).
		/// </summary>
		/// <param name="audioConfig">The target audio config</param>
		/// <param name="duration">Duration of the fade in phase</param>
		/// <param name="parent">The source of the audio</param>
		/// <param name="onComplete">Callback after the fade in completed</param>
		/// <returns></returns>
		public static AudioSource FadeInAndPlay(this AudioConfig audioConfig, float duration = 1.0f, Transform parent = null, Action onComplete = null)
		{
			var channel = audioConfig.Play(0f, parent);
			AudioSystem.Instance.FadeTo(channel, audioConfig.VolumeScale, duration, onComplete);
			return channel;
		}

		/// <summary>
		/// Fade out channels in the audio config. Can also stop the playback after the fade out.
		/// </summary>
		/// <param name="audioConfig">The target audio config</param>
		/// <param name="duration">Duration of the fade in phase</param>
		/// <param name="autoStop">Auto stop the audio after the playback</param>
		/// <param name="parent">The target parent transform</param>
		/// <param name="onComplete">Callback after the fade out completed</param>
		public static void FadeOut(this AudioConfig audioConfig, float duration = 1.0f, bool autoStop = true, Transform parent = null, Action onComplete = null)
		{
			var channels = audioConfig.GetChannels(parent);
			foreach (var channel in channels)
			{
				FadeOut(channel, duration, autoStop, onComplete);
			}
		}

		/// <summary>
		/// Fade out the target audio source. Can also stop the playback after the fade out.
		/// </summary>
		/// <param name="audioSource">The target audio source</param>
		/// <param name="duration">Duration of the fade in phase</param>
		/// <param name="autoStop">Auto stop the audio after the playback</param>
		/// <param name="onComplete">Callback after the fade out completed</param>
		public static void FadeOut(this AudioSource audioSource, float duration = 1.0f, bool autoStop = true, Action onComplete = null)
		{
			var targetChannel = audioSource;
			AudioSystem.Instance.FadeTo(audioSource, 0f, duration, () =>
			{
				if (autoStop && targetChannel == audioSource) AudioSystem.Instance.Stop(targetChannel);
				if (onComplete != null) onComplete.Invoke();
			});
		}

		/// <summary>
		/// Fade channels in the audio config from a volume to a volume.
		/// </summary>
		/// <param name="audioConfig">The target audio config</param>
		/// <param name="from">From volume</param>
		/// <param name="to">To volume</param>
		/// <param name="duration">Duration of the fade phase</param>
		/// <param name="parent">The target parent transform</param>
		/// <param name="onComplete">Callback after the fade completed</param>
		public static void Fade(this AudioConfig audioConfig, float from, float to, float duration = 1.0f, Transform parent = null, Action onComplete = null)
		{
			var channels = audioConfig.GetChannels(parent);
			foreach (var channel in channels)
			{
				AudioSystem.Instance.Fade(channel, from, to, duration, onComplete);
			}
		}

		/// <summary>
		/// Fade the target audio source from a volume to a volume.
		/// </summary>
		/// <param name="audioSource">The target audio source</param>
		/// <param name="from">From volume</param>
		/// <param name="to">To volume</param>
		/// <param name="duration">Duration of the fade phase</param>
		/// <param name="onComplete">Callback after the fade completed</param>
		public static void Fade(this AudioSource audioSource, float from, float to, float duration = 1.0f, Action onComplete = null)
		{
			AudioSystem.Instance.Fade(audioSource, from, to, duration, onComplete);
		}

		/// <summary>
		/// Fade channels in the audio config from current volume to a volume.
		/// </summary>
		/// <param name="audioConfig">The target audio config</param>
		/// <param name="volume">To volume</param>
		/// <param name="duration">Duration of the fade phase</param>
		/// <param name="parent">The target parent transform</param>
		/// <param name="onComplete">Callback after the fade completed</param>
		public static void FadeTo(this AudioConfig audioConfig, float volume, float duration = 1.0f, Transform parent = null, Action onComplete = null)
		{
			var channels = audioConfig.GetChannels(parent);
			foreach (var channel in channels)
			{
				AudioSystem.Instance.FadeTo(channel, volume, duration, onComplete);
			}
		}

		/// <summary>
		/// Fade the target audio source from current volume to a volume.
		/// </summary>
		/// <param name="audioSource">The target audio source</param>
		/// <param name="volume">To volume</param>
		/// <param name="duration">Duration of the fade phase</param>
		/// <param name="onComplete">Callback after the fade completed</param>
		public static void FadeTo(this AudioSource audioSource, float volume, float duration = 1.0f, Action onComplete = null)
		{
			AudioSystem.Instance.FadeTo(audioSource, volume, duration, onComplete);
		}
	}
}