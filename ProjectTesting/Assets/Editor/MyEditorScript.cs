﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

public class MyEditorScript : MonoBehaviour 
{

	static string[] SCENES = FindEnabledEditorScenes();

	static string APP_NAME = "Testeo";
	static string TARGET_DIR = "C:/Main/Builds/Builds Windows/Testeo Widow Build";
	static string TARGET_DIR_ANDROID = "C:/Main/Repositories/TesteoRepo";

	[MenuItem ("Custom/CI/Build Widows")]
	static void PerformWindowsBuild ()
	{
		 string target_dir = APP_NAME + ".exe";
		 GenericBuild(SCENES, TARGET_DIR + "/" + target_dir, BuildTarget.StandaloneWindows, BuildOptions.None);
	}

	[MenuItem ("Custom/CI/Build Android")]
	static void PerformAndroidBuild ()
	{
		 string target_dir = APP_NAME + ".apk";
		 GenericBuild(SCENES, TARGET_DIR_ANDROID + "/" + target_dir, BuildTarget.Android, BuildOptions.None);
	}
	
	private static string[] FindEnabledEditorScenes() {
		List<string> EditorScenes = new List<string>();
		foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes) 
		{
			if (!scene.enabled) continue;
				EditorScenes.Add(scene.path);
		}
		return EditorScenes.ToArray();
	}

	static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);
		string res = BuildPipeline.BuildPlayer(scenes,target_dir, build_target, build_options);
		if (res.Length > 0) 
		{
			throw new Exception("BuildPlayer failure NOOOO: " + res);
		}
	}
}
