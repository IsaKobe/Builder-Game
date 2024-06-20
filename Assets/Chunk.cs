using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Resource localRes = new();
    public Human human;
    public void removeRes(int which) // removes set resource
    {
        localRes.ammount[which]--;
        if (localRes.ammount.Sum() == 0) // if none left destroy self
        {
            Destroy(gameObject);
        }
    }
    IEnumerator clear() {
        while (human != null)
        {
            yield return new WaitForSeconds(10);
            if (human.planA.interests.Count == 0)
            {
                gameObject.GetComponent<Chunk>().human = null;
                print(human);
                break;
            }else if(human.planA.interests[0].transform != gameObject.transform)
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
        StartCoroutine("clear");
    }
}
