using UnityEngine;
using System.Collections;

public static class Helper
{

    public struct ClipPlanePoints
    {
        public Vector3 UpperLeft;
        public Vector3 UpperRight;
        public Vector3 LowerLeft;
        public Vector3 LowerRight;
    }

    public static ClipPlanePoints ClipPlaneAtNear(Vector3 pos)
    {
        ClipPlanePoints clipPlanePoints = new ClipPlanePoints();

        if (Camera.mainCamera == null) {
            return clipPlanePoints;
        }
        Transform transform = Camera.mainCamera.transform;

        float halfFOV = (Camera.mainCamera.fieldOfView / 2) * Mathf.Deg2Rad;
        float aspect = Camera.mainCamera.aspect;
        float distance = Camera.mainCamera.nearClipPlane;
        float height = distance * Mathf.Tan(halfFOV);
        float width = height * aspect;

        clipPlanePoints.LowerRight = pos + (transform.right * width);
        clipPlanePoints.LowerRight -= transform.up * height;
        clipPlanePoints.LowerRight += transform.forward * distance;

        clipPlanePoints.LowerLeft = pos - (transform.right * width);
        clipPlanePoints.LowerLeft -= transform.up * height;
        clipPlanePoints.LowerLeft += transform.forward * distance;

        clipPlanePoints.UpperRight = pos + (transform.right * width);
        clipPlanePoints.UpperRight += transform.up * height;
        clipPlanePoints.UpperRight += transform.forward * distance;

        clipPlanePoints.UpperLeft = pos - (transform.right * width);
        clipPlanePoints.UpperLeft += transform.up * height;
        clipPlanePoints.UpperLeft += transform.forward * distance;

        return clipPlanePoints;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        // make sure it runs at least once
        do
        {
            if (angle < -360) {
                angle += 360;
            }
            if (angle > 360) {
                angle -= 360;
            }

        } while (angle < -360 || angle > 360);

        return Mathf.Clamp(angle, min, max);
    }


}
