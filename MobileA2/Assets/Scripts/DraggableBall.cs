using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class DraggableBall : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager; 
    [SerializeField] private Camera arCamera;                 

    private readonly List<ARRaycastHit> _planeHits = new List<ARRaycastHit>();
    private bool _isDragging;
    private int _activeTouchId = -1;

    void Awake()
    {
        if (arCamera == null) arCamera = Camera.main;
    }

    void OnEnable() => EnhancedTouchSupport.Enable();
    void OnDisable() => EnhancedTouchSupport.Disable();

    void Update()
    {
        foreach (var touch in Touch.activeTouches)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    TryStartDrag(touch);
                    break;

                case TouchPhase.Moved:
                    if (_isDragging && touch.touchId == _activeTouchId)
                        DragTo(touch.screenPosition);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (touch.touchId == _activeTouchId)
                    {
                        _isDragging = false;
                        _activeTouchId = -1;
                    }
                    break;
            }
        }
    }
    
    private void TryStartDrag(Touch touch)
    {
        if (_isDragging) return; 

        Ray ray = arCamera.ScreenPointToRay(touch.screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
        {
            _isDragging = true;
            _activeTouchId = touch.touchId;
        }
    }
    
    private void DragTo(Vector2 screenPosition)
    {
        if (raycastManager.Raycast(screenPosition, _planeHits, TrackableType.PlaneWithinPolygon))
        {
            transform.position = _planeHits[0].pose.position;
        }
    }
}