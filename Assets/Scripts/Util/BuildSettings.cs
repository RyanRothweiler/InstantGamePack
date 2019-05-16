using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSettings
{
	public static string FilePath = "/Resources/BuildSettings.json";
	public static string ResourcesFilePath = "BuildSettings";

	public enum BuildType { Development, Release }
	public BuildType buildType;
}