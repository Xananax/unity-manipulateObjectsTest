using UnityEngine;
using System.Collections;

public class FollowMouse : MonoBehaviour {
	
	public float maxDistance = 40f;
	public float smooth = 5f;
	
	void FixedUpdate(){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, maxDistance)){
			float oldY = transform.position.y;
			Vector3 desiredPos = new Vector3(hit.point.x,oldY,hit.point.z);
			transform.position = Vector3.Lerp(transform.position, desiredPos,smooth*Time.deltaTime);
		}
	}
		
}
