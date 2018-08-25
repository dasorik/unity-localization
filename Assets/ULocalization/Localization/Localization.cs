using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ULocalization
{
	public static class Localization
	{
		public class NoKeyException : Exception
		{
			public NoKeyException(string message) : base(message) { }
		}

		class Translations
		{
			Dictionary<string, string> translations = new Dictionary<string, string>();

			public void Add(string languageCode, string value)
			{
				translations.Add(languageCode, value);
			}

			public string this[string baseLanguageCode, string preciseLanguageCode]
			{
				get
				{
					if (preciseLanguageCode != null)
					{
						if (translations.ContainsKey(preciseLanguageCode))
							return translations[baseLanguageCode];
					}

					if (translations.ContainsKey(baseLanguageCode))
						return translations[baseLanguageCode];

					throw new System.Exception($"Attempted to get a translation for {(preciseLanguageCode == null ? baseLanguageCode : $"{preciseLanguageCode} and {baseLanguageCode}")}, but it was not present in the list of translations");
				}
			}
		}

		static string baseLanguageCode;
		static string preciseLanguageCode;

		static Dictionary<string, Translations> keyDatabase;
		static bool initialized;

		static Localization()
		{
			Initialize();
		}

		[RuntimeInitializeOnLoadMethod]
		static void Initialize()
		{
			if (initialized)
				return;

			keyDatabase = new Dictionary<string, Translations>();

			foreach (TextAsset languageFile in LocalizationConfig.GetResourceFiles())
			{
				if (languageFile == null)
					continue;

				string[,] table = CSVParser.ReadCSV(languageFile, ',', '\"');
				int rows = table.GetLength(0);
				int columns = table.GetLength(0);

				string[] headers = new string[columns - 1];

				for (int i = 1; i < columns; i++)
					headers[i - 1] = table[0, i];
				
				for (int i = 1; i < rows; i++)
				{
					Translations translations = new Translations();

					for (int j = 0; j < headers.Length; j++)
					{
						string translation = table[i, j + 1];
						IEnumerable<string> formatParameters;

						if (translation.HasFormatParameters(out formatParameters))
						{
							int counter = 0;
							foreach (string parameter in formatParameters)
								translation = translation.Replace($"{{{parameter}}}", $"{{{counter++}}}");
						}

						translations.Add(headers[j], translation);
					}

					if (keyDatabase.ContainsKey(table[i, 0]))
						throw new Exception($"Duplicate key, '{table[i, 0]}' has already been added to the key database");

					keyDatabase.Add(table[i, 0], translations);
				}

				initialized = true;
			}
		}

		/// <summary>
		/// Translate a localstring to the current selected language and culture
		/// </summary>
		/// <param name="localstring">The localstring to translate</param>
		/// <returns>The localstring translated into the current selected language and culuture (if possible)</returns>
		public static string Translate(localstring localstring)
		{
			object[] args = new object[localstring.Count()];

			int counter = 0;
			foreach (object value in localstring)
			{
				if (value is localstring)
					args[counter++] = Translate(value as localstring);
				else if (value != null)
					args[counter++] = value.ToString();
				else
					args[counter++] = string.Empty;
			}

			if (!keyDatabase.ContainsKey(localstring.key))
				throw new NoKeyException($"Unable to find key '{localstring.key}' in the list of stored translations");

			return string.Format(keyDatabase[localstring.key][baseLanguageCode, preciseLanguageCode], args);
		}

		/// <summary>
		/// Set the current locale
		/// </summary>
		/// <param name="languageCode">The language code for the current locale in the in the format 'xx' (ie. 'en')</param>
		/// <param name="countryCode">The country code for the current locale in the in the format 'XX' (ie. 'US')</param>
		public static void SetLanguage(string languageCode, string countryCode)
		{
			if (!Regex.IsMatch(languageCode, @"[a-z]{2}"))
				throw new System.Exception($"Invalid language code provided ({languageCode}). Please ensure this is in the format 'xx'");

			if (!Regex.IsMatch(countryCode, @"[A-Z]{2}"))
				throw new System.Exception($"Invalid country code provided ({countryCode}). Please ensure this is in the format 'XX'");

			Localization.baseLanguageCode = languageCode;
			Localization.preciseLanguageCode = $"{languageCode}-{countryCode}";
		}

		/// <summary>
		/// Set the current locale. Please ensure this is in the format 'xx', or 'xx-XX' (ie. 'en'/'en-US')
		/// </summary>
		/// <param name="isoCode">The iso code for the current locale in the in the format 'xx', or 'xx-XX' (ie. 'en'/'en-US')</param>
		public static void SetLanguage(string isoCode)
		{
			if (!Regex.IsMatch(isoCode, @"[a-z]{2}(-[A-Z]{2})?"))
				throw new System.Exception($"Invalid iso code provided ({isoCode}). Please ensure this is in the format 'xx', or 'xx-XX'");

			baseLanguageCode = Regex.Match(isoCode, @"[a-z]{2}").Value;
			preciseLanguageCode = isoCode;
		}

		/// <summary>
		/// Sets the current locale based on Unity's SystemLanguage enum (not precise)
		/// </summary>
		/// <param name="language">The system language to use</param>
		public static void SetLanguage(SystemLanguage language)
		{
			switch (language)
			{
				case SystemLanguage.Afrikaans:
					baseLanguageCode = "af"; break;
				case SystemLanguage.Arabic:
					baseLanguageCode = "ar"; break;
				case SystemLanguage.Basque:
					baseLanguageCode = "eu"; break;
				case SystemLanguage.Belarusian:
					baseLanguageCode = "be"; break;
				case SystemLanguage.Bulgarian:
					baseLanguageCode = "bg"; break;
				case SystemLanguage.Catalan:
					baseLanguageCode = "ca"; break;
				case SystemLanguage.Chinese:
				case SystemLanguage.ChineseSimplified:
				case SystemLanguage.ChineseTraditional:
					baseLanguageCode = "zh"; break;
				case SystemLanguage.Czech:
					baseLanguageCode = "cs"; break;
				case SystemLanguage.Danish:
					baseLanguageCode = "da"; break;
				case SystemLanguage.Dutch:
					baseLanguageCode = "nl"; break;
				case SystemLanguage.English:
					baseLanguageCode = "en"; break;
				case SystemLanguage.Estonian:
					baseLanguageCode = "et"; break;
				case SystemLanguage.Faroese:
					baseLanguageCode = "fo"; break;
				case SystemLanguage.Finnish:
					baseLanguageCode = "fi"; break;
				case SystemLanguage.French:
					baseLanguageCode = "fr"; break;
				case SystemLanguage.German:
					baseLanguageCode = "de"; break;
				case SystemLanguage.Greek:
					baseLanguageCode = "el"; break;
				case SystemLanguage.Hebrew:
					baseLanguageCode = "he"; break;
				case SystemLanguage.Hungarian:
					baseLanguageCode = "hu"; break;
				case SystemLanguage.Icelandic:
					baseLanguageCode = "is"; break;
				case SystemLanguage.Indonesian:
					baseLanguageCode = "id"; break;
				case SystemLanguage.Italian:
					baseLanguageCode = "it"; break;
				case SystemLanguage.Japanese:
					baseLanguageCode = "ja"; break;
				case SystemLanguage.Korean:
					baseLanguageCode = "ko"; break;
				case SystemLanguage.Latvian:
					baseLanguageCode = "lv"; break;
				case SystemLanguage.Lithuanian:
					baseLanguageCode = "lt"; break;
				case SystemLanguage.Norwegian:
					baseLanguageCode = "no"; break;
				case SystemLanguage.Polish:
					baseLanguageCode = "pl"; break;
				case SystemLanguage.Portuguese:
					baseLanguageCode = "pt"; break;
				case SystemLanguage.Romanian:
					baseLanguageCode = "ro"; break;
				case SystemLanguage.Russian:
					baseLanguageCode = "ru"; break;
				case SystemLanguage.SerboCroatian:
					baseLanguageCode = "hr"; break; // ???
				case SystemLanguage.Slovak:
					baseLanguageCode = "sk"; break;
				case SystemLanguage.Slovenian:
					baseLanguageCode = "sl"; break;
				case SystemLanguage.Spanish:
					baseLanguageCode = "es"; break;
				case SystemLanguage.Swedish:
					baseLanguageCode = "sv"; break;
				case SystemLanguage.Thai:
					baseLanguageCode = "th"; break;
				case SystemLanguage.Turkish:
					baseLanguageCode = "tr"; break;
				case SystemLanguage.Ukrainian:
					baseLanguageCode = "uk"; break;
				case SystemLanguage.Vietnamese:
					baseLanguageCode = "vi"; break;
				case SystemLanguage.Unknown:
					baseLanguageCode = "en"; break;
				default:
					throw new Exception($"Provided an invalid SystemLanguage: {language}");
			}

			preciseLanguageCode = null; // System language can't be precise
		}
	}
}