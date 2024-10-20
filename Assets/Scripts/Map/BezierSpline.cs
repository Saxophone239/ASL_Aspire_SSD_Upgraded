using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class BezierSpline : MonoBehaviour
{
	public Transform StartingPos;
	public Transform EndingPos;
    public List<Transform> controlPoints = new List<Transform>();
    public int curveResolution = 20;
    public bool loop = false;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        if (controlPoints.Count < 4 || (controlPoints.Count - 1) % 3 != 0)
        {
            Debug.LogWarning("The number of control points must be at least 4 and in multiples of 3 plus 1.");
            return;
        }

        DrawSpline();
    }

    private void DrawSpline()
    {
        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < controlPoints.Count - 3; i += 3)
        {
            DrawBezierCurve(i, positions);
        }

        if (loop)
        {
            DrawBezierCurve(controlPoints.Count - 3, positions);
        }

        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }

    private void DrawBezierCurve(int startIndex, List<Vector3> positions)
    {
        Vector3 p0 = controlPoints[startIndex].position;
        Vector3 p1 = controlPoints[startIndex + 1].position;
        Vector3 p2 = controlPoints[startIndex + 2].position;
        Vector3 p3 = controlPoints[ClampIndex(startIndex + 3)].position;

        for (int i = 0; i <= curveResolution; i++)
        {
            float t = i / (float)curveResolution;
            Vector3 point = CalculateBezierPoint(t, p0, p1, p2, p3);
            positions.Add(point);
        }
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float ttt = tt * t;
        float uuu = uu * u;

        Vector3 point = uuu * p0; // (1-t)^3 * p0
        point += 3 * uu * t * p1; // 3 * (1-t)^2 * t * p1
        point += 3 * u * tt * p2; // 3 * (1-t) * t^2 * p2
        point += ttt * p3; // t^3 * p3

        return point;
    }

    private int ClampIndex(int index)
    {
        if (loop)
        {
            return index % controlPoints.Count;
        }
        else
        {
            return Mathf.Clamp(index, 0, controlPoints.Count - 1);
        }
    }

    private void OnValidate()
    {
        if (lineRenderer != null && controlPoints.Count >= 4)
        {
            DrawSpline();
        }

		if (StartingPos == null || EndingPos == null) return;

		if (controlPoints.Count == 4)
		{
			controlPoints[0].transform.position = StartingPos.position;
			controlPoints[3].transform.position = EndingPos.position;
		}
    }
	
	private void OnDrawGizmos()
    {
        if (controlPoints.Count < 4 || (controlPoints.Count - 1) % 3 != 0)
        {
            return;
        }

        Gizmos.color = Color.yellow;

        for (int i = 0; i < controlPoints.Count - 3; i += 3)
        {
            DrawGizmoBezierCurve(i);
        }

        if (loop)
        {
            DrawGizmoBezierCurve(controlPoints.Count - 3);
        }
    }

    private void DrawGizmoBezierCurve(int startIndex)
    {
        Vector3 p0 = controlPoints[startIndex].position;
        Vector3 p1 = controlPoints[startIndex + 1].position;
        Vector3 p2 = controlPoints[startIndex + 2].position;
        Vector3 p3 = controlPoints[ClampIndex(startIndex + 3)].position;

        Vector3 lastPos = p0;

        for (int i = 1; i <= curveResolution; i++)
        {
            float t = i / (float)curveResolution;
            Vector3 newPos = CalculateBezierPoint(t, p0, p1, p2, p3);
            Gizmos.DrawLine(lastPos, newPos);
            lastPos = newPos;
        }
    }
}