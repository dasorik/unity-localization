using UnityEngine;
using System.Collections;
using System;
using System.IO;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
#endif

namespace ULocalization
{
	public abstract class Config : ScriptableObject
	{
#if UNITY_EDITOR

		[DidReloadScripts]
		static void ReloadConfig()
		{
			IEnumerable<Type> configTypes = Assembly.GetAssembly(typeof(Config)).GetTypes()
				.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Config)));

			foreach (Type type in configTypes)
				CreateConfigIfRequired(type);
		}

		static void CreateConfigIfRequired(Type configType)
		{
			if (!configType.IsSubclassOf(typeof(Config)))
				throw new Exception($"Passed in a type that was not a config type ({configType.FullName})");
			
			string folderPath = $"Resources/Config";
			string assetPath = Path.Combine(folderPath, $"{configType.FullName}.asset");
			string realFolderPath = Path.Combine(Application.dataPath, folderPath);
			string relativeDataPath = Path.Combine("Assets", assetPath);

			if (AssetDatabase.LoadAssetAtPath(relativeDataPath, configType) == null)
			{
				var asset = (Config)CreateInstance(configType);
				asset.OnCreate();

				if (!Directory.Exists(realFolderPath))
					Directory.CreateDirectory(realFolderPath);

				AssetDatabase.CreateAsset(asset, relativeDataPath);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}

#endif

		protected virtual void OnCreate() { }
	}

	public abstract class Config<T> : Config
		where T : Config<T>
	{

		static T configInstance;
		protected static T config
		{
			get
			{
				if (configInstance == null)
					configInstance = GetConfig();

				return configInstance;
			}
		}

		static T GetConfig()
		{

#if UNITY_EDITOR

			string assetPath = $"Assets/Resources/Config/{typeof(T).FullName}.asset";
			T config = AssetDatabase.LoadAssetAtPath<T>(assetPath);

#else

			string assetPath = $"Config/{typeof(T).FullName}.asset";
			T config = Resources.Load<T>(assetPath);

#endif

			if (config == null)
				throw new Exception($"Unable to find a config file for type {typeof(T).FullName}");

			return config;

		}

	}
}