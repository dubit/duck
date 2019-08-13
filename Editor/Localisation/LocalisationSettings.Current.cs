using UnityEditor;
using UnityEngine;

namespace DUCK.Localisation.Editor
{
	public partial class LocalisationSettings
	{
		private static LocalisationSettings current;
		public static LocalisationSettings Current
		{
			get
			{
				if (current != null) return current;

				// create or load it...
				var assets = AssetDatabase.FindAssets($"t:{nameof(LocalisationSettings)}");
				if (assets.Length > 0)
				{
					current = AssetDatabase.LoadAssetAtPath<LocalisationSettings>(
						AssetDatabase.GUIDToAssetPath(assets[0]));
				}

				return current;
			}
			set { current = value; }
		}

		public static LocalisationSettings Create()
		{
			current = CreateInstance<LocalisationSettings>();
			ProjectWindowUtil.CreateAsset(current, "Assets/LocalisationSettings.asset");
			AssetDatabase.SaveAssets();
			return current;
		}
	}
}