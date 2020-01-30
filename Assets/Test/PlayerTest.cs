using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    [Header("Properties")]
    public float speed = 20.0f;
    public float hSensitivity = 3;
    public float vSensitivity = 1;
    public float climbModifier = 1;
    public float cameraDistanceModifier = 5;
    public float relocateSpeed = 1;
    public int climbables;
    public bool relocateCamera = false;

    [Header("Objects")]
    public EventHandlerTest eventHandler;
    public GameObject ball;
    public Transform ballCenter;
    public Transform cameraHolder; // for vertical rotation of camera
    public Transform camObject;

    /* Private objects */
    private Rigidbody rb;
    private BallStickTest ballScript;

    /* Vars */
    float horizontal;
    float vertical;
    float mousex;
    float mousey;
    float camOffset, radius;
    private Vector3 mov;
    private Vector3 pos;
    private Vector3 rot;
    private Vector3 camPosition;

    void Start()
    {
        rb = ball.GetComponent<Rigidbody>();
        ballScript = ball.GetComponent<BallStickTest>();
    }

    void FixedUpdate()
    {
        /* Move ball */
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        mov = transform.forward * vertical + transform.right * horizontal;

        // Upwards force if climbables exist
        if (climbables > 0 && (Mathf.Abs(vertical) > 0 || Mathf.Abs(horizontal) > 0))
        {
            mov += transform.up * climbModifier;
        }
        
        rb.AddForce(mov * speed * ballScript.mass);

        /* Move player object */
        pos = new Vector3(ballCenter.position.x, ballCenter.position.y, ballCenter.position.z);
        transform.position = pos;

        /* Rotate player object - only horizontal */
        mousex = Input.GetAxis("Mouse X");
        rot = new Vector3(0, mousex * hSensitivity, 0);
        transform.Rotate(rot);

        /* Rotate camera holder - only vertical */
        // TODO: this needs to be clamped
        mousey = Input.GetAxis("Mouse Y");
        rot = new Vector3(-mousey * vSensitivity, 0, 0); 
        cameraHolder.Rotate(rot);
    }

    void Update()
    {
        if (!relocateCamera)
            return;

        // Calculate camera position
        radius = ballScript.GetRadius();
        camOffset = radius * cameraDistanceModifier;
        camPosition = new Vector3(0, camOffset, -camOffset);

        // Slerp until it reaches target
        camObject.localPosition = Vector3.MoveTowards(camObject.localPosition, camPosition, relocateSpeed * Time.deltaTime);

        if (camPosition == camObject.localPosition)
            relocateCamera = false;
    }
}
