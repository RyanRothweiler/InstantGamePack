using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Build.Reporting;


public static class EditorUtil
{
	[MenuItem("Util/ClearPlayerPrefs")]
	static void WriteVersionTest()
	{
		PlayerPrefs.DeleteAll();
	}
}