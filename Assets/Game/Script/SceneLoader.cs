using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene
    {
        GameScene,
        LoadingMenu,
        MenuScene,
        LobbyScene,
        CharacterSelectScene
    }
    private static Scene targetScene;

    public static void LoadSceneUsingLoadingMenu(Scene scene)
    {
        targetScene = scene;
        SceneManager.LoadScene(Scene.LoadingMenu.ToString());
    }
    public static void LoadNetworkScene(Scene scene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
    }
    public static void LoadScene(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }

    public static Scene GetTargetScene()
    {
        return targetScene;
    }

}
