using UnityEngine;
using System;
using System.Collections;

public class SimpleDraggable : MonoBehaviour {

	private bool mousePressed = false;
	private bool previousMousePressed = false;
	private bool rigidBodyKinematic = false;
	private Transform currentObject;
	private Rigidbody currentRigidBody;
	private Vector3 objectPosition;
	private Vector3 objectHitPoint;
	private float oldX;
	private float oldY;
	private float oldZ;
	private Vector3 offset;
	private Line line;
	private GameObject hook;
	private float hookScale = 0.1f;
	
	public bool drawLine = true;
	public float lineWidth = 0.1f;
	public Material lineMaterial;
	public float maxDistance = 40f;
	public float smooth = 5f;
	public LayerMask layerMask = -1;
	public float mouseHorizon = Mathf.Infinity;
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
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		bool doesHit = Physics.Raycast(ray, out hit, mouseHorizon, currentObject?-1:layerMask.value);
		
		if(!currentObject && !doesHit){previousMousePressed=true;return;}
		if(!currentObject && previousMousePressed==true){previousMousePressed=true;return;}
		if(!currentObject){
			SetCurrentObject(hit);
		}
		UpdateObject(hit);
	}

	void UpdateObject(RaycastHit hit){
		objectHitPoint = hit.point+offset;
		Vector3 desiredPos = new Vector3(restrict.x?oldX:objectHitPoint.x,restrict.y?oldY:objectHitPoint.y,restrict.z?oldZ:objectHitPoint.z);
		currentObject.position = Vector3.Lerp(currentObject.position, desiredPos,smooth*Time.deltaTime);
		if(line){
			line.SetPoints(currentObject.position-offset,hit.point);
		}
	}

	void SetCurrentObject(RaycastHit hit){
		offset = hit.transform.position - hit.point;
		if(hit.rigidbody){
			rigidBodyKinematic = hit.rigidbody.isKinematic;
			hit.rigidbody.isKinematic = true;
			currentRigidBody = hit.rigidbody;
		}
		if(!hook){
			hook = GameObject.CreatePrimitive(PrimitiveType.Cube);
			hook.name = "SimpleDraggableHook";
			hook.transform.localScale = new Vector3(hookScale, hookScale, hookScale);
			hook.renderer.material = lineMaterial;
			hook.renderer.enabled = drawLine;
			if(!line && drawLine){
				line = gameObject.AddComponent("Line") as Line;
				line.name = "SimpleDraggableLine";
				line.width = lineWidth;
				line.material = lineMaterial;
			}
		}
		hook.transform.position = hit.transform.position - offset;
		hook.transform.parent = hit.transform;
		currentObject = hit.transform;
		oldY = currentObject.position.y;
		oldX = currentObject.position.x;
		oldZ = currentObject.position.z;
		if(drawLine && line){
			hook.renderer.enabled = true;
			line.visible = true;
		}
	}
	
	void ReleaseCurrentObject(){
		if(currentObject){
			currentObject = null;
			if(currentRigidBody){
				currentRigidBody.isKinematic = rigidBodyKinematic;
				currentRigidBody = null;
			}
		}
		if(drawLine && line){
			hook.renderer.enabled = false;
			line.visible = false;
		}
	}
			
}
