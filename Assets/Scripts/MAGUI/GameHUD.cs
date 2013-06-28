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
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Awake()
	{
		instance = this;
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
		s.SetUnit( new Vector2(0,status.Count * 70.0f), ref unit);
		status.Add(s);
	}
	
	/// <summary>
	/// Raises the GU event.
	/// </summary>
	void OnGUI ()
	{
		GUI.skin = skin;
		
		// get the game state
		EGameState state = Game.Instance.GetGameState();
		
		// Draw the nav bar
		DrawNavBar();
		
		switch( state )
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
		
	void DrawResult(string title)
	{
		GUI.Box(new Rect(0,0, Screen.width, Screen.height), title);
		
		GUI.Box(new Rect(Screen.width/2-150,50,300,300), "Mass Attack!");
		
		if (GUI.Button (new Rect (Screen.width/2-100,100, 200, 50), "Continue"))
		{
			// Save my earned gold
			Session.Instance.GameData.gold += Game.Instance.GetGoldEarned();
			Session.Instance.GameData.Save();
			Application.LoadLevel("Menu");
		}
		
		if (GUI.Button (new Rect (Screen.width/2-100,160, 200, 50), "Replay"))
		{
			// Save my earned gold
			Session.Instance.GameData.gold += Game.Instance.GetGoldEarned();
			Session.Instance.GameData.Save();
			Application.LoadLevel(Application.loadedLevel);
		}
		
		if (GUI.Button (new Rect (Screen.width/2-100,280, 200, 50), "Quit"))
		{
			Application.Quit();
		}	
	}
}
