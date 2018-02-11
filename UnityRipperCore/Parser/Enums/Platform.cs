namespace UnityRipper
{
	public enum Platform : uint
	{
		ValidPlayer				= 1,
		StandaloneOSXUniversal	= 2,
		StandaloneOSXPPC		= 3,
		StandaloneOSXIntel		= 4,
		StandaloneWinPlayer		= 5,
		WebPlayerLZMA			= 6,
		WebPlayerLZMAStreamed	= 7,
		iOS						= 9,
		PS3						= 10,
		XBox360					= 11,
		Android					= 13,
		WinGLESEmu				= 14,
		GoogleNaCl				= 16,
		StandaloneLinux			= 17,
		Flash					= 18,
		StandaloneWin64Player	= 19,
		WebGL					= 20,
		MetroPlayerX86			= 21,
		MetroPlayerX64			= 22,
#warning TODO:
		MetroPlayerARM			= 23,
		Linux					= 25,
		WiiU					= 29,

		UnityPackage	= 0xFFFFFFFE,
	}
}
