using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
public class Research_Production : ProductionBuilding
{
    //private int temp_workers_count = 0; Already exists: ProductionBuilding.working
    private Research Research_Script;
    protected override void Awake()
    {
        base.Awake();
        Research_Script = GameObject.Find("Research Tree(Stays Active)").GetComponent<Research>();
    }
    
    protected override void Product()
    {
        /*
        Transform research_transform  = GameObject.Find("Research Tree(Stays Active)").transform;
        research_transform.GetComponent<Research>().research_progress += 10; //add 10 research points
        */
        pTime.currentTime = 0;
    }
    protected override void AfterProduction()
    {
       // base.AfterProduction();
    }
    public override void Work(Human h)
    {
        base.Work(h);
        Research_Script.number_of_researchers++;
    }
}
