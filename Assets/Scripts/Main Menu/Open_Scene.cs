using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Open_Scene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Open_Scenes(int Scene_ID)
    {
        SceneManager.LoadScene(Scene_ID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
