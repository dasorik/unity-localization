using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ULocalization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ULocalization
{
	public class LocalizationConfig : Config<LocalizationConfig>
	{
		[SerializeField] TextAsset[] translationFiles;
		[SerializeField] bool autoGenerateLanguageClass = true;

		public static bool AutoGenerateLanguageClass => config.autoGenerateLanguageClass;

		public static TextAsset[] GetResourceFiles()
		{
			return config.translationFiles;
		}

#if UNITY_EDITOR
		[MenuItem("Config/Localization")]
		static void OpenLocalizationSettings()
		{
			Selection.activeObject = config;
		}
#endif
	}
}