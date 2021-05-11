using UnityEngine;
using System.Collections;

public class TP_Controller : MonoBehaviour {

    public static CharacterController CharController;
    public static TP_Controller Instance;

    // establish input dead zone
    private float deadZone = 0.1f;

    void Awake()
    {
        Instance = this;

        CharController = GetComponent("CharacterController") as CharacterController;
        CharController.height = 0.12f;
        CharController.radius = 0.02f;
        CharController.center = new Vector3(0, 0.06f, 0);

        TP_Camera.SetupCamera();
    }
	
	void Update ()
    {
        // no camera no update
        if (!Camera.mainCamera) {
            return;
        }

        GetLocomotionInput();
        HandleActionInput();
        TP_Motor.Instance.UpdateMotor();
	}

    void GetLocomotionInput()
    {
        TP_Motor.Instance.VertVelocity = TP_Motor.Instance.MoveVector.y;    // must be done before zeroing move vector
        //TP_Motor.Instance.MoveVector = Vector3.zero;
        TP_Motor.Instance.InputVector = Vector3.zero;
        TP_Motor.Instance.TurnInput = 0;

        // check dead zone on input and increment speed vector
        if (Input.GetAxis("Vertical") > deadZone || Input.GetAxis("Vertical") < deadZone)
        {
            TP_Motor.Instance.InputVector += new Vector3(0, 0, Input.GetAxis("Vertical"));
        }

        // increment rotation of the character
        if (Input.GetAxis("Horizontal") > deadZone || Input.GetAxis("Horizontal") < deadZone)
        {
            TP_Motor.Instance.TurnInput += Input.GetAxis("Horizontal");
            //TP_Motor.Instance.moveVector += new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        }
    }

    void HandleActionInput()
    {
        if (Input.GetButton("Jump"))
        {
            Jump();
        }
    }

    void Jump()
    {
        TP_Motor.Instance.Jump();
    }

}
