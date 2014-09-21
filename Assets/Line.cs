using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class Line : MonoBehaviour {


	private bool _isSet;
	private float _width = 0.1f;
	private Material _material;
	private bool _enabled;
	private LineRenderer line;
	private Vector3 _pointA = new Vector3(0,0,0);
	private Vector3 _pointB = new Vector3(0,0,0);
	
	public float width {
		get {return _width;}
		set {
			_width = value;
			if(line){
				line.SetWidth(_width,_width);
			}
		}
	}
	
	public Material material {
		get {return _material;}
		set {
			_material = value;
			if(line){
				line.material = _material;
			}
		}
	}
	
	public Vector3 pointA{
		get {return _pointA;}
		set {
			_pointA = value;
			if(line){
				line.SetPosition(0,value);
			}
		}
	}
	
	public Vector3 pointB{
		get {return _pointB;}
		set {
			_pointB = value;
			if(line){
				line.SetPosition(1,value);
			}
		}
	}
	
	public float distance {
		get {
			return Vector3.Distance(_pointA,_pointB);
		}
	}
	
	[HideInInspector]
	public bool visible{
		get {return _enabled;}
		set {if(value){Enable();}else{Disable();}}
	}

	void Start(){
		Create();
	}
			
	void Create(){
		if(!line){
			line = gameObject.AddComponent("LineRenderer") as LineRenderer;
		}
		if(!_isSet){
			line.SetWidth(_width, _width);
			line.SetVertexCount(2);
			line.renderer.material = material;
			line.renderer.enabled = _enabled;
			line.SetPosition(0,_pointA);
			line.SetPosition(1,_pointB);
			_isSet = true;
		}
	}
	
	public void Disable(){
		_enabled = false;
		if(line){
			line.renderer.enabled = false;
		}
	}
	
	public void Enable(){
		if(!line){Create();}
		_enabled = true;
		line.renderer.enabled = true;
	}
	
	public void SetPoints(Vector3 p1, Vector3 p2){
		if(!line){Create();}
		pointA = p1;
		pointB = p2;
	}

	public void Destroy(){
		line = null;
	}
}
