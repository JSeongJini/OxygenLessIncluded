using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private void Start()
    {
        transform.position = new Vector3(96f, 54f, transform.position.z);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow)&&transform.position.x > (int)10)
        {
            transform.position += Vector3.left * Time.deltaTime * 10;
        }else if (Input.GetKey(KeyCode.RightArrow)&&transform.position.x < (int)180)
        {
            transform.position += Vector3.right * Time.deltaTime * 10;
        }
        else if (Input.GetKey(KeyCode.DownArrow)&&transform.position.y > (int)10)
        {
            transform.position += Vector3.down * Time.deltaTime * 10;
        }
        else if (Input.GetKey(KeyCode.UpArrow)&&transform.position.y < (int)100)
        {
            transform.position += Vector3.up * Time.deltaTime * 10;
        }
    }
}
