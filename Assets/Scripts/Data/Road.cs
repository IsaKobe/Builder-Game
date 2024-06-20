using UnityEngine;
using UnityEngine.EventSystems;

public class Road : GSelection, IPointerEnterHandler
{
    override public void OnPointerEnter(PointerEventData eventData)
    {
        g = transform.parent.parent.GetComponentInParent<GridTiles>();
        if (g.markedTiles.Count > 0 || g.selMode == selectionMode.build)
        {
            g.Enter(new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z)));
        }

        g = null;
    }

    override public void OnPointerExit(PointerEventData eventData)
    {
        g = transform.parent.parent.GetComponentInParent<GridTiles>();
        if (g.markedTiles.Count > 0 || g.selMode == selectionMode.build)
        {
            g.Exit(new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z)));
        }
        g = null;
    }

    override public void OnPointerUp(PointerEventData eventData)
    {
        g = transform.parent.parent.GetComponentInParent<GridTiles>();
        if ((g.markedTiles.Count > 0 || g.selMode == selectionMode.build) && eventData.button == PointerEventData.InputButton.Left)
        {
            g.Up();
        }
        else
        {
            g.BreakAction();
        }
        g = null;
    }

    override public void OnPointerDown(PointerEventData eventData)
    {
        g = transform.parent.parent.GetComponentInParent<GridTiles>();
        if ((g.markedTiles.Count > 0 || g.selMode == selectionMode.build) && eventData.button == PointerEventData.InputButton.Left)
        {
            g.Down(new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z)));
        }
        else
        {
            g.BreakAction();
        }
        g = null;
    }

}
