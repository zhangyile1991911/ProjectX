using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PlayModeLauncher : MonoBehaviour
{
    [MenuItem("自定义工具/启动")]
    public static void LoadSceneAndPlay()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("You cannot perform this action while in play mode.");
            return;
        }

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Resources/Boot.unity");
            EditorApplication.EnterPlaymode();
        }
    }
}
