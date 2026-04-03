using System.Collections;
using TMPro;
using UnityEngine;

/*
 * [용도]
 * 키보드로 게임 내 플레이어 캐릭터의 "움직임"(이동, 충돌 처리 등)을 제어
 * [역할]
 * 1. InputManager에서 전달받은 입력 신호를 바탕으로 캐릭터를 "실제 이동"시킴.
 * 2. 음식과의 충돌을 판정함.
 * 3. 점수 관리 및 UI 업데이트 요청.[나중에 분리] 
 * [참조]
 * CharacterController: 플레이어의 물리적 이동을 담당 
 * PenaltySystem: 시간 경과에 따른 점수 감점 로직 담당
 */

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Move, 
        Jump
    }

    private PlayerAnimator playerAnim;
    private bool isGrounded;

    private PlayerState _state = PlayerState.Idle;
    private Vector3 _moveDir = Vector3.zero;

    Vector3 _gravityVelocity = Vector3.zero; // 누적될 중력 속도   

    [SerializeField] 
    float _speed = 10.0f;

    [SerializeField] 
    float _rotationSpeed = 10.0f;

    CharacterController _controller;
    PenaltySystem _penalty;

    void Start()
    {
        Application.targetFrameRate = 60;

        ////키보드 입출력
        _controller = GetComponent<CharacterController>(); 
        _penalty = GetComponent<PenaltySystem>();

        // 중복 등록 방지
        if (Managers.Input != null)
        {
            Managers.Input.KeyAction -= OnKeyboard;
            Managers.Input.KeyAction += OnKeyboard;
        }

        playerAnim = GetComponent<PlayerAnimator>();
    }

    void Update()
    {
        isGrounded = _controller.isGrounded;
        playerAnim.SetGrounded(isGrounded);

        switch(_state)
        {
            case PlayerState.Idle: UpdateIdle(); break;
            case PlayerState.Move: UpdateMove(); break;
            case PlayerState.Jump: UpdateJump(); break;
        }

        if(isGrounded && _gravityVelocity.y < 0)
        {
            if (_state != PlayerState.Jump)
                _gravityVelocity.y = -2f;
        }
        else
        {
            _gravityVelocity.y += -9.81f * Time.deltaTime;
        }

        _controller.Move((_moveDir + _gravityVelocity) * Time.deltaTime);   
    }

    void OnDestroy()
    {
        //캐릭터가 파괴될 때(씬 전환 등의 상황) 이벤트 연결을 끊어줌.
        //인게임 -> 메인화면 -> 인게임으로 돌아오는 과정에서 이동이 안되는 걸 방지하려고 넣음.
        if (Managers.Input != null)
        {
            Managers.Input.KeyAction -= OnKeyboard;
        }
    }

    void UpdateIdle()
    {
        playerAnim.SetSpeed(0);

        if(_moveDir.magnitude > 0.1f)
            _state = PlayerState.Move;

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            JumpState();
        }
    }

    void UpdateMove()
    {
        if(!Input.anyKey)
        {
            _moveDir = Vector3.zero;
        }

        playerAnim.SetSpeed(_moveDir.magnitude);

        //멈추면 idle
        if(_moveDir.magnitude <= 0.1f)
            _state = PlayerState.Idle;

        if (Input.GetButtonDown("Jump") && isGrounded)
            JumpState();
    }

    void UpdateJump()
    {
        if (isGrounded && _gravityVelocity.y <= 0)
            _state = PlayerState.Idle;
    }

    void JumpState()
    {
        _state = PlayerState.Jump;
        _gravityVelocity.y = 7.0f;
        playerAnim.TriggerJump();
    }


    void OnKeyboard()
    {
        Vector3 dir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) dir += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) dir += Vector3.back;
        if (Input.GetKey(KeyCode.A)) dir += Vector3.left;
        if (Input.GetKey(KeyCode.D)) dir += Vector3.right;

        if (dir.magnitude > 0.0001f)
        {
            dir = dir.normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), _rotationSpeed * Time.deltaTime);
            _moveDir = dir * _speed;
        }
        else
        {
            _moveDir = Vector3.zero;
        }
    }


    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        HandleCollection(hit.gameObject);
    }

    //충돌
    public void HandleCollection(GameObject go)
    {
        if (go == null) return;

        if (go.CompareTag("Target") || go.CompareTag("Avoid"))
        {
            //어떤 아이템을 먹으면 패널티 타이머를 리셋시킴
            if (_penalty != null)
                _penalty.ResetPenaltyTimer();

            int scoreGain = go.CompareTag("Target") ? 10 : -5;
            ScoreManager.Instance.ChangeScore(scoreGain);

            go.SetActive(false);
        }
    }

    
    
}