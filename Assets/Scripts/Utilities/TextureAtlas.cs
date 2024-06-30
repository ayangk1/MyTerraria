using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "TextureAtlas",menuName = "Atlas/TextureAtlas")]
public class TextureAtlas : ScriptableObject
{
    public Sprite inventoryNormal;
    public Sprite inventoryHighLight;

    [Header("Weapons")] 
    public Sprite bow;
    public Sprite axe;
    public Sprite sword;
    public Sprite pickaxe;
    public Sprite hammer;
}
