using System;
using MAUnit;
using MACore;
using UnityEngine;

namespace MAGUI
{
	public class Status
	{		
		public Vector2 offset;
		public Unit unit = null;
		
		public static readonly float BUTTON_SIZE = 128;
		public static readonly float HP_BAR_WIDTH = 120;
		
		private bool init = false;
		private bool wasOnCD = false;
		
		
		/// <summary>
		/// Sets the unit.
		/// </summary>
		/// <param name='pos'>
		/// Position.
		/// </param>
		/// <param name='unit'>
		/// Unit.
		/// </param>
		public void SetUnit(Vector2 pos, ref Unit unit)
		{
			offset = pos;
			this.unit = unit;
		}		
		
		/// <summary>
		/// Draw this instance.
		/// </summary>
		public void Draw ()
		{
			if( unit == null ) return;
			
			
			// THe icon
			Power power 	= GetPower();
			bool onCooldown = IsOnCooldown();
			
			// Clear the availibty
			if( wasOnCD && !onCooldown && power != null )
				power.OnAvailable();
			
			
			wasOnCD = onCooldown;			
			GUI.enabled = !onCooldown;
			
			
			
			GUI.DrawTexture( new Rect(offset.x,offset.y,BUTTON_SIZE,BUTTON_SIZE/2), unit.icon );

			if( unit.IsDead() )
			{
				GUI.Box( 		new Rect(offset.x,offset.y,BUTTON_SIZE,BUTTON_SIZE/2), GameHUD.Instance.dead );
				return;
			}			
			
			if( GUI.RepeatButton(    	new Rect(offset.x,offset.y,BUTTON_SIZE,BUTTON_SIZE/2), "" ) )
			{
				ActivatePower();
			}
			
			
			if( IsOnCooldown() && power!=null )
			{
				GUI.Box( 	new Rect(offset.x,offset.y,BUTTON_SIZE,BUTTON_SIZE/2), "", GUI.skin.customStyles[2] );
				//GUI.Label( 		new Rect( offset.x + 3,hpOffset.y-6, 100, 30), unit.CurrentHealth + "/"+unit.health, GUI.skin.customStyles[MAHUD.GUISKIN_LARGE_SUBTITLE] );
				//GUI.Label( 		new Rect( offset.x + BUTTON_SIZE/2, offset.y + BUTTON_SIZE/2 , BUTTON_SIZE, BUTTON_SIZE/2), cd, GUI.skin.customStyles[MAHUD.GUISKIN_LARGE_SUBTITLE] );
				DrawBar(new Vector2(offset.x+3, offset.y+52), power.GetCooldown(), power.cooldown, HP_BAR_WIDTH, power.GetCooldown().ToString("n2"));
			}
			GUI.enabled = true;

			if( unit.IsReady() )
			{
				//float hp_pos = Mathf.Max(0, unit.CurrentHealth/unit.health * HP_BAR_WIDTH);
				//GUI.Box( 		new Rect( hpOffset.x, hpOffset.y, hp_pos, 9), "", GUI.skin.customStyles[2] );
				//GUI.Label( 		new Rect( hpOffset.x + HP_BAR_WIDTH/3,hpOffset.y-6, 100, 30), unit.CurrentHealth + "/"+unit.health, GUI.skin.customStyles[MAHUD.GUISKIN_LARGE_SUBTITLE] );
				DrawBar(new Vector2(offset.x+3, offset.y+40), unit.CurrentHealth, unit.health, HP_BAR_WIDTH, unit.CurrentHealth+"/"+unit.health);
			}
			

		}
		
		/// <summary>
		/// Draws the bar.
		/// </summary>
		/// <param name='pos'>
		/// Position.
		/// </param>
		/// <param name='currentValue'>
		/// Current value.
		/// </param>
		/// <param name='maxValue'>
		/// Max value.
		/// </param>
		/// <param name='maxWidth'>
		/// Max width.
		/// </param>
		/// <param name='label'>
		/// Label.
		/// </param>
		private void DrawBar(Vector2 pos, float currentValue, float maxValue, float maxWidth, string label )
		{
			float barPos = Mathf.Max(0, currentValue/maxValue * maxWidth);
			GUI.Box( 	new Rect( pos.x, pos.y, barPos, 9), "", GUI.skin.customStyles[2] );
			GUI.Label( 	new Rect( pos.x + maxWidth/3,pos.y-6, maxWidth, 30), label, GUI.skin.customStyles[MAHUD.GUISKIN_LARGE_SUBTITLE] );			
		}
		
		/// <summary>
		/// Gets the power.
		/// </summary>
		/// <returns>
		/// The power.
		/// </returns>
		private Power GetPower()
		{
			if( unit is Defender )
				return ((Defender)unit).power;
			
			return null;
		}
		
		/// <summary>
		/// Determines whether this instance is on cooldown.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance is on cooldown; otherwise, <c>false</c>.
		/// </returns>
		private bool IsOnCooldown()
		{
			Power power = GetPower();
			
			if( power == null )
				return true;
			return power.GetCooldown()>0;
		}
		
		/// <summary>
		/// Activates the power.
		/// </summary>
		private void ActivatePower()
		{
			Power power = GetPower();
			
			Game.Instance.ActivatePower(power);
			/*
			
			if( power == null || power.GetCooldown()>0 ) 
				return;
				
			// Living ready units can activate a power!
			if( Input.GetMouseButton(0) && unit.IsReady() && !unit.IsDead() )
			{
				if( !init )
				{
					power.OnActivateBegin();
					init = true;
				}
			}			
			
			if( Input.GetMouseButtonUp(0) )
			{
				init = false;
				power.OnActivateEnd();
			}	
			*/		
		}
	}
}

