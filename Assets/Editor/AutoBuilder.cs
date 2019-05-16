using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Build.Reporting;

public static class AutoBuilder
{
	public class Version
	{
		public int major;
		public int minor;
		public int build;

		// Used for android
		public int bundleVersionCode;

		public Version () { }

		public Version (int major, int minor)
		{
			this.major = major;
			this.minor = minor;
			this.build = 0;
			this.bundleVersionCode = 0;
		}

		public string ToString()
		{
			return (string.Format("{0}.{1}.{2}_{3}", major, minor, build, bundleVersionCode));
		}
	}

	public static string VersionFilePath = "/Resources/Version.json";

	static string[] GetScenePaths()
	{
		string[] scenes = new string[EditorBuildSettings.scenes.Length];

		for (int i = 0; i < scenes.Length; i++) {
			scenes[i] = EditorBuildSettings.scenes[i].path;
		}

		return scenes;
	}

	static void UpdateAndSetVersionInfo(BuildSettings.BuildType type)
	{
		// Update versions
		{
			Version newVer = GetVersion();
			newVer.build++;
			newVer.bundleVersionCode++;
			WriteVersion(newVer);

			PlayerSettings.bundleVersion = newVer.ToString();
			PlayerSettings.Android.bundleVersionCode = newVer.bundleVersionCode;
		}

		// Write build settings
		{
			BuildSettings settings = new BuildSettings();
			settings.buildType = type;
			string jsonString = JsonUtility.ToJson(settings);
			System.IO.File.WriteAllText(Application.dataPath + BuildSettings.FilePath, jsonString);
		}
	}

	[MenuItem("Build/Android/Development")] 	static void AndroidDevelopmentBuild() 	{ PerformAndroidBuild(BuildSettings.BuildType.Development); }
	[MenuItem("Build/Android/Release")] 		static void AndroidReleaseBuild() 		{ PerformAndroidBuild(BuildSettings.BuildType.Release); }
	static void PerformAndroidBuild (BuildSettings.BuildType type)
	{
		UpdateAndSetVersionInfo(type);

		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
		BuildReport report = BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/Android/" + Application.productName + ".apk", BuildTarget.Android, BuildOptions.AutoRunPlayer);
		if (report.summary.result == BuildResult.Succeeded) {
			Debug.Log("Build succeeded.");
		} else {
			Debug.Log("Build failed.");
		}
	}

	static Version GetVersion()
	{
		string fullPath = Application.dataPath + VersionFilePath;
		if (System.IO.File.Exists(fullPath)) {
			string fileText = System.IO.File.ReadAllText(fullPath);
			Version ver = JsonUtility.FromJson<Version>(fileText);
			return (ver);
		} else {
			Debug.Log("No version file found in resources at " + fullPath);
		}

		return (new Version(2019, 1));
	}

	[MenuItem("Build/CreateVersionFile")]
	static void WriteVersionTest()
	{
		WriteVersion(new Version(2019, 1));
	}
	static void WriteVersion(Version newVersion)
	{
		string jsonString = JsonUtility.ToJson(newVersion);
		System.IO.File.WriteAllText(Application.dataPath + VersionFilePath, jsonString);
	}

	[MenuItem("Build/SetBuildType/Development")]
	static void SetDevelopmentBuild()
	{
		BuildSettings settings = new BuildSettings();
		settings.buildType = BuildSettings.BuildType.Development;
		string jsonString = JsonUtility.ToJson(settings);
		System.IO.File.WriteAllText(Application.dataPath + BuildSettings.FilePath, jsonString);
	}

	[MenuItem("Build/SetBuildType/Release")]
	static void SetReleaseBuild()
	{
		BuildSettings settings = new BuildSettings();
		settings.buildType = BuildSettings.BuildType.Release;
		string jsonString = JsonUtility.ToJson(settings);
		System.IO.File.WriteAllText(Application.dataPath + BuildSettings.FilePath, jsonString);
	}

}
