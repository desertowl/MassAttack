using System;
using UnityEngine;
using MAUnit;
using MAPlayer;
using MACore;

namespace MAGUI
{
	public class LobbyDefender : MonoBehaviour
	{
		public Texture unlockable, upgradeable, locked, divider;
		public Texture VelicLabel, CretianLabel, DurusLabel, RaskerLabel;
		public GameObject unitSelectBackground;		
		public AudioClip unlockTalent, unlockDefender;

		[HideInInspector]
		public Defender template;
		
		[HideInInspector]
		public Defender defender;
		
		private GameObject background;
		private Menu parent;
		private DefenderData data;

		private float scale = 7.0f;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MAGUI.TalentTreeGUI"/> class.
		/// </summary>
		/// <param name='menu'>
		/// Menu.
		/// </param>
		/// <param name='data'>
		/// Data.
		/// </param>
		public void Initialize(Menu menu, DefenderData data)
		{
			parent		= menu;
			this.data 	= data;
			
			// Set the local scale
			transform.localScale				= new Vector3(scale, scale, scale);
			
			// Get an uninitanciated prefab
			template							= data.GetDefender();
			defender 							= GameObject.Instantiate(template) as Defender;
			defender.transform.localScale 		= new Vector3(scale,scale,scale);
			defender.transform.parent 	  		= transform;
			defender.transform.localPosition	= Vector3.zero;	
			
			// Add background object
			background = GameObject.Instantiate(unitSelectBackground) as GameObject;
			background.transform.parent 		= transform;
			
			background.transform.localPosition 	= new Vector3(-0.38f,0.7f,-2);
			background.transform.localRotation	= Quaternion.Euler(90,0,0);	
			background.transform.localScale		= new Vector3(0.2120589f, 0.08835784f, 0.08835784f);
		}
		
		private Texture GetDefenderLabel()
		{
			switch( template.type )
			{
				case EDefender.Berserker:
					return CretianLabel;
				case EDefender.Engineer:
					return RaskerLabel;
				case EDefender.Sniper:
					return VelicLabel;
			}
			return DurusLabel;
		}
		
		private Color GetDefenderColor()
		{
			if( data.bLocked )
				return new Color(0.133f, 0.133f, 0.133f );
			
			switch( template.type )
			{
				case EDefender.Berserker:
					return new Color(1,0,0,1);
				case EDefender.Engineer:
					return new Color(0,1,0,1);
				case EDefender.Sniper:
					return new Color(1,0,1,1);
			}
			return new Color(0.1f,0,1,1);
		}		
		
		public void DrawPick(Vector2 origin)
		{			
			ShowDefenderPick(origin, Screen.width/2-10, Screen.height/3-30);				
		}
		
		/// <summary>
		/// Shows the defender unlock.
		/// </summary>
		/// <param name='offset'>
		/// Offset.
		/// </param>
		/// <param name='width'>
		/// Width.
		/// </param>
		/// <param name='height'>
		/// Height.
		/// </param>
		/// <param name='defenderData'>
		/// Defender data.
		/// </param>
		private void ShowDefenderPick(Vector2 offset, float width, float height)
		{					
			bool hasUnlocks		= Session.Instance.GameData.HasUnlocks();			
			Vector3 screenspace = new Vector3(offset.x + width/2.8f, Screen.height/1.5f-offset.y, Camera.main.nearClipPlane+10);
			Vector3 worldspace	= Camera.main.ScreenToWorldPoint(screenspace);
			
			transform.position	= worldspace;
			defender.transform.localScale = Vector3.one;

			
			Vector3 center  	= Camera.main.WorldToScreenPoint( background.transform.position );
			Vector3 hotspot  	= Camera.main.WorldToScreenPoint( background.transform.GetChild(0).position );
			hotspot.y 			= Math.Abs(hotspot.y - Screen.height);
			center.y 			= Math.Abs(center.y - Screen.height);
			
			// Set the background color
			background.renderer.material.SetColor("_Color", GetDefenderColor() );
			
			// Get the proper texture
			Texture status;
			if( data.bLocked )
			{
				defender.gameObject.SetActive(false);
				status = hasUnlocks?unlockable:locked;
			}
			else
			{
				defender.gameObject.SetActive(true);
				status = upgradeable;
			}
			
			float size = (center.y-hotspot.y) * 2.0f;
			
			// Draw the status			
			GUI.DrawTexture( new Rect(hotspot.x-size, hotspot.y, size, size), status );
			if( GUI.Button( new Rect(offset.x+32, offset.y-8, width, height), "", GUI.skin.customStyles[0] ) )
			{
				if( data.bLocked && hasUnlocks )
				{
					UnityEngine.AudioSource.PlayClipAtPoint(unlockDefender, Camera.main.transform.position);
					DefenderData next = parent.DoUnlock(data.GetDefenderType());
					
					if( next != null )
						data = next;
				}
				else
				{
					if( !data.bLocked )
						parent.SelectLobbyDefender(this);
				}
			}
			
			GUI.DrawTexture( new Rect(hotspot.x-size - size/3, hotspot.y + size/2, size, size/2), GetDefenderLabel() );
			//GUI.Label(		new Rect( position.x + 32, offset.y +22, 	100,  64 ),  	template.name,	 GUI.skin.customStyles[MAHUD.GUISKIN_WHITE_SUBTITLE] );			
		}			
		
		/// <summary>
		/// Shows the talent tree.
		/// </summary>
		/// <param name='offset'>
		/// Offset.
		/// </param>
		/// <param name='width'>
		/// Width.
		/// </param>
		/// <param name='height'>
		/// Height.
		/// </param>
		/// <param name='defender'>
		/// Defender.
		/// </param>
		public void ShowTalentTree(Vector2 offset, float width, float height)
		{			
			float bufferY 	= 8;
			float bufferX 	= 0;
			float buttonWidth 	= (width*0.55f);
			float buttonHeight 	= height * 0.18f;
			int lastPre 		= 0;
			int peers 			= 0;
			int level 			= 0;
			
			
			// Render the defender
			Vector3 screenspace = new Vector3(offset.x + width*1.45f, Screen.height/22f-offset.y, Camera.main.nearClipPlane+20);
			Vector3 worldspace	= Camera.main.ScreenToWorldPoint(screenspace);
			defender.transform.localScale = new Vector3(2.5f,2.5f,2.5f);
			transform.position 	= worldspace;			
			
			
			// Render out the power
			ShowPower( defender, new Vector2(offset.x+10, offset.y+10), width*1.1f+10, buttonHeight );
			
			// Render out each talent, there should never be more than 2 per "level"
			Talent [] talents = defender.GetComponents<Talent>();
			
			
			
			offset.y += buttonHeight+20;
			
			Vector2 line = new Vector2(offset.x + bufferX, offset.y + level * (buttonHeight+10) + bufferY - 10);
			foreach( Talent talent in talents )
			{
				if( talent.prerequisite != lastPre )
				{
					lastPre = talent.prerequisite;
					level++;
					peers = 0;
				
					line = new Vector2(offset.x + bufferX, offset.y + level * (buttonHeight + 20));
					
					GUI.Label(			new Rect(line.x, line.y -22, width + 10, 12), "" + lastPre + "+", GUI.skin.customStyles[MAHUD.GUISKIN_DESCRIPTION] );		
					GUI.DrawTexture( 	new Rect(line.x, line.y -3, width + 10, 1), divider );
				}
				
				Vector2 position = new Vector2(offset.x+10 + peers * (buttonWidth+10) + bufferX, line.y);
				ShowTalent(position, buttonWidth, buttonHeight, defender, talent);
				peers++;
			}			
		}
		
		/// <summary>
		/// Shows the talent.
		/// </summary>
		/// <param name='position'>
		/// Position.
		/// </param>
		/// <param name='buttonWidth'>
		/// Button width.
		/// </param>
		/// <param name='buttonHeight'>
		/// Button height.
		/// </param>
		/// <param name='defender'>
		/// Defender.
		/// </param>
		/// <param name='talent'>
		/// Talent.
		/// </param>
		private void ShowTalent(Vector2 position, float buttonWidth, float buttonHeight, Defender defender, Talent talent)
		{
			GUI.enabled = Session.Instance.CanUnlock(talent);
			if( GUI.Button( new Rect( position.x, position.y, buttonWidth, buttonHeight ), "", GUI.skin.customStyles[MAHUD.GUISKIN_SKILLBOX]))
			{
				UnityEngine.AudioSource.PlayClipAtPoint(unlockTalent, Camera.main.transform.position);
				Session.Instance.Unlock(talent);
			}
			
			
			GUI.Label(		new Rect( position.x+3, position.y+3, buttonHeight-6, buttonHeight-6 ), "", GUI.skin.customStyles[MAHUD.GUISKIN_TALENTBOX] );
			
			if( talent.icon != null )
				GUI.DrawTexture(new Rect( position.x+3, position.y+3, buttonHeight-6, buttonHeight-6 ), talent.icon );
			
			GUI.Label(new Rect( position.x+buttonHeight, position.y, buttonHeight, buttonHeight ), talent.name, GUI.skin.customStyles[MAHUD.GUISKIN_LARGE_SUBTITLE] );
			GUI.Label(new Rect( position.x, position.y, buttonWidth, buttonHeight ), "("+talent.GetUnlocked() + "/"+talent.max+")", GUI.skin.customStyles[MAHUD.GUISKIN_TALENT_COUNT] );
			GUI.Label(new Rect( position.x+buttonHeight, position.y + 16, buttonWidth-buttonHeight, buttonHeight ), 	talent.desc, GUI.skin.customStyles[MAHUD.GUISKIN_DESCRIPTION] );		
			
			//GUI.Label(new Rect( position.x+buttonHeight, position.y + buttonHeight - 16,buttonWidth - 30, buttonHeight ), talent.cost+"g", GUI.skin.customStyles[MAHUD.GUISKIN_TALENT_COUNT] );
			GUI.Label(new Rect( position.x, position.y + buttonHeight -20, buttonWidth, buttonHeight ), talent.cost+"g", GUI.skin.customStyles[MAHUD.GUISKIN_TALENT_COUNT] );
			GUI.enabled = true;
		}
		
		/// <summary>
		/// Shows the power.
		/// </summary>
		/// <param name='defender'>
		/// Defender.
		/// </param>
		/// <param name='position'>
		/// Position.
		/// </param>
		/// <param name='width'>
		/// Width.
		/// </param>
		/// <param name='height'>
		/// Height.
		/// </param>
		public static void ShowPower(Defender defender, Vector2 position, float width, float height)
		{
			GUI.Label( new Rect( position.x, position.y, width, height ), "", GUI.skin.customStyles[MAHUD.GUISKIN_TALENTBOX]);

			GUI.Label( new Rect( position.x+3, position.y+3, height-6, height-6 ), "", GUI.skin.customStyles[MAHUD.GUISKIN_TALENTBOX] );
			GUI.DrawTexture(new Rect( position.x+3, position.y+3, height-6, height-6 ), defender.power.label );
			
			GUI.Label(new Rect( position.x+height, position.y, width, height ), defender.power.displayName, GUI.skin.customStyles[MAHUD.GUISKIN_LARGE_SUBTITLE] );
			GUI.Label(new Rect( position.x+height, position.y + 16, width-height, height ), defender.power.desc, GUI.skin.customStyles[MAHUD.GUISKIN_DESCRIPTION] );		
		}
	}
}

