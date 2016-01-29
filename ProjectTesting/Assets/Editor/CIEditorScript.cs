using UnityEngine;
using UnityEditor;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEditor.UI;

public class CIEditorScript : MonoBehaviour
{
	private static bool DebugOn;
	private static string[] Scenes = FindEnabledEditorScenes();
	
	private static string GameName = "TestGame";
	private static string ProjectWorkspace = "/Users/Shared/Jenkins/Home/jobs/TestProject";
	private static string platform = "android";
	private static string TargetDir = "target/";	// Relative to ${WORKSPACE} Jenkins Enviroment Variable
	
	private static Dictionary<string, BuildTarget> BuildPlatforms = new Dictionary<string, BuildTarget>();
	private static Dictionary<string, string> Extensions = new Dictionary<string, string>();
	private static Dictionary<string, string> Arguments = new Dictionary<string, string>();
	
	static void InitData()
	{
		BuildPlatforms.Add("android", BuildTarget.Android);
		BuildPlatforms.Add("pc", BuildTarget.StandaloneWindows);
		BuildPlatforms.Add("osx", BuildTarget.StandaloneOSXIntel);
		BuildPlatforms.Add("ios", BuildTarget.iOS);
		
		Extensions.Add("android", ".apk");
		Extensions.Add("pc", ".exe");
		Extensions.Add("osx", ".app");
		Extensions.Add("ios", "XCodeProject");

	}
	
	[MenuItem(("Custom/CI/Run"))]
	static void Run()
	{
		InitData();
		
		GetCmdLineArguments();
		SetBuildSpecifics();

		//GenericBuild(Scenes, BuildOutputPath(), BuildPlatforms[platform], BuildOptions.AcceptExternalModificationsToPlayer);

		GenericBuild(Scenes, BuildOutputPath(), BuildPlatforms[platform], BuildOptions.None);

	}

	private static string BuildOutputPath() {
		return ProjectWorkspace + "/" + TargetDir + Extensions[platform];
	}
	
	static void GetCmdLineArguments()
	{
		// unity -quit MyEditorScript.Run   ...    -args gameType=MEMORAMA,IOS=true
		string cmdLine = System.Environment.CommandLine;
		
		DebugOn = cmdLine.Contains("-debug");
		
		string[] separatingChars = { "-args", "..." };
		
		if (cmdLine.Contains("-args "))
		{
			Arguments = new Dictionary<string, string>();
			
			cmdLine = cmdLine.Split(separatingChars, StringSplitOptions.None)[1];
			string[] tuplas = cmdLine.Split(',');
			
			foreach (string tupla in tuplas)
			{
				Arguments.Add(tupla.Split('=')[0].TrimEnd().TrimStart(), tupla.Split('=')[1].TrimEnd().TrimStart());
			}
		}
		
		DebugLog("Arguments:");
		foreach (var argument in Arguments)
		{
			DebugLog("[" + argument.Key + " = " + argument.Value + "]");
		}
	}
	
	static void SetBuildSpecifics()
	{
		platform = Arguments.ContainsKey("PLATFORM") ? Arguments["PLATFORM"] : platform;
		ProjectWorkspace = Arguments.ContainsKey("WORKSPACE") ? Arguments["WORKSPACE"] : ProjectWorkspace;
		GameName = Arguments.ContainsKey("GAME") ? Arguments["GAME"] : GameName;
		TargetDir = Arguments.ContainsKey("TARGET_DIR") ? Arguments["TARGET_DIR"] : TargetDir;
		
		if (Arguments.ContainsKey("DEFINES"))
		{
			string[] defines = Arguments["DEFINES"].Split(';');
			DebugLog("DEFINES: " + defines.ToString());
			foreach (string define in defines)
			{
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, define);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, define);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, define);
				DebugLog(define + ", ");
			}
			
			
		}
		
	}
	
	private static string[] FindEnabledEditorScenes()
	{
		List<string> EditorScenes = new List<string>();
		foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
		{
			if (!scene.enabled) continue;
			EditorScenes.Add(scene.path);
		}
		return EditorScenes.ToArray();
	}
	
	static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);
		string res = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);
		if (res.Length > 0)
		{
			throw new Exception("BuildPlayer failure NOOOO: " + res);
		}
	}
	static void DebugLog(object p)
	{
		if (DebugOn)
			Debug.Log("XXXXXXXXXXX " + p + "\n");
	}
}
