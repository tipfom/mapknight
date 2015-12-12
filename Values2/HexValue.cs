using System;
using System.Collections.Generic;

namespace mapKnight.Values
{
	public struct HInt
	{
		public int Value{ get; private set; }

		public string Code{ get; private set; }

		public HInt (string code) : this ()
		{
			Code = code;

			code = code.Replace ("#", "");
			char[] hexchars = code.ToCharArray ();
			Array.Reverse (hexchars);
			for (int i = 0; i < hexchars.Length; i++) {
				Value += HexNumDecNumIndex [hexchars [i]] * (int)Math.Pow ((double)16, (double)i);
			}
		}

		public HInt (uint value) : this ()
		{
			Value = (int)value;

			int lastvalue = Value;
			string code = "";
			while (lastvalue > 16) {
				code += DecNumHexNumIndex [lastvalue % 16];
				lastvalue = (int)(lastvalue / 16);
			}
			code += DecNumHexNumIndex [lastvalue];
			char[] hexchars = code.ToCharArray ();
			Array.Reverse (hexchars);
			Code = new string (hexchars);
		}

		private static Dictionary<char, int> HexNumDecNumIndex = new Dictionary<char, int> () {
			{ '0',0 },
			{ '1',1 },
			{ '2',2 },
			{ '3',3 },
			{ '4',4 },
			{ '5',5 },
			{ '6',6 },
			{ '7',7 },
			{ '8',8 },
			{ '9',9 },
			{ 'a',10 },
			{ 'A',10 },
			{ 'b',11 },
			{ 'B',11 },
			{ 'c',12 },
			{ 'C',12 },
			{ 'd',13 },
			{ 'D',13 },
			{ 'e',14 },
			{ 'E',14 },
			{ 'f',15 },
			{ 'F',15 }
		};

		private static Dictionary<int, char> DecNumHexNumIndex = new Dictionary<int, char> () { 
			{ 0, '0' },
			{ 1,'1' },
			{ 2,'2' },
			{ 3,'3' },
			{ 4,'4' },
			{ 5,'5' },
			{ 6,'6' },
			{ 7,'7' },
			{ 8,'8' },
			{ 9,'9' },
			{ 10,'A' },
			{ 11,'B' },
			{ 12,'C' },
			{ 13,'D' },
			{ 14,'E' },
			{ 15,'F' }
		};
	}
}