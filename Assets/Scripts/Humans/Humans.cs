using UnityEngine;
using UnityEngine.Tilemaps;

public class Humans : MonoBehaviour
{
    //Don't missmatch with "Human" this is a script for the parent object, no inheritence thou
    [SerializeField] public Resources resources;
    [SerializeField] public Tilemap ops;
    [SerializeField] public Tilemap marks;
    [SerializeField] public GridTiles grid;
}
