using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class Cat_button : MonoBehaviour
{
    // Start is called before the first frame update
    public int category;
    void Start()
    {
        category = Variables.Object(transform).Get<int>("category");
    }

    public void ChangeChategory()
    {
        transform.parent.parent.GetComponent<Research>().selected_category = category;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
