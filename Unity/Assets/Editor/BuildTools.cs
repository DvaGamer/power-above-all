using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace PowerAboveAll.Editor
{
    public static class BuildTools
    {
        [MenuItem("Power Above All/Open campaign")]
        public static void OpenCampaign() { EditorSceneManager.OpenScene("Assets/Scenes/Main.unity"); }

        [MenuItem("Power Above All/Build Windows")]
        public static void BuildWindows()
        {
            PlayerSettings.companyName = "Power Above All";
            PlayerSettings.productName = "Power Above All";
            PlayerSettings.defaultScreenWidth = 1440;
            PlayerSettings.defaultScreenHeight = 900;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.runInBackground = false;
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            var graphics = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
            var shaders = graphics.FindProperty("m_AlwaysIncludedShaders");
            foreach (string name in new[] { "Standard", "Unlit/Color", "Sprites/Default", "Legacy Shaders/Transparent/Diffuse" })
            {
                Shader shader = Shader.Find(name);
                if (shader == null) throw new InvalidOperationException("Missing shader: " + name);
                bool found = false;
                for (int i = 0; i < shaders.arraySize; i++) if (shaders.GetArrayElementAtIndex(i).objectReferenceValue == shader) found = true;
                if (!found) { int index = shaders.arraySize++; shaders.GetArrayElementAtIndex(index).objectReferenceValue = shader; }
            }
            graphics.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();
            Directory.CreateDirectory("Builds/Windows");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = new[] { "Assets/Scenes/Main.unity" }, locationPathName = "Builds/Windows/Power Above All.exe",
                target = BuildTarget.StandaloneWindows64, options = BuildOptions.Development
            });
            if (report.summary.result != BuildResult.Succeeded) throw new InvalidOperationException("Windows build failed: " + report.summary.result);
            Debug.Log("Power Above All build succeeded: " + report.summary.outputPath);
        }
    }
}
