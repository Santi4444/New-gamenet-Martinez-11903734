using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovementController : MonoBehaviour
{
    public Joystick joystick;
    //finger swipe
    public FixedTouchField fixedTouchField; 


    private RigidbodyFirstPersonController rigidbodyFirstPersonController;

    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        //gives the rbfpcontoller the script 
        rigidbodyFirstPersonController = this.GetComponent<RigidbodyFirstPersonController>();

        //allows the animation by getting the animator
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //use this in fixed update only if you are modifying movements with physics
    void FixedUpdate()
    {


        //makes it the joystick horizontal and vertical
        rigidbodyFirstPersonController.joystickInputAxis.x = joystick.Horizontal;
        rigidbodyFirstPersonController.joystickInputAxis.y = joystick.Vertical;
        //Touchfield is now where you touch it
        rigidbodyFirstPersonController.mouseLook.lookInputAxis = fixedTouchField.TouchDist;

        //animation for the fps view
        animator.SetFloat("horizontal", joystick.Horizontal);
        animator.SetFloat("vertical", joystick.Vertical);


        if (Mathf.Abs(joystick.Horizontal ) > 0.9 || Mathf.Abs(joystick.Vertical) > 0.9)
        {
            //says that you are running in the animator
            animator.SetBool("isRunning", true);

            //increase speed
            rigidbodyFirstPersonController.movementSettings.ForwardSpeed = 10;


        }
        else
        {
            //says you are running in the animator
            animator.SetBool("isRunning", false);

            //decrease speed
            rigidbodyFirstPersonController.movementSettings.ForwardSpeed = 5;

        }


    }
}
