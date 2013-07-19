using System;
using UnityEngine;
using MACore;

namespace MAGUI
{
	public class MAHUD : MonoBehaviour
	{
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
		public static readonly int GUISKIN_BACKBUTTON 	= 10;
		public static readonly int GUISKIN_SPACEBUTTON 	= 11;
		public static readonly int GUISKIN_MODALBACK 	= 12;	
		public static readonly int GUISKIN_TOOLTIP_RIGHT= 13;
		public static readonly int GUISKIN_MODAL		= 14;
		
		
		public Texture goldcoin;
		public Texture arrow;
		protected readonly float NAV_BAR_HEIGHT = 35.0f;	
		
		private string LevelToLoad;
		
		public enum EModalType
		{
			Simple,
			Acknowledge,
			Confirm,
			Loading
		}
		
		public class ModalData
		{
			public Rect rect;
			public string text;
			public EModalType type;
			public Vector2 target;
			
			
			public ModalData(Rect rect, string text, EModalType type)
			{
				this.rect = rect;
				this.text = text;
				this.type = type;
			}
			
			public ModalData(string text, EModalType type, Vector2 target)
			{
				float ModalSize = Screen.width/3;
				this.rect 		= new Rect( (Screen.width - ModalSize) / 2, (Screen.height - ModalSize)/2, ModalSize, ModalSize);

				this.text = text;
				this.type = type;	
				this.target = target;
			}
			
			public ModalData(string text, EModalType type) : this(text, type, Vector2.zero)
			{	
			}
			
			public bool IsSimple()
			{
				return type == EModalType.Simple;
			}
		}
		public ModalData Modal = null;
		
		
		/// <summary>
		/// Draws the nav bar.
		/// </summary>
		public virtual void DrawNavBar()
		{		
			// Draw the background
			//GUI.Box(			new Rect(0,0,Screen.width, NAV_BAR_HEIGHT), "" );
			GUI.DrawTexture( 	new Rect(Screen.width-100, 3, NAV_BAR_HEIGHT/2, NAV_BAR_HEIGHT/2), goldcoin );
			GUI.Label(			new Rect(Screen.width-54, 6, NAV_BAR_HEIGHT, NAV_BAR_HEIGHT), ""+GetDisplayGold()+"g", GUI.skin.customStyles[3] );
		}
		
		/// <summary>
		/// Creates the loading modal.
		/// </summary>
		public virtual void LoadLevel(string level)
		{
			Modal = new ModalData("Loading!\n\nPlease wait...", EModalType.Loading);
			LevelToLoad = level;
		}

		/// <summary>
		/// Draws the modal.
		/// </summary>
		/// <param name='rect'>
		/// Rect.
		/// </param>
		/// <param name='text'>
		/// Text.
		/// </param>
		public virtual bool DrawModal()
		{
			if( Modal == null || Modal.text == null || Modal.text.Length == 0 )
				return false;
			
			Rect rect = Modal.rect;	
			if( Modal.type == EModalType.Confirm )
			{				
				// Background modal
				GUI.Box( new Rect(0,0, Screen.width, Screen.height), "", GUI.skin.customStyles[GUISKIN_MODALBACK] );			
				
				// Actual Text
				GUI.Label( new Rect( rect.x, rect.y, rect.width, rect.height), Modal.text, GUI.skin.customStyles[GUISKIN_MODAL] );
				Time.timeScale 	= 0.0f;
				
				float btnWidth 	= rect.width/3;
				float btnHeight = btnWidth/2;
				if( GUI.Button( new Rect( rect.x+rect.width/8, rect.y + (rect.height-btnHeight-5), btnWidth, btnHeight), "Ok" ) )
				{
					Time.timeScale 	= 1.0f;
					Modal = null;
					return true;
				}
				if( GUI.Button( new Rect( rect.x+rect.width - rect.width/8 - btnWidth, rect.y + (rect.height-btnHeight-5), btnWidth, btnHeight), "Cancel" ) )
				{
					Time.timeScale 	= 1.0f;
					Modal = null;
					return false;
				}				
			}
			else if( Modal.type == EModalType.Acknowledge )
			{				
				// Background modal
				GUI.Box( new Rect(0,0, Screen.width, Screen.height), "", GUI.skin.customStyles[GUISKIN_MODALBACK] );			
				
				// Actual Text
				GUI.Label( new Rect( rect.x, rect.y, rect.width, rect.height), Modal.text, GUI.skin.customStyles[GUISKIN_MODAL] );
				Time.timeScale 	= 0.0f;
				
				float btnWidth 	= 100;
				float btnHeight = 50;
				if( GUI.Button( new Rect( rect.x+(rect.width-btnWidth)/2, rect.y + (rect.height-btnHeight-5), btnWidth, btnHeight), "Ok" ) )
				{
					Time.timeScale 	= 1.0f;
					Modal = null;
					return true;
				}
			}
			else if( Modal.type == EModalType.Loading )
			{				
				// Background modal
				GUI.Box( new Rect(0,0, Screen.width, Screen.height), "", GUI.skin.customStyles[GUISKIN_MODALBACK] );			

				// Actual Text
				GUI.Label( new Rect( rect.x, rect.y, rect.width, rect.height), Modal.text, GUI.skin.customStyles[GUISKIN_MODAL] );
				
				if( LevelToLoad != null )
				{
					if( LevelToLoad == "Reload" )
						Application.LoadLevel(Application.loadedLevel);
					else
						Application.LoadLevel(LevelToLoad);
					
					LevelToLoad = null;
				}
			}
			else //if( Modal.type == EModalType.Simple )
			{
				// Actual Text
				GUI.Label( new Rect( rect.x, rect.y, rect.width, rect.height), Modal.text, GUI.skin.customStyles[GUISKIN_MODAL] );

			}	
			
			// Draw the arrow
			if( Modal.target != Vector2.zero )
			{
				int w = arrow.width;
				int h = arrow.height;	

				float offset = Mathf.Sin(Time.realtimeSinceStartup*10)*12;
				GUI.DrawTexture( new Rect( Modal.target.x - w/2, Modal.target.y - h + offset, w,h ), arrow );
			}
			
			return false;
		}
		
		/// <summary>
		/// Gets the display gold.
		/// </summary>
		/// <returns>
		/// The display gold.
		/// </returns>
		protected virtual int GetDisplayGold()
		{
			return Session.Instance.GameData.gold;
		}
	}
}

