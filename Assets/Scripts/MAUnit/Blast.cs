using System;
using UnityEngine;

namespace MAUnit
{
	public class Blast : MonoBehaviour, IPower
	{
		/// <summary>
		/// Start this instance.
		/// </summary>
	    void Start()
		{
			ConstructBlast(1000);
	    }
		
		public float ConstructBlast(float range)
		{
	        gameObject.AddComponent("MeshFilter");
	        gameObject.AddComponent("MeshRenderer");
	        Mesh mesh 		= GetComponent<MeshFilter>().mesh;
	        mesh.Clear();
	        mesh.vertices 	= new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, range), new Vector3(range, 0, 0) };
	        mesh.uv 		= new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
	        mesh.triangles 	= new int[] 	{0, 1, 2};
		}
		
		// Use this for initialization
		public void OnActivateBegin ()
		{
			
		}
		
		// Update is called once per frame
		public void OnActivateUpdate ()
		{
		}
		
		// Use this for execution
		public void OnActivateEnd ()
		{
		}
	}
}

