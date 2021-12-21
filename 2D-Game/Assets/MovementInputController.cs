using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementInputController : MonoBehaviour
{
    private Vector2 movementInput;
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        Debug.Log(movementInput);      
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Jump Button now just pushed down");
        }

        if (context.performed)
        {
            Debug.Log("Jump is being held down");
        }
        if (context.canceled)
        {
            Debug.Log("Jump button has been released");
        }
    }

}
