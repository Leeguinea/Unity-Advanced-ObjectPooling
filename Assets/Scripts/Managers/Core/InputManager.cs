using UnityEngine;
using System;

/*
 * [용도]
 * 사용자의 키보드나 마우스 입력을 실시간으로 감지 및 전달
 * [역할]
 * 1. 입력을 받는게 아닌 받았다는 신호를 전달함.
 */
public class InputManager
{
    public Action KeyAction = null;

    public void OnUpdate()
    {
        if(Input.anyKey == false)
            return;

        if(KeyAction != null)
            KeyAction.Invoke();
    }
}
