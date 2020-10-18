using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] float parallaxMulti;

    [SerializeField] Transform camera;
    Vector3 lastCameraPos;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main.transform;
        lastCameraPos = camera.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        transform.position += new Vector3((camera.position.x - lastCameraPos.x) * parallaxMulti,0,0);
        lastCameraPos = camera.position;
    }
}
