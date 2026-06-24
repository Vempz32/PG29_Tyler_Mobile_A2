using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
    
public class ARGuidanceUI : MonoBehaviour
{
    [Header("AR Foundation refs")]
    [SerializeField] private ARPlaneManager planeManager;

    [Header("UI refs")]
    [SerializeField] private GameObject instructionPanel; 
    [SerializeField] private TextMeshProUGUI instructionText;

    private bool _objectPlaced;

    public void NotifyObjectPlaced()
    {
        _objectPlaced = true;
    }


    public void ResetGuidance()
    {
        _objectPlaced = false;
    }

    void Update()
    {
        instructionPanel.SetActive(!_objectPlaced);
        if (_objectPlaced) return;

        instructionText.text = GetMessage();
    }

    private string GetMessage()
    {
        // --- Startup states ---
        switch (ARSession.state)
        {
            case ARSessionState.Unsupported:
                return "AR is not supported on this device.";
            case ARSessionState.CheckingAvailability:
                return "Checking AR support…";
            case ARSessionState.NeedsInstall:
            case ARSessionState.Installing:
                return "Installing AR support — this may take a moment…";
            case ARSessionState.SessionInitializing:
                return "Starting camera…";
        }
        
        if (ARSession.notTrackingReason != NotTrackingReason.None)
            return GetRecoveryMessage(ARSession.notTrackingReason);


        if (planeManager.trackables.count == 0)
            return "Slowly move your phone around to scan the floor or table.";

        return "Tap a highlighted surface to place your object.";
    }

    private string GetRecoveryMessage(NotTrackingReason reason)
    {
        switch (reason)
        {
            case NotTrackingReason.InsufficientLight:
                return "Too dark to track — move to a brighter area.";
            case NotTrackingReason.InsufficientFeatures:
                return "Point your camera at a surface with more detail or texture.";
            case NotTrackingReason.ExcessiveMotion:
                return "Move your phone more slowly.";
            case NotTrackingReason.Relocalizing:
                return "Hold still for a moment while AR relocates itself…";
            default:
                return "Tracking lost — move your phone slowly to recover.";
        }
    }
}
