using UnityEngine;

[AddComponentMenu("Camera-Control/Keyboard Orbit")]

public class KeyboardOrbit : MonoBehaviour {

	public LayerMask layerMask = -1;
	
	public Transform target;
	public float distance = 20.0f;
	public float zoomSpd = 2.0f;
	
	public float xSpeed = 240.0f;
	public float ySpeed = 123.0f;
	
	public int yMinLimit = -723;
	public int yMaxLimit = 877;
	
	private float x = 0.0f;
	private float y = 0.0f;
	
	// The movement amount when zooming.
	public bool doZoom = true;
	public float zoomStep = 30f;
	public float zoomSpeed = 5f;
	public float min = 5f;
	public float max = 60;
	private float heightWanted;
	private float distanceWanted;
	private float smooth = 5f;
	
	public void Start () {
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
		
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
	}
	
	void Update (){
		if (Input.GetButtonUp("Fire2")){
			RaycastHit hit;
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, layerMask.value)){
				target = hit.transform;
			}
		}
	}
	
	public void LateUpdate () {
		if (target) {
			x -= Input.GetAxis("Horizontal") * xSpeed * 0.02f;
			y += Input.GetAxis("Vertical") * ySpeed * 0.02f;
			
			y = ClampAngle(y, yMinLimit, yMaxLimit);
			
			if( doZoom ){
				float mouseInput = Input.GetAxis("Mouse ScrollWheel");
				if(mouseInput!=0){
					distanceWanted -= zoomStep * mouseInput;
					distanceWanted = Mathf.Clamp(distanceWanted, min, max);
					
					distance = Mathf.Lerp(distance, distanceWanted, Time.deltaTime * zoomSpeed);
				}

			}
			
			Quaternion rotation = Quaternion.Euler(y, x, 0.0f);
			Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
			
			transform.rotation = Quaternion.Lerp(transform.rotation,rotation,smooth*Time.deltaTime);
			transform.position = Vector3.Lerp(transform.position,position,smooth*Time.deltaTime);
		}
	}
	
	public static float ClampAngle (float angle, float min, float max) {
		if (angle < -360.0f)
			angle += 360.0f;
		if (angle > 360.0f)
			angle -= 360.0f;
		return Mathf.Clamp (angle, min, max);
	}
}