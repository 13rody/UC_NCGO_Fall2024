using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

[RequireComponent(typeof(CharacterController))]
public class ServerPlayerMovement : NetworkBehaviour
{
    [SerializeField] private Animator _myAnimator;
    [SerializeField] private NetworkAnimator _myNetworkAnimator;
    [SerializeField] private float _pSpeed;
    [SerializeField] private Transform _pTransform;

    public CharacterController _CC;
    private MyPlayerInputAction _playerInput;
    void Start()
    {
        if (_myAnimator != null)
        {
            _myAnimator = gameObject.GetComponent<Animator>();
        }
        if (_myNetworkAnimator != null)
        {
            _myNetworkAnimator = gameObject.GetComponent<NetworkAnimator>();
        }

        _playerInput = new MyPlayerInputAction();
        _playerInput.Enable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsOwner) return;
        Vector2 moveInput = _playerInput.PLayer.Movement.ReadValue<Vector2>();

        bool isJumping = _playerInput.PLayer.Jumping.triggered;
        bool isPunching = _playerInput.PLayer.Punching.triggered;
        bool isSprinting = _playerInput.PLayer.Sprinting.triggered;
        if(IsServer)
        {
            Move(moveInput,isJumping,isPunching,isSprinting);
        }
        else if (IsClient)
        {
            MoveServerRPC(moveInput, isJumping, isPunching, isSprinting);
        }

    }

    private void Move(Vector2 _input, bool isJumping, bool isPunching, bool isSprinting)
    {
        Vector3 _moveDirection = _input.x * _pTransform.right + _input.y * _pTransform.forward;

        _myAnimator.SetBool("IsWalking",_input.x != 0 || _input.y != 0);

        if(isJumping)
        {
            _myNetworkAnimator.SetTrigger("JumpTrigger");
        }

        if(isPunching)
        {
            _myNetworkAnimator.SetTrigger("PunchTrigger");
        }

        _myAnimator.SetBool("isSprinting", isSprinting);

        if (isSprinting)
        {
            _CC.Move(_moveDirection * (_pSpeed * 1.3f) * Time.deltaTime);
        }
        else
        {
            _CC.Move(_moveDirection * _pSpeed * Time.deltaTime);
        }
    }

    [Rpc(SendTo.Server)]
    private void MoveServerRPC(Vector2 _input, bool isJumping, bool isPunching, bool isSprinting)
    {
        Move(_input, isJumping, isPunching, isSprinting);
    }
}
