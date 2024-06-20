using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterClass : MonoBehaviour
{
    protected Vector3Int ToInt(Vector3 v)
    {
        return Vector3Int.FloorToInt(v);
    }
    protected Vector3 ToHalf(Vector3 v)
    {
        return new Vector3(v.x + 0.5f, v.y, v.z + 0.5f);
    }
}
