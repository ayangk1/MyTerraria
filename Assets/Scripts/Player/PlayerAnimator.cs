using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public enum AnimName
{
    idle,

}
public class PlayerAnimator : MonoBehaviour
{
    private CharacterEvent characterEvent;
    private PlayerController playerController;
    private Character character;

    private Animator animator;
    public Animator animatorLegs;

    public GameObject itemInHand;
    public GameObject rightArm;

    public bool isRight;

    private float armAngle;

    private static readonly int IsMove = Animator.StringToHash("IsMove");
    private static readonly int Interact = Animator.StringToHash("Interact");
    private static readonly int ArmAngle = Animator.StringToHash("ArmAngle");

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        characterEvent = GetComponent<CharacterEvent>();
        playerController = GetComponent<PlayerController>();
        character = GetComponent<Character>();

        itemInHand.GetComponent<PolygonCollider2D>().enabled = false;
        characterEvent.walkEvent.OnEventRaised += OnWalk;
        characterEvent.JumpEvent.OnEventRaised += OnJump;
        characterEvent.interactEvent.OnEventRaised += OnInteract;
        characterEvent.attackEvent.OnEventRaised += OnAttack;
    }

    private void OnDisable()
    {
        characterEvent.walkEvent.OnEventRaised -= OnWalk;
        characterEvent.JumpEvent.OnEventRaised -= OnJump;
        characterEvent.interactEvent.OnEventRaised -= OnInteract;
        characterEvent.attackEvent.OnEventRaised -= OnAttack;
    }

    public void GenerateAttackItem()
    {
        var obj = ObjectPool.Instance.GetFormPool(2);
        obj.transform.position = itemInHand.transform.position;
    }

    public void AttackTexture()
    {
        switch (GameController.Instance.currSlot.GetItemInSlot().item.tileName)
        {
            case "bow":
                itemInHand.GetComponent<SpriteRenderer>().sprite = AtlasManager.Instance.textureAtlas.bow;
                break;
            case "axe":
                itemInHand.GetComponent<SpriteRenderer>().sprite = AtlasManager.Instance.textureAtlas.axe;
                itemInHand.GetComponent<PolygonCollider2D>().enabled = true;
                break;
            case "pickaxe":
                itemInHand.GetComponent<SpriteRenderer>().sprite = AtlasManager.Instance.textureAtlas.pickaxe;
                itemInHand.GetComponent<PolygonCollider2D>().enabled = true;
                break;
            case "sword":
                itemInHand.GetComponent<SpriteRenderer>().sprite = AtlasManager.Instance.textureAtlas.sword;
                itemInHand.GetComponent<PolygonCollider2D>().enabled = true;
                break;
            case "hammer":
                itemInHand.GetComponent<SpriteRenderer>().sprite = AtlasManager.Instance.textureAtlas.hammer;
                itemInHand.GetComponent<PolygonCollider2D>().enabled = true;
                break;
        }


    }

    private void InteractDir()
    {
        var arm = GameObject.FindWithTag("AttackPos").transform;
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var playerPos = arm.position;
        var targetdir = mousePos - playerPos;
        var dir = Vector2.Dot(targetdir.normalized, transform.right - playerPos);
        transform.localRotation = Quaternion.Euler(0, dir > 0 ? 180 : 0, 0);
        isRight = !(dir > 0);
    }

    public void AttackOver()
    {
        itemInHand.GetComponent<SpriteRenderer>().sprite = null;
        animator.SetFloat(ArmAngle, 0);
        itemInHand.GetComponent<PolygonCollider2D>().enabled = false;
    }

    private void OnAttack(Item item)
    {
        switch (item.tileName)
        {
            case "bow":
                var arm = GameObject.FindWithTag("AttackPos").transform;
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var playerPos = arm.position;
                var targetdir = mousePos - playerPos;
                var armdir = playerPos + new Vector3(0, -1) - playerPos;
                var angle = Vector2.Angle(armdir, targetdir);
                armAngle = angle;
                itemInHand.transform.localPosition = new Vector3(0, -0.1f, 0);
                itemInHand.transform.localRotation = Quaternion.Euler(0, 0, -90f);
                InteractDir();
                animator.Play("Arm");
                animator.SetFloat(ArmAngle, angle / 180);
                break;
            case "sword":
                CloseAct(new Vector3(0, -0.2f, 0),Quaternion.Euler(0, 0, -135f));
                break;
            case "axe":
                CloseAct(new Vector3(0, -0.2f, 0),Quaternion.Euler(0, 0, -135f));
                break;
            case "pickaxe":
                CloseAct(new Vector3(0, -0.2f, 0),Quaternion.Euler(0, 0, -135f));
                break;
            case "hammer":
                CloseAct(new Vector3(0, -0.2f, 0),Quaternion.Euler(0, 0, -135f));
                break;
            default:
                break;
        }
    }

    private void CloseAct(Vector3 pos,Quaternion qua)
    {
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "ArmClose")
        {
            itemInHand.transform.localPosition = pos;
            itemInHand.transform.localRotation = qua;
            animator.Play("ArmClose");
            InteractDir();
        }
    }

    private void OnJump(bool m_bool)
    {
        animatorLegs.SetBool("IsJump",m_bool);
    }

    private void OnWalk(bool m_bool)
    {
        if(!character.canMove) return;

        animator.SetBool(IsMove, m_bool);
        animatorLegs.SetBool(IsMove, m_bool);
    }
    private void OnInteract(bool m_bool)
    {

        //animator.Play("Interact");
    }
}
