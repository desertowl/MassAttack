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
	// Class Variables
	private static Menu _instance;
	public static Menu Instance { get {return _instance;} }	
	
	// Member bariables
	public LobbyDefender lobbyDefenderTemplate;
	public Texture logo;
	
	public Material unavailable;
	
	public GUISkin skin;
	public Texture upgrade;
	public Texture playnow;
	public GameObject space; // the final frontier
	//public List<Level> levels;
	public Session template;
	public GameObject splash;
	
	
	private List<LobbyDefender> defenders 	= null;
	private LobbyDefender selected 			= null;
	private readonly float NavButtonSize 	= 90;
	
	
	
	public enum EMenuState
	{
		MainMenu,
		Map,
		UpgradeStore,
		TalentTree
	}
	private EMenuState _state = EMenuState.MainMenu;
	public EMenuState State
	{
		get { return _state; }
		set
		{	
			_state = value;
			
			
			if( _state == EMenuState.TalentTree )
			{
				SetBackgroundColor( Defender.GetDefenderColor( selected.defender.type ) );
			}
			else
			{
				ClearBackgroundColor();
			}
			
			space.SetActive(_state == EMenuState.Map);
			if( _state == EMenuState.Map )
			{
				ActivateSpace();
			}
			if( _state == EMenuState.UpgradeStore )
			{
				InitializeDefenders();
			}
			else
			{
				HideLobbyDefenders();			
			}
		}
	}
	
	private void ActivateSpace()
	{
		Vector3 last = Vector3.zero;
		
		List<Planet> planets = new List<Planet>(space.GetComponentsInChildren<Planet>());
		planets.Sort( (p1, p2)  => p2.id - p1.id );
		
		foreach( Planet planet in planets )
		{
			
			if( planet.UpdateAccessable() && planet.level.id >= 0 )
			{
				Vector3 next = planet.transform.position;
				AddLine( last, next );
				last = next;
			}
		}
		
		float ModalSize 	= 128;
		if( Session.Instance.GameData.level == 1 && Session.Instance.GameData.gold >= 20 )
			Modal = new ModalData("You may upgrade your character by pressing the upgrade button", EModalType.Acknowledge);
		
		else if( Session.Instance.GameData.HasUnlocks() )
		{
			Modal = new ModalData("You may select a new character!", EModalType.Acknowledge);
			//Modal = new ModalData(new Rect( Screen.width - NavButtonSize/5 - ModalSize, Screen.height - NavButtonSize - ModalSize, ModalSize, ModalSize), "Unlock A Character!", EModalType.Acknowledge);
		}		
	}
	
	private void AddLine(Vector3 start, Vector3 end )
	{
		if( start == Vector3.zero ) return;
		
		GameObject sub 		= GameObject.Instantiate( Resources.Load("Levels/Line") ) as GameObject;
		LineRenderer line 	= sub.GetComponent<LineRenderer>();
		sub.transform.parent = space.transform;
		
		line.SetPosition(0, start);
		line.SetPosition(1, end);
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	public void Update()
	{
		// Only check for the hit levels in the map state
		if( State != EMenuState.Map || !Session.Instance.GameData.HasDefenders() )
			return;
		
		
		if( Input.GetMouseButtonDown(0) )
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			RaycastHit hit;
			if( Physics.Raycast(ray, out hit, 1000.0f) )
			{
				Planet planet 				= hit.collider.gameObject.GetComponent<Planet>();
				
				if( !planet.IsAvailable() )
					return;

				planet.level.id 			 = planet.id;
				Session.Instance.TargetLevel = planet.level;
				Application.LoadLevel("Game");
			}
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
		_instance = this;
	}
	
	public void SetBackgroundColor( Color color )
	{
		//SkyMaterial.SetColor("_foreground", color );
		//RenderSettings.skybox = SkyMaterial;
	}
	public void ClearBackgroundColor()
	{
		//RenderSettings.skybox = StarsMaterial;
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
		
		if( Modal == null )
		{
			GUI.enabled = GameData.HasGameData();
			if (GUI.Button (new Rect (valign, hoffset, buttonWidth, buttonHeight), "Continue Game"))
			{
				// Load up the existing game data
				Session.Instance.GameData 	= GameData.Load();
				State 						= EMenuState.Map;
			}
			GUI.enabled = true;		
			
			if (GUI.Button (new Rect (valign, hoffset+10+buttonHeight, buttonWidth, buttonHeight), "New Game"))
			{
				
				if( GameData.HasGameData() )
				{
					Modal = new ModalData("Starting a new game will erase your current progress!\n\nAre you sure?", EModalType.Confirm);
				}
				else
				{
					StartNewGame();
				}
			}
			
			if (GUI.Button (new Rect (valign, hoffset+20+(2*buttonHeight), buttonWidth, buttonHeight), "Quit"))
			{
				Application.Quit();
			}	
		}
		
		if( DrawModal() )
			StartNewGame();
	}
	
	private void StartNewGame()
	{
		// Create some brand new game data
		Session.Instance.GameData 	= new GameData();
		Session.Instance.GameData.Clear();
		State 						= EMenuState.Map;
	}
	
	/// <summary>
	/// Draws the nav bar.
	/// </summary>
	public override void DrawNavBar()
	{		
		// Draw the background
		base.DrawNavBar();
		
		GUIStyle style = GUI.skin.customStyles[GUISKIN_BACKBUTTON];
		/*
		string label = "";
		
		switch( State )
		{
			case EMenuState.Map:
			case EMenuState.TalentTree:
				label = "Upgrades";
				break;
			case EMenuState.UpgradeStore:
				style = GUI.skin.customStyles[GUISKIN_SPACEBUTTON];
				label = "Map";
				break;			
		}
		*/
		GUI.enabled = Modal == null;
		if( GUI.Button( new Rect( Screen.width - NavButtonSize - 2, Screen.height - NavButtonSize - 2, NavButtonSize, NavButtonSize), "", style ) )
		{
			switch( State )
			{
				case EMenuState.Map:
				case EMenuState.TalentTree:
					State = EMenuState.UpgradeStore;
					break;
				case EMenuState.UpgradeStore:
					State = EMenuState.Map;
					break;			
			}
		}
		GUI.enabled = true;
	}
	
	/// <summary>
	/// Shows the upgrade store.
	/// </summary>
	public void ShowLobbyDefenders()
	{
		DrawNavBar();

		float width 	= Screen.width * 0.45f;
		
		// Show the trees
		for( int id=0;id<4;id++ )
		{
			// Get the 1st four defenders
			float offsetY 	= NAV_BAR_HEIGHT+3;
				
			if( id >= 2 )
				offsetY		+= Screen.height*0.45f;
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

		selected.gameObject.SetActive(_state == EMenuState.TalentTree);
		selected.ShowTalentTree( new Vector2(0,5), Screen.width*0.6f, Screen.height-2*NAV_BAR_HEIGHT);
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
	
	/// <summary>
	/// Gets the planets.
	/// </summary>
	/// <returns>
	/// The planets.
	/// </returns>
	private List<Planet> GetPlanets()
	{
		List<Planet> planets = new List<Planet>(space.transform.GetComponentsInChildren<Planet>());
		
		planets.Sort( (a,b) => a.id - b.id );
		return planets;
	}
	
	public void ShowMap()
	{
		//bool hasDefenders = Session.Instance.GameData.HasDefenders();
		
		// Get the available levels		
		// int currentLevel 	= Session.Instance.GameData.level;		
		DrawNavBar();
		DrawModal();
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
