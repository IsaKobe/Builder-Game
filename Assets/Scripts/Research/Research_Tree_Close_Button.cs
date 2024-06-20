using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Research_Tree_Close_Button : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void Close()
    {
        transform.parent.gameObject.SetActive(false);
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
