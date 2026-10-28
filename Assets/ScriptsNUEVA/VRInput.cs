using UnityEngine;
using UnityEngine.XR; // API básica de Unity para dispositivos XR (visor y controles)

// Clase "estática": no se pone en ningún objeto, solo sirve como caja de herramientas
// que cualquier otro script puede llamar, por ejemplo VRInput.GetBool(...).
public static class VRInput
{
    // Obtiene la posición y rotación de un dispositivo (cabeza, mano izquierda o derecha).
    // Devuelve true si el dispositivo está conectado y se pudo leer.
    public static bool TryGetPose(XRNode node, out Vector3 position, out Quaternion rotation)
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;

        // Pedimos a Unity el dispositivo que corresponde a ese "nodo" (cabeza, mano...).
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        if (!device.isValid) return false; // No está conectado o todavía no se detecta

        // Leemos posición y rotación. Cada TryGetFeatureValue devuelve true si pudo leerlo.
        bool hasPosition = device.TryGetFeatureValue(CommonUsages.devicePosition, out position);
        bool hasRotation = device.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation);
        return hasPosition && hasRotation;
    }

    // Lee un valor numérico de un control, por ejemplo qué tanto está apretado el grip (0 a 1).
    public static float GetFloat(XRNode node, InputFeatureUsage<float> usage)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        if (device.isValid && device.TryGetFeatureValue(usage, out float value))
            return value;
        return 0f; // Si no se pudo leer, lo tratamos como "no presionado"
    }

    // Lee un botón (true = presionado, false = no presionado).
    public static bool GetBool(XRNode node, InputFeatureUsage<bool> usage)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        if (device.isValid && device.TryGetFeatureValue(usage, out bool value))
            return value;
        return false;
    }
}