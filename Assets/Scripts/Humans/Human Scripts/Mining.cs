using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class Mining : MasterClass
{
    public IEnumerator Dig()
    {
        Humans h = GetComponentInParent<Humans>();
        Rock r = gameObject.GetComponent<Human>().jData.objects.r;
        yield return new WaitForSeconds(r.data.hardness);
        Instantiate(r.data.chunk, new(r.transform.position.x, r.transform.position.y + 0.75f, r.transform.position.z), r.data.chunk.transform.rotation, r.transform.parent.parent.Find("Chunks")); // spawns chunk of resources
        h.grid.RemoveTiles(r); // destroyes the mined block
        h.GetComponent<JobQueue>().RemoveJob(gameObject.GetComponent<Human>().jData.ID); // removes job order
        StartCoroutine(gameObject.GetComponent<Human>().LookForNew());
    }
}
