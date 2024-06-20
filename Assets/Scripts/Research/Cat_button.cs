using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Cat_button : MonoBehaviour
{
    // Start is called before the first frame update
    public int category;
    void Start()
    {
    }

    public void ChangeChategory()
    {
        transform.parent.parent.parent.GetComponent<Research>().selected_category = category;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
