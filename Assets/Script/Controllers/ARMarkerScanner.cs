using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ARMarkerScanner : MonoBehaviour
{
    private ARTrackedImageManager imageManager;

    // Variables pour stocker la position vue par l'AR
    public Vector3 markerARPosition;
    public Quaternion markerARRotation;
    public bool isMarkerFound = false;

    public Transform activeMarkerTransform;

    private void Awake()
    {
        imageManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        imageManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnDisable()
    {
        imageManager.trackedImagesChanged -= OnImageChanged;
    }

    private void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        // Si l'image vient d'être trouvée ou est mise à jour
        foreach (var trackedImage in args.added)
        {
            UpdateMarkerInfo(trackedImage);
        }

        foreach (var trackedImage in args.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                UpdateMarkerInfo(trackedImage);
            }
        }
    }

    private void UpdateMarkerInfo(ARTrackedImage trackedImage)
    {
        markerARPosition = trackedImage.transform.position;
        markerARRotation = trackedImage.transform.rotation;
        isMarkerFound = true;

        activeMarkerTransform = trackedImage.transform;

        Debug.Log($"[CALIBRATION AR] Marqueur détecté à la position : {markerARPosition}");
    }
}