using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float offsetX, offsetY;
    [SerializeField] float size, minSize, maxSize;
    [SerializeField] Transform Player;

    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = Player.position - Vector3.forward * 10;
        size = cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        ModifySize();
    }

    private void LateUpdate()
    {
        Follow();
    }

    void ModifySize()
    {
        if(Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            size += 0.1f;
            if(size > maxSize) size = maxSize;
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            size -= 0.1f;
            if(size < minSize) size = minSize;
        }

        cam.orthographicSize = size;
    }

    void Follow()
    {
        float relX = Player.position.x - this.transform.position.x;
        float relY = Player.position.y - this.transform.position.y;

        float x = this.transform.position.x;
        float y = this.transform.position.y;
        float z = this.transform.position.z;

        if (relX > offsetX)
        {
            x = Player.position.x - offsetX;
        }
        else if (relX < -offsetX)
        {
            x = Player.position.x + offsetX;
        }

        if (relY > offsetY)
        {
            y = Player.position.y - offsetY;
        }
        else if (relY < -offsetY)
        {
            y = Player.position.y + offsetY;
        }

        this.transform.position = new(x, y, z);
    }
}
