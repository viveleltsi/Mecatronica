using Dan.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DropperV1Behaviour : MonoBehaviour, IDropperBehaviour
{
    #region Falling variables

    /// <summary>
    /// At wich altitude the landing gear must be deployed
    /// </summary>
    public float DeployLandingGearAltitude = 10f;

    /// <summary>
    /// Acceleration when take off
    /// </summary>
    public float Acceleration = 10f;

    public float AltitudeToDisapear = 1000f;

    #endregion

    /// <summary>
    /// Link to the animator
    /// </summary>
    public Animator Animator;

    /// <summary>
    /// Falling speed by altitude
    /// </summary>
    public AnimationCurve FallingSpeedByAltitude;

    /// <summary>
    /// Actual vertical speed
    /// </summary>
    private float _verticalSpeed;

    /// <summary>
    /// Is the landing gear deployed
    /// </summary>
    private bool _landingGearDeployed = false;

    /// <summary>
    /// Is the thruster enabled
    /// </summary>
    private bool _thrusterEnabled = false;

    /// <summary>
    /// position where to land
    /// </summary>
    private Vector3 _positionToLand;

    /// <summary>
    /// Position with correction for the flip script
    /// </summary>
    private Vector3 _positionToLandCorrected;

    /// <summary>
    /// Callback when the dropper has finished to drop the building
    /// </summary>
    private Action _finishedDropCallback;

    /// <summary>
    /// All particles system
    /// </summary>
    [SerializeField]
    private List<ParticleSystem> _particleSystems;

    /// <summary>
    /// Smoke particle on ground whem dropper is landing
    /// </summary>
    [SerializeField]
    private ParticleSystem _groundSmoke;

    /// <summary>
    /// Prefab for flipping the contenant from vertical position to horizontal
    /// </summary>
    [SerializeField]
    private GameObject _verticalFlipper;

    /// <summary>
    /// Data descriptor of the content to drop
    /// </summary>
    private BuildingDescriptor _buildingDescriptor;

    /// <summary>
    /// Content to be dropped
    /// </summary>
    private GameObject _content;

    /// <summary>
    /// Script to handle the flip of the content
    /// </summary>
    private VerticalFlipperBehaviour _flipScript;

    /// <summary>
    /// State of the dropper
    /// </summary>
    private DropperV1Step _state = DropperV1Step.Idle;

    /// <summary>
    /// Start the falling process
    /// </summary>
    /// <param name="positionToLand"></param>
    /// <param name="DropFinished"></param>
    public void StartFalling(Vector3 positionToLand,BuildingDescriptor buildingDescriptor,  Action DropFinished)
    {
        _buildingDescriptor = buildingDescriptor;
        _finishedDropCallback = DropFinished;
        _state = DropperV1Step.Falling;
        _positionToLand = positionToLand;
        CreateDropContent();
    }

    public void FlyAway()
    {
        _state = DropperV1Step.Flying;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleSpeed();
        HandlePosition();
    }

    /// <summary>
    /// Handle the speed of the dropper
    /// </summary>
    private void HandleSpeed()
    {
        var altitude = GetAltitude();
        switch (_state)
        {
            case DropperV1Step.Idle:
                _verticalSpeed = 0f;
                break;
            case DropperV1Step.Falling:
                ComputeFallingSpeed();
                if(_landingGearDeployed == false
                    && altitude <= DeployLandingGearAltitude)
                {
                    DeployLandingGear();
                }
                if (altitude <= 0f)
                {
                    Landing();
                }
                if(_thrusterEnabled == false &&
                    altitude <= FallingSpeedByAltitude.keys.Last().time)
                {
                    EnableThruster(true);
                }
                break;
            case DropperV1Step.Landing:
                EnableThruster(false);
                _verticalSpeed = 0f;
                break;
            case DropperV1Step.Flying:
                EnableThruster(true);
                _verticalSpeed += Acceleration * Time.fixedDeltaTime;
                if (_landingGearDeployed
                    && altitude >= DeployLandingGearAltitude)
                {
                    DeployLandingGear(false);
                }
                if(altitude >= AltitudeToDisapear)
                {
                    Destroy(gameObject);
                    Destroy(this);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Create the drop content
    /// </summary>
    private void CreateDropContent()
    {
        _content = BuildingManager.Instance.CreateBuildingNotRegistred(_buildingDescriptor, transform);
        var center = _content.transform.Find("Center");
        var pivot = _content.transform.Find("RotationPivot");
        var decalage = Mathf.Abs(pivot.position.x - center.position.x - 2.8f);
        _positionToLandCorrected = transform.position = transform.position + Vector3.left * decalage;
        _positionToLandCorrected.y = _positionToLand.y;
    }

    /// <summary>
    /// Landing the dropper on ground
    /// </summary>
    private void Landing()
    {
        
        _state = DropperV1Step.Landing;
        transform.position = _positionToLandCorrected;
        EnableThruster(false);
        _verticalSpeed = 0f;
        Debug.Log("Start ground particle");
        _groundSmoke.Play();
        UnlockContent();
        Invoke("FlyAway", 3f);
        Invoke("StartFlipping", 5f);
        
    }

    private void StartFlipping()
    {
        _flipScript.StartFlipping();
    }

    private void UnlockContent()
    {
        //Flip business etc etc
        //Create the object to drop
        //Create flip business
        var flip = Instantiate<GameObject>(_verticalFlipper);
        _flipScript = flip.GetComponent<VerticalFlipperBehaviour>();
        _flipScript.AssignObject(_content,_positionToLand);
        
    }

    /// <summary>
    /// Drop the content on the ground
    /// </summary>
    private void DropFinished()
    {
        DropFinishedCallBack();
    }


    private void DropFinishedCallBack()
    {
        if (_finishedDropCallback != null)
        {
            _finishedDropCallback.Invoke();
        }
    }

    /// <summary>
    /// Deploy the landing gear (start animation)
    /// </summary>
    /// <param name="deploy"></param>
    private void DeployLandingGear(bool deploy = true)
    {
        _landingGearDeployed = deploy;
        Animator.SetBool("DisablingLandingGear", !_landingGearDeployed);
    }

    /// <summary>
    /// Compute the falling speed
    /// </summary>
    private void ComputeFallingSpeed()
    {
        var altitude = Mathf.Clamp(GetAltitude(),0, FallingSpeedByAltitude.keys.Last().time);
        _verticalSpeed = FallingSpeedByAltitude.Evaluate(altitude);
    }

    /// <summary>
    /// Get the current altitude from the destination
    /// </summary>
    /// <returns></returns>
    private float GetAltitude()
    {
        return transform.position.y - _positionToLand.y;
    }

    /// <summary>
    /// Enable thrusters or not
    /// </summary>
    /// <param name="enable"></param>
    private void EnableThruster(bool enable = true)
    {
        _thrusterEnabled = enable;
        foreach (var particle in _particleSystems)
        {
            if (enable)
                particle.Play();
            else
                particle.Stop();
        }
    }

    private void HandlePosition()
    {
        if (_verticalSpeed != 0f)
        {
            transform.position = transform.position + Vector3.up * _verticalSpeed * Time.fixedDeltaTime;
        }
    }

    private enum DropperV1Step
    {
        Idle,
        Falling,
        Landing,
        Flying
    }
}
