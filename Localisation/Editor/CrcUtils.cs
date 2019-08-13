namespace DUCK.Localisation.Editor
{
	public static class CrcUtils
	{
		/// <summary>
		/// The 'encoding version' used to generate int hash values from string-based keys in GetCRC. If this changes,
		/// the value here needs to be updated, as the keys in the localisation tables will no longer be valid.
		/// </summary>
		public const int KEY_CRC_ENCODING_VERSION = 1;

		public static int GetCrc(string category, string key)
		{
			return GetCrcWithEncodingVersion(category, key, KEY_CRC_ENCODING_VERSION);
		}

		public static int GetCrcWithEncodingVersion(string category, string key, int encodingVersion)
		{
			switch (encodingVersion)
			{
				case 0:
					return key.GetHashCode();
				//case 1: Latest - matches KeyCRCEncodingVersion
				default:
					return (category + "/" + key).GetHashCode();
			}
		}
	}
}