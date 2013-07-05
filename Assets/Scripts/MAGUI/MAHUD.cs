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
		
		
		public Texture goldcoin;
		protected readonly float NAV_BAR_HEIGHT = 35.0f;		
		
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
		/// Draws the modal.
		/// </summary>
		/// <param name='rect'>
		/// Rect.
		/// </param>
		/// <param name='text'>
		/// Text.
		/// </param>
		public virtual void DrawModal(Rect rect, string text)
		{
			if( text == null || text.Length == 0 )
				return;
			
			// Background modal
			GUI.Box( new Rect(0,0, Screen.width, Screen.height), "", GUI.skin.customStyles[GUISKIN_MODALBACK] );
			
			// Actual Text
			//GUI.Box( rect, "", GUI.skin.customStyles[GUISKIN_TOOLTIP_RIGHT] );
			GUI.Label( new Rect( rect.x, rect.y, rect.width, rect.height), text, GUI.skin.customStyles[GUISKIN_TOOLTIP_RIGHT] );
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

