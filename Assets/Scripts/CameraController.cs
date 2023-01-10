using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 dir = Vector3.right;

    bool isMove = false;

    private void Start()
    {
        transform.position = new Vector3(96f, 54f, transform.position.z);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            SetDir(0);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            SetDir(1);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            SetDir(2);
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            SetDir(3);

        if (Input.GetKeyUp(KeyCode.LeftArrow) ||
            Input.GetKeyUp(KeyCode.RightArrow) ||
            Input.GetKeyUp(KeyCode.DownArrow) ||
            Input.GetKeyUp(KeyCode.UpArrow))
            OffMove();

        if (isMove)
            Move();
    }

    public void SetDir(int _dir) 
    {
        isMove = true;
        switch (_dir)
        {
            case 0:
                dir = Vector3.left;
                break;
            case 1:
                dir = Vector3.right;
                break;
            case 2:
                dir = Vector3.down;
                break;
            case 3:
                dir = Vector3.up;
                break;
        }
    }

    public void OffMove()
    {
        isMove = false;
    }


    public void Move()
    {
        if (transform.position.x > 10f &&
            transform.position.x < 180f &&
            transform.position.y > 10f &&
            transform.position.y < 180f)
        {
            transform.position += dir * Time.deltaTime * 10f;
        }
    }

    public void Left()
    {
        if(transform.position.x > 10f)
            transform.position += Vector3.left * Time.deltaTime * 10;
    }

    public void Right()
    {
        if (transform.position.x < 180f)
            transform.position += Vector3.right * Time.deltaTime * 10;
    }

    public void Down()
    {
        if (transform.position.y > 10)
            transform.position += Vector3.down * Time.deltaTime * 10;
    }

    public void Up()
    {
        if (transform.position.x < 100f)
            transform.position += Vector3.up * Time.deltaTime * 10;
    }
}
