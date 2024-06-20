using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

[Serializable]
public class FluidNetwork
{
    public List<Pipe> pipes = new();
    public List<Building> buildings = new();
    public int networkID = -1;

    public FluidNetwork()
    {
        CreateID();
    }
    public FluidNetwork(Pipe _pipe)
    {
        pipes.Add(_pipe);
        CreateID();
    }
    
    void CreateID()
    {
        if (MyGrid.fluidNetworks.Count > 0)
            networkID = MyGrid.fluidNetworks.Last().networkID + 1;
        else
            networkID = 0;
    }
    /// <summary>
    /// takes other network and merges it with this intance
    /// </summary>
    /// <param name="_mergeWith">network to merge with</param>
    public void Merge(FluidNetwork _mergeWith)
    {
        foreach(Pipe pipe in _mergeWith.pipes)
        {
            pipes.Add(pipe);
            pipe.network = this;
        }
        foreach(Building building in _mergeWith.buildings)
        {
            buildings.Add(building);
            // TODO: call to building
        }
        MyGrid.fluidNetworks.Remove(_mergeWith);
    }
    public void Split(Pipe spliter)
    {
        if (spliter.transform.childCount == 0)
        {
            MyGrid.fluidNetworks.Remove(spliter.network);
            return;
        }
        else if(spliter.transform.childCount > 1)
        {
            pipes.Remove(spliter);
            DoSplit(0, 1, spliter.transform);
        }
    }
    void DoSplit(int childA, int childB, Transform pipeTransform)
    {
        if (childA == pipeTransform.childCount || childB == pipeTransform.childCount)
            return;
        Pipe pipeA = pipeTransform.transform.GetChild(childA).GetComponent<PipePart>().connectedPipe;
        Pipe pipeB = pipeTransform.transform.GetChild(childB).GetComponent<PipePart>().connectedPipe;
        if (PathFinder.FindPath(new(pipeA.gameObject), new(pipeB.gameObject), typeof(Pipe)).Count == 0)
        {
            if(childA == 0)
            {
                FluidNetwork fluidNetwork = new();
                MyGrid.fluidNetworks.Add(fluidNetwork);
                fluidNetwork.changeNetwork(pipeB);
                DoSplit(childB, childB + 1, pipeTransform);
            }
            else
            {
                DoSplit(0, childB, pipeTransform);
            }
        }
        else
        {
            DoSplit(childA, childB+1, pipeTransform);
        }
    }
    private void changeNetwork(Pipe pipe)
    {
        if (pipe.network.networkID == -1)
            return;       
        pipe.network.pipes.Remove(pipe);
        pipes.Add(pipe);
        if (pipe.GetComponent<BuildPipe>())
            buildings.Add(pipe.GetComponent<BuildPipe>().connectedBuilding);
        pipe.network = this;

        foreach (Pipe connected in pipe.GetComponentsInChildren<PipePart>().Select(q => q.connectedPipe).Where(q=> q != null))
        {
            if (!connected)
                continue;
            if (connected.network.networkID != networkID)
            {
                changeNetwork(connected);
            }
        }
    }
}