using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawCircleSliderfromOSC : MonoBehaviour
{
    //source www.youtube.com/watch?v=BoDwchoM9Ic

    public verticalLine line;
    public int vertexCount = 40;
    public float lineWidth = 0.2f;
    public float radius;
    public float diameter_mm;
    public bool circleFillScreen = false;
    public bool circleSliderControl = true;
    public GameObject positionRef;
    public float hfactor = 0.5f;

    private LineRenderer lineRenderer;
    private float adj_zpos;

    // Start is called before the first frame update
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupCircle();
    }

    private void Update()
    {

        if (circleSliderControl)
        {
            radius = line.slPos.y;
            diameter_mm = radiustomm(radius);
            float radius_adj = (radius - 0)/line.sizeFactor; //scalse factor
            adj_zpos = positionRef.transform.position.z;//f;

            if (Globals.GlobalVar.prev_radius != radius_adj)
            {

                //Debug.Log("radius: " + radius);
                //radius = radius+0f;
                Globals.GlobalVar.prev_radius = radius_adj;
                Globals.GlobalVar.report_diameter = diameter_mm;
                float deltaTheta = (2f * Mathf.PI) / vertexCount;
                float theta = 0f;

                lineRenderer.positionCount = vertexCount;

                for (int i = 0; i < lineRenderer.positionCount; i++)
                {
                    Vector3 pos = new Vector3((radius_adj * Mathf.Cos(theta))+hfactor, ((radius_adj * Mathf.Sin(theta))+line.yposFactor), adj_zpos);
                    lineRenderer.SetPosition(i, pos);
                    theta += deltaTheta;
                }
            }
        }
    }

    private float radiustomm(float rd)
    {
        if (rd < 0) { rd = rd * -1;}

        // 5 is the maximum received radius
        // 400 mm is the height of the ms studio display
        float factor = 400/5;
        return rd * factor;
    

    }

    private void SetupCircle()
    {
        lineRenderer.widthMultiplier = lineWidth;

        if (circleFillScreen)
        {
            radius = Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelRect.yMax, adj_zpos)),
                Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelRect.yMin, adj_zpos))) * 0.5f - lineWidth;

            float deltaTheta = (2f * Mathf.PI) / vertexCount;
            float theta = 0f;

            lineRenderer.positionCount = vertexCount;

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                Vector3 pos = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0f);
                lineRenderer.SetPosition(i, pos);
                theta += deltaTheta;
            }
        }

    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (circleFillScreen)
        {
            float deltaTheta = (2f * Mathf.PI) / vertexCount;
            float theta = 0f;

            Vector3 oldPos = Vector3.zero;
            for (int i = 0; i < vertexCount + 1; i++)
            {
                Vector3 pos = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), adj_zpos);
                Gizmos.DrawLine(oldPos, transform.position + pos);
                oldPos = transform.position + pos;

                theta += deltaTheta;
            }
        }
    }
#endif
}
