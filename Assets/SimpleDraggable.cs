using UnityEngine;
using System;
using System.Collections;

public class SimpleDraggable : MonoBehaviour {

	private bool mousePressed = false;
	private bool previousMousePressed = false;
	private bool rigidBodyKinematic = false;
	private Transform currentObject;
	private Rigidbody currentRigidBody;
	private RigidbodyConstraints oldConstraints;
	private Vector3 objectPosition;
	private Vector3 objectHitPoint;
	private Vector3 old;
	private Vector3 offset;
	private Line line;
	private GameObject hook;
	private float hookScale = 0.1f;
	private float mass = 0;
	
	public LayerMask layerMask = -1;
	public bool drawLine = true;
	public float smooth = 5f;
	public float strength = 10f;
	public float lineWidth = 0.1f;
	public Material lineMaterial;
	public float maxDistance = Mathf.Infinity;
	public bool deriveDistanceFromMass = false;
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
		if(currentObject && !doesHit){previousMousePressed=true;ReleaseCurrentObject();return;}
		if(!currentObject){
			SetCurrentObject(hit);
		}
		UpdateObject(hit);
	}

	static float setDimension(bool _restrict,float _old, float _new){
		float r =  _restrict? _old : _new;
		return r;
	}

	void UpdateObject(RaycastHit hit){
		objectHitPoint = hit.point+offset;
		float X = setDimension(restrict.x,old.x,objectHitPoint.x);
		float Y = setDimension(restrict.y,old.y,objectHitPoint.y);
		float Z = setDimension(restrict.z,old.z,objectHitPoint.z);
		Vector3 desiredPos = new Vector3(X,Y,Z);
		if(maxDistance!=Mathf.Infinity && maxDistance>0){
			float dist = Vector3.Distance(currentObject.position,desiredPos);
			if(dist>maxDistance){
				ReleaseCurrentObject();
				return;
			}
		}
		currentObject.position = Vector3.Lerp(currentObject.position, desiredPos,(smooth*Time.deltaTime)/(mass*10));
		if(line){
			line.SetPoints(currentObject.position-offset,hit.point);
		}
	}
	
	public static float GetMass(Collider collider,float density = 1){
		float volume = 0;
		float mass = 0;
		if(collider.GetType() == typeof(SphereCollider)){
			volume = (collider.bounds.size.x * Mathf.PI)/6;
		}else if(collider.GetType() == typeof(CapsuleCollider)){
			volume = ((Mathf.PI * collider.bounds.size.x * collider.bounds.size.y) * collider.bounds.size.z)/4;
		}else{
			volume = collider.bounds.size.x * collider.bounds.size.y * collider.bounds.size.z;
		}
		mass = (density * volume)/10;
		return mass;
	}

	void SetCurrentObject(RaycastHit hit){
		offset = hit.transform.position - hit.point;
		if(hit.rigidbody){
			rigidBodyKinematic = hit.rigidbody.isKinematic;
			oldConstraints = hit.rigidbody.constraints;
			hit.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
			hit.rigidbody.isKinematic = true;
			currentRigidBody = hit.rigidbody;
			mass = hit.rigidbody.mass;
		}else{
			mass = SimpleDraggable.GetMass(hit.collider);
			if(mass<=0){mass=1;}
		}
		if(deriveDistanceFromMass && mass>0){
			maxDistance = strength/mass;
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
		old.y = currentObject.position.y;
		old.x = currentObject.position.x;
		old.z = currentObject.position.z;
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
				currentRigidBody.constraints = oldConstraints;
				currentRigidBody = null;
			}
		}
		if(drawLine && line){
			hook.renderer.enabled = false;
			line.visible = false;
		}
	}
			
}
