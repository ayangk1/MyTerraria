using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    Vector2 moveInput;
    public bool isMoving;
    public bool isInteract;
    
    public float moveSpeed;
    [SerializeField]
    private int jumpStep = 1;
    float jumpTime;
    private PlayerTouching playerTouching;
    private CharacterEvent characterEvent;
    private Character character;
    [Header("补偿速度")]
    public float lightSpeed;//轻击速度

    public float stepSpeed;
    public float heavySpeed;
    [Header("打击感")]
    public float shakeTime;
    public int lightPause;
    public float lightStrength;
    public int heavyPause;
    public float heavyStrength;

    [Space]
    public float interval = 2f;
    public bool isHit;
    public bool isDash;
    public bool isJump;
    
    public bool isWallJump;
    public bool isFaceRight => transform.localScale.x > 0 ? true : false;
    public float jumpForce;
    public float dashForce;
    private new Rigidbody2D rigidbody;
    
    public GameObject jumpEffect;
    readonly Keyboard keyboard = Keyboard.current;
    public GameController gameController;

    private void OnEnable()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerTouching = GetComponent<PlayerTouching>();
        characterEvent = GetComponent<CharacterEvent>();
        character = GetComponent<Character>();
        characterEvent.interactEvent.OnEventRaised += OnInteract;

        moveSpeed = 4;
    }

    private void OnDisable()
    {
        characterEvent.interactEvent.OnEventRaised -= OnInteract;
    }

    private void OnInteract(bool m_bool)
    {
        isInteract = true;
    }

    void Update()
    {
        Run();
        Jump();
    }

    private void Run()
    {
        if (!character.canMove) return;

        rigidbody.velocity = new Vector2(moveInput.x * moveSpeed, rigidbody.velocity.y);
        
        SetFacingDir(moveInput);
        characterEvent.walkEvent.RaiseEvent(isMoving);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        isMoving = moveInput != Vector2.zero;
    }
    
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.action.IsPressed())
        {
            characterEvent.interactEvent.RaiseEvent(true);
        }
    }

    private void SetFacingDir(Vector2 moveInput)
    {
        if (moveInput.x > 0) transform.localRotation = Quaternion.Euler(0, 0, 0);
        else if (moveInput.x < 0) transform.localRotation = Quaternion.Euler(0, 180, 0);
    }
    
    public void Jump()
    {
        if (!character.canMove) return;

        jumpTime -= Time.deltaTime;
        if (keyboard.spaceKey.wasPressedThisFrame && playerTouching.IsGround && jumpStep == 1)
        {
            if (jumpTime <= 0)
                jumpTime = 0.5f;
            isJump = true;
            jumpStep += 1;
            rigidbody.velocity = new Vector2(0, jumpForce);
            characterEvent.JumpEvent.RaiseEvent(isJump);
        }
        else if (keyboard.spaceKey.wasPressedThisFrame && !playerTouching.IsGround && jumpStep == 2)
        {
            jumpStep = 1;
            rigidbody.velocity = new Vector2(0, jumpForce);
            characterEvent.JumpEvent.RaiseEvent(isJump);
        }
    }
    
    private void FixedUpdate()
    {
        if (jumpTime <= 0 && playerTouching.IsGround)
        {
            jumpStep = 1;
            isJump = false;
            characterEvent.JumpEvent.RaiseEvent(isJump);
        }
        if (playerTouching.isStep) isWallJump = true;
    }
}