using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
public class Building : GSelection, IPointerMoveHandler
{
    Color myColor;
    public Build build = new();
    Vector3Int lastPos;
    bool active = false;
    protected virtual void Awake()
    {
        myColor = gameObject.GetComponent<MeshRenderer>().material.color; // saves the original color
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
        g = GameObject.Find("Grid").GetComponent<GridTiles>();

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            g.Up();
            UpdText();
        }
        else
        {
            g.BreakAction(gameObject);
        }
    }
    public void UpdText()
    {
        if (build.selected) // if the building is open in the info window
        {
            GameObject.Find("Info Window").GetComponent<InfoWindow>().UpdateResText(this);
        }
    }

    virtual public void FinishBuild()
    {
        build.constructed = true; // sets constructed to true, clears resource for which it was built, and changes color to the original one
        build.localRes = new();
        changeColor(new Color()); // builds
        if (build.selected)
        {
            GameObject.Find("Info Window").GetComponent<InfoWindow>().UpdateView(this); // if selected, update the info window
            UpdText();
        }
    }

    public void Deconstruct(Vector3 instantPos)
    {
        if(build.constructed || build.localRes.ammount.Sum() > 0) // if there is anything to salvage
        {
            Chunk c = Instantiate(g.chunk, instantPos, Quaternion.identity, GameObject.Find("Chunks").transform).GetComponent<Chunk>();
            for (int i = 0; i < c.localRes.ammount.Length; i++)
            {
                if (build.constructed) // if constructed return half construction cost and all produced resources
                {
                    c.localRes.ammount[i] += build.cost.ammount[i] / 2;
                    c.localRes.ammount[i] += build.localRes.ammount[i];
                    continue;
                }
                c.localRes.ammount[i] = (build.localRes.ammount[i] / 2); // return half of delivered resources
                
            }
        }
        Destroy(gameObject); // destroy self
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