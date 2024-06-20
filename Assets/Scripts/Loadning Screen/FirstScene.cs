using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstScene : MonoBehaviour
{
    private void Awake()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync(1);
        load.completed += onLoad;
    }
    void onLoad(AsyncOperation aO)
    {
        GameObject.Find("Loading Screen").transform.GetChild(0).GetComponent<LoadingScreen>().LoadMainMenu();
    }
}
