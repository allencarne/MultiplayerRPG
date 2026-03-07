using System;
using UnityEngine;

public class LeaveLobby : MonoBehaviour
{
    public static event Action OnLeaveButton;

    public void LeaveButton()
    {
        OnLeaveButton?.Invoke();
    }
}
