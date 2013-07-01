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
	
	private Dictionary<EDefender, Defender> previews;
	
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
			previews = new Dictionary<EDefender, Defender>();
			state = EMenuState.MainMenu;
			Instantiate(template);
		}
		else
		{
			// Have them unlock a character first
			state = Session.Instance.GameData.HasDefenders()?EMenuState.Map:EMenuState.UpgradeStore;
		}
	}

	/// <summary>
	/// Raises the GU event.
	/// </summary>
	void OnGUI ()
	{
		GUI.skin = skin;		
		
		if( state == EMenuState.UpgradeStore ) 
		{
			ShowUpgradeStore();
		}
		else if( state == EMenuState.Map )
		{
			ClearPreviews();
			ShowMap();
		}
		else
		{
			ClearPreviews();
			ShowMainMenu();
		}
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
		GUI.enabled = state == EMenuState.UpgradeStore && Session.Instance.GameData.HasDefenders();
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
		
		
		for( int id=0;id<4;id++ )
		{
			// Get the 1st four defenders
			EDefender type 	= (EDefender)id;
			Vector2 pos 	= new Vector2(width*count, NAV_BAR_HEIGHT);
			
			// Get the saved data
			DefenderData data = Session.Instance.GameData.GetDefenderData(type);
			
			if( data != null )
				ShowTalentTree(pos, width, height, data);
			else
				ShowDefenderUnlock(pos, width, height, new DefenderData(type, true));	
	
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
	private void ShowDefenderUnlock(Vector2 offset, float width, float height, DefenderData defenderData)
	{		
		// Get an uninitanciated prefab
		Defender template = defenderData.GetDefender();
		
		float yinc		= width/6;
		float iconSize = 64;
		float buttonSize = (width-25)/2;
		///GUI.Box(new Rect(offset.x, offset.y,width,height), template.name);
		
		Vector2 position = new Vector2(offset.x, offset.y);
		
		GUI.enabled = Session.Instance.GameData.HasUnlocks();
		
		if( GUI.Button( new Rect(position.x, position.y, width, width), "" ) )
		{
			DoUnlock(defenderData.GetDefenderType());
		}
		
		// Get the available levels
		//GUI.Label( new Rect(offset.x + 20, offset.y+5, 100, 20), defender.name);
		
		
		int lastPre = 0;
		int peers = 0;
		int level = 0;
		float scale = 8.0f;
		
		Vector3 screenspace = new Vector3(offset.x + width/2, offset.y, Camera.main.nearClipPlane+10);
		
		
		Vector3 pos = Camera.main.ScreenToWorldPoint(screenspace);
		Defender defender;
		if( !previews.ContainsKey(template.type) )
		{
			defender = Instantiate(template) as Defender;
			defender.transform.position = pos;
			defender.transform.localScale = new Vector3(scale,scale,scale);
			
			previews.Add(template.type, defender);
		}
		else
		{
			defender = previews[template.type];
			defender.transform.position = pos;// + new Vector3(Mathf.Sin(Time.fixedTime), 0, 0);
		}
		
		GUI.Label(		new Rect( position.x, position.y, 	width,  64 ),  			template.name,	 GUI.skin.customStyles[4] );
		GUI.Label(		new Rect( position.x, position.y+yinc, width, 64 ), 		template.desc, 	 GUI.skin.customStyles[5] );		
		
		float sub = width-20;
		GUI.Box( 		new Rect( position.x+10, position.y+(yinc*2)-5, sub, (yinc*2)+5 ),"" );
		GUI.Box(		new Rect( position.x+20, position.y+(yinc*2), 64, 64 ), 	template.icon);
		GUI.Label(		new Rect( position.x+84, position.y+(yinc*2), sub, 64 ), 	defender.power.displayName, 	 GUI.skin.customStyles[6] );
		GUI.Label(		new Rect( position.x+20, position.y+(yinc*3), sub, 64 ), 	defender.power.desc, 	 GUI.skin.customStyles[5] );
	
		
		if( GUI.enabled )
			GUI.Label(		new Rect( position.x+10, position.y+(yinc*5), width,  64 ),  "UNLOCK NOW",	 GUI.skin.customStyles[4] );
		GUI.enabled = true;
	}	
	
	public void DoUnlock(EDefender type)
	{
		if( Session.Instance.GameData.Unlock(type) )
			state = EMenuState.Map;
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
		
		GUI.enabled = Session.Instance.GameData.HasDefenders();
		
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
		
		GUI.enabled = true;
	}	
	
	
	/// <summary>
	/// Clears the previews.
	/// </summary>
	private void ClearPreviews()
	{
		if( previews == null )
		{
			previews = new Dictionary<EDefender, Defender>();
			return;
		}
		List<Defender> preview = new List<Defender>(previews.Values);
		
		foreach( Defender local in preview )
			Destroy(local.gameObject);
		previews.Clear();
	}	
}
