using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FluidTank : Building
{
    [Header("Fluid")]
    [SerializeField] NetworkAccesBuilding networkAcces = new();
    [SerializeField] Color fillColor;

    public override void UniqueID()
    {
        base.UniqueID();
        networkAcces.ID(transform.GetChild(2));
    }
    public override void PlaceBuilding(GridTiles gT)
    {
        base.PlaceBuilding(gT);
        networkAcces.PlacePipes(transform.GetChild(2));
    }
    public override bool CanPlace()
    {
        if (!networkAcces.ConnectPipes(transform.GetChild(2)) || !base.CanPlace())
            return false;
        return true;
    }
    public override void FinishBuild()
    {
        networkAcces.ConnectToNetwork(transform.GetChild(2));
        base.FinishBuild();
    }
    public override InfoWindow OpenWindow(bool setUp = false)
    {
        InfoWindow info = base.OpenWindow(setUp);
        if (info)
        {
            Transform storageMenu = info.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(1);
            if (setUp)
            {
                storageMenu.gameObject.SetActive(true);
                storageMenu.GetChild(0).gameObject.SetActive(false);
                storageMenu.GetChild(1).gameObject.SetActive(true);
                storageMenu.GetChild(1).GetChild(0).GetChild(3).GetComponent<Image>().color = fillColor;
            }
            storageMenu.GetChild(1).GetChild(0).GetChild(3).GetComponent<Image>().fillAmount = (float)networkAcces.fluid.ammount[0] / (float)networkAcces.fluid.capacity[0];
            storageMenu.GetChild(1).GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = $"{networkAcces.fluid.ammount[0]} / {networkAcces.fluid.capacity[0]}";
            storageMenu.GetChild(1).GetChild(0).GetChild(3).GetChild(0).GetComponent<TMP_Text>().text = $"{networkAcces.fluid.ammount[0]} / {networkAcces.fluid.capacity[0]}";
        }
        return info;
    }
    public override void DestoyBuilding()
    {
        base.DestoyBuilding();
        networkAcces.DisconnectFromNetwork(transform.GetChild(2));
    }
    public override Fluid GetFluid()
    {
        return networkAcces.fluid;
    }
}
