using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicLineRenderer : MonoBehaviour
{
    [SerializeField] Transform startPoint;

    List<Transform> raycasterPoints;
    LineRenderer lineRenderer;

    private void Start()
    {
        raycasterPoints = new List<Transform>();
        raycasterPoints.Add(startPoint);
        lineRenderer = GetComponent<LineRenderer>();
        if(lineRenderer != null)
        {
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, startPoint.position);
        }
    }

    RaycastHit rayHit;
    void FixedUpdate()
    {
        if(raycasterPoints.Count > 0)
        {
            var lookAtPoint = raycasterPoints[GetTargetPointIndex()];
            Debug.Log(lookAtPoint.name);
            if(Physics.Linecast(transform.position, lookAtPoint.position, out rayHit))
            {
                if(rayHit.collider.gameObject == lookAtPoint.gameObject)
                {
                    if(raycasterPoints[raycasterPoints.Count - 1] != lookAtPoint)
                    {
                        GameObject removeLrp = raycasterPoints[raycasterPoints.Count - 1].gameObject;
                        raycasterPoints.RemoveAt(raycasterPoints.Count - 1);
                        Destroy(removeLrp);
                    }
                }
                lookAtPoint = raycasterPoints[raycasterPoints.Count - 1];
                if(Physics.Linecast(transform.position, lookAtPoint.position, out rayHit))
                {
                    if(rayHit.collider.gameObject != lookAtPoint.gameObject)
                    {
                        if(!raycasterPoints.Find(x => x.gameObject == rayHit.collider.gameObject))
                        {
                            GameObject lrp = new GameObject("lrp " + raycasterPoints.Count);
                            lrp.AddComponent<SphereCollider>().radius = 0.1f;
                            lrp.transform.position = rayHit.point;
                            lrp.transform.rotation = Quaternion.FromToRotation(lrp.transform.up, rayHit.normal) * lrp.transform.rotation;
                            raycasterPoints.Add(lrp.transform);
                        }
                    }
                }
            }
        }
        lineRenderer.positionCount = raycasterPoints.Count + 1;
        lineRenderer.SetPositions(raycasterPoints.Select(x => x.position).ToArray());
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, transform.position);
    }
    int GetTargetPointIndex() => (raycasterPoints.Count - 2) < 0 ? 0 : raycasterPoints.Count - 2;
}
