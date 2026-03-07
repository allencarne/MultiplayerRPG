using System;
using UnityEngine;

public class LeaveSession : MonoBehaviour
{
    public static event Action OnLeaveButton;

    public void LeaveButton()
    {
        OnLeaveButton?.Invoke();
    }
}
