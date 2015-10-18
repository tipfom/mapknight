using System;

namespace mapKnight_Android{
	public enum TexCorner : byte{
		TopLeft = 0x0,
		TopRight = 0x1,
		BottomLeft = 0x2,
		BottomRight = 0x3
	}

	public enum MessageType{
		Debug = 0,
		Info = 1,
		Warn = 2,
		Error = 3,
		WTF = 1337
	}
}