using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionSystem : MonoBehaviour
{

    public Transform detectionPoint;
    private const float detectionRadius = 0.2f;
    public LayerMask detectionLayer;


    void Update()
    {
        if(DetectObject())
        {
            if(InteractInput())
            {
                Debug.Log("Interact");
                
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
                
            }
        }
    }

    bool InteractInput()
    {
       return Input.GetKeyDown(KeyCode.E);
    }

    bool DetectObject()
    {
        return Physics2D.OverlapCircle(detectionPoint.position,detectionRadius,detectionLayer);
    }
}
