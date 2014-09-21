using UnityEngine;
using System;
using System.Collections;

public class Draggable : MonoBehaviour {

	public bool attachToCenterOfMass = false;
	public LayerMask layerMask = -1;
	public bool doNotBreak = false;
	public bool drawLine = true;
	public float spring = 50.0f;
	public float damper = 5.0f;
	public float distance = 0.2f;
	public float smooth = 5f;
	public float breakForceModifier = 0f;
	public float breakTorqueModifier = 0f;
	public float strength = 1f;
	public float lineWidth = 0.1f;
	public Material lineMaterial;
	public RestrictDimension restrict = new RestrictDimension();
	public float mouseHorizon = Mathf.Infinity;
	public bool doDebug = false;
	
	private float drag;
	private float angularDrag;
	private float mass;
	private float breakForce = Mathf.Infinity;
	private float breakTorque = Mathf.Infinity;
	private bool holdingAnObject = false;
	private float oldDrag = 0f;
	private float oldAngularDrag = 0f;
	private float oldX;
	private float oldY;
	private float oldZ;
	private RigidbodyConstraints oldConstraints;
	private bool mousePressed = false;
	private bool previousMousePressed = false;
	private LineRenderer line;
	private SpringJoint springJoint;
	private Rigidbody rigibody;
	private GameObject dragger;
	private GameObject hook;
	private float hookScale = 0.1f;
	private float halfHookScale;
	private Rigidbody rb;

	void Start(){
		halfHookScale = hookScale/2;
	}

	void ApplyRigidBodyValues(){
		if(rb){
			mass = rb.mass;
			drag = rb.drag;
			angularDrag = rb.angularDrag;
			if(!doNotBreak){
				float growth = (mass / Mathf.Pow(mass,2))*strength;
				breakForce = growth + breakForceModifier;
				breakTorque = growth + breakTorqueModifier;
			}else{
				breakForce = Mathf.Infinity;
				breakTorque = Mathf.Infinity;
			}
		}
	}
			
	void FixedUpdate (){
		if(doDebug){ApplyRigidBodyValues();}
		// Make sure the user pressed the mouse down
		mousePressed = Input.GetButton("Fire1");
		if (!mousePressed){
			DetachSpringJoint();
			previousMousePressed = false;
			return;
		}
		if(mousePressed && !springJoint && holdingAnObject){ //springJoint broke
			OnJointBreak();
			previousMousePressed = true;
			return;
		}

		// We need to actually hit an object
		RaycastHit hit;
		bool doesHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, mouseHorizon, holdingAnObject?-1:layerMask.value);
		
		if(!holdingAnObject && !doesHit || !hit.rigidbody || hit.rigidbody.isKinematic){previousMousePressed = true;return;}
		if(!holdingAnObject && previousMousePressed==true){previousMousePressed=true;return;} //we only want to trigger hold if the click happens on the object
		previousMousePressed = true;
		if(!holdingAnObject){
			holdingAnObject = true;
			rb = hit.rigidbody;
			ApplyRigidBodyValues();
			CreateSpringJoint();
			AttachSpringJoint(hit);
		}
		if(doDebug){ApplySpringJointValues();}
		StartCoroutine("DragObject",hit.distance);
	}
	
	void CreateSpringJoint(){
		if(!dragger){
			dragger = new GameObject("Rigidbody DraggableDragger");
			Rigidbody body = dragger.AddComponent("Rigidbody") as Rigidbody;
			body.isKinematic = true;
			if(drawLine){
				line = dragger.AddComponent("LineRenderer") as LineRenderer;
				line.SetWidth(lineWidth, lineWidth);
				line.SetVertexCount(2);
				line.renderer.material = lineMaterial;
				line.renderer.enabled = true;
				line.name = "DraggableLine";
			}
			hook = GameObject.CreatePrimitive(PrimitiveType.Cube);
			hook.name = "DraggableHook";
			hook.transform.localScale = new Vector3(hookScale, hookScale, hookScale);
			hook.renderer.material = lineMaterial;
			hook.renderer.enabled = drawLine;
		}
		if (!springJoint){
			springJoint = dragger.AddComponent ("SpringJoint") as SpringJoint;
			springJoint.name = "DraggableSpringJoint";
			ApplySpringJointValues();
		}
	}

	void ApplySpringJointValues(){
		if(springJoint){
			springJoint.spring = spring;
			springJoint.damper = damper;
			springJoint.maxDistance = distance;
			springJoint.breakForce = breakForce;
			springJoint.breakTorque = breakTorque;
		}
	}
			
	void AttachSpringJoint(RaycastHit hit){
		if(springJoint.connectedBody){return;}
		springJoint.transform.position = hit.point;
		if (attachToCenterOfMass){
			Vector3 anchor = transform.TransformDirection(hit.rigidbody.centerOfMass) + hit.rigidbody.transform.position;
			anchor = springJoint.transform.InverseTransformPoint(anchor);
			springJoint.anchor = anchor;
		}
		else{
			springJoint.anchor = Vector3.zero;
		}
		hook.transform.parent = rb.transform;
		hook.transform.position = new Vector3(hit.point.x+halfHookScale,hit.point.y+halfHookScale,hit.point.z+halfHookScale);
		springJoint.connectedBody = hit.rigidbody;
		oldDrag = springJoint.connectedBody.drag;
		oldAngularDrag = springJoint.connectedBody.angularDrag;
		oldX = transform.position.x;
		oldY = transform.position.y;
		oldZ = transform.position.z;
		springJoint.connectedBody.drag = drag;
		springJoint.connectedBody.angularDrag = angularDrag;
		oldConstraints = rb.constraints; //will return constraints to values set when game was started...sucks
		if(restrict.x || restrict.y || restrict.z){
			if(restrict.x){
				rb.constraints = rb.constraints | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX;
			}
			if(restrict.y){
				rb.constraints = rb.constraints | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
			}
			if(restrict.z){
				rb.constraints = rb.constraints | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionZ;
			}
		}
		if(drawLine && line){
			line.renderer.enabled = true;
			hook.renderer.enabled = true;
		}
	}
	
	void OnJointBreak(){
		holdingAnObject = false;
		Restore();
	}
	
	void Restore(){
		rb.drag = oldDrag;
		rb.angularDrag = oldAngularDrag;
		rb.constraints = oldConstraints;
		holdingAnObject = false;
		if(springJoint){
			springJoint.connectedBody = null;
		}
		if(line){
			line.renderer.enabled = false;
			hook.renderer.enabled = false;
		}
		rb = null;
	}
	
	void DetachSpringJoint(){
		if(!springJoint || !springJoint.connectedBody){return;}
		Restore();
	}
	
	IEnumerator DragObject(float distance){
		while(mousePressed && springJoint){
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			Vector3 point = ray.GetPoint(distance);
			if(line){
				line.SetPosition(0,hook.transform.position);
				line.SetPosition(1,point);
			}
			if(restrict.x || restrict.y || restrict.z){
				point = new Vector3(restrict.x?oldX:point.x,restrict.y?oldY:point.y,restrict.z?oldZ:point.z);
			}
			dragger.transform.position = Vector3.Lerp(dragger.transform.position, point,smooth*Time.deltaTime);
			yield return false;
		}
	}
}
