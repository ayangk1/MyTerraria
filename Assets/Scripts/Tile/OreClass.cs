using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "OreClass",menuName = "Terraria/OreClass")]
public class OreClass : TileClass
{
    [field:SerializeField,Range(0,1)]public float OreFrequency { get; private set; }
    [field:SerializeField,Range(0,1)]public float OreRadius { get; private set; }
    [field:SerializeField]public int MinY { get; private set; }
    [field:SerializeField]public int MaxY { get; private set; }
    [field:SerializeField,Range(0,1)]public float Offset { get; private set; }
}
