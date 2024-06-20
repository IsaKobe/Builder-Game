using UnityEngine;
using UnityEngine.EventSystems;

public class GSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    protected GridTiles g;
    virtual public void OnPointerEnter(PointerEventData eventData)
    {
        g = transform.parent.parent.GetComponentInParent<GridTiles>();
        g.Enter(new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z)));
        g = null;
    }

    virtual public void OnPointerExit(PointerEventData eventData)
    {
        g = transform.parent.parent.GetComponentInParent<GridTiles>();
        g.Exit(new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z)));
        g = null;
    }

    virtual public void OnPointerUp(PointerEventData eventData)
    {
        g = transform.parent.parent.GetComponentInParent<GridTiles>();

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            g.Up();
            g = null;
        }
        else
        {
            g.BreakAction();
        }
    }

    virtual public void OnPointerDown(PointerEventData eventData)
    {
        g = transform.parent.parent.GetComponentInParent<GridTiles>();
        if (g.drag == false && eventData.button == PointerEventData.InputButton.Left)
        {
            g.Down(new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z)));
            g = null;
        }
        else
        {
        }
        
    }
}
