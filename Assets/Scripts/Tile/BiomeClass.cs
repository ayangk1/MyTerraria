using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeClass",menuName = "Terraria/BiomeClass")]
public class BiomeClass : ScriptableObject
{
    [field:SerializeField] public string biomeName { get; private set; }
    [field:SerializeField] public float heightAddtion { get; private set; }
    [field:SerializeField] public float heightMulti { get; private set; }
    [field:SerializeField,Range(0,1)] public float caveThreshold { get; private set; }
    [field:SerializeField,Range(0,1)] public float caveScale { get; private set; }
    [field:SerializeField,Range(0,1)] public float plantsThreshold { get; private set; }
    [field:SerializeField,Range(0,1)] public float plantsFrequncy { get; private set; }
    [field:SerializeField,Range(0,1)] public float treeThreshold { get; private set; }
    [field:SerializeField,Range(0,1)] public float treeFrequncy { get; private set; }
    [field:SerializeField] public Vector2Int treeHeight { get; private set; }
    [field:SerializeField,Range(0,1)] public float heightScale { get; private set; }
    [field:SerializeField] public OreClass[] ores { get; private set; }
    [field:SerializeField] public BiomeTileAtlas tileAtlas { get; private set; }
}
