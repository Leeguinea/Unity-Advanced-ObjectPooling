using UnityEngine;

/*
 * [역할]
 * 1. 캐릭터 애니메이션 관련 스크립트.
 */
public class PlayerAnimator : MonoBehaviour
{
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    //외부에서 호출할 애니메이션 제어 함수
    public void SetSpeed(float speed)
    {
        anim.SetFloat("Speed", speed);
    }

    public void TriggerJump()
    {
        anim.SetTrigger("Jump");
    }

    public void SetGrounded(bool isGrounded)
    {
        anim.SetBool("isGrounded", isGrounded);
    }
}
