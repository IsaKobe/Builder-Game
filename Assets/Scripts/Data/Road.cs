using UnityEngine;
using UnityEngine.EventSystems;

public class Road : GSelection, IPointerEnterHandler
{
    Color c;
    private void Awake()
    {
        c = gameObject.GetComponent<MeshRenderer>().material.color;
    }
    override public void OnPointerEnter(PointerEventData eventData)
    {
        g = transform.parent.parent.GetComponentInParent<GridTiles>();
        if (g.markedTiles.Count > 0 || g.selMode == SelectionMode.build || g.selMode == SelectionMode.nothing)
        {
            g.Enter(new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z)), gameObject);
        }
        g = null;
    }

    override public void OnPointerExit(PointerEventData eventData)
    {
        g = transform.parent.parent.GetComponentInParent<GridTiles>();
        if (g.markedTiles.Count > 0 || g.selMode == SelectionMode.build || g.selMode == SelectionMode.nothing)
        {
            g.Exit(gameObject);
        }
        g = null;
    }

    override public void OnPointerUp(PointerEventData eventData)
    {
        g = transform.parent.parent.GetComponentInParent<GridTiles>();
        if ((g.markedTiles.Count > 0 || g.selMode == SelectionMode.build) && eventData.button == PointerEventData.InputButton.Left)
        {
            g.Up();
        }
        else if(eventData.button != PointerEventData.InputButton.Left)
        {
            g.BreakAction(transform.position);
        }
        g = null;
    }

    override public void OnPointerDown(PointerEventData eventData)
    {
        g = transform.parent.parent.GetComponentInParent<GridTiles>();
        if ((g.markedTiles.Count > 0 || g.selMode == SelectionMode.build) && eventData.button == PointerEventData.InputButton.Left)
        {
            g.Down(gameObject);
        }
        else if (eventData.button != PointerEventData.InputButton.Left)
        {
            g.BreakAction(transform.position);
        }
        g = null;
    }

}
