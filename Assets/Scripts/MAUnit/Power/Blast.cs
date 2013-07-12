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
		public ParticleSystem blastColumn;
		
		public float spread = 90.0f;
		public Material aiming;
		private GameObject sub;
		private MeshFilter filter;
		
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
			
			sub.transform.parent 		= gameObject.transform;
			sub.transform.localPosition = new Vector3(0,0,0);
		}
		
		/*
		public void Update()
		{
			Debug.DrawRay(GetDefender().weaponParent.transform.position, Vector3.forward, Color.cyan);

			// get all the targets
			Debug.Log("MONSETERS: " + Game.Instance.Monsters.Count );
			foreach( Monster monster in Game.Instance.Monsters )
			{
				if( cone.IsWithin(monster.transform.position) )
					Debug.DrawLine(GetDefender().weaponParent.transform.position, monster.transform.position, Color.green);
				else
					Debug.DrawLine(GetDefender().weaponParent.transform.position, monster.transform.position, Color.red);
			}
		}
		*/
		
		// Use this for initialization
		public override void OnActivateBegin ()
		{
			Time.timeScale = 0.3f;

			transform.forward = Vector3.forward;
			ConstructBlast(1000, spread);
			
			
			Defender defender = GetDefender();
			
			defender.weapon.CooldownBegin();
			defender.powerTargeting = true;
			
			GetComponent<Animator>().SetBool("Aiming", true);
		}
		
		// Update is called once per frame
		public void Update ()
		{
			// Sanity check
			if( !Activating )
				return;
			
			/*
			transform.RotateAround(Vector3.up, 10*Time.deltaTime);
			Vector3 angles 	= transform.localRotation.eulerAngles;
			cone.dir 		= angles.y;
			*/
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if( Physics.Raycast(ray, out hit, 100.0f, ~LayerMask.NameToLayer("Ground") ) )
			{
				Vector3 pnt = hit.point;
				pnt.y 		= transform.position.y;
				//cone.dir 	= Vector3.Angle(pnt, hit.point );
				
				transform.LookAt(pnt);
				
				//Debug.Log("Cone Dir: "+ cone.dir );
				Vector3 angles 	= transform.localRotation.eulerAngles;
				cone.dir 		= angles.y;
			}
			
			
			Vector3 pos 	= transform.position;//GetDefender().weaponParent.transform.position;
			pos.y			= GetDefender().weaponParent.transform.position.y;
			cone.origin		= pos;
			
			sub.transform.position = cone.origin;//GetDefender().weaponParent.transform.position;		
		}
		
		/// <summary>
		/// Use this for execution.
		/// </summary>
		public override void OnActivateEnd ()
		{
			CooldownBegin();
			Time.timeScale = 1;
			PlaySound();
			
			// Show the blast column
			PlayBlastColumns();			
			
			GameObject.Destroy(sub);

			// initialize my list
			List<Monster> targets = new List<Monster>();

			// get all the targets
			foreach( Monster monster in Game.Instance.Monsters )
			{
				
				if( cone.IsWithin(monster.transform.position) )
				{
					Debug.DrawLine(cone.origin, monster.transform.position, Color.green);
					targets.Add(monster);
				}
				else
					Debug.DrawLine(cone.origin, monster.transform.position, Color.red);
			}
			
			// Do damage to them
			Defender defender 	= GetComponent<Defender>();

			foreach( Monster target in targets )
				Game.Instance.DoDamage(defender, this, target);

			defender.powerTargeting = false;
			GetComponent<Animator>().SetBool("Aiming", false);
			

		}
		
		private void PlayBlastColumns()
		{
			if( blastColumn == null )
				return;
			
			float halfSpread = spread/2;
			for( float angle = -halfSpread; angle< halfSpread; angle+= 5 )
			{
				ParticleSystem effect = Instantiate(blastColumn, cone.origin, transform.rotation) as ParticleSystem;
				effect.transform.Rotate( new Vector3(270, angle, 0) );
				Destroy(effect.gameObject, blastColumn.duration);
			}
		}
		
		public override void OnAvailable(){}
	}
}

