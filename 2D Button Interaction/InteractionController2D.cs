using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionController2D : MonoSingleton<InteractionController2D>
{    
    bool activeInteraction;
    Camera cam;

    InteractionButton2D lastButton;

    private void OnEnable()
    {
        cam = Camera.main;
        activeInteraction = true;

        SceneManager.sceneLoaded += SceneLoaded;
    }
    private void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        cam = Camera.main;
    }
    private void Update()
    {
        if (!activeInteraction) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (cam != null)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D raycast = Physics2D.GetRayIntersection(ray);
                if (raycast.collider != null)
                {
                    if (raycast.collider.gameObject.TryGetComponent(out InteractionButton2D component))
                    {
                        lastButton = component;
                        lastButton.OnPressed();
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (lastButton != null)
            {
                lastButton.OnRelased();
                lastButton = null;
            }
        }
    }
    public void DeactiveInteraction() => activeInteraction = false;
    public void ActivateInteraction() => activeInteraction = true;
}
