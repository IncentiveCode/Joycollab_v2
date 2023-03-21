using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MirrorBuildScript
{
    [MenuItem("Build/Build all")]
    public static void BuildAll() 
    {
        BuildWindowsServer();
        BuildLinuxServer();
        BuildWindowsClient();
        BuildWebGLClient();
    }

    [MenuItem("Build/Build server (Windows)")]
    public static void BuildWindowsServer() 
    {
        Debug.Log("Building Server (Windows) ...");

        BuildPlayerOptions opt = new BuildPlayerOptions();
        opt.scenes = new[] { 
            "Assets/World/Scenes/MainMenu.unity",
            "Assets/World/Scenes/World.unity",
            "Assets/World/Scenes/Square.unity",
            "Assets/World/Scenes/Loading.unity"
        };
        opt.locationPathName = "Builds/Windows/Server/Server.exe";
        opt.target = BuildTarget.StandaloneWindows64;
        opt.options = BuildOptions.CompressWithLz4;
        opt.subtarget = (int)StandaloneBuildSubtarget.Server;

        BuildPipeline.BuildPlayer(opt);
        Debug.Log(" Done.");
    }

    [MenuItem("Build/Build server (Linux)")]
    public static void BuildLinuxServer() 
    {
        Debug.Log("Building Server (Linux) ...");

        BuildPlayerOptions opt = new BuildPlayerOptions();
        opt.scenes = new[] { 
            "Assets/World/Scenes/MainMenu.unity",
            "Assets/World/Scenes/World.unity",
            "Assets/World/Scenes/Square.unity",
            "Assets/World/Scenes/Loading.unity"
        };
        opt.locationPathName = "Builds/Linux/Server/Server.x86_64";
        opt.target = BuildTarget.StandaloneLinux64;
        opt.options = BuildOptions.CompressWithLz4HC;
        opt.subtarget = (int)StandaloneBuildSubtarget.Server;

        BuildPipeline.BuildPlayer(opt);
        Debug.Log(" Done.");
    }

    [MenuItem("Build/Build client (Windows)")]
    public static void BuildWindowsClient() 
    {
        BuildPlayerOptions opt = new BuildPlayerOptions();
        opt.scenes = new[] { "Assets/Scenes/Main.unity" };
        opt.locationPathName = "Builds/Windows/Client/Client.exe";
        opt.target = BuildTarget.StandaloneWindows64;
        opt.options = BuildOptions.CompressWithLz4;

        Debug.Log("Building Client (Windows) ...");
        BuildPipeline.BuildPlayer(opt);
        Debug.Log(" Done.");
    }

    [MenuItem("Build/Build client (WebGL)")]
    public static void BuildWebGLClient() 
    {
        PlayerSettings.WebGL.emscriptenArgs = "";

        BuildPlayerOptions opt = new BuildPlayerOptions();
        opt.scenes = new[] { "Assets/Scenes/Main.unity" };
        opt.locationPathName = "Builds/WebGL";
        opt.target = BuildTarget.WebGL;
        opt.options = BuildOptions.None;

        Debug.Log("Building Client (WebGL) ...");
        BuildPipeline.BuildPlayer(opt);
        Debug.Log(" Done.");
    }
}