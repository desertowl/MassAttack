using System;
using UnityEngine;
using MACore;

namespace MAGUI
{
	public class MAHUD : MonoBehaviour
	{
		public Texture goldcoin;
		protected readonly float NAV_BAR_HEIGHT = 35.0f;		
		
		/// <summary>
		/// Draws the nav bar.
		/// </summary>
		public virtual void DrawNavBar()
		{		
			// Draw the background
			GUI.Box(			new Rect(0,0,Screen.width, NAV_BAR_HEIGHT), "" );
			GUI.DrawTexture( 	new Rect(Screen.width-100, 3, NAV_BAR_HEIGHT-6, NAV_BAR_HEIGHT-6), goldcoin );
			GUI.Label(			new Rect(Screen.width-54, 6, NAV_BAR_HEIGHT, NAV_BAR_HEIGHT), ""+GetDisplayGold()+"g", GUI.skin.customStyles[3] );
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

