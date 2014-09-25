using UnityEngine;
using System;
using System.Collections;

public class ForceDraggable : MonoBehaviour {

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
	public float maxDistance = Mathf.Infinity;
	public float mouseHorizon = Mathf.Infinity;
	public RestrictDimension restrict = new RestrictDimension();
	public bool doDebug = false;
	
	private float drag;
	private float angularDrag;
	private float mass;
	public float breakForce = Mathf.Infinity;
	private float breakTorque = Mathf.Infinity;
	private bool holdingAnObject = false;
	private float oldX;
	private float oldY;
	private float oldZ;
	private bool mousePressed = false;
	private bool previousMousePressed = false;
	private Line line;
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
			if(!doNotBreak){
				float growth = (mass / Mathf.Pow(mass,2))*strength;
				breakForce = Mathf.Clamp(growth + breakForceModifier,0.01f,Mathf.Infinity);
				breakTorque = Mathf.Clamp(growth + breakTorqueModifier,0.01f,Mathf.Infinity);
			}else{
				breakForce = Mathf.Infinity;
				breakTorque = Mathf.Infinity;
			}
		}
	}
	
	void Update(){
		// Make sure the user pressed the mouse down
		mousePressed = Input.GetButton("Fire1");
	}
	
	void FixedUpdate (){;
		if (!mousePressed){
			DetachSpringJoint();
			previousMousePressed = false;
		}
		else if(mousePressed && !springJoint && holdingAnObject){ //springJoint broke
			OnJointBreak();
			previousMousePressed = true;
		}else{
			// We need to actually hit an object
			RaycastHit hit;
			bool doesHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, mouseHorizon, holdingAnObject?-1:layerMask.value);
			
			if(!holdingAnObject && !doesHit || !hit.rigidbody || hit.rigidbody.isKinematic){previousMousePressed = true;return;}
			if(!holdingAnObject && previousMousePressed==true){previousMousePressed=true;return;} //we only want to trigger hold if the click happens on the object
			if(holdingAnObject && !doesHit){previousMousePressed=true;DetachSpringJoint();return;}
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
	}
	
	void CreateSpringJoint(){
		if(!dragger){
			dragger = new GameObject("Rigidbody DraggableDragger");
			Rigidbody body = dragger.AddComponent("Rigidbody") as Rigidbody;
			body.isKinematic = true;
			body.useGravity = false;
		}
		if(drawLine && !line){
			line = dragger.AddComponent("Line") as Line;
			line.width = lineWidth;
			line.material = lineMaterial;
			line.name = "ForceDraggableLine";
		}
		if(!hook){
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
		hook.transform.parent = rb.transform;
		hook.transform.position = new Vector3(hit.point.x+halfHookScale,hit.point.y+halfHookScale,hit.point.z+halfHookScale);
		dragger.rigidbody.position = hit.point;
		dragger.rigidbody.isKinematic = false;
		dragger.rigidbody.useGravity = true;
		springJoint.transform.position = hit.point;
		if (attachToCenterOfMass){
			Vector3 anchor = transform.TransformDirection(hit.rigidbody.centerOfMass) + hit.rigidbody.transform.position;
			anchor = springJoint.transform.InverseTransformPoint(anchor);
			springJoint.anchor = anchor;
		}
		else{
			springJoint.anchor = Vector3.zero;
		}
		springJoint.connectedBody = hit.rigidbody;
		oldX = transform.position.x;
		oldY = transform.position.y;
		oldZ = transform.position.z;
		if(drawLine && line){
			line.visible = true;
			hook.renderer.enabled = true;
		}
	}
	
	void OnJointBreak(){
		Restore();
	}
	
	void Restore(){
		holdingAnObject = false;
		if(springJoint){
			springJoint.connectedBody = null;
		}
		if(line){
			line.visible = false;
			hook.renderer.enabled = false;
		}
		rb = null;
		dragger.rigidbody.isKinematic = false;
		dragger.rigidbody.velocity = new Vector3(0,0,0);
		dragger.rigidbody.isKinematic = true;
		dragger.rigidbody.useGravity = false;
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
				line.SetPoints(hook.transform.position,point);
			}
			if(restrict.x || restrict.y || restrict.z){
				point = new Vector3(restrict.x?oldX:point.x,restrict.y?oldY:point.y,restrict.z?oldZ:point.z);
			}
			if(maxDistance!=Mathf.Infinity && maxDistance>0){
				float dist = Vector3.Distance(dragger.transform.position, point);
				if(dist>maxDistance){
					Restore();
					yield return false;
				}
			}
			dragger.rigidbody.AddForce(point-dragger.transform.position,ForceMode.Force);
			yield return false;
		}
	}
}
