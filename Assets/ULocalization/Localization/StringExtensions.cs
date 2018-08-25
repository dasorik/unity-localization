#if UNITY_EDITOR
using Microsoft.CSharp;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ULocalization
{
	public static class StringExtensions
	{
#if UNITY_EDITOR
		static CSharpCodeProvider keywordChecker = new CSharpCodeProvider();
#endif

		const string MATCH_PATTERN = @"(?<=\{)[a-zA-z][a-zA-Z0-9]*?(?=\})";

		public static bool HasFormatParameters(this string translation, out IEnumerable<string> parameters)
		{
			var matches = Regex.Matches(translation, MATCH_PATTERN);
			parameters = matches.Cast<Match>().Select(m => m.Value).Distinct();
			return matches.Count > 0;
		}

#if UNITY_EDITOR
		public static bool HasFormatParametersEscape(this string translation, out IEnumerable<string> parameters)
		{
			var matches = Regex.Matches(translation, MATCH_PATTERN);
			parameters = matches.Cast<Match>().Select(m => keywordChecker.IsValidIdentifier(m.Value) ? m.Value : $"@{m.Value}").Distinct();
			return matches.Count > 0;
		}
#endif
	}
}