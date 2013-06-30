using System;
using System.Collections.Generic;
using UnityEngine;
using MACore;
using MAUnit;

namespace MAUnit
{
	/// <summary>
	/// Blast -- for the sniper!
	/// </summary>
	public class Blast : Power
	{
		public Material aiming;
		private GameObject sub;
		private MeshFilter filter;
		private MeshRenderer renderer;
		
		public Cone cone;
		
		/// <summary>
		/// Constructs the blast.
		/// </summary>
		/// <param name='range'>
		/// Range.
		/// </param>
		/// <param name='spread'>
		/// Spread.
		/// </param>
		private void ConstructBlast(float range, float spread)
		{
			sub = new GameObject();
			
	        sub.AddComponent("MeshFilter");
	        sub.AddComponent("MeshRenderer");
			
			cone = new Cone(new Vector3(0,0,0), spread, 0, range);
			
			Vector3 upper, lower;
			cone.GetConeBounds(out upper, out lower);
			
			// Construct the mesh
	        Mesh mesh 		= sub.GetComponent<MeshFilter>().mesh;
	        mesh.Clear();
	        mesh.vertices 	= new Vector3[] { new Vector3(0, 0, 0), upper, lower };
	        mesh.uv 		= new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
	        mesh.triangles 	= new int[] 	{0, 1, 2};
			
			sub.renderer.material = aiming;
			
			sub.transform.parent = gameObject.transform;
			sub.transform.localPosition = new Vector3(0,0,0);
			
			GetDefender().SpinningUp = true;
		}
		
		// Use this for initialization
		public override void OnActivateBegin ()
		{
			Time.timeScale = 0.3f;
			ConstructBlast(1000, 25.0f);
		}
		
		// Update is called once per frame
		public override void OnActivateUpdate ()
		{
			sub.transform.RotateAround(Vector3.up, 10*Time.deltaTime);
			Vector3 angles 	= sub.transform.localRotation.eulerAngles;
			cone.dir 		= angles.y;
			cone.origin		= transform.position;
			
			// Draw the cone
			Vector3 mag			= new Vector3(0,0,cone.range);
			Vector3 endpoint	= Quaternion.Euler(0, cone.dir, 0)  * mag;
			Debug.DrawLine(transform.position, endpoint);
		}
		
		/// <summary>
		/// Use this for execution.
		/// </summary>
		public override void OnActivateEnd ()
		{
			CooldownBegin();
			PlaySound();
			Time.timeScale = 1;
			GameObject.Destroy(sub);

			// initialize my list
			List<Monster> targets = new List<Monster>();
			
			// get all the targets
			foreach( Monster monster in Game.Instance.Monsters )
			{
				if( cone.IsWithin(monster.transform.position) )
					targets.Add(monster);
			}
			
			// Do damage to them
			Defender defender 	= GetComponent<Defender>();

			foreach( Monster target in targets )
				Game.Instance.DoDamage(defender, this, target);
			
			GetDefender().SpinningUp = false;
		}
		
		public override void OnAvailable(){}
	}
}

