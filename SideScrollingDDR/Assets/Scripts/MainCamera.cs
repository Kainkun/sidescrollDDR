using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    Player player;

    public LineRenderer TRlines;
    public LineRenderer BRlines;
    public LineRenderer BLlines;
    public LineRenderer TLlines;

    public float cameraSpeed = 1;

    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        if (Player.instance)
            player = Player.instance;
        else
            Debug.LogError("No Player");

        transform.position = player.transform.position;
    }

    private void Update()
    {
        SetLines();
    }

    void SetLines()
    {
        TRlines.SetPosition(0, player.transform.position - transform.position);
        BRlines.SetPosition(0, player.transform.position - transform.position);
        BLlines.SetPosition(0, player.transform.position - transform.position);
        TLlines.SetPosition(0, player.transform.position - transform.position);

        TRlines.SetPosition(1, cam.ViewportToWorldPoint(new Vector2(1, 1)) - transform.position);
        BRlines.SetPosition(1, cam.ViewportToWorldPoint(new Vector2(1, 0)) - transform.position);
        BLlines.SetPosition(1, cam.ViewportToWorldPoint(new Vector2(0, 0)) - transform.position);
        TLlines.SetPosition(1, cam.ViewportToWorldPoint(new Vector2(0, 1)) - transform.position);
    }


    Vector3 camGoTo;
    private void FixedUpdate()
    {
        camGoTo = Vector2.Lerp(transform.position, player.transform.position, Time.deltaTime * cameraSpeed);
        camGoTo.z = -10;
        transform.position = camGoTo;
    }
}
