using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Research research_script;
    public Building buildPrefab;
    public int unlocked_by; // The research that unlocks this building (-1 = unlocked on start)

    // selects tile to build
    public void SelPrefab()
    {
        GridTiles sel = GameObject.Find("Grid").GetComponent<GridTiles>();
        sel.buildingPrefab = buildPrefab;
        sel.ChangeSelMode(SelectionMode.build);
    }

    private void Awake()
    {
        research_script = GameObject.Find("Research Tree(Stays Active)").GetComponent<Research>();
    }

    private void Start()
    {
        if (unlocked_by != -1) //If not unlocked on start
        {
            GetComponent<Button>().interactable = false;
            research_script.researches[unlocked_by].unlocks.Add(transform);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject.Find("Build menus").transform.GetChild(2).GetComponent<PreBuildInfo>().DisplayInfo(buildPrefab, transform.GetComponent<RectTransform>().position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameObject.Find("Build menus").transform.GetChild(2).GetComponent<PreBuildInfo>().HideInfo();
    }
}
