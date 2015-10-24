using System;

namespace mapKnight_Android
{
	public struct Version
	{
		public DateTime BuildDate;

		public int Major;
		public int Minor;
		public int Build;
		public int Revision;

		public Version (string version) : this ()
		{
			string[] versionparts = version.Split (new char[]{ '.' }, StringSplitOptions.RemoveEmptyEntries);
			if (versionparts.Length == 4) {	
				int.TryParse (versionparts [0], out Major);
				int.TryParse (versionparts [1], out Minor);
				int.TryParse (versionparts [2], out Build);
				int.TryParse (versionparts [3], out Revision);

				BuildDate = new DateTime (2000, 1, 1, 0, 0, 0).AddDays (Build).AddSeconds (Revision * 2).AddHours (1);
			}
		}

		public override string ToString ()
		{
			return Major.ToString () + "." + Minor.ToString () + "." + Build.ToString () + "-" + Revision.ToString () + " updated " + BuildDate.ToString ("dd/MM/yyyy HH:mm:ss");
		}
	}
}

