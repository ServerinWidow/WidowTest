using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEditor.UI;

public class MyEditorScript : MonoBehaviour
{
    private static bool DebugOn;
    private static string[] Scenes = FindEnabledEditorScenes();

    private static string GameName = "Testeo";
    private static string TargetDir = "C:/";
    private static string ProjectWorkspace = "C:/";

    private static string platform = "android";

    private static Dictionary<string, BuildTarget> BuildPlatforms = new Dictionary<string, BuildTarget>();
    private static Dictionary<string, string> Extensions = new Dictionary<string, string>();
    private static Dictionary<string, string> Arguments = new Dictionary<string, string>();

    static void InitData()
    {
        BuildPlatforms.Add("android", BuildTarget.Android);
        BuildPlatforms.Add("pc", BuildTarget.StandaloneWindows);
        BuildPlatforms.Add("ios", BuildTarget.iOS);

        Extensions.Add("android", ".apk");
        Extensions.Add("pc", ".exe");
        Extensions.Add("ios", ".xcode");
    }

    [MenuItem(("Custom/CI/Run"))]
    static void Run()
    {
        InitData();

        GetCmdLineArguments();
        SetBuildSpecifics();

        string target_dir = GameName + Extensions[platform];
        DebugLog("target_dir: " + target_dir);
        DebugLog("BuildPlatforms[platform]: " + BuildPlatforms[platform]);
        DebugLog("workspace: " + ProjectWorkspace);


        //GenericBuild(Scenes, TargetDir + "/" + target_dir, BuildPlatforms[platform], BuildOptions.None);
    }

    static void GetCmdLineArguments()
    {
        // unity -quit MyEditorScript.Run   ...    -args gameType:MEMORAMA,IOS:true
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
                Arguments.Add(tupla.Split(':')[0].TrimEnd().TrimStart().ToLower(), tupla.Split(':')[1].TrimEnd().TrimStart().ToLower());
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
        platform = Arguments.ContainsKey("platform") ? Arguments["platform"] : platform;
        ProjectWorkspace = Arguments.ContainsKey("workspace") ? Arguments["workspace"] : ProjectWorkspace;

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
            Debug.Log(p + "\n");
    }
}
