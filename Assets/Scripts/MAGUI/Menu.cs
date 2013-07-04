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
	public LobbyDefender lobbyDefenderTemplate;
	public Texture logo;
	
	public GUISkin skin;
	public Texture upgrade;
	public Texture playnow;
	public List<Level> levels;
	public Session template;
	public GameObject splash;
	
	
	public static readonly int GUISKIN_DESCRIPTION = 0;
	public static readonly int GUISKIN_HPBAR_EMPTY = 1;
	public static readonly int GUISKIN_HPBAR_FULL  = 2;
	public static readonly int GUISKIN_TALENT_COUNT= 3;
	public static readonly int GUISKIN_LARGE_CYAN  = 4;
	public static readonly int GUISKIN_DESC_TEXT   = 5;
	public static readonly int GUISKIN_LARGE_SUBTITLE = 6;
	public static readonly int GUISKIN_WHITE_SUBTITLE = 7;
	public static readonly int GUISKIN_SKILLBOX 	= 8;
	public static readonly int GUISKIN_TALENTBOX 	= 9;
	
	
	private List<LobbyDefender> defenders 	= null;
	private LobbyDefender selected 			= null;
	
	private enum EMenuState
	{
		MainMenu,
		Map,
		UpgradeStore,
		TalentTree
	}
	private EMenuState _state = EMenuState.MainMenu;
	private EMenuState State
	{
		get { return _state; }
		set
		{	
			_state = value;
			
			if( _state == EMenuState.UpgradeStore )
				InitializeDefenders();
			else
				HideLobbyDefenders();			
		}
	}
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	public void Awake()
	{
		if( Session.Instance == null )
		{
			State 	= EMenuState.MainMenu;

			Instantiate(template);
		}
		else
		{
			// Have them unlock a character first
			State = Session.Instance.GameData.HasDefenders()?EMenuState.Map:EMenuState.UpgradeStore;
		}
	}
	
	/// <summary>
	/// Selects the lobby defender.
	/// </summary>
	/// <param name='defender'>
	/// Defender.
	/// </param>
	public void SelectLobbyDefender(LobbyDefender defender)
	{
		selected 	= defender;
		State 		= EMenuState.TalentTree;
	}

	/// <summary>
	/// Raises the GU event.
	/// </summary>
	void OnGUI ()
	{
		GUI.skin = skin;		
		
		splash.SetActive(State == EMenuState.MainMenu);
		
		if( State == EMenuState.UpgradeStore ) 
		{
			ShowLobbyDefenders();
		}
		else if( State == EMenuState.Map )
		{
			ShowMap();
		}
		else if( State == EMenuState.TalentTree )
		{
			ShowTalentTree();
		}
		else
		{
			ShowMainMenu();
		}
	}
	
	/// <summary>
	/// Shows the main menu.
	/// </summary>
	public void ShowMainMenu()
	{
		//GUI.Box(new Rect(Screen.width/2-150,50,300,300), "Mass Attack!");
		
		float hoffset		= 100;
		float buttonWidth 	= Screen.width/2;
		float buttonHeight 	= buttonWidth/4;
		float valign 		= 10 + 128;
		
		GUI.DrawTexture(new Rect(10,10, 512, 128), logo );
		
		if (GUI.Button (new Rect (valign, hoffset, buttonWidth, buttonHeight), "New Game"))
		{
			// Create some brand new game data
			Session.Instance.GameData 	= new GameData();
			Session.Instance.GameData.Clear();
			State 						= EMenuState.Map;
		}
		
		GUI.enabled = GameData.HasGameData();
		if (GUI.Button (new Rect (valign, hoffset+10+buttonHeight, buttonWidth, buttonHeight), "Continue Game"))
		{
			// Load up the existing game data
			Session.Instance.GameData 	= GameData.Load();
			State 						= EMenuState.Map;
		}
		GUI.enabled = true;
		
		if (GUI.Button (new Rect (valign, hoffset+20+(2*buttonHeight), buttonWidth, buttonHeight), "Quit"))
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
		GUI.enabled = State == EMenuState.UpgradeStore && Session.Instance.GameData.HasDefenders();
		if (GUI.Button (new Rect(0,0, NAV_BAR_HEIGHT*3, NAV_BAR_HEIGHT), new GUIContent("Play", playnow)))
			State = EMenuState.Map;
		
		// Show the upgrade button
		GUI.enabled = State == EMenuState.Map;
		if (GUI.Button (new Rect(NAV_BAR_HEIGHT*3+10,0, NAV_BAR_HEIGHT*3, NAV_BAR_HEIGHT), new GUIContent("Research", upgrade)))
			State = EMenuState.UpgradeStore;
		GUI.enabled=true;
	}
	
	/// <summary>
	/// Shows the upgrade store.
	/// </summary>
	public void ShowLobbyDefenders()
	{
		DrawNavBar();

		float width 	= Screen.width/2;
		float height	= Screen.height-NAV_BAR_HEIGHT;
		
		// Show the trees
		for( int id=0;id<4;id++ )
		{
			// Get the 1st four defenders
			EDefender type 	= (EDefender)id;

			float offsetX   = 10;
			float offsetY 	= Screen.height/8.0f;
				
			if( id >= 2 )
				offsetY		+= Screen.height/2;
			Vector2 pos 	= new Vector2( (width*(id%2) )+10, offsetY);	
			
			defenders[id].gameObject.SetActive(_state == EMenuState.UpgradeStore );
			defenders[id].DrawPick(pos);
		}
	}
	
	private void InitializeDefenders()
	{
		// Get the data
		if( defenders != null )
			return;
		
		defenders = new List<LobbyDefender>();
		for( int id=0;id<4;id++ )
		{
			// Get the 1st four defenders
			EDefender type 	= (EDefender)id;
			
			// Get the saved data
			DefenderData data = Session.Instance.GameData.GetDefenderData(type);

			if( data == null )
				data = new DefenderData(type, true);
			else
				data.bLocked = false;
			
			LobbyDefender defender = GameObject.Instantiate(lobbyDefenderTemplate) as LobbyDefender;
			defender.Initialize(this, data);
			defenders.Add(defender);
		}		
	}
	
	public void ShowTalentTree()
	{
		DrawNavBar();
		
		selected.gameObject.SetActive(_state == EMenuState.TalentTree );
		selected.ShowTalentTree( new Vector2(0,NAV_BAR_HEIGHT+12), Screen.width/2, Screen.height-2*NAV_BAR_HEIGHT);
	}
	
	/// <summary>
	/// Dos the unlock.
	/// </summary>
	/// <returns>
	/// The unlock.
	/// </returns>
	/// <param name='type'>
	/// Type.
	/// </param>
	public DefenderData DoUnlock(EDefender type)
	{
		return Session.Instance.GameData.Unlock(type);
		//if( data != null  ) state = EMenuState.Map;
		//return data;
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
	/// Cears the trees.
	/// </summary>
	private void HideLobbyDefenders()
	{
		if( defenders == null )
			return;
		
		foreach( LobbyDefender defender in defenders )
			defender.gameObject.SetActive(false);
	}	
}
