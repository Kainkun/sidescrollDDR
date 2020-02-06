using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    Player playerScript;

    public float cameraSpeed = 1;

    private void Awake()
    {
        
    }

    void Start()
    {
        if (Player.instance)
            playerScript = Player.instance;
        else
            Debug.LogError("No Player");
    }



    Vector3 camGoTo;
    private void FixedUpdate()
    {
        camGoTo = Vector2.Lerp(transform.position, playerScript.transform.position, Time.deltaTime * cameraSpeed);
        camGoTo.z = -10;
        transform.position = camGoTo;
    }
}
