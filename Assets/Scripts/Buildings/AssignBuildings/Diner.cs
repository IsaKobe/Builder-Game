using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diner : ProductionBuilding
{
    int servings = 0;
    int product = 10;
    public override void FinishBuild()
    {
        base.FinishBuild();
    }
    protected override void Product()
    {
        servings += product;
    }
}
