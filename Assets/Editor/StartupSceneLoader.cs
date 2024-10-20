// using UnityEditor;
// using UnityEditor.SceneManagement;

// [InitializeOnLoad]
// public static class StartupSceneLoader
// {
//     static StartupSceneLoader()
//     {
//         EditorApplication.playModeStateChanged += LoadStartupScene;
//     }

//     private static void LoadStartupScene(PlayModeStateChange change)
//     {
//         if (change == PlayModeStateChange.ExitingEditMode)
//         {
//             EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
//         }

//         if (change == PlayModeStateChange.EnteredPlayMode)
//         {
//             if (EditorSceneManager.GetActiveScene().buildIndex != 0)
//             {
//                 EditorSceneManager.LoadScene(0);
//             }
//         }
//     }
// }
