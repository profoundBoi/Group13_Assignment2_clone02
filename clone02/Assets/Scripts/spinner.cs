using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinner : MonoBehaviour
{
    [SerializeField] float xRotation = 0f;
    [SerializeField] float yRotation = 0f;
    [SerializeField] float zRotation = 0f;
    void Start()
    {

    }

    void Update()
    {

        transform.Rotate(xRotation, yRotation, zRotation);
    }
}
