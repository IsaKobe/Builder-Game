using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Chunk : MasterClass
{
    public Resource localRes = new();
    public Human human;
    public void RemoveRes(int which) // removes set resource
    {
        localRes.ammount[which]--;
        if (localRes.ammount.Sum() == 0) // if none left destroy self
        {
            Destroy(gameObject);
        }
    }
    IEnumerator Clear() {
        while (human != null)
        {
            yield return new WaitForSeconds(10);
            if (human.planA.interest == null)
            {
                gameObject.GetComponent<Chunk>().human = null;
                break;
            }else if(human.planA.interest.transform != gameObject.transform)
            {
                gameObject.GetComponent<Chunk>().human = null;
                print(human);
                break;
            }
        }
    }

    public void AssingH(Human h)
    {
        human = h;
        StartCoroutine(Clear());
    }
}
