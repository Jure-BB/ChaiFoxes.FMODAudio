﻿using Microsoft.Xna.Framework;

// DO NOT include FMOD namespace in ANY of your classes.
// Use FMOD.SomeClass instead.
// FMOD classes seriously interfere with System namespace.

namespace ChaiFoxes.FMODAudio
{
	/// <summary>
	/// FMOD sound channel wrapper. Takes horrible FMOD wrapper and makes it look pretty.
	/// Basically, a playing sound instance.
	/// </summary>
	public class SoundChannel
	{
		/// <summary>
		/// FMOD channel object. Use it if you need full FMOD functionality.
		/// </summary>
		public FMOD.Channel Channel => _channel;
		protected FMOD.Channel _channel; // Can't use "out" on properties. 

		/// <summary>
		/// Sound, from which this channel has been created.
		/// </summary>
		public readonly Sound Sound;


		/// <summary>
		/// Tells if channel is looping.
		/// </summary>
		public bool Looping
		{
			get
			{
				_channel.getLoopCount(out int loops);
				return (loops == -1);
			}
			set 
			{
				if (value)
				{
					Loops = -1;
				}
				else
				{
					Loops = 0;
				}
			}
		}
		
		/// <summary>
		/// Amount of loops. 
		/// > 0 - Specific count.
		/// 0 - No loops.
		/// -1 - Infinite loops.
		/// </summary>
		public int Loops
		{
			get
			{
				// Do you have some lööps, bröther?
				_channel.getLoopCount(out int loops);
				return loops;
			}
			set
			{
				if (value == 0)
				{
					Mode = FMOD.MODE.LOOP_OFF;
				}
				else
				{
					Mode = FMOD.MODE.LOOP_NORMAL;
				}

				_channel.setLoopCount(value);
			}
		}

		/// <summary>
		/// Sound pitch. Affects speed too.
		/// 1 - Normal pitch.
		/// More than 1 - Higher pitch.
		/// Less than 1 - Lower pitch.
		/// </summary>
		public float Pitch
		{
			get
			{
				_channel.getPitch(out float pitch);
				return pitch;
			}
			set => 
				_channel.setPitch(value);
		}

		/// <summary>
		/// Sound volume.
		/// 1 - Normal volume.
		/// 0 - Muted.
		/// </summary>
		public float Volume
		{
			get
			{
				_channel.getVolume(out float volume);
				return volume;
			}
			set => 
				_channel.setVolume(value);
		}

		/// <summary>
		/// Low pass filter. Makes sound muffled.
		/// 1 - No filtering.
		/// 0 - Full filtering.
		/// </summary>
		public float LowPass
		{
			get
			{
				_channel.getLowPassGain(out float lowPassGain);
				return lowPassGain;
			}
			set => 
				_channel.setLowPassGain(value);
		}

		/// <summary>
		/// Sound mode. Mainly used for 3D sound.
		/// </summary>
		public FMOD.MODE Mode
		{
			get
			{
				_channel.getMode(out FMOD.MODE mode);
				return mode;
			}
			set =>
				_channel.setMode(value);
		}


		/// <summary>
		/// If true, allows sound to be positioned in 3D space.
		/// </summary>
		public bool Is3D
		{
			get => 
				(Mode & FMOD.MODE._3D) != 0;
			set
			{
				if (value)
				{
					Mode = FMOD.MODE._3D;
				}
				else
				{
					Mode = FMOD.MODE._2D;
				}
			}
		}
		
		/// <summary>
		/// Sound's position in 3D space. Can be used only id 3D positioning is enabled.
		/// </summary>
		public Vector3 Position3D
		{
			get
			{
				_channel.get3DAttributes(out FMOD.VECTOR pos, out FMOD.VECTOR vel);
				return pos.ToVector3();
			}
			set
			{
				var fmodPos = value.ToFmodVector();
				var fmodVel = Velocity3D.ToFmodVector();
				_channel.set3DAttributes(ref fmodPos, ref fmodVel);
			}
		}

		/// <summary>
		/// Sound's velocity in 3D space. Can be used only id 3D positioning is enabled.
		/// </summary>
		public Vector3 Velocity3D
		{
			get
			{
				_channel.get3DAttributes(out FMOD.VECTOR pos, out FMOD.VECTOR vel);
				return vel.ToVector3();
			}
			set
			{
				var fmodPos = Position3D.ToFmodVector();
				var fmodVel = value.ToFmodVector();
				_channel.set3DAttributes(ref fmodPos, ref fmodVel);
			}
		}

		/// <summary>
		/// Distance from the source where attenuation begins.
		/// </summary>
		public float MinDistance3D
		{
			get
			{
				_channel.get3DMinMaxDistance(out float minDistance, out float maxDistance);
				return minDistance;
			}
			set =>
				_channel.set3DMinMaxDistance(value, MaxDistance3D);
		}
		
		/// <summary>
		/// Distance from the source where attenuation ends.
		/// </summary>
		public float MaxDistance3D
		{
			get
			{
				_channel.get3DMinMaxDistance(out float minDistance, out float maxDistance);
				return maxDistance;
			}
			set =>
				_channel.set3DMinMaxDistance(MinDistance3D, value);
		}



		/// <summary>
		/// Tells if sound is playing.
		/// </summary>
		public bool IsPlaying
		{
			get
			{
				_channel.isPlaying(out bool isPlaying);
				return isPlaying;
			}
		}
		
		/// <summary>
		/// Track position in milliseconds.
		/// </summary>
		public uint TrackPosition
		{
			get
			{
				_channel.getPosition(out uint position, FMOD.TIMEUNIT.MS);
				return position;
			}
			set => 
				_channel.setPosition(value, FMOD.TIMEUNIT.MS);
		}



		public SoundChannel(Sound sound, FMOD.Channel channel)
		{
			Sound = sound;
			_channel = channel;

			Loops = Sound.Loops;
			Volume = Sound.Volume;
			Pitch = Sound.Pitch;
			LowPass = sound.LowPass;
			Mode = sound.Mode;
			Is3D = sound.Is3D;
			Position3D = sound.Position3D;
			Velocity3D = sound.Velocity3D;
			MinDistance3D = sound.MinDistance3D;
			MaxDistance3D = sound.MaxDistance3D;
		}

		
		public void Pause() =>
			_channel.setPaused(true);

		public void Resume() =>
			_channel.setPaused(false);

		public void Stop() =>
			_channel.stop();

	}
}
