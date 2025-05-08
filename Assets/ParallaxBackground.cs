using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject camera;
    [SerializeField] private float parallaxFactor;

    void Start()
    {
        camera = FindObjectOfType<Camera>().gameObject;
    }

    void Update()
    {
        transform.position = new Vector2(0, camera.transform.position.y * parallaxFactor);
    }
}
