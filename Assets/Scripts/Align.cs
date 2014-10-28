using UnityEngine;
using System.Collections;

public class Align : MonoBehaviour {
    public Camera cam;
    public enum align
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
    public align Alignment;

	void Start () {
        float frustumHeight = 2.0f * cam.farClipPlane * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustumWidth = frustumHeight * cam.aspect;
        Vector3 newPosition;
        if (Alignment.Equals(align.TopLeft))
        {
            newPosition = new Vector3(cam.transform.position.x - frustumWidth / 2, cam.transform.position.y + frustumHeight / 2, transform.position.z);
            transform.position = newPosition;
        }
        else if (Alignment.Equals(align.TopRight))
        {
            newPosition = new Vector3(cam.transform.position.x + frustumWidth / 2, cam.transform.position.y + frustumHeight / 2, transform.position.z);
            transform.position = newPosition;
        }
        else if (Alignment.Equals(align.BottomLeft))
        {
            newPosition = new Vector3(cam.transform.position.x - frustumWidth / 2, cam.transform.position.y - frustumHeight / 2, transform.position.z);
            transform.position = newPosition;
        }
        else if (Alignment.Equals(align.BottomRight))
        {
            newPosition = new Vector3(cam.transform.position.x + frustumWidth / 2, cam.transform.position.y - frustumHeight / 2, transform.position.z);
            transform.position = newPosition;
        }
	}
}
