using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Text.RegularExpressions;

namespace ULocalization
{
	[CustomEditor(typeof(LocalizationConfig))]
	public class LocalizationConfigEditor : Editor
	{
		SerializedProperty p_translationFiles;
		SerializedProperty p_autoGenerateLanguageClass;
		ReorderableList list;

		void OnEnable()
		{
			p_translationFiles = serializedObject.FindProperty("translationFiles");
			p_autoGenerateLanguageClass = serializedObject.FindProperty("autoGenerateLanguageClass");

			list = new ReorderableList(serializedObject, p_translationFiles, true, false, true, true);
			list.drawHeaderCallback += (Rect rect) => { EditorGUI.LabelField(rect, "Translation Files"); };
			list.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) =>
			{
				EditorGUI.PropertyField(new Rect(rect.x, rect.y + 1, rect.width, rect.height - 4),
					p_translationFiles.GetArrayElementAtIndex(index), GUIContent.none);
			};
		}

		void OnDisable()
		{
			TryCreateFile();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			EditorGUILayout.HelpBox("Assign translation files here, provided translations must be in .csv format", MessageType.Info);
			list.DoLayoutList();

			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.HelpBox("If this is enabled, a class will be generated that contains intellisense friendly keys. This will be automatically regenerated if one of the above assets is modified", MessageType.Info);

			EditorGUILayout.PropertyField(p_autoGenerateLanguageClass);

			if (EditorGUI.EndChangeCheck())
				TryCreateFile();

			EditorGUILayout.Space();

			bool wasEnabled = GUI.enabled;
			GUI.enabled = p_autoGenerateLanguageClass.boolValue;

			if (GUILayout.Button("Regenerate Language File"))
				TryCreateFile();

			GUI.enabled = wasEnabled;

			serializedObject.ApplyModifiedProperties();
		}

		void TryCreateFile()
		{
			if (p_autoGenerateLanguageClass.boolValue && p_translationFiles.arraySize > 0)
			{
				LanguageGenerator.ParseAndBuildLanguageClass(LocalizationConfig.GetResourceFiles());
			}
			else
			{
				if (File.Exists(LanguageGenerator.filePath))
					File.Delete(LanguageGenerator.filePath);

				AssetDatabase.Refresh();
			}
		}
	}
}