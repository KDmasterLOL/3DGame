using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public static event Action Use;

    private void Update()
    {
        if (lookVelocity.sqrMagnitude > 0.001)
            Look(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
    }

    [Header("Movement settings")]
    [SerializeField, Min(1)] private float _moveSpeed = 10;
    [SerializeField, Range(1, 100)] private float _lookSensitive = 10;
    [SerializeField, Range(1, 20)] private float _gravity = 3f, _jumpStrength = 10;
    [Header("Objects reference")]
    [SerializeField] private GameObject Head;
    [Space]
    [SerializeField] private float _damage = 10;

    private CharacterController _body;
    private DefaultInput _inputActions;

    private bool _isJump = false;
    private Vector3 _jumpVelocity = Vector3.zero, moveVec = Vector3.zero;
    private float _fallVelocity = 0;

    private Vector2 lookVelocity = Vector2.zero, moveVelocity = Vector2.zero;

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private void Move(float delta)
    {
        moveVec = Vector3.zero;
        if (_body.isGrounded == false)
            _fallVelocity -= _gravity * delta;
        if(_isJump && _jumpVelocity.sqrMagnitude > 0.001)
        {
            moveVec = _jumpVelocity;
        }
        else if (moveVelocity.sqrMagnitude > 0.001)
        {
            moveVelocity.Normalize();
            moveVec.x = moveVelocity.x;
            moveVec.z = moveVelocity.y;
            moveVec = _body.transform.TransformDirection(moveVec);
            moveVec *= _moveSpeed;
        }
        moveVec.y = _fallVelocity;

        if (_fallVelocity < 0 && _body.isGrounded && _isJump)
            _isJump = false;


        _body.Move(moveVec * delta);
    }


    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private void Jump()
    {
        print($"Jump, {_body.isGrounded}, {_fallVelocity}");
        if (_body.isGrounded)
        {
            _isJump = true;
            _jumpVelocity = moveVec;
            _fallVelocity = _jumpStrength;
        }
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private void Look(float delta)
    {
        lookVelocity *= (delta * _lookSensitive);
        transform.Rotate(0, lookVelocity.x, 0);

        var headRotation = Head.transform.localEulerAngles;
        headRotation.x -= lookVelocity.y;
        headRotation.x = Mathf.Clamp(headRotation.x > 180 ? headRotation.x - 360 : headRotation.x, -70, 70);
        Head.transform.localEulerAngles = headRotation;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private void Dead() => Destroy(gameObject);

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private void Shoot()
    {
        if (Physics.Raycast(Head.transform.position, Head.transform.forward, out var raycastHit, 100, LayerMask.GetMask("Character")))
        {
            if (raycastHit.transform.CompareTag(Tags.Enemy))
            {
                print($"{raycastHit.transform.name}");
                var enemy = raycastHit.transform.GetComponent<IEnemy>();
                enemy.TakeHit(_damage);
            }
        }
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _body = GetComponentInChildren<CharacterController>();
        SetInput();
    }

    
    private void SetInput()
    {
        _inputActions = new();
        _inputActions.Player.Enable();
        _inputActions.Player.Move.performed += (c) => moveVelocity = c.ReadValue<Vector2>();
        _inputActions.Player.Move.canceled += (c) => moveVelocity =  Vector2.zero;
        _inputActions.Player.Look.performed += (c) => lookVelocity = c.ReadValue<Vector2>();
        _inputActions.Player.Look.canceled += (c) => lookVelocity = Vector2.zero;
        _inputActions.Player.Fire.performed += (c) => Shoot();
        _inputActions.Player.Jump.performed += (c) => Jump();
        _inputActions.Player.Use.performed += (c) => Use?.Invoke();
    }
}
