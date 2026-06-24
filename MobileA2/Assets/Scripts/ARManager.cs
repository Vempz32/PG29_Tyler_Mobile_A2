using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class ARManager : MonoBehaviour
{
    [SerializeField] private GameObject placementPrefab;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private ARAnchorManager anchorManager;

    [SerializeField] private bool hidePlanes = true;
    
    private ARRaycastManager _raycastManager;
    private readonly List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    private GameObject _plane;

    void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
    }

    void OnEnable() => EnhancedTouchSupport.Enable();
    void OnDisable() => EnhancedTouchSupport.Disable();

    void Update()
    {
        if (Touch.activeTouches.Count == 0) return;
 
        var touch = Touch.activeTouches[0];
        if (touch.phase != TouchPhase.Began) return;
 
        if (_raycastManager.Raycast(touch.screenPosition, _hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = _hits[0].pose;
            PlaceObject(hitPose);
        }
    }
    
    private async void PlaceObject(Pose pose)
    {

        if (_plane)
        {
            _plane = Instantiate(placementPrefab, pose.position, pose.rotation);
        }
        else
        {
            _plane.transform.SetParent(null); 
            _plane.transform.SetPositionAndRotation(pose.position, pose.rotation);
        }

        var result = await anchorManager.TryAddAnchorAsync(pose);
 
        if (result.status.IsSuccess())
        {
            ARAnchor anchor = result.value;
            _plane.transform.SetParent(anchor.transform, worldPositionStays: true);
        }
        else
        {
            Debug.LogWarning("Anchor creation failed — object placed but will not be drift-corrected.");
        }
 
        if (hidePlanes)
            TogglePlaneVisuals(false);
    }
    private void TogglePlaneVisuals(bool visible)
    {
        foreach (var plane in planeManager.trackables)
            plane.gameObject.SetActive(visible);
    }
}
