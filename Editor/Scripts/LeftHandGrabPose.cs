using NaughtyAttributes;
using UnityEngine;
using UserController;

public class LeftHandGrabPose : HandGrabPose
{
    private void Start()
    {
        leftHand = true;
        InitializePoint();
    }
    protected override string GetName()
    {
        return "LeftHand";
    }

#if UNITY_EDITOR
    protected override void GetPrefab()
    {
        if (prefabGrabHandController == null)
            prefabGrabHandController = HandSimulatorPrefabs.Instance.LeftHandSimulator;
    }
#endif
}
