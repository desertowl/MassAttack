using System;
using System.Collections.Generic;
using UnityEngine;
using MACore;

namespace MAUnit
{
	public abstract class RadialPower : Power
	{
		public Material material;
		public float radius;
		
		// The instance of the area to spawn
		protected GameObject instance;		
		
		
		/// <summary>
		/// Constructs the area.
		/// </summary>
		protected void ConstructArea()
		{
			instance = new GameObject();
			
	        instance.AddComponent("MeshFilter");
	        instance.AddComponent("MeshRenderer");
			
			// Construct the mesh
	        Mesh mesh 		= instance.GetComponent<MeshFilter>().mesh;
	        mesh.Clear();
			
			List<Vector3> verts = new List<Vector3>();
			List<Vector2> uvs 	= new List<Vector2>();
			List<int> tris  	= new List<int>();
			verts.Add( new Vector3(0, 0, 0) );
			uvs.Add (new Vector2(0, 0));
			
			float inc = 10;
			
			bool change = false;
			for( float x=0;x<360.0f;x+=inc )
			{
				verts.Add( GetPositionAtAngle(x) );
				
				uvs.Add( new Vector2(change?0:1, 1) );
				change = !change;

				if( verts.Count>=2 )
				{
					tris.Add(0);
					tris.Add(verts.Count-2);
					tris.Add(verts.Count-1);					
				}
			}
			
			tris.Add (0);
			tris.Add(verts.Count-1);
			tris.Add(1);					
			
			
			mesh.vertices	= verts.ToArray();
	        mesh.uv 		= uvs.ToArray();
	        mesh.triangles 	= tris.ToArray();
			
			// Set the material & transform
			instance.renderer.material 	= material;
			instance.transform.parent 	= gameObject.transform;
			instance.transform.localPosition = new Vector3(0,0,0);
		}
		
		/// <summary>
		/// Gets the position at angle.
		/// </summary>
		/// <returns>
		/// The position at angle.
		/// </returns>
		/// <param name='deg'>
		/// Deg.
		/// </param>
		private Vector3 GetPositionAtAngle(float deg)
		{
			// Get the angle
			float a = Mathf.Deg2Rad * deg;
			
			// Create the position
			return new Vector3( Mathf.Sin(a) * radius, 0.0f, Mathf.Cos(a) * radius);
		}
		
		
		/// <summary>
		/// Gets the monsters in range.
		/// </summary>
		/// <returns>
		/// The monsters in range.
		/// </returns>
		protected List<Monster> GetMonstersInRange()
		{
			// list of resulting monsters
			List<Monster> result = new List<Monster>();
			
			foreach( Monster monster in Game.Instance.Monsters )
			{
				// get the distance between me and this monster!
				float dist = Vector3.Distance(transform.position, monster.transform.position);

				if( dist < radius )
					result.Add(monster);
			}
			return result;
		}		
	}
}

