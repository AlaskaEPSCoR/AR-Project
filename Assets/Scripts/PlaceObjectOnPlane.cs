using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]

public class PlaceObjectOnPlane : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    InputAction touchInput;

    private ARRaycastManager aRRaycastManager;
    private ARPlaneManager aRPlaneManager; 
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake(){
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();
        touchInput.performed += _ => { PlaceObject(); };
        //Debug.Log("Action triggered!");
    }
    private void OnEnable(){
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += FingerDown;
        touchInput.Enable();
    }
    private void OnDisable(){
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
        touchInput.Disable();
    }

    void PlaceObject(){
        Debug.Log("Action triggered!");
        foreach(ARRaycastHit hit in hits){
            Pose pose = hit.pose;
            GameObject obj = Instantiate(prefab, pose.position, pose.rotation);
            }
    }
    void FingerDown(EnhancedTouch.Finger finger) {
        if (finger.index != 0) return;

        if(aRRaycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon)){
            foreach(ARRaycastHit hit in hits){
                Pose pose = hit.pose;
                GameObject obj = Instantiate(prefab, pose.position, pose.rotation);

            if (aRPlaneManager.GetPlane(hit.trackableId).alignment ==PlaneAlignment.HorizontalUp){
                Vector3 prefabPosition = obj.transform.position;
                prefabPosition.y = 0f;
                Vector3 cameraPosition = Camera.main.transform.position;
                cameraPosition.y = 0f;
                Vector3 direction = cameraPosition - prefabPosition;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                obj.transform.rotation = targetRotation;
            }
            }
        }
    }

}
