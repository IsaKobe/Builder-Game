using UnityEngine;

public class Building : MonoBehaviour
{
    public building build = new();

    /*public Resource getNeeds(int capacity)
    {
        Resource r = new();
        int i = 0;
        int j = 0;
        while (i < capacity) // resources in r is lower than capacity
        {
            if (j >= r.ammount.Length) // if j is out of bounds
            {
                break;
            }
            else if (build.cost.ammount[j] > 0 && build.cost.ammount[j] - build.localRes.ammount[j] != 0 && r.ammount[j] < build.cost.ammount[j] - build.localRes.ammount[j]) // if needed, 
            {
                r.ammount[j]++;
                i++;
            }
            else
            {
                j++;
            }
        }
        return r;
    }*/
    public Resource getDiff(Resource inventory)
    {
        Resource r = new Resource(); 
        for (int i = 0; i < build.cost.ammount.Length; i++) // foreach different resource
        {
            r.ammount[i] = build.cost.ammount[i] - (build.localRes.ammount[i] + inventory.ammount[i]); // cost - (delivered + inventory)
        }
        return r;
    }
}