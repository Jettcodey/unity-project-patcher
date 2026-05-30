using System.IO;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Nomnom.UnityProjectPatcher.Editor.Steps {
    /// <summary>
    /// Imports the TextMeshPro essentials so the user doesn't have to use the pop-up.
    /// </summary>
    public readonly struct ImportTextMeshProStep : IPatcherStep 
    {
        public UniTask<StepResult> Run() 
        {
            string tmpPath = Path.Combine(Application.dataPath, "TextMesh Pro");
            
            // Skip execution if TMP is already installed
            if (Directory.Exists(tmpPath)) 
            {
                return UniTask.FromResult(StepResult.Success);
            }

            try 
            {
#if UNITY_6000_0_OR_NEWER
                EditorUtility.DisplayProgressBar("Installing packages", "Importing TMP Essentials & Extras", 1f);
                Debug.Log("Importing TMP Essentials & Extras...");
                
                // Unity 6000+ stores TMP resources inside the uGUI package
                AssetDatabase.ImportPackage("Packages/com.unity.ugui/Package Resources/TMP Essential Resources.unitypackage", false);
                AssetDatabase.ImportPackage("Packages/com.unity.ugui/Package Resources/TMP Examples & Extras.unitypackage", false);
#else
                EditorUtility.DisplayProgressBar("Installing packages", "Installing TMP Essential Resources", 1f);
                
                // Older versions use the dedicated textmeshpro package
                AssetDatabase.ImportPackage("Packages/com.unity.textmeshpro/Package Resources/TMP Essential Resources.unitypackage", false);
#endif
            }
            catch 
            {
                Debug.LogError("Failed to install TMP packages. Verify the relevant package is installed in the Package Manager.");
            }
            finally 
            {
                // Ensures the progress bar is ALWAYS cleared, even on exceptions
                EditorUtility.ClearProgressBar();
            }
            
            return UniTask.FromResult(StepResult.Success);
        }

        public void OnComplete(bool failed) { }
    }
}