using System;
using UnityEngine;

namespace MAUnit
{
	public class Blast : Power
	{
		public Material aiming;
		private GameObject sub;
		private MeshFilter filter;
		private MeshRenderer renderer;
		
		/// <summary>
		/// Start this instance.
		/// </summary>
	    void Start()
		{
			
	    }
		
		public void ConstructBlast(float range)
		{
			sub = new GameObject();
			
	        sub.AddComponent("MeshFilter");
	        sub.AddComponent("MeshRenderer");
			
			// Construct the mesh
	        Mesh mesh 		= sub.GetComponent<MeshFilter>().mesh;
	        mesh.Clear();
	        mesh.vertices 	= new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, range), new Vector3(range, 0, 0) };
	        mesh.uv 		= new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
	        mesh.triangles 	= new int[] 	{0, 1, 2};
			
			sub.renderer.material = aiming;
			
			sub.transform.parent = gameObject.transform;
			sub.transform.localPosition = new Vector3(0,0,0);
		}
		
		// Use this for initialization
		public override void OnActivateBegin ()
		{
			Time.timeScale = 0.1f;
			ConstructBlast(1000);
		}
		
		// Update is called once per frame
		public override void OnActivateUpdate ()
		{
			sub.transform.RotateAround(Vector3.up, 3*Time.deltaTime);
		}
		
		// Use this for execution
		public override void OnActivateEnd ()
		{
			Time.timeScale = 1;
			GameObject.Destroy(sub);
		}
	}
}

