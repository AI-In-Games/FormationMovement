using UnityEngine;
using System.Collections;

public class ExtendedFlycam : MonoBehaviour
{ 
    public float cameraSensitivity = 360;
    public float normalMoveSpeed = 10;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    private void Start()
    {
        var startRotation = transform.localRotation.eulerAngles;
        rotationX = startRotation.y;
        rotationY = -startRotation.x;
        
        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
    }

    void Update()
    {
        if (!Input.GetMouseButton(1))
            return;

        rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

        var moveSpeed = normalMoveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            moveSpeed *= fastMoveFactor;
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            moveSpeed *= slowMoveFactor;

        transform.position += transform.forward * moveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        transform.position += transform.right * moveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;

        if (Input.GetKey(KeyCode.E)) { transform.position += transform.up * moveSpeed * Time.deltaTime; }
        if (Input.GetKey(KeyCode.Q)) { transform.position -= transform.up * moveSpeed * Time.deltaTime; }
    }
}
