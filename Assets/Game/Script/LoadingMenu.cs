using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMenu : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private void Start()
    {
        Time.timeScale = 1;
        SceneLoader.Scene targetScene = SceneLoader.GetTargetScene();
        StartCoroutine(LoadAsynchronusly(targetScene));
        slider.value = 0f;
    }

    private void FixedUpdate()
    {
        slider.value += 0.02f;
    }
    private IEnumerator LoadAsynchronusly(SceneLoader.Scene targetScene)
    {
        yield return new WaitForSeconds(1.0f);
        if (KitchenNetworkMultiplayer.isMultiplayer)
        {
            SceneLoader.LoadScene(targetScene);
        }
        else
        {
            SceneLoader.LoadNetworkScene(targetScene);
        }
        
    }
}
