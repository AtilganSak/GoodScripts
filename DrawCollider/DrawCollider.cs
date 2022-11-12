using UnityEngine;

public enum Shape
{
    Box,
    Sphere,
    Mesh
}
public class DrawCollider : MonoBehaviour
{
    [SerializeField] Shape shape;
    [SerializeField] Color gizmoColor = Color.white;
    [SerializeField] bool wire;

    BoxCollider boxCollider;
    SphereCollider sphereCollider;
    MeshCollider meshCollider;

    private void OnDrawGizmos()
    {
        FindReferences();

        // Convert transform local space to world space
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = gizmoColor;

        switch (shape)
        {
            case Shape.Box:
                if (!wire)
                    Gizmos.DrawCube(boxCollider.center, boxCollider.size);
                else
                    Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
                break;
            case Shape.Sphere:
                if (!wire)
                    Gizmos.DrawSphere(sphereCollider.center, sphereCollider.radius);
                else
                    Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
                break;
            case Shape.Mesh:
                if (!wire)
                    Gizmos.DrawMesh(meshCollider.sharedMesh);
                else
                    Gizmos.DrawWireMesh(meshCollider.sharedMesh);
                break;
        }
    }
    void FindReferences()
    {
        switch (shape)
        {
            case Shape.Box:
                if (boxCollider == null)
                    boxCollider = GetComponent<BoxCollider>();
                break;
            case Shape.Sphere:
                if (sphereCollider == null)
                    sphereCollider = GetComponent<SphereCollider>();
                break;
            case Shape.Mesh:
                if (meshCollider == null)
                    meshCollider = GetComponent<MeshCollider>();
                break;
        }
    }
}
