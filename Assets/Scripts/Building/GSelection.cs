using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    protected GridTiles g;
    virtual public void OnPointerEnter(PointerEventData eventData)
    {
        g = transform.parent.parent.parent.gameObject.GetComponent<GridTiles>();
        g.Enter(new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z)));
        g = null;
    }

    virtual public void OnPointerExit(PointerEventData eventData)
    {
        g = transform.parent.parent.parent.gameObject.GetComponent<GridTiles>();
        g.Exit(new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z)));
        g = null;
    }

    virtual public void OnPointerUp(PointerEventData eventData)
    {
        g = transform.parent.parent.parent.gameObject.GetComponent<GridTiles>();
        g.Up();
        g = null;
    }

    virtual public void OnPointerDown(PointerEventData eventData)
    {
        g = transform.parent.parent.parent.gameObject.GetComponent<GridTiles>();
        g.Down(new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z)));
        g = null;
    }
}
