using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
namespace UserController
{
    [ExecuteInEditMode]
    public class HandControllerSimulate : MonoBehaviour
    {
        public Animator anim;
        public GrabbingType grabType = GrabbingType.None;
        public float animationFrame = 1f;
        public Transform graspParent;
        private Transform currentPoint;
        private List<Transform> graspPoints = new List<Transform>();
        private int iteratorGraspPoint = -1;

        // Start is called before the first frame update
        void Start()
        {
            if (anim == null)
                anim = GetComponent<Animator>();
        }

        private void OnValidate()
        {
            if (anim == null)
                anim = GetComponent<Animator>();

            if(gameObject.activeInHierarchy)
                anim.Update(Time.deltaTime);
        }

        private void Update()
        {
            float value = Mathf.Clamp(animationFrame, 0f, 1f);
            switch (grabType)
            {
                case GrabbingType.None:
                    anim.SetFloat("TriggerHandGrab", 0f);
                    anim.SetFloat("TriggerFingerGrab", 0f);
                    break;
                case GrabbingType.FingerGrab:
                    anim.SetFloat("TriggerFingerGrab", value);
                    anim.SetFloat("TriggerHandGrab", 0f);
                    break;
                case GrabbingType.HandGrab:
                    anim.SetFloat("TriggerHandGrab", value);
                    anim.SetFloat("TriggerFingerGrab", 0f);
                    break;
            }
            anim.Update(Time.deltaTime);
        }
        private void OnDestroy()
        {
        }
        public void SetVariables(GrabbingType grabbingType, float animationValue)
        {
            grabType = grabbingType;
            animationFrame = animationValue;
            anim.Update(Time.deltaTime);
            GetGraspPoints();
        }
        public void GetGraspPoints()
        {
            if (graspParent == null)
                return;
            if (graspParent.childCount == graspPoints.Count)
                return;

            graspPoints = new List<Transform>();
            graspPoints.AddRange(graspParent.GetComponentsInChildren<Transform>());
            graspPoints.RemoveAt(0);
        }
        public Transform GetNextGraspPoint()
        {
            if (graspPoints.Count == 0)
            {
                currentPoint = graspParent.transform;
                return currentPoint;
            }
            if (currentPoint != null)
                currentPoint.GetComponent<MeshRenderer>().enabled = false;

            iteratorGraspPoint++;

            if (iteratorGraspPoint >= graspPoints.Count)
            {
                iteratorGraspPoint = 0;
            }
            currentPoint = graspPoints[iteratorGraspPoint];
            if (currentPoint != null)
                currentPoint.GetComponent<MeshRenderer>().enabled = true;

            return currentPoint;
        }
        public void SetOffSet(Vector3 offSet)
        {
            if (currentPoint != null)
                currentPoint.localPosition = offSet;
        }

    }
}
#endif
