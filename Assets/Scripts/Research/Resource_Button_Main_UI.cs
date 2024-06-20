using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource_Button_Main_UI : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform research_tree;
    void Start()
    {
        research_tree = GameObject.Find("Research Tree(Stays Active)").transform.GetChild(0);
    }
    
    public void Open_Research()
    {
        research_tree.gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
