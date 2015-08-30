using UnityEngine;
using System.Collections;

public class ClothTest : MonoBehaviour {
	public Color[] vertColors;
	public float gizmoSize = .005f;
	public Color color;
	SkinnedMeshRenderer _renderer;
	SkinnedCloth _cloth;
	ClothSkinningCoefficient[] clothParameters;
	Vector3[] _vertices;
	// Use this for initialization
	void Start () {
		_cloth = gameObject.GetComponent<SkinnedCloth> ();
		clothParameters = _cloth.coefficients;
		//UpdateCloth ();
		_renderer = GetComponent<SkinnedMeshRenderer> ();
		_vertices = _renderer.sharedMesh.vertices;
	}

	void UpdateCloth()
	{
		// fill the coeffss
		float dist = 0f;
		for (int i=0; i<clothParameters.Length; i++) {
			clothParameters[i].maxDistance = dist;
			dist += 0.00003f;	
		}
		Debug.Log (dist);
		_cloth.coefficients = clothParameters;
	}
	
	// Update is called once per frame
	void OnDrawGizmosSelected () {
		//Debug.Log("here");
		Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
		Gizmos.matrix = rotationMatrix;
		if(_vertices == null ||  _vertices.Length <1)return;
		for(int i = 0; i < _vertices.Length; i++)
		{
			float val = (i/_vertices.Length);
			Color c = /*(1-val) * vertColors[0] + val * vertColors[1]; //(vertColors[0] + vertColors[1])/ 2f  * (i/_vertices.Length) ;//*/Color.Lerp(vertColors[0],vertColors[1], val);
			Gizmos.color = c;
			color = c;
			Gizmos.DrawSphere( _vertices[i] ,gizmoSize);

		}
	}
}
