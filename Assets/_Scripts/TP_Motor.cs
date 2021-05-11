using UnityEngine;
using System.Collections;

public class TP_Motor : MonoBehaviour
{
    public static TP_Motor Instance;

    public ParticleAnimator SnowTrailParticle;

    public float TurnSpeed = 64.0f;
    public float MoveSpeed = 24.0f;
    public float JumpSpeed = 24.0f;
    public float SlideSpeedMultiplier = 100.0f; // Speed player slides multiplier

    public float Friction = 0.16f;      // friction of sliding
    public float Gravity = 9.8f * 6f;   // The gravity drop per sec, also the constant gravity acting on the player even on ground
    public float TerminalVelocity = 450.0f; // The max dropping velocity

    public float TurnFrictionThreshold = 10.0f; // Speed player turns
    public float SlideThreshold = 0.98f;        // The threshold for when to start sliding

    public Vector3 MoveVector { get; set; }
    public Vector3 InputVector { get; set; }
    public float VertVelocity { get; set; }
    public float TurnInput { get; set; }


    private float turnAngle;
    private Vector3 inputFriction;
    private Vector3 prevMoveDirection;

    private ParticleAnimator myTrailSnow;
    private TrailRenderer myTrailTrack;

    /**
     * Setup function
     */
    void Awake()
    {
        myTrailSnow = SnowTrailParticle;
        myTrailTrack = myTrailSnow.GetComponent("TrailRenderer") as TrailRenderer;

        inputFriction = Vector3.zero;
        Instance = this;
    }


    /**
     * Process user input to character movement motion
     */
    void ProcessInputMotion()
    {
        // Transform move vector
        InputVector = transform.TransformDirection(InputVector);

        // Normalize the move vector if magnitude is greater than zero
        if (InputVector.magnitude > 1)
        {
            InputVector = Vector3.Normalize(InputVector);
        }

        // Multiply input move vector by move speed
        InputVector *= MoveSpeed;
    }


    /**
     * Process all physical motion calculation
     */
    void ProcessPhysicsMotion()
    {
        // set default animation
        if (!animation.isPlaying) {
            animation.Play("idle");
        }

        if (TP_Controller.CharController.isGrounded)
        {
            // Check for sliding if applicable
            ApplySlide();

            // Apply friction when not sliding
            ApplyFriction();
        }

        // Start drawing snow particles
        ToggleTrail();

        // Reapply vertical velocity to move vector's y and gravity
        ApplyGravity();
        
        // Move the character in the world space
        TP_Controller.CharController.Move((MoveVector + InputVector) * Time.deltaTime);

    }




    /**
     * Process rotation
     */
    void ProcessRotation()
    {
        //turnVector = Vector3.RotateTowards(moveVector, turnVector, turnSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);

        // convert a numeric input into a rotational angle
        TurnInput *= TurnSpeed;
        TurnInput *= Time.deltaTime;
        turnAngle += TurnInput;
        turnAngle = Helper.ClampAngle(turnAngle, -360, 360);
        transform.Rotate(new Vector3(0, TurnInput, 0));
        
        //Vector3 turnVec = Vector3.Project(InputVector, Vector3.right) * TurnInput;
        //Debug.Log(turnVec + "===" + InputVector);
        //InputVector += turnVec;
        //inputFriction = InputVector * TurnInput;
    }


    /**
     * Apply friction to character on the horizontal movement zone
     */
    void ApplyFriction()
    {
        
        Vector3 frictionVector = Vector3.zero;
        Vector2 flatInput = new Vector2(InputVector.x, InputVector.z);
        Vector2 flatMove = new Vector2(MoveVector.x, MoveVector.z) + flatInput;

        float turnAngle = (flatInput == Vector2.zero && flatMove == Vector2.zero) ? 0 : Vector2.Angle(flatMove, flatInput);
        float actualFriction = Friction;
        //Debug.Log(flatMove + "###" + flatInput + ">>>>" + turnAngle + "==== INPUT EULER ===" + Quaternion.LookRotation(InputVector).eulerAngles.y);

        // calculate turn friction
        if ((turnAngle > TurnFrictionThreshold) && (turnAngle < 180))
        {
            actualFriction += actualFriction * (turnAngle / 180);
        }

        // apply normal ground friction
        if (MoveVector.x > 0)
        {
            frictionVector += new Vector3(-actualFriction, 0f, 0f);
        }
        else if (MoveVector.x < 0)
        {
            frictionVector += new Vector3(actualFriction, 0f, 0f);
        }

        if (MoveVector.z > 0)
        {
            frictionVector += new Vector3(0f, 0f, -actualFriction);
        }
        else if (MoveVector.z < 0)
        {
            frictionVector += new Vector3(0f, 0f, actualFriction);
        }

        
        MoveVector = MoveVector + frictionVector + inputFriction;
    }


    /**
     * Apply gravity based on preset Gravity varaible
     */
    void ApplyGravity()
    {
        // apply constant vertical velocity
        MoveVector = new Vector3(MoveVector.x, VertVelocity, MoveVector.z);

        // cap max vertical velocity
        if (MoveVector.y > -TerminalVelocity)
        {
            float speedDropMultiplier = 1 + (MoveVector.magnitude / 100);
            MoveVector = new Vector3(MoveVector.x, MoveVector.y - (Gravity * speedDropMultiplier * Time.deltaTime), MoveVector.z);
        }

        // stop dropping when ground is touched (The closer to 0 the threashhold of MoveVector.y, the more bouncy it is)
        if (TP_Controller.CharController.isGrounded && MoveVector.y < -Gravity)
        {
            MoveVector = new Vector3(MoveVector.x, -Gravity, MoveVector.z);
        }
    }


    /**
     * Apply sliding mechanics and calculation, this function should be called only when on ground.
     */
    void ApplySlide()
    {
        Vector3 slideDirection = Vector3.zero;
        RaycastHit hitInfo;

        // cast a ray from one unit up of here to find slide direction
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hitInfo))
        {
            // slide if within slide threashold
            if (hitInfo.normal.y < SlideThreshold)
            {
                //float tiltAngle = Vector3.Angle(Vector3.up, hitInfo.normal);
                //Vector3 projVec = Vector3.Project(Vector3.up, hitInfo.normal);
                slideDirection = new Vector3(hitInfo.normal.x, 0, hitInfo.normal.z);
                //Debug.Log("@" + tiltAngle + " =====" + projVec + " ||||| " + (projVec - Vector3.up) + "#" + hitInfo.normal);
            }

            // Adjust the character to rotate with ground
            Quaternion groundQuaternion = Quaternion.FromToRotation(Vector3.up, hitInfo.normal) * Quaternion.AngleAxis(turnAngle, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, groundQuaternion, Time.deltaTime * 10);
        }

        // controllable slide that increases speed based on the slope of sliding.
        MoveVector += slideDirection * (SlideSpeedMultiplier * (1 - hitInfo.normal.y));
        MoveVector = Vector3.Slerp(MoveVector, Vector3.Project(MoveVector, transform.forward.normalized), Time.deltaTime);
    }


    /**
     * Make character align with carema when moving. (deprecated)
     */
    void SnapAlignCharWithCam()
    {
        if (MoveVector.x != 0 || MoveVector.z != 0)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Camera.mainCamera.transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }


    /**
     * Allow the character to jump
     */
    public void Jump()
    {
        if (TP_Controller.CharController.isGrounded)
        {
            // apply vertical velocity as character is on the ground at start of jump.
            VertVelocity = JumpSpeed;
            MoveVector = new Vector3(MoveVector.x, VertVelocity, MoveVector.z);
            animation.Play("jump");
        }
    }

    /**
     * Toggle snow smoke trail upon touching snow ground
     */
    void ToggleTrail()
    {
        Vector3 pureMovementSpeed = new Vector3(MoveVector.x, 0, MoveVector.z) + InputVector;
        if ((myTrailSnow.particleEmitter.emit == false) && (TP_Controller.CharController.isGrounded) && (pureMovementSpeed.magnitude > 1f))
        {
            RaycastHit whatIsUnder;
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out whatIsUnder)) {
                if (whatIsUnder.collider == Terrain.activeTerrain.collider)
                {
                    myTrailSnow.particleEmitter.emit = true;
                    //myTrailTrack.enabled = true;
                }
            }
        }
        else
        {
            myTrailSnow.particleEmitter.emit = false;
            myTrailTrack.enabled = false;
        }
    }

	/**
	 * Check if player has reached end
	 */
    void CheckedWin()
    {
        if (transform.position.x > 4900f || transform.position.x < -4900f || transform.position.z > 4900f || transform.position.z < -4900f)
        {
            Debug.Log("GAME OVER !!!");
            HUD.gameWon = true;     //This line will pause the game and display win message + final time.
        }
    }


    /**
     * Main update loop
     */
    public void UpdateMotor()
    {
        ProcessInputMotion();
        ProcessPhysicsMotion();
        ProcessRotation();
		CheckedWin();
        //SnapAlignCharWithCam();
    }


}
