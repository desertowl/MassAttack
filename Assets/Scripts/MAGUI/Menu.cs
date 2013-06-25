using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MAGUI;
using MAUnit;
using MACore;
using MAPlayer;
using JsonFx;

public class Menu : MAHUD
{
	public GUISkin skin;
	public Texture upgrade;
	public Texture playnow;
	public List<Level> levels;
	public Session template;
	
	private enum EMenuState
	{
		MainMenu,
		Map,
		UpgradeStore,
	}
	private EMenuState state = EMenuState.MainMenu;
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	public void Awake()
	{
		if( Session.Instance == null )
		{
			state = EMenuState.MainMenu;
			Instantiate(template);
		}
		else
		{
			state = EMenuState.Map;
		}
	}

	/// <summary>
	/// Raises the GU event.
	/// </summary>
	void OnGUI ()
	{
		GUI.skin = skin;		
		
		if( state == EMenuState.UpgradeStore ) 
			ShowUpgradeStore();
		else if( state == EMenuState.Map )
			ShowMap();
		else
			ShowMainMenu();
			
	}
	
	/// <summary>
	/// Shows the main menu.
	/// </summary>
	public void ShowMainMenu()
	{
		GUI.Box(new Rect(Screen.width/2-150,50,300,300), "Mass Attack!");
		
		if (GUI.Button (new Rect (Screen.width/2-100,100, 200, 50), "New Game"))
		{
			// Create some brand new game data
			Session.Instance.GameData 	= new GameData();
			Session.Instance.GameData.Clear();
			state 						= EMenuState.Map;
		}
		
		GUI.enabled = GameData.HasGameData();
		if (GUI.Button (new Rect (Screen.width/2-100,160, 200, 50), "Continue Game"))
		{
			// Load up the existing game data
			Session.Instance.GameData 	= GameData.Load();
			state 						= EMenuState.Map;
		}
		GUI.enabled = true;
		
		if (GUI.Button (new Rect (Screen.width/2-100,280, 200, 50), "Quit"))
		{
			Application.Quit();
		}			
	}
	
	/// <summary>
	/// Draws the nav bar.
	/// </summary>
	public override void DrawNavBar()
	{		
		// Draw the background
		base.DrawNavBar();
		
		// Show the play button
		GUI.enabled = state == EMenuState.UpgradeStore;
		if (GUI.Button (new Rect(0,0, NAV_BAR_HEIGHT*3, NAV_BAR_HEIGHT), new GUIContent("Play", playnow)))
			state = EMenuState.Map;
		
		// Show the upgrade button
		GUI.enabled = state == EMenuState.Map;
		if (GUI.Button (new Rect(NAV_BAR_HEIGHT*3+10,0, NAV_BAR_HEIGHT*3, NAV_BAR_HEIGHT), new GUIContent("Research", upgrade)))
			state = EMenuState.UpgradeStore;
		GUI.enabled=true;
	}
	
	/// <summary>
	/// Shows the upgrade store.
	/// </summary>
	public void ShowUpgradeStore()
	{
		DrawNavBar();
		
		float count		= 0;
		float width 	= ((Screen.width/4)+10);
		float height	= Screen.height-NAV_BAR_HEIGHT;
		
		// Get the data
		List<DefenderData> defenders = Session.Instance.GameData.GetDefenderData();
		
		foreach( DefenderData defender in defenders )
		{
			Vector2 pos = new Vector2(width*count, NAV_BAR_HEIGHT);
			ShowTalentTree(pos, width, height, defender);
			count++;
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
	private void ShowTalentTree(Vector2 offset, float width, float height, DefenderData defenderData)
	{		
		// Get an uninitanciated prefab
		Defender defender = defenderData.GetDefender();
		
		float iconSize = 32;
		float buttonSize = (width-25)/2;
		GUI.Box(new Rect(offset.x, offset.y,width,height), defender.name);
		
		// Get the available levels
		//GUI.Label( new Rect(offset.x + 20, offset.y+5, 100, 20), defender.name);
		GUI.DrawTexture( new Rect(offset.x+5, offset.y+5, iconSize,iconSize), defender.icon);
		
		int lastPre = 0;
		int peers = 0;
		int level = 0;
		// Render out each talent, there should never be more than 2 per "level"
		Talent [] talents = defender.GetComponents<Talent>();
		foreach( Talent talent in talents )
		{
			if( talent.prerequisite != lastPre )
			{
				lastPre = talent.prerequisite;
				level++;
				peers = 0;
			}
			
			Vector2 position = new Vector2(offset.x+10 + peers * (buttonSize+10), offset.y + level * (buttonSize+10) + iconSize);
			ShowTalent(position, buttonSize, defender, talent);
			peers++;
		}
		
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
		if( GUI.Button( new Rect( position.x, position.y, buttonSize, buttonSize ), talent.icon))
		{
			//print ("Should unlock talent here");	
			Session.Instance.Unlock(talent);
		}
		
		GUI.Label(new Rect( position.x, position.y, buttonSize, buttonSize ), 	talent.name );
		GUI.Label(new Rect( position.x, position.y, buttonSize, buttonSize ), 	"("+talent.GetUnlocked() + "/"+talent.max+")", GUI.skin.customStyles[3] );
		GUI.Label(new Rect( position.x, position.y, buttonSize, buttonSize ), 	talent.desc, GUI.skin.customStyles[0] );		
		GUI.enabled = true;
	}
	
	public void ShowMap()
	{
		DrawNavBar();
		
		// Get the available levels
		
		Vector2 pos;
		int count = 0;
		int iconSize = 128;
		foreach( Level level in levels )
		{
			pos = new Vector2(Screen.width/2-(iconSize/2), 120 + count*(iconSize+10));
			
			count++;
			GUI.Label( new Rect(pos.x + 20, pos.y+5, 100, 20), "Level " + count);
			if (GUI.Button (new Rect (pos.x, pos.y, iconSize, iconSize), level.icon))
			{
				Session.Instance.TargetLevel = level;
				Application.LoadLevel("Game");
			}		
		}		
	}	
	
}
