using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
public class Building : GSelection, IPointerMoveHandler
{
    Color myColor;
    public building build = new();
    Vector3Int lastPos;
    bool active = false;
    private void Awake()
    {
        myColor = gameObject.GetComponent<MeshRenderer>().material.color;
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        g = transform.parent.parent.GetComponent<GridTiles>();
        g.Enter(ToInt(transform.position), gameObject);
        active = true;
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        g.Exit(gameObject);
        active = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if(active && g.selMode == SelectionMode.build)
        {
            g = transform.parent.parent.GetComponent<GridTiles>();
            Vector3 v = Camera.main.ScreenToWorldPoint(
            Input.mousePosition + new Vector3(0, 0, Camera.main.transform.position.y - gameObject.transform.position.y));
            Vector3Int vInt = new(Mathf.FloorToInt(v.x), 1, Mathf.FloorToInt(v.z));
            if (lastPos != vInt)
            {
                g.Enter(vInt, gameObject);
            }
            lastPos = vInt;
        }
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
    }
    public void UpdText()
    {
        if (build.selected)
        {
            GameObject.Find("Info Window").GetComponent<InfoWindow>().UpdateResText(build);
        }
    }

    virtual public void FinishBuild()
    {
        build.constructed = true;
        build.localRes = new();
        changeColor(new Color()); // builds
        if (build.selected)
        {
            GameObject.Find("Info Window").GetComponent<InfoWindow>().UpdateView(this);
        }
    }

    public void Deconstruct(Vector3 instantPos)
    {
        if(build.constructed || build.localRes.ammount.Sum() > 0)
        {
            Chunk c = Instantiate(g.chunk, instantPos, Quaternion.identity, GameObject.Find("Chunks").transform).GetComponent<Chunk>();
            for (int i = 0; i < c.localRes.ammount.Length; i++)
            {
                c.localRes.ammount[i] = (build.localRes.ammount[i] / 2);
                c.localRes.ammount[i] += build.cost.ammount[i] / 2;
            }
        }
        Destroy(gameObject);
    }
    
    public void changeColor(Color color)
    {
        if (color != new Color()) // signal for restart of color
        {
            gameObject.GetComponent<MeshRenderer>().material.color = color;
        }
        else
        {
            if (build.constructed)
            {
                gameObject.GetComponent<MeshRenderer>().material.color = myColor;
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().material.color = color;
            }
        }
    }
    public virtual Resource GetDiff(Resource inventory)
    {
        Resource r = new(); 
        for (int i = 0; i < build.cost.ammount.Length; i++) // foreach different resource
        {
            r.ammount[i] = build.cost.ammount[i] - (build.localRes.ammount[i] + inventory.ammount[i]); // cost - (delivered + inventory)
        }
        return r;
    }
}