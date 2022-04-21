using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMovementScript : MonoBehaviour
{
    public float speed = 20;
    public float rotationSpeed = 200;
    public float currentSpeed = 0;


    //public bool isControlEnabled;

    void Start()
    {
        //isControlEnabled = false;
    }
    void LateUpdate()
    {
        //if (isControlEnabled)
        //{
            //car controls
            float translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

            transform.Translate(0, 0, translation);
            currentSpeed = translation;

            transform.Translate(rotation, 0, 0);
            currentSpeed = rotation;
        //transform.Rotate(0, rotation, 0);

        //}
    }
}
