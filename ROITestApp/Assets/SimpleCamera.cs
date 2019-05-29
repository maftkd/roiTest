using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCamera : MonoBehaviour
{
    public float moveSpeed, lookSpeed;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Horizontal") > 0)
        {
            transform.position += transform.right * Time.deltaTime * moveSpeed;
        }
        if (Input.GetAxis("Horizontal") < 0)
        {
            transform.position -= transform.right * Time.deltaTime * moveSpeed;
        }
        if (Input.GetAxis("Vertical") > 0)
        {
            transform.position += transform.forward * Time.deltaTime * moveSpeed;
        }
        if (Input.GetAxis("Vertical") < 0)
        {
            transform.position -= transform.forward * Time.deltaTime * moveSpeed;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position -= transform.up * Time.deltaTime * moveSpeed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position += transform.up * Time.deltaTime * moveSpeed;
        }

        transform.eulerAngles += new Vector3(-1f*Input.GetAxis("Mouse Y") * lookSpeed, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }
}
