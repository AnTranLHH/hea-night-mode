using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VisibleMesh : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private float _viewAngle = 30f;
    [SerializeField]
    private float _viewRadius = 10f;
    [SerializeField]
    private LayerMask _obstacleMask;
    [SerializeField]
    private float _initialAngleStep = 10f;
    [SerializeField]
    private float _jaggedThreshold = 1.5f;
    [SerializeField]
    private int _maxExtraRefinements = 5;

    private Mesh _mesh;
    private List<Vector3> _hitPoints = new();

    private void Start()
    {
        _mesh = new Mesh();
        _mesh.MarkDynamic();
        GetComponent<MeshFilter>().mesh = _mesh;
    }

    private void Update()
    {
        ScanHitPoints();
        // for (int i = 0; i < _hitPoints.Count; i++)
        // {
        //     Debug.DrawRay(_target.position, _hitPoints[i] - _target.position, Color.red);
        // }
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < _hitPoints.Count; i++)
        {
            float color = i * 255f / _hitPoints.Count / 255f;
            Gizmos.color = new Color(color, color, color, 1f);
            Gizmos.DrawSphere(_hitPoints[i], 0.05f);
        }
    }

    private void LateUpdate()
    {
        Vector3 forward = _target.transform.forward;
        Vector3 origin = _target ? _target.position : transform.position;
        Vector3 zeroDirection = Quaternion.Euler(0f, -_viewAngle / 2f, 0f) * Vector3.forward;
        Debug.DrawRay(_target.position, zeroDirection.normalized * 100f, Color.blue);
        _hitPoints.Sort((a, b) =>
        {
            float angleA = Vector3.Angle(zeroDirection, a - origin);
            float angleB = Vector3.Angle(zeroDirection, b - origin);
            return angleA.CompareTo(angleB);
        });
        FormVisibleMesh();
    }

    private void ScanHitPoints()
    {
        _hitPoints.Clear();

        Vector3 origin = _target ? _target.position : transform.position;
        float halfFov = _viewAngle * 0.5f;
        float baseYaw = _target ? _target.eulerAngles.y : transform.eulerAngles.y;

        // Ensure at least one sample
        if (_initialAngleStep <= 0f) _initialAngleStep = _viewAngle;

        float angle = -halfFov;
        // sample initial points across the view angle
        float prevAngle = angle;
        Vector3 prevPoint = GetPointAtAngle(prevAngle, origin, baseYaw);
        _hitPoints.Add(prevPoint);

        while (angle < halfFov)
        {
            float nextAngle = Mathf.Min(angle + _initialAngleStep, halfFov);
            Vector3 nextPoint = GetPointAtAngle(nextAngle, origin, baseYaw);

            // refine between prev and next if they differ significantly
            if (Vector3.Distance(prevPoint, nextPoint) > _jaggedThreshold && _maxExtraRefinements > 0)
            {
                RefineBetween(prevAngle, nextAngle, prevPoint, nextPoint, _maxExtraRefinements, origin, baseYaw, _hitPoints);
            }

            // append nextPoint and advance
            _hitPoints.Add(nextPoint);
            prevAngle = nextAngle;
            prevPoint = nextPoint;
            angle = nextAngle;
        }
    }
    private void FormVisibleMesh()
    {
        if (_mesh == null) return;
        if (_hitPoints == null || _hitPoints.Count == 0)
        {
            _mesh.Clear();
            return;
        }

        Vector3 origin = _target ? _target.position : transform.position;

        int vertCount = _hitPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        int[] triangles = new int[(_hitPoints.Count - 1) * 3];

        // center vertex (origin) in local space
        vertices[0] = transform.InverseTransformPoint(origin);
        uvs[0] = new Vector2(0.5f, 0.5f);

        // outer ring vertices
        for (int i = 0; i < _hitPoints.Count; i++)
        {
            Vector3 local = transform.InverseTransformPoint(_hitPoints[i]);
            vertices[i + 1] = local;

            // Simple UV mapping: project XZ to unit circle around center
            Vector2 uv = new Vector2(local.x - vertices[0].x, local.z - vertices[0].z) / (_viewRadius * 2f) + new Vector2(0.5f, 0.5f);
            uvs[i + 1] = uv;
        }

        // triangle fan from center
        for (int i = 0; i < _hitPoints.Count - 1; i++)
        {
            int ti = i * 3;
            triangles[ti] = 0;
            triangles[ti + 1] = i + 1;
            triangles[ti + 2] = i + 2;
        }

        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.uv = uvs;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
    }

    // returns the world-space hit point (or edge at radius) for a local FOV angle (relative to center)
    private Vector3 GetPointAtAngle(float localAngleDeg, Vector3 origin, float baseYaw)
    {
        float globalAngle = baseYaw + localAngleDeg;
        Vector3 dir = Quaternion.Euler(0f, globalAngle, 0f) * Vector3.forward;
        if (Physics.Raycast(origin, dir, out RaycastHit hit, _viewRadius, _obstacleMask))
            return hit.point;
        return origin + dir * _viewRadius;
    }

    // recursively insert mid points between angleA and angleB when the geometry between them is jagged
    private void RefineBetween(float angleA, float angleB, Vector3 pointA, Vector3 pointB, int depth, Vector3 origin, float baseYaw, List<Vector3> outList)
    {
        if (depth <= 0) return;

        float midAngle = (angleA + angleB) * 0.5f;
        Vector3 midPoint = GetPointAtAngle(midAngle, origin, baseYaw);

        // refine left segment if necessary
        if (Vector3.Distance(pointA, midPoint) > _jaggedThreshold)
            RefineBetween(angleA, midAngle, pointA, midPoint, depth - 1, origin, baseYaw, outList);

        // add the midpoint between A and B
        outList.Add(midPoint);

        // refine right segment if necessary
        if (Vector3.Distance(midPoint, pointB) > _jaggedThreshold)
            RefineBetween(midAngle, angleB, midPoint, pointB, depth - 1, origin, baseYaw, outList);
    }

}