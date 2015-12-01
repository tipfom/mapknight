using System;

namespace XML
{
	public struct Version
	{
        public static bool operator <(Version version1, Version version2)
        {
            if (version1.Major < version2.Major || (version1.Major == version2.Major && version1.Minor < version2.Minor) || (version1.Major == version2.Major && version1.Minor == version2.Minor && version1.Build < version2.Build))
            {
                return true;
            }
            return false;
        }

        public static bool operator >(Version version1, Version version2)
        {
            if (version1.Major > version2.Major || (version1.Major == version2.Major && version1.Minor > version2.Minor) || (version1.Major == version2.Major && version1.Minor == version2.Minor && version1.Build > version2.Build))
            {
                return true;
            }
            return false;
        }

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

				BuildDate = new DateTime (2000, 1, 1, 0, 0, 0).AddDays (Build).AddSeconds (Revision * 2).ToUniversalTime ();
			}
		}

		public override string ToString ()
		{
			return Major.ToString () + "." + Minor.ToString () + "." + Build.ToString () + "-" + Revision.ToString ();
		}
	}
}

