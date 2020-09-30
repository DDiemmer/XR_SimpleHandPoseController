using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UserController;

public class XRSimpleGrabPresets : MonoBehaviour
{
    public GrabbingType grabbingType = GrabbingType.None;
    [Range(0.0025f, 1f)]
    public float animateFrame = 1f;
}
