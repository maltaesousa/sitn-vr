//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Basic throwable object
//
//=============================================================================

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Valve.VR;
using Valve.VR.InteractionSystem;

//-------------------------------------------------------------------------
[RequireComponent( typeof( Interactable ) )]
[RequireComponent( typeof( Rigidbody ) )]
[RequireComponent( typeof(VelocityEstimator))]
public class SitnMovable : MonoBehaviour
{
	[EnumFlags]
	[Tooltip( "The flags used to attach this object to the grabber." )]
	public SitnGrabber.AttachmentFlags attachmentFlags = SitnGrabber.AttachmentFlags.ParentToHand | SitnGrabber.AttachmentFlags.DetachFromOtherHand | SitnGrabber.AttachmentFlags.TurnOnKinematic;

    [Tooltip("The local point which acts as a positional and rotational offset to use while held")]
    public Transform attachmentOffset;

	[Tooltip( "How fast must this object be moving to attach due to a trigger hold instead of a trigger press? (-1 to disable)" )]
    public float catchingSpeedThreshold = -1;

    public ReleaseStyle releaseVelocityStyle = ReleaseStyle.GetFromHand;

    [Tooltip("The time offset used when releasing the object with the RawFromHand option")]
    public float releaseVelocityTimeOffset = -0.011f;

    public float scaleReleaseVelocity = 1.1f;

	[Tooltip( "When detaching the object, should it return to its original parent?" )]
	public bool restoreOriginalParent = false;

        

	protected VelocityEstimator velocityEstimator;
    protected bool attached = false;
    protected float attachTime;
    protected Vector3 attachPosition;
    protected Quaternion attachRotation;
    protected Transform attachEaseInTransform;

	public UnityEvent onPickUp;
    public UnityEvent onDetachFromHand;
    public UnityEvent<SitnGrabber> onHeldUpdate;

        
    protected RigidbodyInterpolation hadInterpolation = RigidbodyInterpolation.None;

    protected new Rigidbody rigidbody;

    [HideInInspector]
    public Interactable interactable;


    //-------------------------------------------------
    protected virtual void Awake()
	{
		velocityEstimator = GetComponent<VelocityEstimator>();
        interactable = GetComponent<Interactable>();



        rigidbody = GetComponent<Rigidbody>();
        rigidbody.maxAngularVelocity = 50.0f;


        if(attachmentOffset != null)
        {
            // remove?
            //interactable.handFollowTransform = attachmentOffset;
        }

	}


    //-------------------------------------------------
    protected virtual void OnHandHoverBegin( SitnGrabber grabber )
	{
        // "Catch" the throwable by holding down the interaction button instead of pressing it.
        // Only do this if the throwable is moving faster than the prescribed threshold speed,
        // and if it isn't attached to another grabber
        if ( !attached && catchingSpeedThreshold != -1)
        {
            float catchingThreshold = catchingSpeedThreshold * SteamVR_Utils.GetLossyScale(Player.instance.trackingOriginTransform);

            GrabTypes bestGrabType = grabber.GetBestGrabbingType();

            if ( bestGrabType != GrabTypes.None )
			{
				if (rigidbody.velocity.magnitude >= catchingThreshold)
				{
					grabber.AttachObject( gameObject, bestGrabType, attachmentFlags );
				}
			}
		}

	}


    //-------------------------------------------------
    protected virtual void OnHandHoverEnd( SitnGrabber grabber )
	{
 
	}


    //-------------------------------------------------
    protected virtual void HandHoverUpdate( SitnGrabber grabber )
    {
        GrabTypes startingGrabType = grabber.GetGrabStarting();
            
        if (startingGrabType != GrabTypes.None)
        {
			grabber.AttachObject( gameObject, startingGrabType, attachmentFlags, attachmentOffset );
        }
	}

    //-------------------------------------------------
    protected virtual void OnAttachedToHand( SitnGrabber grabber )
	{
        //Debug.Log("<b>[SteamVR Interaction]</b> Pickup: " + grabber.GetGrabStarting().ToString());

        hadInterpolation = this.rigidbody.interpolation;

        attached = true;

		onPickUp.Invoke();

		grabber.HoverLock( null );
            
        rigidbody.interpolation = RigidbodyInterpolation.None;
            
		velocityEstimator.BeginEstimatingVelocity();

		attachTime = Time.time;
		attachPosition = transform.position;
		attachRotation = transform.rotation;

	}


    //-------------------------------------------------
    protected virtual void OnDetachedFromHand(Hand grabber)
    {
        attached = false;

        onDetachFromHand.Invoke();

        grabber.HoverUnlock(null);
            
        rigidbody.interpolation = hadInterpolation;

        Vector3 velocity;
        Vector3 angularVelocity;

        GetReleaseVelocities(grabber, out velocity, out angularVelocity);

        rigidbody.velocity = velocity;
        rigidbody.angularVelocity = angularVelocity;
    }


    public virtual void GetReleaseVelocities(Hand grabber, out Vector3 velocity, out Vector3 angularVelocity)
    {
        if (grabber.noSteamVRFallbackCamera && releaseVelocityStyle != ReleaseStyle.NoChange)
            releaseVelocityStyle = ReleaseStyle.ShortEstimation; // only type that works with fallback grabber is short estimation.

        switch (releaseVelocityStyle)
        {
            case ReleaseStyle.ShortEstimation:
                velocityEstimator.FinishEstimatingVelocity();
                velocity = velocityEstimator.GetVelocityEstimate();
                angularVelocity = velocityEstimator.GetAngularVelocityEstimate();
                break;
            case ReleaseStyle.AdvancedEstimation:
                grabber.GetEstimatedPeakVelocities(out velocity, out angularVelocity);
                break;
            case ReleaseStyle.GetFromHand:
                velocity = grabber.GetTrackedObjectVelocity(releaseVelocityTimeOffset);
                angularVelocity = grabber.GetTrackedObjectAngularVelocity(releaseVelocityTimeOffset);
                break;
            default:
            case ReleaseStyle.NoChange:
                velocity = rigidbody.velocity;
                angularVelocity = rigidbody.angularVelocity;
                break;
        }

        if (releaseVelocityStyle != ReleaseStyle.NoChange)
            velocity *= scaleReleaseVelocity;
    }

    //-------------------------------------------------
    protected virtual void HandAttachedUpdate( SitnGrabber grabber)
    {


        if (grabber.IsGrabEnding(this.gameObject))
        {
            grabber.DetachObject(gameObject, restoreOriginalParent);

            // Uncomment to detach ourselves late in the frame.
            // This is so that any vehicles the player is attached to
            // have a chance to finish updating themselves.
            // If we detach now, our position could be behind what it
            // will be at the end of the frame, and the object may appear
            // to teleport behind the grabber when the player releases it.
            //StartCoroutine( LateDetach( grabber ) );
        }

        if (onHeldUpdate != null)
            onHeldUpdate.Invoke( grabber );
    }


    //-------------------------------------------------
    protected virtual IEnumerator LateDetach( SitnGrabber grabber )
	{
		yield return new WaitForEndOfFrame();

		grabber.DetachObject( gameObject, restoreOriginalParent );
	}


    //-------------------------------------------------
    protected virtual void OnHandFocusAcquired( SitnGrabber grabber )
	{
		gameObject.SetActive( true );
		velocityEstimator.BeginEstimatingVelocity();
	}


    //-------------------------------------------------
    protected virtual void OnHandFocusLost( SitnGrabber grabber )
	{
		gameObject.SetActive( false );
		velocityEstimator.FinishEstimatingVelocity();
	}
}

public enum ReleaseStyle
{
    NoChange,
    GetFromHand,
    ShortEstimation,
    AdvancedEstimation,
}

