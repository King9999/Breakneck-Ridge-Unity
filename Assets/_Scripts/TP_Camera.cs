using UnityEngine;
using System.Collections;

public class TP_Camera : MonoBehaviour {

    public static TP_Camera Instance;

    public Transform TargetLookAt;
    public float Distance = 25.0f;
    public float DistanceMin = 3.0f;    // zoom distance min
    public float DistanceMax = 50.0f;   // zoom distance max
    public float DistanceSmooth = 0.05f;
    public float DistanceResumeSmooth = 1.0f;

    public float X_MouseSensitivity = 5.0f;
    public float Y_MouseSensitivity = 5.0f;
    public float MouseWheelSensitivity = 24.0f;
    public float Y_MinLimit = -40.0f;   // vertical view lower bond
    public float Y_MaxLimit = 80.0f;    // vertical view upper bond
    public float X_Smooth = 0.05f;
    public float Y_Smooth = 0.1f;

    public float OcclusionDistanceStep = 0.5f;
    public int MaxOcclusionChecks = 10;

    private float velX = 0.0f;
    private float velY = 0.0f;
    private float velZ = 0.0f;
    private float mouseX = 0.0f;
    private float mouseY = 0.0f;
    private float velDistance = 0.0f;
    private float startDistance = 0.0f;
    private float desiredDistance = 0.0f;
    private float distanceSmoother = 0.0f;
    private float preOccludedDistance = 0.0f;

    private Vector3 desiredPosition = Vector3.zero;
    private Vector3 updatePosition = Vector3.zero;


	void Awake ()
    {
        Instance = this;
	}

    void Start()
    {
        // Make sure distance is a distance between the min max values
        Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
        startDistance = Distance;
        Reset();
    }
	
	void LateUpdate ()
    {
        if (TargetLookAt == null) {
            return;
        }

        HandlePlayerInput();

        // a loop to check for occlusion fix
        int iCount = 0;
        do {
            iCount++;
            CalculateDesiredPosition();
        } while (CheckIsOccluded(iCount));

        //CheckCameraPoints(TargetLookAt.position, desiredPosition);

        UpdatePosition();
	}


    void HandlePlayerInput()
    {
        var deadZone = 0.01f;

        // check for mouse RMB is down
        if (Input.GetMouseButton(1))
        {
            // Get mouse Axis input
            mouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity;
            mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;
        }
        else
        {
            // reset it behind the character
            mouseX = TP_Controller.Instance.transform.eulerAngles.y;
            mouseY = 10.0f;
        }

        // limit the mouse Y rotation
        mouseY = Helper.ClampAngle(mouseY, Y_MinLimit, Y_MaxLimit);

        // get input for mouse wheel with dead zone
        float mousewheelAxis = Input.GetAxis("Mouse ScrollWheel");
        if ((mousewheelAxis < -deadZone) || (mousewheelAxis > deadZone))
        {
            desiredDistance = Mathf.Clamp(Distance - mousewheelAxis * MouseWheelSensitivity, DistanceMin, DistanceMax);
            preOccludedDistance = desiredDistance;
            distanceSmoother = DistanceSmooth;
        }
    }


    void CalculateDesiredPosition()
    {
        // to make sure distance is reset, mainly for camera to resume back from camera occlusion
        ResetDesiredDistance();

        // evaluate distance with smoothing
        Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velDistance, distanceSmoother);

        // calculate desired position
        desiredPosition = CalculatePosition(mouseY, mouseX, Distance);
    }



    void UpdatePosition()
    {
        var posX = Mathf.SmoothDamp(updatePosition.x, desiredPosition.x, ref velX, X_Smooth);
        var posY = Mathf.SmoothDamp(updatePosition.y, desiredPosition.y, ref velY, Y_Smooth);
        var posZ = Mathf.SmoothDamp(updatePosition.z, desiredPosition.z, ref velZ, X_Smooth);
        updatePosition = new Vector3(posX, posY, posZ);
        transform.position = updatePosition;
        transform.LookAt(TargetLookAt);
    }


    public void Reset()
    {
        mouseX = 0.0f;
        mouseY = 10.0f;

        Distance = startDistance;
        desiredDistance = Distance;
        preOccludedDistance = Distance;
    }


    void ResetDesiredDistance()
    {
        // camera has been repositioned by the player after occlusion
        if (desiredDistance < preOccludedDistance) {

            Vector3 posVec = CalculatePosition(mouseY, mouseX, preOccludedDistance);
            
            // make sure roll back position is not occluded
            float nearestDistance = CheckCameraPoints(TargetLookAt.position, posVec);

            // make sure the hit object is not something behind the pre occluded location, as that does not matter
            // also if there is something else in the way, don't go all the way back to pre occluded.
            if (nearestDistance == -1 || nearestDistance > preOccludedDistance) {
                desiredDistance = preOccludedDistance;
            }
        }
    }


    Vector3 CalculatePosition(float rotX, float rotY, float dist)
    {
        // create direction vector to point behind the character
        Vector3 direction = new Vector3(0, 0, -dist);

        // calculate the rotation of mouse to scene
        Quaternion rotation = Quaternion.Euler(rotX, rotY, 0);

        return TargetLookAt.position + (rotation * direction);
    }



    bool CheckIsOccluded(int count)
    {
        bool isOccluded = false;
        float nearestDistance = CheckCameraPoints(TargetLookAt.position, desiredPosition);

        if (nearestDistance != -1)
        {
            if (count < MaxOcclusionChecks)
            {
                // move cam forward a little
                isOccluded = true;
                Distance -= OcclusionDistanceStep;

                // hard coded so that the camera don't wabble when too close
                if (Distance < 0.25) {
                    Distance = 0.25f;
                }
            } else {
                // the safe zone
                Distance = nearestDistance - Camera.mainCamera.nearClipPlane;
            }

            // don't smooth, because already arrived at a location, just need fixing occlution
            desiredDistance = Distance;
            distanceSmoother = DistanceResumeSmooth;
        }
        return isOccluded;
    }



    float CheckCameraPoints(Vector3 from, Vector3 to)
    {
        float nearestDistance = -1.0f; // camera never hit if return with this value;

        RaycastHit hitInfo;

        Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneAtNear(to);

        // draw lines in the editor to make it easier to visualize
        Debug.DrawLine(from, to + transform.forward * -camera.nearClipPlane, Color.red);
        Debug.DrawLine(from, clipPlanePoints.UpperLeft, Color.blue);
        Debug.DrawLine(from, clipPlanePoints.UpperRight, Color.blue);
        Debug.DrawLine(from, clipPlanePoints.LowerLeft, Color.blue);
        Debug.DrawLine(from, clipPlanePoints.LowerRight, Color.blue);
        Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight, Color.cyan);
        Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight, Color.cyan);
        Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft, Color.cyan);
        Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft, Color.cyan);

        // finding the nearest point
        if (Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo) && hitInfo.collider.tag != "Player") {
            nearestDistance = hitInfo.distance;
        }

        if (Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo) && hitInfo.collider.tag != "Player") {
            if (hitInfo.distance < nearestDistance || nearestDistance == -1) {
                nearestDistance = hitInfo.distance;
            }
        }

        if (Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo) && hitInfo.collider.tag != "Player") {
            if (hitInfo.distance < nearestDistance || nearestDistance == -1) {
                nearestDistance = hitInfo.distance;
            }
        }

        if (Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo) && hitInfo.collider.tag != "Player") {
            if (hitInfo.distance < nearestDistance || nearestDistance == -1) {
                nearestDistance = hitInfo.distance;
            }
        }

        // check bumper
        if (Physics.Linecast(from, to + transform.forward * -camera.nearClipPlane, out hitInfo) && hitInfo.collider.tag != "Player") {
            if (hitInfo.distance < nearestDistance || nearestDistance == -1) {
                nearestDistance = hitInfo.distance;
            }
        }

        return nearestDistance;
    }


    /*
     * Create a camera if it does not exist and position it at the location of look at object
     */
    public static void SetupCamera()
    {
        GameObject tmpCamera;
        GameObject tmpLookAt;
        TP_Camera myCamera;

        // define the camera
        if (Camera.mainCamera != null)
        {
            tmpCamera = Camera.mainCamera.gameObject;
        }
        else
        {
            tmpCamera = new GameObject("Main Camera");
            tmpCamera.AddComponent("Camera");
            tmpCamera.tag = "MainCamera";
        }

        // attach the script
        tmpCamera.AddComponent("TP_Camera");
        myCamera = tmpCamera.GetComponent("TP_Camera") as TP_Camera;

        // find look at target
        tmpLookAt = GameObject.Find("targetLookAt") as GameObject;

        if (tmpLookAt == null)
        {
            tmpLookAt = new GameObject("targetLookAt");
            tmpLookAt.transform.position = Vector3.zero;
        }


        // assign myCamera to target look at
        myCamera.TargetLookAt = tmpLookAt.transform;

    }

}
