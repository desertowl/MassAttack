using System;
using UnityEngine;
using MAGUI;

namespace MACore
{
	public class Planet : MonoBehaviour
	{
		public int id;
		public Level level;
		public GameObject ring;
		private bool isCurrent = false;
		private Vector3 defaultScale;
		private float rotationRate = 0.2f;
		private Vector3 axis;
		private bool current;
		private bool complete;
		
		/// <summary>
		/// Awake this instance.
		/// </summary>
		public void Awake()
		{
			rotationRate 	= UnityEngine.Random.value * 0.3f;
			defaultScale 	= ring.transform.localScale;
			axis 			= new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value );
			level.id		= id;
			
			UpdateAccessable();
		}
		
		public void UpdateAccessable()
		{
			if( Session.Instance == null ) return;
			
			int currentPlayerLevel 	= Session.Instance.GameData.level;
			current					= currentPlayerLevel == id;
			complete				= currentPlayerLevel > id;	
			
			if( complete )
				SetRingColor( new Color(0,1,1) );
			else if( current )
				SetRingColor( new Color(1,1,0) );
			else
				ring.SetActive(false);
			
			
		}
		
		/// <summary>
		/// Sets the color of the ring.
		/// </summary>
		/// <param name='color'>
		/// Color.
		/// </param>
		private void SetRingColor(Color color)
		{
			ring.SetActive(true);
			ring.renderer.material.SetColor("Tint Color", color);
		}
		
		public void Update()
		{
			transform.RotateAround(axis, rotationRate * Time.deltaTime );
			
			if( !current || !Session.Instance.GameData.HasDefenders() ) return;
			
			// Goes from 0 to 1
			float mod = Mathf.Sin(Time.fixedTime*3)/2 + 0.5f;
			
			Vector3 nextScale = new Vector3();
			nextScale.x = defaultScale.x + (mod*defaultScale.x/3.0f);
			nextScale.y = nextScale.x;
			nextScale.z = nextScale.x;
			
			ring.transform.localScale = nextScale;
			
		}
		
		public bool IsAvailable()
		{
			return current||complete;
		}
		
		/// <summary>
		/// Raises the GU event.
		/// </summary>
		public void OnGUI()
		{
			// Dont render anything if its not in the map state
			if( Menu.Instance.State != Menu.EMenuState.Map || !current)
				return;
			
			if( Menu.Instance != null )
			{
				GUI.skin 			= Menu.Instance.skin;
				Vector3 screenspace = Camera.main.WorldToScreenPoint(transform.position);
				screenspace.y 		= Screen.height - screenspace.y;				
				
				GUI.Label( new Rect(screenspace.x, screenspace.y, 100, 12), "Level " + (id+1), GUI.skin.customStyles[MAHUD.GUISKIN_WHITE_SUBTITLE] );
			}
		}
	}
}

