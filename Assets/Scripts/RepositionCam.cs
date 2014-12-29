using UnityEngine;
using System.Collections;

public class RepositionCam : MonoBehaviour {

	// Use this for initialization
	void Start () {

        float height = 2f * camera.orthographicSize;
        float width = height * camera.aspect;

        Vector3 newPosition = new Vector3(transform.position.x - width / 2, transform.position.y - height / 2, 0);
        
        float offsetX = transform.position.x + (0f - newPosition.x);
        float offsetY = transform.position.y + (0f - newPosition.y);
        transform.position = new Vector3(offsetX, offsetY, -10);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
