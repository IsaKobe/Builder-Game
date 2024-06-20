using UnityEngine;
using UnityEngine.EventSystems;

public class GSelection : MasterClass, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    protected GridTiles g;
    virtual public void OnPointerEnter(PointerEventData eventData)
    {
        g = GameObject.Find("Grid").GetComponent<GridTiles>();
        g.Enter(ToInt(transform.localPosition), gameObject);
        g = null;
    }

    virtual public void OnPointerExit(PointerEventData eventData)
    {
        g = GameObject.Find("Grid").GetComponent<GridTiles>();
        //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition).normalized;
        g.Exit(gameObject);
        g = null;
    }

    virtual public void OnPointerUp(PointerEventData eventData)
    {
        g = GameObject.Find("Grid").GetComponent<GridTiles>();

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            g.Up();
        }
        else
        {
            g.BreakAction(transform.position);
        }
    }

    virtual public void OnPointerDown(PointerEventData eventData)
    {
        g = GameObject.Find("Grid").GetComponent<GridTiles>();
        if (g.drag == false && eventData.button == PointerEventData.InputButton.Left)
        {
            g.Down(gameObject);
            
        }
        else
        {
        }
        
    }
}
