using UnityEngine;
using System;
using System.Collections;

public class SimpleDraggable : MonoBehaviour {

	private bool mousePressed = false;
	private bool previousMousePressed = false;
	private bool rigidBodyKinematic = false;
	private Transform currentObject;
	private Rigidbody currentRigidBody;
	private float oldX;
	private float oldY;
	private float oldZ;
	
	public float maxDistance = 40f;
	public float smooth = 5f;
	public LayerMask layerMask = -1;
	public RestrictDimension restrict = new RestrictDimension();
	
	void FixedUpdate(){
		mousePressed = Input.GetButton("Fire1");
		if (!mousePressed){
			ReleaseCurrentObject();
			previousMousePressed = false;
			return;
		}
		
		// We need to actually hit an object
		RaycastHit hit;
		bool doesHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, currentObject?-1:layerMask.value);
		
		if(!currentObject && !doesHit){previousMousePressed=true;return;}
		if(!currentObject && previousMousePressed==true){previousMousePressed=true;return;}
		if(!currentObject){
			SetCurrentObject(hit);
		}
		
		Vector3 desiredPos = new Vector3(restrict.x?oldX:hit.point.x,restrict.y?oldY:hit.point.y,restrict.z?oldZ:hit.point.z);
		currentObject.position = Vector3.Lerp(currentObject.position, desiredPos,smooth*Time.deltaTime);
	}

	void SetCurrentObject(RaycastHit hit){
		if(hit.rigidbody){
			rigidBodyKinematic = hit.rigidbody.isKinematic;
			hit.rigidbody.isKinematic = true;
			currentRigidBody = hit.rigidbody;
		}
		currentObject = hit.transform;
		oldY = currentObject.position.y;
		oldX = currentObject.position.x;
		oldZ = currentObject.position.z;
	}
	
	void ReleaseCurrentObject(){
		if(currentObject){
			currentObject = null;
			if(currentRigidBody){
				currentRigidBody.isKinematic = rigidBodyKinematic;
				currentRigidBody = null;
			}
		}
	}
			
}
