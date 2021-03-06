﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

// DO NOT include FMOD namespace in ANY of your classes.
// Use FMOD.SomeClass instead.
// FMOD classes seriously interfere with System namespace.

namespace ChaiFoxes.FMODAudio
{

	/// <summary>
	/// Audio manager. Controls main audiosystem parameters.
	/// 
	/// NOTE: My wrappers don't provide full FMOD functionality. For example,
	/// DSPs and advanced 3D stuff are largely left untouched. I may extend my audio
	/// classes to add new features. For now, you have to use FMOD classes directly.
	/// </summary>
	public static partial class AudioMgr
	{
		/// <summary>
		/// Low-level FMOD sound system.
		/// </summary>
		public static FMOD.System FMODSystem;
		

		/// <summary>
		/// Root directory for sounds and music.
		/// </summary>
		private static string _rootDir;
		

		/// <summary>
		/// Initializes FMOD with default parameters. 
		/// 
		/// If you want to use only the default wrapper, call
		/// LoadNativeLibrary() instead.
		/// </summary>
		public static void Init(string rootDir) =>
			Init(rootDir, 256, 4, 32, FMOD.INITFLAGS.CHANNEL_LOWPASS | FMOD.INITFLAGS.CHANNEL_DISTANCEFILTER);
		
		/// <summary>
		/// Initializes FMOD with custom parameters. 
		/// 
		/// If you want to use only the default wrapper, call
		/// LoadNativeLibrary() instead.
		/// </summary>
		public static void Init(
			string rootDir, 
			uint dspBufferLength, 
			int dspBufferCount, 
			int maxChannels,
			FMOD.INITFLAGS initFlags
		)
		{
			_rootDir = rootDir;
			LoadNativeLibrary();		
			
			FMOD.Factory.System_Create(out FMOD.System system);
			FMODSystem = system;

			// Too high values will cause sound lag.
			FMODSystem.setDSPBufferSize(dspBufferLength, dspBufferCount);

			FMODSystem.init(maxChannels, initFlags, (IntPtr)0);
		}
		
		public static void Update() =>
			FMODSystem.update();
		
		public static void Unload() =>
			FMODSystem.release();
		
		
		/// <summary>
		/// Creates new channel group with given name.
		/// </summary>
		public static FMOD.ChannelGroup CreateChannelGroup(string name)
		{
			FMODSystem.createChannelGroup(name, out FMOD.ChannelGroup group);
			return group;
		}


		/// <summary>
		/// Loads sound from file.
		/// Use this function to load short sound effects.
		/// </summary>
		public static Sound LoadSound(string name)
		{
			var buffer = LoadFileAsBuffer(Path.Combine(_rootDir, name));
			
			var info = new FMOD.CREATESOUNDEXINFO();
			info.length = (uint)buffer.Length;
			info.cbsize = Marshal.SizeOf(info);

			FMODSystem.createSound(
				buffer, 
				FMOD.MODE.OPENMEMORY | FMOD.MODE.CREATESAMPLE, 
				ref info,
				out FMOD.Sound newSound
			);

			return new Sound(newSound);
		}
		
		/// <summary>
		/// Loads streamed sound stream from file.
		/// Use this function to load music and long ambience tracks.
		/// </summary>
		public static Sound LoadStreamedSound(string name)
		{
			
			var buffer = LoadFileAsBuffer(Path.Combine(_rootDir, name));

			// Internal FMOD pointer points to this memory, so we don't want it to go anywhere.
			var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

			var info = new FMOD.CREATESOUNDEXINFO();
			info.length = (uint)buffer.Length;
			info.cbsize = Marshal.SizeOf(info);

			FMODSystem.createStream(
				buffer, 
				FMOD.MODE.OPENMEMORY | FMOD.MODE.CREATESTREAM, 
				ref info,
				out FMOD.Sound newSound
			);

			return new Sound(newSound, buffer, handle);
		}
		
	

		/// <summary>
		/// Loads file as a byte array.
		/// </summary>
		private static byte[] LoadFileAsBuffer(string path)
		{
			// TitleContainer is cross-platform Monogame file loader.
			var stream = TitleContainer.OpenStream(path);
			
			// File is opened as a stream, so we need to read it to the end.
			byte[] buffer = new byte[16*1024];
			byte[] bufferRes;
			using (MemoryStream ms = new MemoryStream())
			{
				int read;
        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            ms.Write(buffer, 0, read);
        }
        bufferRes =  ms.ToArray();
			}

			return bufferRes;
		}

	}
}
