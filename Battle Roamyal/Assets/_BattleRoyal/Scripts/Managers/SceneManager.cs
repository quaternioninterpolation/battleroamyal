// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 4:12 p.m.
// // --
using UnityEngine;
using System.Collections;
using System;
using MovementEffects;
using System.Collections.Generic;
using System.Linq;

public class SceneManager : SingletonMonobehaviour<SceneManager>
{
    private CameraFade cameraFade;

    private UnityEngine.SceneManagement.Scene currentScene;
    private BRScene currentSceneScript;

    protected override void Awake()
    {
        base.Awake();

        CreateCameraFade();
        var config = GetComponent<ConfigBase>();

        //Start the scenes
        GoToScene(config.GetScenes()[0], false);
    }

    private void CreateCameraFade()
    {
        GameObject fadeObject = new GameObject("Camera Fader");
        fadeObject.transform.SetParent(transform);
        cameraFade = fadeObject.AddComponent<CameraFade>();
    }

    public void GoToScene(string sceneName, bool animate, Action callback = null)
    {
        if (animate)
        {
            //Transition in
            cameraFade.FadeIn(() => 
            {
                LoadSceneIntoMemory(sceneName, () =>
                {
                    OnSceneStarted();
                    cameraFade.FadeIn(callback);
                });
            });
        }
        else
        {
            LoadSceneIntoMemory(sceneName, null);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (currentSceneScript != null)
        {
            currentSceneScript.OnSceneStop();
            currentSceneScript = null;
        }
    }

    private void OnSceneStarted()
    {
        currentSceneScript.OnSceneStart();
    }

    private void LoadSceneIntoMemory(string scene, Action callback)
    {
        Timing.RunCoroutine(LoadScene_Coroutine(scene, callback));
    }

    private IEnumerator<float> LoadScene_Coroutine(string scene, Action callback)
    {
        //Unload other scene
        if (currentSceneScript != null)
        {
            currentSceneScript.OnSceneStop();
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(currentScene);
            currentSceneScript = null;
        }

        AsyncOperation asyncOp = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        yield return Timing.WaitUntilDone(asyncOp);

        currentScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(scene);
        GameObject sceneGO = currentScene.GetRootGameObjects().Where((arg1) => arg1.GetComponent<BRScene>() != null).FirstOrDefault();
        if (sceneGO != null)
        {
            currentSceneScript = sceneGO.GetComponent<BRScene>();
        }
        else
        {
            Debug.LogException(new SceneManagerLoadException(scene, "Scene does not have a root object with a script extending BRScene..."));
        }

        callback?.Invoke();
    }

    // -- Exceptions

    private class SceneManagerLoadException : Exception
    {
        private string message;
                public SceneManagerLoadException(string level, string message)
            : base("Scene manager cannot load level "+level+"\n"+message)
        {
        }
    }

    private class SceneManagerInitException : Exception
    {
        private string message;
        public SceneManagerInitException(string message)
            : base("Scene manager cannot initialise\n"+message)
        {
        }
    }
}
