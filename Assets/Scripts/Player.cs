﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Control Options")]
    public Controller controller;

    [Header("Speeds")]
    public float HorizontalSpeed;
    public float VerticalSpeed;

    [Header("Held object options")]
    public GameObject HeldObject;
    public Vector2 HeldOffset;
    private List<Collider2D> colliders = new List<Collider2D>();
    private bool Holding;

    [Header("Controls")]
    public Control MoveHorizontal;
    public Control MoveVertical;
    public Control LookHorizontal;
    public Control LookVertical;
    public Control Grab;
    public Control Shoot;

    private Rigidbody2D rb;
    private float theta = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(controller.ControlState[MoveHorizontal] * HorizontalSpeed, controller.ControlState[MoveVertical] * -VerticalSpeed);
        if (colliders.Count > 0)
        {
            if (controller.ControlState[Grab] == 1)
            {
                colliders[0].transform.position = transform.TransformPoint(new Vector3(HeldOffset.x, HeldOffset.y));
                colliders[0].transform.rotation = transform.rotation;
                if (colliders[0].GetComponent<Gun>()?.held == false)
                {
                    colliders[0].GetComponent<Rigidbody2D>().isKinematic = true;
                    colliders[0].GetComponent<Rigidbody2D>().freezeRotation = true;
                    colliders[0].GetComponent<Gun>().held = true;
                }

                if (controller.ControlState[Shoot] == 1)
                {
                    Debug.Log("Pew!!");
                }
            }
            else
            {
                foreach (Collider2D item in colliders)
                {
                    if (colliders[0].GetComponent<Gun>()?.held == true)
                    {
                        colliders[0].GetComponent<Rigidbody2D>().isKinematic = false;
                        colliders[0].GetComponent<Rigidbody2D>().freezeRotation = false;
                        colliders[0].GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;
                        colliders[0].GetComponent<Gun>().held = false;
                    }
                }
            }
        }

        if (controller.ControlState[LookVertical] != 0 || controller.ControlState[LookHorizontal] != 0)
        {
            if (Mathf.Atan(controller.ControlState[LookVertical] / controller.ControlState[LookHorizontal]) / Mathf.PI == 0.5)
            {
                theta = - Mathf.PI / 2;
            }
            else if (Mathf.Atan(controller.ControlState[LookVertical] / controller.ControlState[LookHorizontal]) / Mathf.PI == -0.5)
            {
                theta = Mathf.PI / 2;
            }
            else
            {
                theta = Mathf.Atan(controller.ControlState[LookVertical] / controller.ControlState[LookHorizontal]);
            }
            

            if (controller.ControlState[LookHorizontal] > 0)
            {
                 theta += Mathf.PI;
            }

            theta -= Mathf.PI / 2;
            theta = theta * -360 / (2 * Mathf.PI);
        }
        
        transform.rotation = Quaternion.Euler(0, 0, theta);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!colliders.Contains(other)) { colliders.Add(other); }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        colliders.Remove(other);
    }
}
