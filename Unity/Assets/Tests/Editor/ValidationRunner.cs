using System.IO;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    public static class ValidationRunner
    {
        [MenuItem("Power Above All/Verify/Edit Mode tests")]
        public static void Run()
        {
            var api = ScriptableObject.CreateInstance<TestRunnerApi>();
            api.RegisterCallbacks(new Results());
            api.Execute(new ExecutionSettings(new Filter { testMode = TestMode.EditMode,
                assemblyNames = new[] { "PowerAboveAll.EditModeTests" } }) { runSynchronously = true });
            Object.DestroyImmediate(api);
        }
        private sealed class Results : ICallbacks
        {
            public void RunStarted(ITestAdaptor tests) { }
            public void TestStarted(ITestAdaptor test) { }
            public void TestFinished(ITestResultAdaptor result) { }
            public void RunFinished(ITestResultAdaptor result)
            {
                string path = Path.GetFullPath("../output/unity-edit-tests.xml");
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                TestRunnerApi.SaveResultToFile(result, path);
                Debug.Log("Power Above All tests: " + result.TestStatus + "; " + path);
            }
        }
    }
}
