using UnityEngine;

public class Rock : GSelection
{
    public rock data;
    Color c;
    private void Awake()
    {
        c = gameObject.GetComponent<MeshRenderer>().material.color;
    }
}