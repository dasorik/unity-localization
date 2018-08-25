using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ULocalization
{
	public class CSVParser
	{
		public static string[,] ReadCSV(TextAsset file, char seperator = ',', char textWrapper = '\"')
		{
			string[] lines = file.text.Split('\n').Where(l => !string.IsNullOrEmpty(l)).ToArray();

			int columnCount = int.MaxValue;
			string[,] table = null;
			List<string> tempList = new List<string>();

			for (int i = 0; i < lines.Length; i++)
			{
				ParseLine(lines[i], seperator, textWrapper, tempList);

				if (i == 0)
				{
					columnCount = tempList.Count;
					table = new string[lines.Length, columnCount];
				}

				if (tempList.Count != columnCount)
					throw new System.Exception($"An error occured during parsing, column count of {tempList.Count} did not equal the number of columns in the first row ({columnCount})");

				for (int j = 0; j < columnCount; j++)
					table[i, j] = tempList[j];
			}

			return table;
		}

		static void ParseLine(string line, char seperator, char textWrapper, List<string> tempList)
		{
			string token;
			int charIndex = 0;

			tempList.Clear();

			while (ParseToken(line, seperator, textWrapper, ref charIndex, out token))
				tempList.Add(token);
		}

		static bool ParseToken(string line, char seperator, char textWrapper, ref int charIndex, out string token)
		{
			if (charIndex >= line.Length)
			{
				token = null;
				return false;
			}

			int startingChar = charIndex;
			bool insideComment = false;
			bool previousWasTextEnd = false;

			char firstChar = line[charIndex++];

			if (firstChar == textWrapper)
			{
				insideComment = true;
			}
			else if (firstChar == seperator)
			{
				token = string.Empty;
				return true;
			}

			while (charIndex < line.Length)
			{
				char currentChar = line[charIndex++];

				if (currentChar == seperator && (!insideComment || previousWasTextEnd))
					break;

				previousWasTextEnd = currentChar == textWrapper;
			}

			if (previousWasTextEnd)
				token = line.Substring(startingChar + 1, charIndex - startingChar - 3);
			else
				token = line.Substring(startingChar, charIndex - startingChar - 1);

			return true;
		}
	}
}