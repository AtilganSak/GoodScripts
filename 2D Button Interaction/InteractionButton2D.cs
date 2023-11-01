using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionButton2D : MonoBehaviour
{
    public bool interactable = true;

    public UnityEvent onPressed;
    public UnityEvent onReleased;

    public void OnPressed()
    {
        if (interactable)
        {
            onPressed?.Invoke();        
        }
    }
    public void OnRelased()
    {
        if (interactable)
        {
            onReleased?.Invoke();            
        }
    }
}
