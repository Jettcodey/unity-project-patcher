using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Nomnom.UnityProjectPatcher.Editor.Steps {
    public struct StepsExecutor {
        public static string CurrentStepName { get; private set; }

        public static readonly string LogFilePath = Path.Combine(Directory.GetCurrentDirectory(), "PatcherPipeline.log");
        
        public IPatcherStep[] steps;
        public int index;

        private StepsProgress _progress;
        
        public StepsExecutor(IPatcherStep[] steps) {
            this.steps = steps;
            this.index = 0;

            _progress = new StepsProgress {
                Steps = steps.Select(x => x.GetType().FullName).ToList(),
                CompletedSteps = new List<string>(),
            };
        }

        private static void UnityLogCallback(string condition, string stackTrace, LogType type) {
            try {
                string logMsg = $"[{DateTime.Now:HH:mm:ss}] [{type}] {condition}\n";
                if (type == LogType.Exception || type == LogType.Error) {
                    logMsg += $"{stackTrace}\n";
                }
                File.AppendAllText(LogFilePath, logMsg);
            } catch { } 
        }

        public async UniTask<bool> Execute() {
            var stepIndex = 0;
            
            Application.logMessageReceivedThreaded -= UnityLogCallback;
            Application.logMessageReceivedThreaded += UnityLogCallback;

            try {
                var stepsProgress = StepsProgress.FromPath(StepsProgress.SavePath);
                if (stepsProgress != null) {
                    // might have crashed?
                    if (stepsProgress.InProgress) {
                        Debug.LogWarning($"Detected previous crash at step {stepIndex}. Aborting auto-resume.");
                        ClearProgress(true);
                        return false;
                    }
                    
                    if (stepsProgress.GetCompletion(steps, out stepIndex)) {
                        Debug.Log($"Completed {stepIndex} steps out of {steps.Length}");
                    }

                    // if (stepsProgress.LastResult == StepResult.RestartEditor) {
                    //     EditorUtility.DisplayDialog("Focus the Editor", "Please focus the editor!", "Ok");
                    // }
                    // if (stepsProgress.LastResult == StepResult.RestartEditor) {
                    //     PatcherUtility.FocusUnity();
                    // }
                }
            } catch (Exception ex) {
                Debug.LogError($"Failed to read steps progress:\n{ex}");
                ClearProgress(true);
                return false;
            }
            
            if (stepIndex >= steps.Length) {
                Debug.Log("All steps are done");
                ClearProgress(false);
                EditorUtility.DisplayDialog("Done", "The project has been patched successfully!", "Ok");
                Application.logMessageReceivedThreaded -= UnityLogCallback;
                return true;
            }

            index = stepIndex;

            if (index == 0) {
                if (File.Exists(LogFilePath)) {
                    File.Delete(LogFilePath);
                }
                Debug.Log("---Starting New Patcher Pipeline---");
                SetStartTime();
            }
            
            Debug.Log($"Starting on step {stepIndex} -> {steps[stepIndex].GetType().Name}");
            
            EditorUtility.DisplayProgressBar("Patching", "Patching", 0);
            try {
                for (int i = index; i < steps.Length; i++) {
                    index = i;
                    SaveProgress(true);
                    
                    var step = steps[i];
                    Debug.Log($"Starting step \"{step.GetType().Name}\"");
                    EditorUtility.DisplayProgressBar("Patching", step.GetType().Name, (float)i / steps.Length);
                    
                    try {
                        var startTime = DateTime.Now;
                        CurrentStepName = step.GetType().Name;
                        var result = await step.Run();
                        var endTime = DateTime.Now;
                        var elapsedSeconds = (endTime - startTime).TotalSeconds;
                        AppendStepResult(step, elapsedSeconds);
                        
                        _progress.LastResult = result;
                        Debug.Log($"Step \"{step.GetType().Name}\" took {elapsedSeconds} seconds and returned {result}");
                        
                        switch (result) {
                            case StepResult.RestartEditor:
                                index++;
                                SaveProgress(false);
                                AssetDatabase.StartAssetEditing();
                                EditorApplication.OpenProject(Directory.GetCurrentDirectory());
                                return false;
                            case StepResult.Failure:
                                throw new Exception($"Step {step.GetType().Name} returned StepResult.Failure.");
                            case StepResult.Recompile:
                                index++;
                                SaveProgress(false);
                                
                                if (PatcherUtility.LockedAssemblies) {
                                    PatcherUtility.LockedAssemblies = false;
                                    EditorApplication.UnlockReloadAssemblies();
                                }
                                
#if UNITY_2020_3_OR_NEWER
                                CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.CleanBuildCache);
#else
                                CompilationPipeline.RequestScriptCompilation();
#endif
                                return false;
                            default:
                                Debug.Log($"Step \"{step.GetType().Name}\" completed");
                                break;
                        }
                    } catch (Exception ex) {
                        Debug.LogError($"Step {step.GetType().Name} failed critically:\n{ex}");
                        EditorUtility.DisplayDialog($"Step {step.GetType().Name} Failed", $"Error:\n{ex.Message}\n\nCheck PatcherPipeline.log for full details.", "Ok");
                        ClearProgress(true);
                        return false;
                    } finally {
                        EditorUtility.ClearProgressBar();
                    }
                }
            } finally {
                Application.logMessageReceivedThreaded -= UnityLogCallback;
                EditorUtility.ClearProgressBar();
            }
            
            SetEndTime();
            EditorUtility.ClearProgressBar();
            ClearProgress(false);
            EditorUtility.DisplayDialog("Done", "The project has been patched successfully!", "Ok");
            return true;
        }

        public void SaveProgress(bool inProgress) {
            _progress.CompletedSteps = steps.Take(index).Select(x => x.GetType().FullName).ToList();
            _progress.InProgress = inProgress;
            File.WriteAllText(StepsProgress.SavePath, _progress.ToJson());
        }

        public void ClearProgress(bool failed) {
            CurrentStepName = null;
            File.Delete(StepsProgress.SavePath);
            EditorUtility.ClearProgressBar();

            if (steps == null || steps.Length == 0) return;
            
            foreach (var step in steps) {
                try {
                    step?.OnComplete(failed);
                } catch (Exception ex) {
                    Debug.LogError($"Failed to call OnComplete on \"{step?.GetType().Name}\":\n{ex}");
                }
            }
        }
        
        public void ClearStepResults() {
            if (File.Exists(StepsResults.SavePath)) 
                File.Delete(StepsResults.SavePath);
        }
        
        public void AppendStepResult(IPatcherStep step, double elapsedSeconds) {
            var stepsResults = GetStepResults();
            stepsResults.Results.Add(new StepsResults.Result(step.GetType().FullName ?? "nil", elapsedSeconds));
            stepsResults.ElapsedSeconds += elapsedSeconds;
            SaveStepResults(stepsResults);
        }
        
        public void SetStartTime() {
            ClearStepResults();
            var stepsResults = GetStepResults();
            stepsResults.StartTime = DateTime.Now;
            stepsResults.EndTime = default;
            stepsResults.ElapsedSeconds = -1;
            SaveStepResults(stepsResults);
        }
        
        public void SetEndTime() {
            var stepsResults = GetStepResults();
            stepsResults.EndTime = DateTime.Now;
            SaveStepResults(stepsResults);
        }
        
        private StepsResults GetStepResults() {
            return StepsResults.FromPath(StepsResults.SavePath);
        }
        
        private void SaveStepResults(StepsResults stepsResults) {
            File.WriteAllText(StepsResults.SavePath, stepsResults.ToJson());
        }
    }
}