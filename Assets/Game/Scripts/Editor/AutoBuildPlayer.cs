using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Game.Build
{
    public static class AutoBuildPlayer
    {
        private const string ADMIN_DEFINE = "ADMIN";

        [MenuItem("Game/Tools/Switch To Admin Mode")]
        public static void SwitchToServerEditor()
        {
            DefineForAdmin();
        }

        [MenuItem("Game/Tools/Switch To GameRoom Mode")]
        public static void SwitchToGameRoomEditor()
        {
            DefineForServerRoom();
        }

        [MenuItem("Game/Tools/Switch To Client Mode")]
        public static void SwitchToClientEditor()
        {
            DefineForVRAndroid();
        }

        private static void DefineForAdmin()
        {
            PlayerSettings.virtualRealitySupported = false;
            GlobalDefine(ADMIN_DEFINE, BuildTarget.StandaloneWindows);
            //GlobalUndefine("CURVEDUI_TOUCH", BuildTarget.Android);
            //GlobalUndefine("CURVEDUI_VIVE", BuildTarget.StandaloneWindows);
        }

        private static void DefineForVRAndroid()
        {
            PlayerSettings.virtualRealitySupported = true;
            GlobalUndefine(ADMIN_DEFINE, BuildTarget.StandaloneWindows);
            GlobalUndefine(ADMIN_DEFINE, BuildTarget.Android);
            GlobalUndefine(ADMIN_DEFINE, BuildTarget.iOS);
            //GlobalDefine("CURVEDUI_TOUCH", BuildTarget.Android);
            //GlobalDefine("CURVEDUI_VIVE", BuildTarget.StandaloneWindows);
        }

        private static void DefineForServerRoom()
        {
            PlayerSettings.virtualRealitySupported = false;
            GlobalUndefine(ADMIN_DEFINE, BuildTarget.StandaloneWindows);
        }

        public static void GlobalDefine(string name, BuildTarget target)
        {
            //Debug.Log("Define " + name);
            string scriptDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(target));
            if (!scriptDefines.Contains(name))
            {
                string newScriptDefines = scriptDefines + " " + name;
                if (EditorUserBuildSettings.selectedBuildTargetGroup != 0)
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(target),
                        newScriptDefines);
            }
        }

        public static void GlobalUndefine(string name, BuildTarget target)
        {
            //Debug.Log("Undefine " + name);
            string scriptDefines =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(target));
            if (scriptDefines.Contains(name))
            {
                int playMakerIndex = scriptDefines.IndexOf(name);
                string newScriptDefines = scriptDefines.Remove(playMakerIndex, name.Length);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(target),
                    newScriptDefines);
            }
        }

        private static void SwitchTarget(BuildTarget buildTarget)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(buildTarget),
                buildTarget);
        }

        private static void ChangeBuildSystem(AndroidBuildSystem androiBuildSystem)
        {
            EditorUserBuildSettings.androidBuildSystem = androiBuildSystem;
        }

        //[MenuItem("Game/Tools/Orientation/Set ScreenOrientation Left Right")]
        static void SetScreenOrientationLeftRight()
        {
            SetDefaultScreenOrientation(true);
        }
        //[MenuItem("Game/Tools/Orientation/Set ScreenOrientation Left")]
        static void SetScreenOrientationLeft()
        {
            SetDefaultScreenOrientation();
        }

        public static void SetDefaultScreenOrientation(bool isAutoLanscapeLR = false)
        {
            if (isAutoLanscapeLR)
            {
                PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
                PlayerSettings.allowedAutorotateToLandscapeLeft = true;
                PlayerSettings.allowedAutorotateToLandscapeRight = true;
                PlayerSettings.allowedAutorotateToPortrait = false;
                PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            }
            else
            {
                PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            }
        }

        #region BuildWindows

        public static void BuildAllForWindows()
        {
            BuildServerAdminForWindows();
            BuildServerRoomForWindows();
            BuildBridgerForWindows();
            //BuildClientForWindows();
        }

        public static void BuildAllForWindows64()
        {
            BuildServerAdminForWindows64();
            BuildServerRoomForWindows64();
            //BuildBridgerForWindows64();
            //BuildClientForWindows64();
        }

        //[MenuItem("Game/Tools/Build/StandaloneWindows/All")]
        private static void SwitchTargetAndBuildAllForWindows()
        {
            SwitchTarget(BuildTarget.StandaloneWindows);
            BuildAllForWindows();
        }

        [MenuItem("Game/Tools/Build/StandaloneWindows64/All")]
        private static void SwitchTargetAndBuildAllForWindows64()
        {
            SwitchTarget(BuildTarget.StandaloneWindows64);
            BuildAllForWindows64();
        }


        [MenuItem("Game/Tools/Build/StandaloneWindows64/Admin")]
        private static void BuildServerForWindows64Editor()
        {
            SwitchTarget(BuildTarget.StandaloneWindows64);
            BuildServerAdminForWindows64();
        }

        [MenuItem("Game/Tools/Build/StandaloneWindows64/ServerRoom")]
        private static void BuildServerRoomForWindows64Editor()
        {
            SwitchTarget(BuildTarget.StandaloneWindows64);
            BuildServerRoomForWindows64();
        }

        //[MenuItem("Game/Tools/Build/StandaloneWindows64/Bridger")]
        private static void BuildBridgerForWindows64Editor()
        {
            SwitchTarget(BuildTarget.StandaloneWindows64);
            BuildBridgerForWindows64();
        }

        //[MenuItem("Game/Tools/Build/StandaloneWindows/Admin")]
        private static void BuildServerForWindowsEditor()
        {
            SwitchTarget(BuildTarget.StandaloneWindows);
            BuildServerAdminForWindows();
        }

        //[MenuItem("Game/Tools/Build/StandaloneWindows/ServerRoom")]
        private static void BuildServerRoomForWindowsEditor()
        {
            SwitchTarget(BuildTarget.StandaloneWindows);
            BuildServerRoomForWindows();
        }

        //[MenuItem("Game/Tools/Build/StandaloneWindows/Bridger")]
        private static void BuildBridgerForWindowsEditor()
        {
            SwitchTarget(BuildTarget.StandaloneWindows);
            BuildBridgerForWindows();
        }

        private static void BuildServerAdminForWindows64()
        {
            DefineForAdmin();

            string buildFolder = Path.Combine("Builds", "StandaloneWindows64", "PC");


            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[]
                {
                    "Assets/Game/Scenes/AdminScreen.unity",
                    //"Assets/Game/Scenes/ActiveUserScreen.unity",
                    "Assets/Game/Scenes/ContentListScreen.unity",
                    "Assets/Game/Scenes/BaseScreen.unity",
                },
                locationPathName = Path.Combine(buildFolder, NameBinaryDefine.AdminServerExe),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.ShowBuiltPlayer
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                //StringBuilder arguments = new StringBuilder();
                //arguments.Append("@echo off\n");
                //arguments.Append($"start {NameBinaryDefine.ADMIN_SERVER} ");
                //arguments.Append($"{NameBinaryDefine.AdminServerExe} ");
                ////arguments.Append("-batchmode ");
                ////arguments.Append("-nographic ");
                //arguments.Append($"-logFile ");
                //arguments.Append($"output.log ");

                //File.WriteAllText(Path.Combine(buildFolder, "Start Server.bat"), arguments.ToString());

                Debug.Log("Server admin build succeeded: " + (summary.totalSize / 1024) + " kb");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Server admin build failed");
            }
        }

        private static void BuildServerRoomForWindows64()
        {
            DefineForServerRoom();

            string buildFolder = Path.Combine("Builds", "StandaloneWindows64", "PC", "Server");

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[]
                {
                    "Assets/Game/Scenes/ServerStartScene.unity",
                    "Assets/Game/Scenes/ActiveUserScreen.unity",
                    "Assets/Game/Scenes/ContentListScreen.unity",
                    "Assets/Game/Scenes/BaseScreen.unity",
                },
                locationPathName = Path.Combine(buildFolder, NameBinaryDefine.GameRoomServerExe),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.ShowBuiltPlayer
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Server game room build succeeded: " + (summary.totalSize / 1024) + " kb");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Server game room build failed");
            }
        }

        private static void BuildBridgerForWindows64()
        {
            DefineForServerRoom();

            string buildFolder = Path.Combine("Builds", "StandaloneWindows64", "BridgerApp");

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[]
                {
                    "Assets/Game/Scenes/BridgerScreen.unity",
                },
                locationPathName = Path.Combine(buildFolder, NameBinaryDefine.BridgerExe),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.ShowBuiltPlayer
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                //StringBuilder arguments = new StringBuilder();
                //arguments.Append("@echo off\n");
                //arguments.Append($"start {NameBinaryDefine.BRIDGER} ");
                //arguments.Append($"{NameBinaryDefine.BridgerExe} ");
                ////arguments.Append("-batchmode ");
                ////arguments.Append("-nographic ");
                //arguments.Append($"-logFile ");
                //arguments.Append($"output.log ");

                //File.WriteAllText(Path.Combine(buildFolder, "Start.bat"), arguments.ToString());

                Debug.Log("Bridger build succeeded: " + (summary.totalSize / 1024) + " kb");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Bridger build failed");
            }
        }

        private static void BuildServerAdminForWindows()
        {
            DefineForAdmin();

            string buildFolder = Path.Combine("Builds", "StandaloneWindows", "Server");

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[]
                {
                    "Assets/Game/Scenes/AdminScreen.unity",
                    "Assets/Game/Scenes/ActiveUserScreen.unity",
                    "Assets/Game/Scenes/ContentListScreen.unity",
                    "Assets/Game/Scenes/BaseScreen.unity",
                },
                locationPathName = Path.Combine(buildFolder, NameBinaryDefine.AdminServerExe),
                target = BuildTarget.StandaloneWindows,
                options = BuildOptions.ShowBuiltPlayer
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                //StringBuilder arguments = new StringBuilder();
                //arguments.Append("@echo off\n");
                //arguments.Append($"start {NameBinaryDefine.ADMIN_SERVER} ");
                //arguments.Append($"{NameBinaryDefine.AdminServerExe} ");
                ////arguments.Append("-batchmode ");
                ////arguments.Append("-nographic ");
                //arguments.Append($"-logFile ");
                //arguments.Append($"output.log ");

                //File.WriteAllText(Path.Combine(buildFolder, "Start Server.bat"), arguments.ToString());

                Debug.Log("Server admin build succeeded: " + (summary.totalSize / 1024) + " kb");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Server admin build failed");
            }
        }

        private static void BuildServerRoomForWindows()
        {
            DefineForServerRoom();

            string buildFolder = Path.Combine("Builds", "StandaloneWindows", "Server", "Server");

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[]
                {
                    "Assets/Game/Scenes/ServerStartScene.unity",
                    "Assets/Game/Scenes/ActiveUserScreen.unity",
                    "Assets/Game/Scenes/ContentListScreen.unity",
                    "Assets/Game/Scenes/BaseScreen.unity",
                },
                locationPathName = Path.Combine(buildFolder, NameBinaryDefine.GameRoomServerExe),
                target = BuildTarget.StandaloneWindows,
                options = BuildOptions.ShowBuiltPlayer
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Server game room build succeeded: " + (summary.totalSize / 1024) + " kb");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Server game room build failed");
            }
        }

        private static void BuildBridgerForWindows()
        {
            DefineForServerRoom();

            string buildFolder = Path.Combine("Builds", "StandaloneWindows", "Bridger");

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[]
                {
                    "Assets/Game/Scenes/BridgerScreen.unity",
                },
                locationPathName = Path.Combine(buildFolder, NameBinaryDefine.BridgerExe),
                target = BuildTarget.StandaloneWindows,
                options = BuildOptions.ShowBuiltPlayer
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                StringBuilder arguments = new StringBuilder();
                arguments.Append("@echo off\n");
                arguments.Append($"start {NameBinaryDefine.BRIDGER} ");
                arguments.Append($"{NameBinaryDefine.BridgerExe} ");
                //arguments.Append("-batchmode ");
                //arguments.Append("-nographic ");
                arguments.Append($"-logFile ");
                arguments.Append($"output.log ");

                File.WriteAllText(Path.Combine(buildFolder, "Start.bat"), arguments.ToString());

                Debug.Log("Bridger build succeeded: " + (summary.totalSize / 1024) + " kb");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Bridger build failed");
            }
        }

        #endregion

        #region BuildAndroid

        //[MenuItem("Game/Tools/Build/Android/Oculus Go")]
        private static void BuildOculusGo()
        {
            SetDefaultScreenOrientation();
            SwitchTarget(BuildTarget.Android);
            //ChangeBuildSystem(AndroidBuildSystem.Internal);
            BuildClientForAndroid(NameBinaryDefine.OculusGoApk);
        }

        [MenuItem("Game/Tools/Build/Android/Oculus Quest")]
        private static void BuildOculusQuest()
        {
            SetDefaultScreenOrientation();
            SwitchTarget(BuildTarget.Android);
            ChangeBuildSystem(AndroidBuildSystem.Gradle);
            BuildClientForAndroid(NameBinaryDefine.OculusQuestApk);
        }

        //[MenuItem("Game/Tools/Build/Android/Pico")]
        private static void BuildPico()
        {
            SetDefaultScreenOrientation();
            SwitchTarget(BuildTarget.Android);
            //ChangeBuildSystem(AndroidBuildSystem.Internal);
            BuildClientForAndroid(NameBinaryDefine.PicoApk);
        }

        //[MenuItem("Game/Tools/Build/Android/Tablet")]
        private static void BuildTablet()
        {
            SetDefaultScreenOrientation(true);
            SwitchTarget(BuildTarget.Android);
            ChangeBuildSystem(AndroidBuildSystem.Gradle);
            BuildClientForAndroid(NameBinaryDefine.TabletApk, true);
        }

        private static void BuildClientForAndroid(string apkName, bool isTablet = false)
        {
            DefineForVRAndroid();

            string buildFolder = Path.Combine("Builds", "Android", "Client");



            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = isTablet ? new[]
                {
                    "Assets/Game/Scenes/Tablet/TitleScreen.unity",
                    "Assets/Game/Scenes/Tablet/ActiveUserScreen.unity",
                    "Assets/Game/Scenes/Tablet/ContentListScreen.unity",
                    "Assets/Game/Scenes/Tablet/BaseScreen.unity",
                } : new[]
                {
                    "Assets/Game/Scenes/TitleScreen.unity",
                    "Assets/Game/Scenes/ActiveUserScreen.unity",
                    "Assets/Game/Scenes/ContentListScreen.unity",
                    "Assets/Game/Scenes/Quest/BaseScreen.unity", // Only Using at this brach for Quest - careful if merge from PC.
                },
                locationPathName = Path.Combine(buildFolder, apkName),
                target = BuildTarget.Android,
                options = BuildOptions.ShowBuiltPlayer
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"{apkName} build succeeded: " + (summary.totalSize / 1024) + " kb");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log($"{apkName} build failed");
            }
        }

        #endregion

        #region BuildMacOs

        private static void BuildAllForMacOs()
        {
            BuildAdminForMacOs();
            BuildServerForMacOs();
            BuildBridgerForMacOs();
        }

        //[MenuItem("Game/Tools/Build/StandaloneOSX/All")]
        private static void BuildAllForMacOsEditor()
        {
            SwitchTarget(BuildTarget.StandaloneOSX);
            BuildAllForMacOs();
        }

        //[MenuItem("Game/Tools/Build/StandaloneOSX/Server")]
        private static void BuildAdminForMacOsEditor()
        {
            SwitchTarget(BuildTarget.StandaloneOSX);
            BuildAdminForMacOs();
        }


        //[MenuItem("Game/Tools/Build/StandaloneOSX/RoomServer")]
        private static void BuildRoomServerForMacOsEditor()
        {
            SwitchTarget(BuildTarget.StandaloneOSX);
            BuildServerForMacOs();
        }

        //[MenuItem("Game/Tools/Build/StandaloneOSX/Bridger")]
        private static void BuildBridgerForMacOsEditor()
        {
            SwitchTarget(BuildTarget.StandaloneOSX);
            BuildBridgerForMacOs();
        }

        private static void BuildAdminForMacOs()
        {
            DefineForAdmin();

            string buildFolder = Path.Combine("Builds", "StandaloneOSX", "Server");

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[]
                {
                    "Assets/Game/Scenes/AdminScreen.unity",
                    //"Assets/Game/Scenes/ActiveUserScreen.unity",
                    "Assets/Game/Scenes/ContentListScreen.unity",
                    "Assets/Game/Scenes/BaseScreen.unity",
                },
                locationPathName = Path.Combine(buildFolder, "Admin"),
                target = BuildTarget.StandaloneOSX,
                options = BuildOptions.ShowBuiltPlayer
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Server admin build succeeded: " + (summary.totalSize / 1024) + " kb");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Server admin build failed");
            }
        }


        private static void BuildServerForMacOs()
        {
            DefineForServerRoom();

            string buildFolder = Path.Combine("Builds", "StandaloneOSX", "Server");

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[]
                {
                    "Assets/Game/Scenes/ServerStartScene.unity",
                    "Assets/Game/Scenes/ActiveUserScreen.unity",
                    "Assets/Game/Scenes/ContentListScreen.unity",
                    "Assets/Game/Scenes/BaseScreen.unity",
                },
                locationPathName = Path.Combine(buildFolder, "Server"),
                target = BuildTarget.StandaloneOSX,
                options = BuildOptions.ShowBuiltPlayer
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Server room build succeeded: " + (summary.totalSize / 1024) + " kb");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Server room build failed");
            }
        }

        private static void BuildBridgerForMacOs()
        {
            DefineForServerRoom();

            string buildFolder = Path.Combine("Builds", "StandaloneOSX", "Bridger");

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[]
                {
                    "Assets/Game/Scenes/BridgerScreen.unity",
                },
                locationPathName = Path.Combine(buildFolder, NameBinaryDefine.BRIDGER),
                target = BuildTarget.StandaloneOSX,
                options = BuildOptions.ShowBuiltPlayer
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Bridger build succeeded: " + (summary.totalSize / 1024) + " kb");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Bridger build failed");
            }
        }

# endregion
    }

    /// <summary>
    /// On PreBuild
    /// </summary>
    class BuildProcessSettingApp : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }
        public void OnPreprocessBuild(BuildReport report)
        {
            //AutoBuildPlayer.SetDefaultScreenOrientation(isAutoLanscapeLR: true);
            //PlayerSettings.virtualRealitySupported = false;
            //string prefName = "OVREngineConfigurationUpdater_Enabled";
            //PlayerPrefs.SetInt(prefName, 0);
            //Debug.LogError("On Pre build ............... ");
        }
    }
}