using NaughtyAttributes;
using UnityEngine;
using UserController;

public class HandGrabPose : MonoBehaviour
{
    public GameObject prefabGrabHandController;
    [ReadOnly]
    public GameObject handControlerSimulate;
    #if UNITY_EDITOR
    private HandControllerSimulate controllerSimulate;
    #endif
    public GrabbingType grabbingType = GrabbingType.None;
    [Range(0.0025f, 1f)]
    public float animateFrame = 1f;
    public Transform handAttachPoint;
    [ReadOnly]
    public Vector3 attachRelativePosition;
    [ReadOnly]
    public Vector3 attachRelativeRotation;
    [ReadOnly]
    public Quaternion attachRelativeQRotation;
    [ReadOnly]
    public float handScale = 1f;
    [ReadOnly]
    public float scaleStart = 1f;
    [ReadOnly]
    public bool leftHand = true;

    public Vector3 offSetGraspPosition = Vector3.zero;
    private GameObject attachPoint;

    [ReadOnly]
    public Vector3 originalLocalPoint;

    public bool isReady = false;
    //Todo: Debug have issues on some rotation 
    [Header("Caution !its  have some issues with some rotations")]
    public bool debug = true;

    private GameObject handDebug;

    protected void InitializePoint()
    {
        if (attachRelativePosition != null)
        {
            if (handControlerSimulate != null)
            {
                if (!isReady && debug)
                {
                    handControlerSimulate.SetActive(true);
                    GameObject handClone = Instantiate(handControlerSimulate);
                    handClone.transform.SetParent(transform, true);
                    handControlerSimulate.SetActive(false);
                    handDebug = handClone;
                    SetDebugHand(false);
                }
                else if (isReady)
                {
                    Destroy(handControlerSimulate);
                }
            }
            string attachName = "attachPoint";
            attachName += GetName();
            attachPoint = CreateAttachPoint(attachRelativePosition, attachRelativeRotation, attachName, this.transform);
        }
    }

    protected virtual string GetName()
    {
        return "hand";
    }

    public GameObject CreateAttachPoint(Vector3 attachRelativePosition, Vector3 attachRelativeRotation, string objName, Transform parent)
    {
        GameObject attachPoint;
        if (attachRelativePosition != null)
        {
            attachPoint = Instantiate(new GameObject(), parent, false);
            attachPoint.name = objName;

            SetLocation(attachPoint);

            return attachPoint;
        }
        return null;
    }

    private void SetLocation(GameObject attach)
    {
        attach.transform.rotation = attachRelativeQRotation;
        attach.transform.position = this.transform.position;
        Vector3 scale = GetScaleMagnitude(this.transform, Vector3.one);

        float diff = scale.magnitude / scaleStart;
        attach.transform.position += (attachRelativePosition) * diff;
    }

    private Vector3 GetScaleMagnitude(Transform _transform, Vector3 scale)
    {
        Vector3 newScale = Vector3.Scale(scale, _transform.localScale);
        if (_transform.parent != null)
        {
            newScale = GetScaleMagnitude(_transform.parent, newScale);
        }
        return newScale;
    }

    public Transform GetAttachPoint()
    {
        return attachPoint.transform;
    }

    public void SetDebugHand(bool active)
    {
        if (!isReady && debug && handDebug != null) {
            handDebug.SetActive(active);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying || isReady)
        {
            if (handControlerSimulate != null)
                handControlerSimulate.SetActive(false);
            return;
        }

        if (handControlerSimulate != null)
            handControlerSimulate.SetActive(true);

        GetPrefab();


        if (prefabGrabHandController != null && handControlerSimulate == null)
        {
            handControlerSimulate = Instantiate(prefabGrabHandController);
            //handControlerSimulate.hideFlags = HideFlags.HideInHierarchy;
            handControlerSimulate.transform.localScale = new Vector3(handControlerSimulate.transform.localScale.x, handControlerSimulate.transform.localScale.y, handControlerSimulate.transform.localScale.z * (leftHand ? 1 : -1));
            controllerSimulate = handControlerSimulate.GetComponentInChildren<HandControllerSimulate>();
            handControlerSimulate.transform.position = transform.position;
        }
        else if (handControlerSimulate != null)
        {
            controllerSimulate = handControlerSimulate.GetComponentInChildren<HandControllerSimulate>();
            handControlerSimulate.transform.SetParent(null);
        }
        if (controllerSimulate != null)
        {
            controllerSimulate.SetVariables(grabbingType, animateFrame);
        }
    }
    protected virtual void GetPrefab()
    {
        if (prefabGrabHandController == null)
            prefabGrabHandController = HandSimulatorPrefabs.Instance.RightHandSimulator;
    }

    [Button]
    public void NextGraspPoint()
    {
        if (controllerSimulate != null)
        {
            if (handAttachPoint != null)
                handAttachPoint.localPosition = originalLocalPoint;

            handAttachPoint = controllerSimulate.GetNextGraspPoint();
            originalLocalPoint = handAttachPoint.localPosition;
        }
        UpdatePosition();
    }
    [Button]
    public void UpdatePosition()
    {
        if (Application.isPlaying || isReady)
            return;
        if (handControlerSimulate != null)
            handControlerSimulate.SetActive(true);

        if (handAttachPoint == null)
        {
            NextGraspPoint();
            return;
        }
        Transform useToCompare = handControlerSimulate.transform.GetChild(handControlerSimulate.transform.childCount - 1);
        handScale = useToCompare.localScale.x;

        handAttachPoint.localPosition = originalLocalPoint + offSetGraspPosition;
        //handControlerSimulate.transform.SetParent(null);
        useToCompare.rotation = Quaternion.identity;
        handControlerSimulate.transform.position = this.transform.position;
        handControlerSimulate.transform.rotation = this.transform.rotation;
        Vector3 dir = (useToCompare.position - handAttachPoint.position);
        handControlerSimulate.transform.position += dir;

        Vector3 scale = GetScaleMagnitude(this.transform, Vector3.one);
        scaleStart = scale.magnitude;
        //print(scale.magnitude);
        
        float scaleDiff = 1 - handScale;
        Vector3 centerDiff = (this.transform.localPosition * scaleDiff * scale.x);
        attachRelativePosition = ((useToCompare.position - this.transform.position) * handScale) - (centerDiff);
        attachRelativeRotation = this.transform.rotation.eulerAngles;
        attachRelativeQRotation = this.transform.rotation;
    }

    [Button]
    public void CopyObject()
    {
        //create a copy of object ... 
        GameObject copy = GameObject.Instantiate(this.gameObject, this.gameObject.transform.parent);
        //fix hand grasp pose ...
        HandGrabPose copyHandPose = copy.GetComponent<HandGrabPose>();
        copyHandPose.RemoveHandRef();
        //

    }
    public void RemoveHandRef()
    {
        handControlerSimulate = null;
        controllerSimulate = null;
        handAttachPoint = null;
        attachPoint = null;
        originalLocalPoint = Vector3.zero;
    }
#endif
}
