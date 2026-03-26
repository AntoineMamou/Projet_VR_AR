using UnityEngine;

public class Calibration
{
    public static void Calibrate(Transform objectToCalibrate, Transform poseToAlign, Transform referencePose)
    {
        // Step 1 : Calculation of true local position taking into account scale
        Vector3 scaledExpectedLocalPos = Vector3.Scale(poseToAlign.localPosition, objectToCalibrate.localScale);

        // Step 2 : Rotation
        objectToCalibrate.rotation = referencePose.rotation * Quaternion.Inverse(poseToAlign.localRotation);

        // Step 3 : Apply position
        Vector3 position = referencePose.position - objectToCalibrate.rotation * scaledExpectedLocalPos;
        objectToCalibrate.position = position;
    }
}