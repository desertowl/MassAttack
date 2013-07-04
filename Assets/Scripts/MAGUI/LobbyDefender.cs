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
		public GameObject unitSelectBackground;		

		[HideInInspector]
		public Defender template;
		
		[HideInInspector]
		public Defender defender;
		
		private GameObject background;
		private Menu parent;
		private DefenderData data;

		private float scale = 8.0f;
		
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
			
			// Get an uninitanciated prefab
			template							= data.GetDefender();
			defender 							= GameObject.Instantiate(template) as Defender;
			defender.transform.localScale 		= new Vector3(scale,scale,scale);
			defender.transform.parent 	  		= transform;
			defender.transform.localPosition	= Vector3.zero;						

			// Add background object
			background = GameObject.Instantiate(unitSelectBackground) as GameObject;
			background.transform.parent = defender.transform;
			
			background.transform.localPosition = new Vector3(-0.38f,0.7f,-2);
			background.transform.localRotation	= Quaternion.Euler(90,0,0);	
			background.transform.localScale		= new Vector3(0.2120589f, 0.08835784f, 0.08835784f);
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
			Vector2 position 	= new Vector2(offset.x+width/3, offset.y);
			Vector3 screenspace = new Vector3(offset.x + width/2.8f, Screen.height/1.5f-offset.y, Camera.main.nearClipPlane+10);
			Vector3 worldspace	= Camera.main.ScreenToWorldPoint(screenspace);

			defender.transform.position = worldspace;
			
			
			GUI.Label(		new Rect( position.x+40, position.y+50, 	160,  64 ),  	template.name,	 GUI.skin.customStyles[7] );
			GUI.Label(		new Rect( position.x-40, position.y+50, 	160,  64 ),  	"LOcked? " + data.bLocked + " has Unlocks? " + hasUnlocks );
			
			// Get the proper texture
			Texture status;
			if( data.bLocked )
				status = hasUnlocks?unlockable:locked;
			else
				status = upgradeable;

			GUI.DrawTexture( new Rect(offset.x+width-140, offset.y-8, 128, 128), status );
			if( GUI.Button( new Rect(offset.x, offset.y-8, width, height), template.name ) )
			{
				if( data.bLocked && hasUnlocks )
				{
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
			float buttonSize = (width-25)/2;
			int lastPre 	= 0;
			int peers 		= 0;
			int level 		= 0;
			// Render out each talent, there should never be more than 2 per "level"
			Talent [] talents = defender.GetComponents<Talent>();
			
			Vector2 line = new Vector2(offset.x + bufferX, offset.y + level * (buttonSize/2+10) + bufferY - 10);
			foreach( Talent talent in talents )
			{
				if( talent.prerequisite != lastPre )
				{
					lastPre = talent.prerequisite;
					level++;
					peers = 0;
				
					line = new Vector2(offset.x + bufferX, offset.y + level * (buttonSize/2 + 20));
					
					GUI.Label(			new Rect(line.x, line.y -22, width + 10, 12), "" + lastPre + "+", GUI.skin.customStyles[Menu.GUISKIN_DESCRIPTION] );		
					GUI.DrawTexture( 	new Rect(line.x, line.y -3, width + 10, 1), divider );
				}
				
				Vector2 position = new Vector2(offset.x+10 + peers * (buttonSize+10) + bufferX, line.y);
				ShowTalent(position, buttonSize, defender, talent);
				peers++;
			}
			
			Vector3 screenspace = new Vector3(offset.x + width*1.6f, Screen.height/16.8f-offset.y, Camera.main.nearClipPlane+20);
			Vector3 worldspace	= Camera.main.ScreenToWorldPoint(screenspace);
			
			defender.transform.localScale = new Vector3(18,18,18);
			defender.transform.position = worldspace;			
			
		}
		
		/// <summary>
		/// Shows the talent.
		/// </summary>
		/// <param name='position'>
		/// Position.
		/// </param>
		/// <param name='buttonSize'>
		/// Button size.
		/// </param>
		/// <param name='defender'>
		/// Defender.
		/// </param>
		/// <param name='talent'>
		/// Talent.
		/// </param>
		private void ShowTalent(Vector2 position, float buttonSize, Defender defender, Talent talent)
		{
			GUI.enabled = Session.Instance.CanUnlock(talent);
			if( GUI.Button( new Rect( position.x, position.y, buttonSize, buttonSize/2 ), "", GUI.skin.customStyles[Menu.GUISKIN_SKILLBOX]))
			{
				//print ("Should unlock talent here");	
				Session.Instance.Unlock(talent);
			}
			
			GUI.Label(new Rect( position.x+3, position.y+3, buttonSize/2-6, buttonSize/2-6 ), talent.icon, GUI.skin.customStyles[Menu.GUISKIN_TALENTBOX] );
			
			GUI.Label(new Rect( position.x+buttonSize/2, position.y, buttonSize/2, buttonSize/2 ), talent.name, GUI.skin.customStyles[Menu.GUISKIN_LARGE_SUBTITLE] );
			GUI.Label(new Rect( position.x, position.y, buttonSize, buttonSize/2 ), "("+talent.GetUnlocked() + "/"+talent.max+")", GUI.skin.customStyles[Menu.GUISKIN_TALENT_COUNT] );
			GUI.Label(new Rect( position.x+buttonSize/2, position.y + 24, buttonSize/2, buttonSize/2 ), 	talent.desc, GUI.skin.customStyles[Menu.GUISKIN_DESCRIPTION] );		
			
			GUI.Label(new Rect( position.x+buttonSize/2, position.y + buttonSize/2 -24, buttonSize/2, buttonSize/2 ), talent.cost+"g", GUI.skin.customStyles[Menu.GUISKIN_TALENT_COUNT] );
			GUI.enabled = true;
		}		
	}
}

