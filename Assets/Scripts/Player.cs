﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum Direction { Up, Down, Right, Left}

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Control Options")]
    public Controller controller; //The controller that his player is listening to

    [Header("Speeds")]
    public float HorizontalSpeed;
    public float VerticalSpeed; //The ohirzontal and vertical speed of the player

    [Header("Held object options")]
    public Gun HeldObject = null; //The object that the player is currently holding
    public float ThrowSpeed;

    public List<Collider2D> closeGuns = new List<Collider2D>(); //A list of all the guns in the trigger
    private bool CanDrop = true;

    [Header("Controls")]
    public Control MoveHorizontal;
    public Control MoveVertical;
    public Control LookHorizontal;
    public Control LookVertical;
    public Control Grab;
    public Control Shoot; //The controls for each of the respective actions

    [Header("Graphics")]
    public Sprite UpSprite;
    public Sprite DownSprite;
    public Sprite LeftSprite;
    public Sprite RightSprite; //The sprites for each of their respective directions
    private SpriteRenderer rend; //The sprite renderer attached to this gameobject
    private Direction CurrentDirection = Direction.Down; //The direction that the player is currently facing

    [Header("Health")]
    public float MaxHealth = 100; //The maximum health of the player
    public float Health; //The current health of the player
    public bool dead = false; //Whether the player is dead or not
    private float DamageTime; //How long since the enemy started doing damage
    public float DamageSpeed; //How long it takes an enemy to do damage

    [Header("Bark lines")]
    public GameObject BarkBubble;
    public float ShowTime = 1;

    private Rigidbody2D rb; //The rigidbody2d attached to this gameobject
    private float theta = 90; //The current angle of the rotation stick

    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();

        rend.sprite = DownSprite;

        Health = MaxHealth;
    }


    void Update()
    {       
        rb.velocity = new Vector2(controller.ControlState[MoveHorizontal] * HorizontalSpeed, controller.ControlState[MoveVertical] * -VerticalSpeed); //Set the movement speed according to the controller input        

        if (controller.GetControlDown(Grab) && closeGuns.Count > 0 && HeldObject == null && !dead)
        {//There is a gun close to us, and we are attempting to grab it, and we arent currently holding a gun, and we arent currently dead
            HeldObject = closeGuns[0].GetComponent<Gun>();
            CanDrop = false;
        }

        if (controller.ControlState[Grab] == 0)
        {//Let the player drop the gun after they release the grab button
            CanDrop = true;
        }

        if (controller.ControlState[Shoot] == 1 && HeldObject != null)
        {//We are trying to shoot, and we are holding a gun
            HeldObject.GetComponent<Gun>().Shooting = true;
        }
        else if (controller.ControlState[Shoot] == 0 && HeldObject != null)
        {//We are trying to not shoot, and we are holding a gun
            HeldObject.GetComponent<Gun>().Shooting = false;
        }

        if (controller.GetControlDown(Grab) && HeldObject != null && CanDrop)
        {//We are holding a gun, and trying to drop it
            HeldObject.GetComponent<Rigidbody2D>().velocity = new Vector2(ThrowSpeed * Mathf.Cos(theta * 2 * Mathf.PI / 360), ThrowSpeed * Mathf.Sin(theta * 2 * Mathf.PI / 360));
            HeldObject = null;
        }

        if (HeldObject != null)
        {//We are holding a gun, so move it around the player
            HeldObject.SetAngle(theta, transform.position);

        }

        //Maths to get the angle of the stick assigned to rotation
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

            theta += Mathf.PI;
            theta = theta * -360 / (2 * Mathf.PI);

            
        }

        //Set the current sprite to the correct one for the looking angle
        if (theta < -315 && theta > -405 && CurrentDirection != Direction.Left)
        {
            CurrentDirection = Direction.Left;
            rend.sprite = RightSprite;
        }
        else if (theta < -225 && theta > -315 && CurrentDirection != Direction.Up)
        {
            CurrentDirection = Direction.Up;
            rend.sprite = UpSprite;
        }
        else if (theta < -135 && theta > -225 && CurrentDirection != Direction.Right)
        {
            CurrentDirection = Direction.Right;
            rend.sprite = LeftSprite;
        }
        else if ((theta > -135 || theta < -405) && CurrentDirection != Direction.Down)
        {
            CurrentDirection = Direction.Down;
            rend.sprite = DownSprite;
        }
    }


    public void TakeDamage(float DamageAmount)
    {
        Debug.Log(Health + " " + (Health - DamageAmount) + " " + (MaxHealth / 2));
        if (Health > MaxHealth / 2 && Health - DamageAmount <= MaxHealth / 2)
        {
            Debug.Log("Showing bark line");
            StartCoroutine(ShowBarkLine("That's gonna sting", ShowTime));
        }

        if (Health > MaxHealth / 10 && Health - DamageAmount <= MaxHealth / 10)
        {
            Debug.Log("Showing bark line");
            StartCoroutine(ShowBarkLine("Make it stop!", ShowTime));
        }


        Health -= DamageAmount;
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        rend.color = new Color(1, 1, 1, 0.5f);
        dead = true;
        HeldObject = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            DamageTime = 0;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Enemy"))
        {
            DamageTime += Time.deltaTime;
        }

        if (DamageTime > DamageSpeed)
        {
            DamageTime = 0;
            TakeDamage(collision.gameObject.GetComponent<Enemy>().Damage);
        }
    }

    IEnumerator ShowBarkLine(string Line, float WaitTime)
    {
        BarkBubble.SetActive(true);
        BarkBubble.GetComponentInChildren<TextMeshProUGUI>().text = Line;
        BarkBubble.GetComponent<BarkBubble>().TrackingObject = gameObject;
        yield return new WaitForSeconds(WaitTime);
        BarkBubble.SetActive(false);
    }
}
