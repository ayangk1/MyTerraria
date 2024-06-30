using System;
using System.Xml.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameController : SingletonMonoBehaviour<GameController>
{
    public ItemEventSO attackEvent;

    public InventorySlot currSlot;

    public new GameObject camera;
    public float scrollSpeed;
    public float scrollLerpSpeed;
    public float dragSpeed;
    public float dragLerpSpeed;
    private readonly Mouse mouse = Mouse.current;
    private readonly Keyboard keyboard = Keyboard.current;
    public bool isUnflod;


    public Text text;

    private void OnDisable()
    {
        //characterEvent.attackEvent.OnEventRaised -= OnAttack;
    }

    private void OnAttack(Item item)
    {

    }

    void Update()
    {
        SwitchInventory();
        Building();
        Interact();
        CameraControl();
        UnflodInventory();

        var cell = TerrainManager.Instance.tilemaps[0].WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        text.text = "x :" + cell.x + "-" + "y :" + cell.y;
        text.transform.position = Input.mousePosition;
        // if (currSlot.GetItemInSlot() != null && currSlot.GetItemInSlot().item == AtlasManager.Instance.itemAtlas.torchItem)
        // {
        //     ItemManager.Instance.HoldTorch();
        //     preSlot = currSlot;
        // }
        // else if (preSlot != null && preSlot.GetItemInSlot() != null && preSlot.GetItemInSlot().item == AtlasManager.Instance.itemAtlas.torchItem)
        // {
        //     ItemManager.Instance.UnHoldTorch();
        //     preSlot = currSlot;
        // }
    }

    public void UnflodInventory()
    {
        if (keyboard.tabKey.wasPressedThisFrame && !isUnflod)
        {
            isUnflod = true;
            for (int i = 9; i < InventoryManager.Instance.inventorySize; i++)
                InventoryManager.Instance.inventorySlots[i].gameObject.SetActive(true);
            InventoryManager.Instance.RefrashAllItem();
        }
        else if (keyboard.tabKey.wasPressedThisFrame && isUnflod)
        {
            isUnflod = false;
            for (int i = 9; i < InventoryManager.Instance.inventorySize; i++)
                InventoryManager.Instance.inventorySlots[i].gameObject.SetActive(false);
        }

    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void CameraControl()
    {
        var cvc = camera.GetComponent<CinemachineVirtualCamera>();
        var cco = camera.GetComponent<CinemachineCameraOffset>();
        if (cvc.m_Lens.OrthographicSize is >= 2 and <= 9)
            cvc.m_Lens.OrthographicSize = Mathf.Lerp(cvc.m_Lens.OrthographicSize, cvc.m_Lens.OrthographicSize
                += mouse.scroll.value.y * scrollSpeed, scrollLerpSpeed);
        else if (cvc.m_Lens.OrthographicSize < 2) cvc.m_Lens.OrthographicSize = 2;
        else if (cvc.m_Lens.OrthographicSize > 9) cvc.m_Lens.OrthographicSize = 9;
        if (mouse.middleButton.isPressed)
        {
            camera.GetComponent<CinemachineVirtualCamera>().Follow = null;
            Vector3 moveDir = (Input.GetAxis("Mouse X") * -camera.transform.right + Input.GetAxis("Mouse Y") * -camera.transform.up);
            cco.m_Offset.x = Mathf.Lerp(cco.m_Offset.x, cco.m_Offset.x += moveDir.x * dragSpeed, dragLerpSpeed);
            cco.m_Offset.y = Mathf.Lerp(cco.m_Offset.y, cco.m_Offset.y += moveDir.y * dragSpeed, dragLerpSpeed);
        }
        if (keyboard.spaceKey.isPressed)
        {
            camera.GetComponent<CinemachineCameraOffset>().m_Offset = Vector3.zero;
            camera.GetComponent<CinemachineVirtualCamera>().Follow = GameObject.FindWithTag("Player").transform;
        }
    }

    public void SwitchInventory()
    {
        if (keyboard.digit1Key.wasPressedThisFrame) ForSwitchInventory(0);
        if (keyboard.digit2Key.wasPressedThisFrame) ForSwitchInventory(1);
        if (keyboard.digit3Key.wasPressedThisFrame) ForSwitchInventory(2);
        if (keyboard.digit4Key.wasPressedThisFrame) ForSwitchInventory(3);
        if (keyboard.digit5Key.wasPressedThisFrame) ForSwitchInventory(4);
        if (keyboard.digit6Key.wasPressedThisFrame) ForSwitchInventory(5);
        if (keyboard.digit7Key.wasPressedThisFrame) ForSwitchInventory(6);
        if (keyboard.digit8Key.wasPressedThisFrame) ForSwitchInventory(7);
        if (keyboard.digit9Key.wasPressedThisFrame) ForSwitchInventory(8);

    }

    public void ForSwitchInventory(int index)
    {
        for (int i = 0; i < InventoryManager.Instance.inventorySlots.Length; i++)
        {
            if (i == index)
                InventoryManager.Instance.inventorySlots[i].IsSelected = true;
            else InventoryManager.Instance.inventorySlots[i].IsSelected = false;
        }
    }


    public bool IsTreeTile(TileClass tileClass)
    {
        if (tileClass == null) return false;

        if (tileClass.tree) return true;

        return false;
    }

    public bool IsTorchTile(TileClass tileClass)
    {
        if (tileClass == null) return false;

        if (tileClass.torch) return true;

        return false;
    }

    public bool IsStructureTile(TileClass tileClass)
    {
        if (tileClass == null) return false;

        if (tileClass.structure) return true;

        return false;
    }

    private void Interact()
    {
        Item currItem = currSlot.GetComponentInChildren<InventoryItem>()?.item;
        if (currSlot == null || currSlot.GetItemInSlot() == null || currItem == null) return;

        if (currItem.type == ItemType.Interact)
        {
            if (mouse.leftButton.isPressed)
                attackEvent.RaiseEvent(currItem);
        }
    }

    public void RemoveTile()
    {

    }

    private void Building()
    {
        if (mouse == null) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        var onScreenPosition = mouse.position.ReadValue();
        var ray = Camera.main.ScreenPointToRay(onScreenPosition);
        var worldPoint = Camera.main.ScreenToWorldPoint(onScreenPosition);//世界坐标
        var hit = Physics2D.Raycast(new Vector2(ray.origin.x, ray.origin.y), Vector2.zero, Mathf.Infinity, ~1 << 2);

        //Addons,Background,Ground,Liquid
        var cellPoint = TerrainManager.Instance.tilemaps[0].WorldToCell(worldPoint);
        var revalWorldPos = TerrainManager.Instance.tilemaps[0].CellToWorld(cellPoint);
        var addonsTile = TerrainManager.Instance.GetTileClass(0, cellPoint.x, cellPoint.y);
        var wallTile = TerrainManager.Instance.GetTileClass(1, cellPoint.x, cellPoint.y);
        var groundTile = TerrainManager.Instance.GetTileClass(2, cellPoint.x, cellPoint.y);
        var liquidTile = TerrainManager.Instance.GetTileClass(3, cellPoint.x, cellPoint.y);
        var backgroundTile = TerrainManager.Instance.GetTileClass(4, cellPoint.x, cellPoint.y);

        Item currItem = currSlot.GetComponentInChildren<InventoryItem>()?.item;
        if (currSlot == null || currSlot.GetItemInSlot() == null || currItem == null) return;

        if (mouse.leftButton.wasReleasedThisFrame)
        {
            TerrainManager.Instance.HighTile(0, 0);
            UIManager.Instance.HideGamePrompt();
        }

        if (mouse.leftButton.isPressed)
        {
            //交互-->移除
            if (hit.collider != null)
            {
                TerrainManager.Instance.HighTile(cellPoint.x, cellPoint.y);
                

                if (addonsTile != null)
                {
                    if (currItem.tileName == "axe" && IsTreeTile(TerrainManager.Instance.tileDatas[0, cellPoint.x, cellPoint.y]))
                    {
                        TileClass tileClass = addonsTile;
                        UIManager.Instance.ShowGamePrompt(tileClass,cellPoint.x, cellPoint.y,revalWorldPos);
                
                        if(TerrainManager.Instance.demagedCondition[(int)tileClass.layer, cellPoint.x, cellPoint.y] >= 100)
                            TerrainManager.Instance.CollapseRemoveTile(tileClass, cellPoint.x, cellPoint.y);
                        else
                            TerrainManager.Instance.demagedCondition[(int)tileClass.layer, cellPoint.x, cellPoint.y] += 60 * Time.deltaTime;
                    }
                    else if (currItem.tileName == "pickaxe" && IsStructureTile(TerrainManager.Instance.tileDatas[0, cellPoint.x, cellPoint.y]))
                    {
                        TileClass tileClass = addonsTile;
                        UIManager.Instance.ShowGamePrompt(tileClass,cellPoint.x, cellPoint.y,revalWorldPos);

                        if(TerrainManager.Instance.demagedCondition[(int)tileClass.layer, cellPoint.x, cellPoint.y] >= 100)
                            TerrainManager.Instance.CollapseRemoveTile(tileClass, cellPoint.x, cellPoint.y);
                        else
                            TerrainManager.Instance.demagedCondition[(int)tileClass.layer, cellPoint.x, cellPoint.y] += 60 *Time.deltaTime;
                    }
                    else if (currItem.tileName == "pickaxe" && IsTorchTile(TerrainManager.Instance.tileDatas[0, cellPoint.x, cellPoint.y]))
                    {
                        TileClass tileClass = addonsTile;
                        UIManager.Instance.ShowGamePrompt(tileClass,cellPoint.x, cellPoint.y,revalWorldPos);

                        if(TerrainManager.Instance.demagedCondition[(int)tileClass.layer, cellPoint.x, cellPoint.y] >= 100)
                            TerrainManager.Instance.CollapseRemoveTile(tileClass, cellPoint.x, cellPoint.y);
                        else
                            TerrainManager.Instance.demagedCondition[(int)tileClass.layer, cellPoint.x, cellPoint.y] += 60 *Time.deltaTime;
                    }
                }

                if (wallTile != null)
                {
                    if (currItem.tileName == "hammer")
                    {
                        TileClass tileClass = wallTile;
                        UIManager.Instance.ShowGamePrompt(tileClass,cellPoint.x, cellPoint.y,revalWorldPos);
                        
                        if(TerrainManager.Instance.demagedCondition[(int)tileClass.layer, cellPoint.x, cellPoint.y] >= 100)
                            TerrainManager.Instance.RangeRemoveTile(tileClass, cellPoint.x, cellPoint.y, tileClass.coverRange);
                        else
                            TerrainManager.Instance.demagedCondition[(int)tileClass.layer, cellPoint.x, cellPoint.y] += 60 *Time.deltaTime;
                    }
                }

                if (groundTile != null)
                {
                    if (currItem.tileName == "pickaxe"
                        && !IsTreeTile(TerrainManager.Instance.tileDatas[0, cellPoint.x, cellPoint.y + 1]))
                    {
                        TileClass tileClass = groundTile;
                        UIManager.Instance.ShowGamePrompt(tileClass,cellPoint.x, cellPoint.y,revalWorldPos);
                        
                        if(TerrainManager.Instance.demagedCondition[(int)tileClass.layer, cellPoint.x, cellPoint.y] >= 100)
                            TerrainManager.Instance.RangeRemoveTile(tileClass, cellPoint.x, cellPoint.y, tileClass.coverRange);
                        else
                            TerrainManager.Instance.demagedCondition[(int)tileClass.layer, cellPoint.x, cellPoint.y] += 60 *Time.deltaTime;
                    }
                }
            }
        }

        if (mouse.leftButton.wasPressedThisFrame)
        {
            //交互-->添加
            if (currSlot.IsPlaceItem() && currSlot.GetItemInSlot().Count > 0
                                       && TerrainManager.Instance.JudgePlaceTile(currItem.buildingTile
                                           , cellPoint.x, cellPoint.y, currItem.buildingTile.coverRange))
            {
                TerrainManager.Instance.PlaceTile(currItem.buildingTile, cellPoint.x, cellPoint.y, currItem.buildingTile.coverRange);
                currSlot.GetItemInSlot().Count--;
            }
        }
    }

}
