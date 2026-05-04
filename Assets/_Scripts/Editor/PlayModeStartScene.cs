using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class PlayModeStartScene
{
    private const string StartScenePath = "Assets/Scenes/Menu.unity";

    static PlayModeStartScene()
    {
        SceneAsset startScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(StartScenePath);
        if (startScene != null)
        {
            EditorSceneManager.playModeStartScene = startScene;
        }
    }
}
