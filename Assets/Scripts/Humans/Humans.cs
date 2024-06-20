using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;
public class Humans : MasterClass
{
    //Don't missmatch with "Human" this is a script for the parent object, no inheritence thou
    public Resources resources;
    public Tilemap ops;
    public Tilemap marks;
    public GridTiles grid;

   
    public void DayChange(bool day)
    {
        if (day)
        {
            for (int i = 0; i < transform.childCount; i++)// (Transform g in transform.GetComponentsInChildren<Transform>().ToList())
            {
                Transform t = transform.GetChild(i);
                for (int j = 0; j < t.childCount; j++)// (Human h in g.GetComponentsInChildren<Human>().ToList())
                {
                    t.GetChild(j).GetComponent<Human>().Day();
                }
            }
        }
        else
        {
            for(int i = 0; i < transform.childCount; i++)// (Transform g in transform.GetComponentsInChildren<Transform>().ToList())
            {
                Transform t = transform.GetChild(i);
                for(int j = 0; j < t.childCount; j++)// (Human h in g.GetComponentsInChildren<Human>().ToList())
                {
                    t.GetChild(j).GetComponent<Human>().Night();
                }
            }
        }
        
        
    }
}
