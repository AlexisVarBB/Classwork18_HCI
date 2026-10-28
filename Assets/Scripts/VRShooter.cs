using UnityEngine;
using UnityEngine.XR;

public class VRShooter : MonoBehaviour
{
    public Transform leftHandTransform;
    public Transform rightHandTransform;

    public GameObject cube;
    public Transform protectedRoot;

    public Vector3 aimOffset = Vector3.zero;
    public float sphereSize = 0.1f;
    public float shootSpeed = 8f;
    public float spawnOffset = 0.1f;
    public float sphereLifetime = 20f;

    GameObject cubeTemplate;
    bool prevLeftTrigger, prevRightTrigger, prevDestroy, prevRespawn;

    void Start()
    {
        if (cube != null)
        {
            cubeTemplate = Instantiate(cube, cube.transform.position, cube.transform.rotation);
            cubeTemplate.name = "CubeTemplate";
            cubeTemplate.SetActive(false);
        }
    }

    void Update()
    {
        if (Edge(GetButton(XRNode.LeftHand, CommonUsages.triggerButton), ref prevLeftTrigger))
            Shoot(leftHandTransform);
        if (Edge(GetButton(XRNode.RightHand, CommonUsages.triggerButton), ref prevRightTrigger))
            Shoot(rightHandTransform);

        bool destroyPressed = GetButton(XRNode.RightHand, CommonUsages.primaryButton)
                           || GetButton(XRNode.LeftHand, CommonUsages.primaryButton);
        if (Edge(destroyPressed, ref prevDestroy))
            DestroyAll();

        bool respawnPressed = GetButton(XRNode.RightHand, CommonUsages.secondaryButton)
                           || GetButton(XRNode.LeftHand, CommonUsages.secondaryButton);
        if (Edge(respawnPressed, ref prevRespawn))
            RespawnCube();
    }

    static bool GetButton(XRNode node, InputFeatureUsage<bool> usage)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        return device.isValid && device.TryGetFeatureValue(usage, out bool value) && value;
    }

    static bool Edge(bool now, ref bool previous)
    {
        bool justPressed = now && !previous;
        previous = now;
        return justPressed;
    }

    void Shoot(Transform hand)
    {
        if (hand == null) return;

        Quaternion aim = hand.rotation * Quaternion.Euler(aimOffset);
        Vector3 direction = aim * Vector3.forward;

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "Sphere";
        sphere.transform.position = hand.position + direction * spawnOffset;
        sphere.transform.localScale = Vector3.one * sphereSize;
        sphere.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);

        Rigidbody body = sphere.AddComponent<Rigidbody>();
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        body.linearVelocity = direction * shootSpeed;

        Destroy(sphere, sphereLifetime);
    }

    void DestroyAll()
    {
        Rigidbody[] bodies = FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
        foreach (Rigidbody body in bodies)
        {
            if (protectedRoot != null && body.transform.IsChildOf(protectedRoot)) continue;
            Destroy(body.gameObject);
        }
    }

    void RespawnCube()
    {
        if (cubeTemplate == null) return;

        CustomVRGrab[] existing = FindObjectsByType<CustomVRGrab>(FindObjectsSortMode.None);
        foreach (CustomVRGrab old in existing)
            Destroy(old.gameObject);

        GameObject newCube = Instantiate(cubeTemplate, cubeTemplate.transform.position, cubeTemplate.transform.rotation);
        newCube.name = "GrabCube";
        newCube.SetActive(true);
    }
}
