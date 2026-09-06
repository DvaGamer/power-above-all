using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
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
        { BuildWindowsTo("Builds/Windows"); }

        [MenuItem("Power Above All/Build Windows preview")]
        public static void BuildWindowsPreview()
        { BuildWindowsTo("Builds/WindowsPolish"); }

        public static void BuildWindowsVerification()
        {
            string[] arguments = Environment.GetCommandLineArgs();
            int at = Array.IndexOf(arguments, "-verificationBuildPath");
            if (at < 0 || at + 1 >= arguments.Length || !Path.IsPathRooted(arguments[at + 1]))
                throw new BuildFailedException("Verification requires an absolute -verificationBuildPath.");
            BuildWindowsTo(Path.GetFullPath(arguments[at + 1]));
        }

        [MenuItem("Power Above All/Build Windows development")]
        public static void BuildWindowsDevelopment()
        { BuildWindowsTo("Builds/WindowsDevelopment", BuildOptions.Development); }

        private static void BuildWindowsTo(string directory, BuildOptions options = BuildOptions.None)
        {
            PlayerSettings.companyName = "Power Above All";
            PlayerSettings.productName = "Power Above All";
            PlayerSettings.defaultScreenWidth = 1440;
            PlayerSettings.defaultScreenHeight = 900;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.runInBackground = false;
            PlayerSettings.colorSpace = ColorSpace.Linear;
            // Bu makinede DX12 kapanışta D3D12Core.dll erişim ihlali üretti.
            // Aynı player'ın görünür DX11 altı haftalık rotası kareler ve exit0 ile doğrulandı.
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, new[] { GraphicsDeviceType.Direct3D11 });
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            EnsurePlayerRenderResources();
            Directory.CreateDirectory(directory);
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = new[] { "Assets/Scenes/Main.unity" }, locationPathName = Path.Combine(directory, "Power Above All.exe"),
                target = BuildTarget.StandaloneWindows64, options = options
            });
            if (report.summary.result != BuildResult.Succeeded) throw new InvalidOperationException("Windows build failed: " + report.summary.result);
            Debug.Log("Power Above All build succeeded: " + report.summary.outputPath);
        }

        public static void EnsurePlayerRenderResources()
        {
            foreach (string templateName in new[] { "DioramaOpaque", "DioramaTransparent", "DioramaEmission" })
            {
                string path = "Assets/Resources/BattleMaterials/" + templateName + ".mat";
                Material template = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (template == null || template.shader == null || template.shader.name != "Standard")
                    throw new BuildFailedException("Missing Standard material reference: " + path);
                if (templateName == "DioramaTransparent" && !template.IsKeywordEnabled("_ALPHABLEND_ON"))
                    throw new BuildFailedException("Missing fade shader variant: " + path);
                if (templateName == "DioramaEmission" && !template.IsKeywordEnabled("_EMISSION"))
                    throw new BuildFailedException("Missing emission shader variant: " + path);
            }

            // Duman, ışıklandırılmış Standard yüzeyinden ayrı ve açık alfa geçişi kullanır.
            Material powder = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/BattleMaterials/PowderWash.mat");
            Shader powderShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/Resources/BattleMaterials/PowderWashAlpha.shader");
            if (powder == null || powderShader == null || powder.shader != powderShader ||
                powderShader.name != "PowerAboveAll/PowderWashAlpha" || !powder.HasProperty("_MainTex") ||
                !powder.HasProperty("_Color") || powder.FindPass("POWDER_WASH") < 0 || powder.renderQueue != 3000)
                throw new BuildFailedException("Missing explicit powder alpha material, texture/tint properties or render pass.");

            UnityEngine.Object settings = GraphicsSettings.GetGraphicsSettings();
            var graphics = new SerializedObject(settings);
            SerializedProperty shaders = graphics.FindProperty("m_AlwaysIncludedShaders");
            if (shaders == null) throw new BuildFailedException("Cannot find the player shader inclusion list.");
            bool standardIncluded = false;
            for (int i = 0; i < shaders.arraySize; i++)
            {
                Shader included = shaders.GetArrayElementAtIndex(i).objectReferenceValue as Shader;
                if (included != null && included.name == "Standard") standardIncluded = true;
            }
            // Shader listesi kaynakta tanımlıdır; derleme sırasında ProjectSettings değiştirilmez.
            if (!standardIncluded)
                throw new BuildFailedException("Standard must be included in ProjectSettings/GraphicsSettings.asset (m_AlwaysIncludedShaders).");
            Debug.Log("Player render resources verified: Standard retention, opaque/fade/emission references and dedicated powder alpha pass.");
        }
    }

    // Menü dışından başlatılan oyuncu derlemeleri de aynı shader doğrulamasından geçer.
    public sealed class RuntimeShaderBuildPreparation : IPreprocessBuildWithReport
    {
        public int callbackOrder => -1000;
        public void OnPreprocessBuild(BuildReport report) { BuildTools.EnsurePlayerRenderResources(); }
    }
}
