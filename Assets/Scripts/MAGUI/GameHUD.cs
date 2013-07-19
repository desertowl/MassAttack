using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MAGUI;
using MACore;
using MAUnit;

public class GameHUD : MAHUD
{
	// Class Variables
	public static GameHUD Instance { get { return instance; } }
	private static GameHUD instance;	
	
	// Member variables
	public Texture dead;
	public GUISkin skin;
	private List<Status> status;
	
	private Defender current;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Awake()
	{
		instance = this;
		
		// Get the level
		current = Game.Instance.GetNextUntaughtDefender();
		if( current!=null )
		{
			Invoke("ShowPowerDialog", current.power.cooldown*1.1f );
		}
	}
	
	/// <summary>
	/// Shows the power dialog.
	/// </summary>
	public void ShowPowerDialog()
	{
		// Get the proper status
		Status found = GetStatus(current);
		
		// Quick sanity check
		if( found == null ) return;
		
		float ModalWidth  = Screen.width * 0.8f;
		float ModalHeight = Screen.height * 0.2f;
		Rect rect 		= new Rect( (Screen.width - ModalWidth)/2, (Screen.height - ModalHeight)/2, ModalWidth, ModalHeight);		
		
		// Show the modals as needed
		Vector2 target 	= new Vector2(found.offset.x + Status.BUTTON_SIZE/2, found.offset.y);
		Modal = new ModalData("Special Powers are important!", EModalType.Acknowledge, target);	
		Modal.rect = rect;
	}
	
	/// <summary>
	/// Clear this instance.
	/// </summary>
	public void Clear()
	{
		status = new List<Status>();		
	}
	
	/// <summary>
	/// Add the specified unit.
	/// </summary>
	/// <param name='unit'>
	/// Unit.
	/// </param>
	public void Add(Unit unit)
	{
		Status s = new Status();
		Vector2 pos = new Vector2();
		

		if( unit is Defender )
		{
			Defender def = (Defender)unit;
			switch( def.type )
			{
				case EDefender.Berserker:
					pos.x = 5;
					pos.y = Screen.height - (Status.BUTTON_SIZE/2+3)*2;
					break;
				case EDefender.Engineer:
					pos.x = 5;
					pos.y = Screen.height - (Status.BUTTON_SIZE/2+3);
					break;			
				case EDefender.Sniper:
					pos.x = Screen.width - Status.BUTTON_SIZE - 5;
					pos.y = Screen.height - (Status.BUTTON_SIZE/2+3)*2;
					break;
				case EDefender.Guardian:
					pos.x = Screen.width - Status.BUTTON_SIZE - 5;
					pos.y = Screen.height - (Status.BUTTON_SIZE/2+3);
					break;	
				case EDefender.Sentry:
					pos = AdjustLivingSentries();
					break;
			}		
		}
		
		s.SetUnit( pos, ref unit);
		status.Add(s);
	}
	
	private Vector2 AdjustLivingSentries()
	{
		Vector2 pos;
		int count = 0;
		
		pos.x = 5;
		pos.y = 5;
		
		foreach( Status s in status )
		{
			if( s.unit.IsDead() )
				continue;
			if( (s.unit is Defender && ((Defender)s.unit).type == EDefender.Sentry) || (s.unit is Monster) )
			{
				count++;
				
				s.offset = pos;
				
				pos.x = 5 + Status.BUTTON_SIZE +5;
			}
		}
		
		return pos;
	}
	
	/// <summary>
	/// Raises the GU event.
	/// </summary>
	void OnGUI ()
	{
		GUI.skin = skin;

		// Draw the nav bar
		DrawNavBar();

		// Switch according to the game state
		switch( Game.Instance.State )
		{
			case EGameState.Playing:
				DrawInGameUI();
				break;
			case EGameState.Victory:
				DrawResult("VICTORY!");
				break;	
			case EGameState.Defeat:
				DrawResult("DEFEAT!");
				break;				
		}
		
		
		
		if( DrawModal() )
		{
			if( current != null )
			{
				Session.Instance.GameData.SetTaught( current.type );
				GetStatus(current).disabled = false;
				current = null;
			}
		}

		if( current != null && Modal != null)
		{
			float height = Screen.height * 0.18f;
			LobbyDefender.ShowPower(current, new Vector2(Modal.rect.x, Modal.rect.y - height), Modal.rect.width, height);
			
			Status found = GetStatus(current);
			found.disabled = true;
			found.Draw();
		}
	}
	
	/// <summary>
	///  Gets the display gold. 
	/// </summary>
	/// <returns>
	///  The display gold. 
	/// </returns>
	protected override int GetDisplayGold()
	{
		return Game.Instance.GetGoldEarned();
	}	
	
	/// <summary>
	/// Draws the in game U.
	/// </summary>
	void DrawInGameUI()
	{
		List<Status> statues = new List<Status>(status);
		foreach( Status s in statues )
			s.Draw();	
	}
	
	/// <summary>
	/// Draws the result.
	/// </summary>
	/// <param name='title'>
	/// Title.
	/// </param>
	void DrawResult(string title)
	{
		GUI.Box(new Rect(0,0, Screen.width, Screen.height), title);
		
		GUI.Box(new Rect(Screen.width/2-150,50,300,300), "Mass Attack!");
		
		if (GUI.Button (new Rect (Screen.width/2-100,100, 200, 50), "Continue"))
		{
			// Save my earned gold
			Session.Instance.GameData.LevelComplete(Game.Instance);
			Session.Instance.GameData.Save();
			
			LoadLevel("Menu");
		}
		
		if (GUI.Button (new Rect (Screen.width/2-100,160, 200, 50), "Replay"))
		{
			// Save my earned gold
			Session.Instance.GameData.LevelComplete(Game.Instance);
			Session.Instance.GameData.Save();
			
			LoadLevel("Reload");
		}
		
		if (GUI.Button (new Rect (Screen.width/2-100,280, 200, 50), "Quit"))
		{
			Application.Quit();
		}	
	}
	
	private Status GetStatus(Unit unit)
	{
		foreach( Status s in status )
		{
			if( unit == s.unit )
				return s;
		}		
		return null;
	}
}
