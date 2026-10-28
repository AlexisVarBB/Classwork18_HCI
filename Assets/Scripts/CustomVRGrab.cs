using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Rigidbody))]
public class CustomVRGrab : MonoBehaviour
{
    [Header("Asignación de Controladores")]
    public Transform leftHandTransform;
    public Transform rightHandTransform;

    private Rigidbody rb;
    private Transform currentHand;
    private bool isGrabbed = false;

    private Vector3 lastPosition;
    private Vector3 throwVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        bool leftGripPressed = IsGripPressed(XRNode.LeftHand);
        bool rightGripPressed = IsGripPressed(XRNode.RightHand);

        if (!isGrabbed)
        {
            if (CanGrab(leftHandTransform) && leftGripPressed)
            {
                Grab(leftHandTransform);
            }
            else if (CanGrab(rightHandTransform) && rightGripPressed)
            {
                Grab(rightHandTransform);
            }
        }
        else
        {
            CalculateThrowVelocity();

            bool releasedLeft = (currentHand == leftHandTransform) && !leftGripPressed;
            bool releasedRight = (currentHand == rightHandTransform) && !rightGripPressed;

            if (releasedLeft || releasedRight)
            {
                Release();
            }
        }
    }

    void FixedUpdate()
    {
        if (isGrabbed && currentHand != null)
        {
            rb.MovePosition(currentHand.position);
            rb.MoveRotation(currentHand.rotation);
        }
    }

    private bool CanGrab(Transform hand)
    {
        if (hand == null) return false;
        return Vector3.Distance(transform.position, hand.position) < 0.2f;
    }

    private bool IsGripPressed(XRNode handNode)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(handNode);
        if (device.isValid)
        {
            if (device.TryGetFeatureValue(CommonUsages.gripButton, out bool isPressed))
            {
                return isPressed;
            }
        }
        return false;
    }

    private void Grab(Transform hand)
    {
        isGrabbed = true;
        currentHand = hand;
        rb.isKinematic = true; 
        lastPosition = hand.position;
    }

    private void Release()
    {
        isGrabbed = false;
        rb.isKinematic = false; 
        rb.linearVelocity = throwVelocity * 1.2f; 
        currentHand = null;
    }

    private void CalculateThrowVelocity()
    {
        if (currentHand != null)
        {
            throwVelocity = (currentHand.position - lastPosition) / Time.deltaTime;
            lastPosition = currentHand.position;
        }
    }
}
