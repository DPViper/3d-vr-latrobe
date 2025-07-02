using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
 #if UNITY_EDITOR
using UnityEditor.Build.Reporting;
#endif

namespace LaTrobeVR
{
    public class BuildScript : MonoBehaviour
    {
        [Header("Build Settings")]
        public string buildPath = "Builds/";
        public string appName = "LaTrobe3DVR";
        public bool developmentBuild = true;
        public bool buildAppBundle = false;
        
        [Header("Deployment")]
        public bool autoDeploy = false;
        public string adbPath = "adb";
        public string deviceId = "";
        
        [ContextMenu("Build for Quest 3")]
        public static void BuildForQuest3()
        {
            BuildScript buildScript = FindObjectOfType<BuildScript>();
            if (buildScript != null)
            {
                buildScript.PerformBuild();
            }
            else
            {
                UnityEngine.Debug.LogError("BuildScript component not found in scene!");
            }
        }
        
        public void PerformBuild()
        {
            // Ensure build directory exists
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }
            
            // Get all scenes
            string[] scenes = GetEnabledScenes();
            
            // Set build target
            EditorUserBuildSettings.SwitchActiveBuildTarget(
                BuildTargetGroup.Android, 
                BuildTarget.Android
            );
            
            // Configure build settings
            PlayerSettings.SetApplicationIdentifier(
                BuildTargetGroup.Android, 
                "com.latrobe.campusvr"
            );
            
            PlayerSettings.productName = "La Trobe 3D VR Campus";
            PlayerSettings.companyName = "La Trobe University";
            
            // Android specific settings
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;
            
            // VR settings
            /*PlayerSettings.SetVirtualRealitySDKs(
                BuildTargetGroup.Android, 
                new string[] { "Oculus" }
            );*/
            
            // Build options
            BuildOptions buildOptions = BuildOptions.None;
            if (developmentBuild)
            {
                buildOptions |= BuildOptions.Development;
                buildOptions |= BuildOptions.AllowDebugging;
            }
            
            // Determine build path
            string buildFileName = appName;
            if (buildAppBundle)
            {
                buildFileName += ".aab";
            }
            else
            {
                buildFileName += ".apk";
            }
            
            string fullBuildPath = Path.Combine(buildPath, buildFileName);
            
            // Perform build
            #if UNITY_EDITOR
            BuildReport report = BuildPipeline.BuildPlayer(
                scenes,
                fullBuildPath,
                BuildTarget.Android,
                buildOptions
            );
            
            if (report.summary.result == BuildResult.Succeeded)
            {
                UnityEngine.Debug.Log($"Build successful: {fullBuildPath}");
                
                if (autoDeploy)
                {
                    DeployToDevice(fullBuildPath);
                }
            }
            else
            {
                UnityEngine.Debug.LogError($"Build failed: {report.summary.result}");
            }
            #endif
        }
        
        string[] GetEnabledScenes()
        {
            List<string> enabledScenes = new List<string>();
            
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
                if (scene.enabled)
                {
                    enabledScenes.Add(scene.path);
                }
            }
            
            return enabledScenes.ToArray();
        }
        
        void DeployToDevice(string buildPath)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                // Get first connected device
                deviceId = GetFirstConnectedDevice();
            }
            
            if (string.IsNullOrEmpty(deviceId))
            {
                UnityEngine.Debug.LogError("No Quest device connected!");
                return;
            }
            
            UnityEngine.Debug.Log($"Deploying to device: {deviceId}");
            
            // Install APK
            string installCommand = $"{adbPath} -s {deviceId} install -r \"{buildPath}\"";
            ExecuteCommand(installCommand);
            
            // Launch app
            string launchCommand = $"{adbPath} -s {deviceId} shell am start -n com.latrobe.campusvr/com.unity3d.player.UnityPlayerActivity";
            ExecuteCommand(launchCommand);
            
            UnityEngine.Debug.Log("Deployment completed!");
        }
        
        string GetFirstConnectedDevice()
        {
            string command = $"{adbPath} devices";
            string output = ExecuteCommand(command);
            
            // Parse output to get device ID
            string[] lines = output.Split('\n');
            foreach (string line in lines)
            {
                if (line.Contains("device") && !line.Contains("List of devices"))
                {
                    return line.Split('\t')[0];
                }
            }
            
            return "";
        }
        
        string ExecuteCommand(string command)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C {command}";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;
            
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            
            process.WaitForExit();
            
            if (!string.IsNullOrEmpty(error))
            {
                UnityEngine.Debug.LogWarning($"Command error: {error}");
            }
            
            return output;
        }
        
        [ContextMenu("Setup Build Settings")]
        public static void SetupBuildSettings()
        {
            // Add scenes to build settings
            string[] scenePaths = {
                "Assets/Scenes/LaTrobeVRScene.unity"
            };
            
            EditorBuildSettings.scenes = new EditorBuildSettingsScene[scenePaths.Length];
            for (int i = 0; i < scenePaths.Length; i++)
            {
                EditorBuildSettings.scenes[i] = new EditorBuildSettingsScene(scenePaths[i], true);
            }
            
            // Configure Android settings
            PlayerSettings.SetApplicationIdentifier(
                BuildTargetGroup.Android, 
                "com.latrobe.campusvr"
            );
            
            PlayerSettings.productName = "La Trobe 3D VR Campus";
            PlayerSettings.companyName = "La Trobe University";
            
            // VR settings
            /*PlayerSettings.SetVirtualRealitySDKs(
                BuildTargetGroup.Android, 
                new string[] { "Oculus" }
            );*/
            
            UnityEngine.Debug.Log("Build settings configured!");
        }
        
        [ContextMenu("Test Device Connection")]
        public static void TestDeviceConnection()
        {
            BuildScript buildScript = FindObjectOfType<BuildScript>();
            if (buildScript != null)
            {
                string deviceId = buildScript.GetFirstConnectedDevice();
                if (!string.IsNullOrEmpty(deviceId))
                {
                    UnityEngine.Debug.Log($"Quest device connected: {deviceId}");
                }
                else
                {
                    UnityEngine.Debug.LogWarning("No Quest device found. Make sure device is connected and developer mode is enabled.");
                }
            }
        }
    }
    
    #if UNITY_EDITOR
    public class BuildMenu
    {
        [MenuItem("La Trobe VR/Build for Quest 3")]
        public static void BuildForQuest3()
        {
            BuildScript.BuildForQuest3();
        }
        
        [MenuItem("La Trobe VR/Setup Build Settings")]
        public static void SetupBuildSettings()
        {
            BuildScript.SetupBuildSettings();
        }
        
        [MenuItem("La Trobe VR/Test Device Connection")]
        public static void TestDeviceConnection()
        {
            BuildScript.TestDeviceConnection();
        }
        
        [MenuItem("La Trobe VR/Open Build Folder")]
        public static void OpenBuildFolder()
        {
            string buildPath = "Builds/";
            if (Directory.Exists(buildPath))
            {
                EditorUtility.RevealInFinder(buildPath);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Build folder does not exist yet. Run a build first.");
            }
        }
    }
    #endif
} 