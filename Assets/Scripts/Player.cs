using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Properties")]
    public float speed = 20.0f;
    public float hSensitivity = 3;
    public float vSensitivity = 1;
    public float verticalMinAngle = -20;
    public float verticalMaxAngle = 45;
    public float climbModifier = 1;
    public float cameraDistanceModifier = 5;
    public float relocateSpeed = 1;
    public int climbables;
    public bool relocateCamera = false;

    [Header("Objects")]
    public EventHandler eventHandler;
    public GameObject ball;
    public Transform ballCenter;
    public Transform cameraHolder; // for vertical rotation of camera
    public Transform camObject;

    /* Private objects */
    private Rigidbody rb;
    private BallStick ballScript;

    /* Vars */
    private bool pointerEnabled = false;
    private float horizontal;
    private float vertical;
    private float mousex;
    private float mousey;
    private float verticalRotation = 0;
    private float camOffset, radius;
    private Vector3 mov;
    private Vector3 pos;
    private Vector3 rot;
    private Vector3 camPosition;

    void Start()
    {
        rb = ball.GetComponent<Rigidbody>();
        ballScript = ball.GetComponent<BallStick>();

        MousePointer(pointerEnabled);
    }

    void FixedUpdate()
    {
        // Mouse pointer
        if (Input.GetKeyDown("escape"))
        {
            pointerEnabled = !pointerEnabled;
            MousePointer(pointerEnabled);
        }
        
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
        
        // Limit max speed - values found through experimentation
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, (8 + ballScript.mass / 10));

        /* Move player object */
        pos = new Vector3(ballCenter.position.x, ballCenter.position.y, ballCenter.position.z);
        transform.position = pos;

        /* Rotate player object - only horizontal */
        mousex = Input.GetAxis("Mouse X");
        rot = new Vector3(0, mousex * hSensitivity, 0);
        transform.Rotate(rot);

        /* Rotate camera holder - only vertical */
        mousey = Input.GetAxis("Mouse Y");
        verticalRotation += mousey * vSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, verticalMinAngle, verticalMaxAngle);
        cameraHolder.localEulerAngles = new Vector3(-verticalRotation, 0, 0);
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

    void MousePointer(bool enabled)
    {
        if (enabled)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
