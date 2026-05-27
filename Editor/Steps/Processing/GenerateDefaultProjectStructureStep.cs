using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Nomnom.UnityProjectPatcher.Editor.Steps 
{
    public readonly struct GenerateDefaultProjectStructureStep: IPatcherStep 
	{
        public UniTask<StepResult> Run() 
		{
            var settings = this.GetSettings();
            
            try {
                string[] pathsToCreate = 
				{
                    settings.ProjectUnityPath,
                    settings.ProjectUnityAssetStorePath,
                    settings.ProjectGameFullPath,
                    settings.ProjectGameAssetsFullPath,
                    settings.ProjectGameModsFullPath,
                    Path.Combine(settings.ProjectGameModsFullPath, "plugins"),
                    settings.ProjectGameToolsFullPath
                };

                foreach (var path in pathsToCreate) 
				{
                    if (!PatcherUtility.TryToCreatePath(path)) 
					{
                        Debug.LogError($"[GenerateDefaultProjectStructureStep] Failed to create or access path: {path}");
                        return UniTask.FromResult(StepResult.Failure);
                    }
                }

                var assetsPath = settings.ProjectGameAssetsPath;
                var arSettings = this.GetAssetRipperSettings();
                
                SortAssetTypesSteps.UnsortFolder(assetsPath, "MonoBehaviour", "ScriptableObject", arSettings);
                SortAssetTypesSteps.UnsortFolder(assetsPath, "PrefabInstance", "Prefab", arSettings);
            } 
			catch (Exception ex) 
			{
                Debug.LogError($"[GenerateDefaultProjectStructureStep] Exception occurred:\n{ex}");
                return UniTask.FromResult(StepResult.Failure);
            }
            
            return UniTask.FromResult(StepResult.Success);
        }
        
        public void OnComplete(bool failed) { }
    }
}