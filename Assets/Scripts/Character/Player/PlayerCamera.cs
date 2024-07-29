using System;
using System.Collections;
using System.Collections.Generic;
using ToufFrame;
using UnityEngine;


public class PlayerCamera : MonoSingletonBase<PlayerCamera>
{
    public Camera cameraObject;
    public PlayerManager player;

    [SerializeField] private Transform cameraPivotTransform;

    [Header("CameraSettings")] [SerializeField]
    private float
        cameraSmoothSpeed = 1; // bigger this number , the longer for the camera to reach its position during movement

    [SerializeField] private float leftAndRightRotationSpeed = 220;
    [SerializeField] private float upAndDownRotationSpeed = 220;
    [SerializeField] private float minimunPivot = -20; // the lowest point you are able to look down
    [SerializeField] private float maximumPivot = 60; // the highest point you are able to look up
    [SerializeField] private float cameraCollisionRadius = 0.2f;
    [SerializeField] LayerMask collideWithLayers;


    [Header("Camera Values")] private Vector3 cameraVelocity;

    private Vector3
        cameraObjectPosition; //used for camera colllisions (moves the camera to this position upon colliding)

    [SerializeField] private float leftAndRightLookAngel;
    [SerializeField] private float upAndDownLookAngel;
    private float cameraZPosition; // values used for camera collision
    private float targetCameraZPosition; // values used for camera collision

    [Header("Lock On")] [SerializeField] private float lockOnRadius = 7;
    [SerializeField] private float minimumViewAngle = -35;
    [SerializeField] private float maximumViewAngle = 35;
    [SerializeField] private float lockOnTargetFollowSpeed = 0.2f;
    [SerializeField] private float setCameraHeightSpeed = 1;
    [SerializeField] private float unlockedCameraHeight = 1.65f;
    [SerializeField] private float lockedCameraHeight = 2.0f;
    private Coroutine cameraLockOnHeightCoroutine;
    private List<CharacterManager> availableTargets = new List<CharacterManager>();
    public CharacterManager nearestLockedOnTarget;
    public CharacterManager rightLockedOnTarget;
    public CharacterManager leftLockOnTarget;
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCamaeraActions()
    {
        if (player != null)
        {
            HandleFollowTarget();
            HandleRotations();
            HandleCollisions();
        }

    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position,
            ref cameraVelocity,
            cameraSmoothSpeed);
        transform.position = targetCameraPosition;
    }

    private void HandleRotations()
    {
        //IF LOCKED ON , FORCE ROTATION TOWARDS TARGET
        //ELSE ROTATE RETULALY
        if (player.playerNetworkManager.isLockedOn.Value)
        {
            Vector3 rotationDirection =
                player.playerCombatManager.currentTarget.characterCombatManager.lockedOnTransform.position -
                transform.position;
            rotationDirection.Normalize();
            rotationDirection.y = 0;
            
            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);
            
            //THIS ROTATES THE PIVOT OBJECT
            rotationDirection=player.playerCombatManager.currentTarget.characterCombatManager.lockedOnTransform.position-cameraPivotTransform.position;

            targetRotation = Quaternion.LookRotation(rotationDirection);
            cameraPivotTransform.transform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation,
                lockOnTargetFollowSpeed);
            
            leftAndRightLookAngel = cameraPivotTransform.eulerAngles.y;
            upAndDownLookAngel = cameraPivotTransform.eulerAngles.x;
        }
        //otherwise rotate normally
        else
        {
            //ROTATE LEFT AND RIGHT BASED ON HORIZONTAL MOVEMENT ON THE RIGHT JOYSTICK
            leftAndRightLookAngel +=
                (PlayerInputMgr.Instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
            //rotate up and down based on vertical movement on the right joystick
            upAndDownLookAngel -= (PlayerInputMgr.Instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
            //clamp the up and down rotation
            upAndDownLookAngel = Mathf.Clamp(upAndDownLookAngel, minimunPivot, maximumPivot);

            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotation;


            //ROTATE THE GAMEOBJECT LEFT AND RIGHT
            cameraRotation.y = leftAndRightLookAngel;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            // ROTATE THE PIVOT GAMEOBJECT UP AND DOWN
            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngel;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }
        
        
    }

    private void HandleCollisions()
    {
        targetCameraZPosition = cameraZPosition;
        RaycastHit hit;
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        if (!Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))return;
        
        
        if (hit.collider.CompareTag("Player")) return;
        float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
        targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        Debug.Log("camerapivot position: "+cameraPivotTransform.position+" hit point: "+hit.point);
        Debug.Log("distance from hit object: " + distanceFromHitObject+" targetCameraZPosition: "+targetCameraZPosition);
         
        
        
        if (MathF.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition,
            Time.deltaTime * 100);
        cameraObject.transform.localPosition = cameraObjectPosition;

    }

    public void HandleLocatingLockOnTargets()
    {
        float shortestDistance = Mathf.Infinity; // will be used to determine the target closest to the player
        float
            shortestDistanceOfRightTarget =
                Mathf.Infinity; // will be used to determine shortest distance on one axis to the right of current target（+
        float
            shortestDistanceOfLeftTarget =
                -Mathf.Infinity; // will be used to determine shortest distance on one axis to the left of current target（-

        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius,
            WorldUtilityManager.Instance.GetCharacterLayers());
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();
            if (lockOnTarget != null)
            {
                Vector3 lockOnTargetDirection = lockOnTarget.transform.position - player.transform.position;
                float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                float viewableAngle = Vector3.Angle(lockOnTargetDirection, cameraObject.transform.forward);
                float rightAngle = Vector3.Angle(lockOnTargetDirection, cameraObject.transform.right);

                if (lockOnTarget.isDead.Value)
                    continue;
                if (lockOnTarget.transform.root == player.transform.root)
                    continue;
                if (viewableAngle > maximumViewAngle || viewableAngle < minimumViewAngle)
                    continue;

                RaycastHit hit;
                //TODO: ADD LAYER MASK ENVIRO LAYERS ONLY
                if (Physics.Linecast(player.playerCombatManager.lockedOnTransform.position,
                        lockOnTarget.characterCombatManager.lockedOnTransform.position, out hit,
                        WorldUtilityManager.Instance.GetEnvironmentLayers()))
                {
                    if (hit.transform != lockOnTarget.transform)
                        continue;
                }

                availableTargets.Add(lockOnTarget);

            }
        }

        // we now sort throught our potential targets to see which one we lock onto first
        for (int i = 0; i < availableTargets.Count; i++)
        {
            if (availableTargets[i] != null)
            {
                float distanceFromTarget =
                    Vector3.Distance(player.transform.position, availableTargets[i].transform.position);
                // Vector3 lockTargestDirection = availableTargets[i].transform.position - player.transform.position;

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockedOnTarget = availableTargets[i];
                }

                if (player.playerNetworkManager.isLockedOn.Value)
                {
                    Vector3 relativeEnemyPosition =
                        player.transform.InverseTransformPoint(availableTargets[i].transform.position);
                    var distanceFromLeftTarget = relativeEnemyPosition.x;
                    var distanceFromRightTarget = relativeEnemyPosition.x;
                    
                    if(availableTargets[i] != player.playerCombatManager.currentTarget)continue;

                    if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget 
                        )
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockOnTarget = availableTargets[i];
                    }
                    else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                    {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockedOnTarget = availableTargets[i];
                    }
                }
                
                
            }
            else
            {

                player.playerNetworkManager.isLockedOn.Value = false;
            }
        }
    }

    public void SetLockCameraHeight()
    {
        if (cameraLockOnHeightCoroutine != null)
        {
            StopCoroutine(cameraLockOnHeightCoroutine);
        }

        cameraLockOnHeightCoroutine = StartCoroutine(SetCameraHeight());
    }
    
    public void ClearLockOnTargets()
    {
        nearestLockedOnTarget = null;
        leftLockOnTarget = null;
        rightLockedOnTarget = null;
        availableTargets.Clear();
    }

    public IEnumerator WaitThenFindNewTarget()
    {
        while (player.isPerformingAction)
        {
            yield return null;
        }
        ClearLockOnTargets();
        HandleLocatingLockOnTargets();

        if (nearestLockedOnTarget != null)
        {
            player.playerCombatManager.SetTarget(nearestLockedOnTarget);
            player.playerNetworkManager.isLockedOn.Value = true;
        }
        
    }

    private IEnumerator SetCameraHeight()
    {
        float duration = 1;
        float timer = 0;

        Vector3 velocity = Vector3.zero;
        Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockedCameraHeight);
        Vector3 newUnlockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x,unlockedCameraHeight);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            if (player != null)
            {
                if (player.playerCombatManager.currentTarget != null)
                {
                    cameraPivotTransform.transform.localPosition=Vector3.SmoothDamp(
                        cameraPivotTransform.transform.localPosition,newLockedCameraHeight,ref velocity,setCameraHeightSpeed);
                    cameraPivotTransform.transform.localRotation = Quaternion.Slerp(
                        cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0),
                        lockOnTargetFollowSpeed);
                }
                else
                {
                    cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition,
                         newUnlockedCameraHeight, ref velocity, setCameraHeightSpeed);
                }
            }

            yield return null;
        }

        if (player != null)
        {
            if (player.playerCombatManager.currentTarget != null)
            {
                cameraPivotTransform.transform.localPosition = newLockedCameraHeight;
                cameraPivotTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                cameraPivotTransform.transform.localPosition = newUnlockedCameraHeight;
            }
        }

        yield return null;
    }
 
}