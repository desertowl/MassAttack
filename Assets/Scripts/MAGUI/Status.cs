using System;
using MAUnit;
using UnityEngine;

namespace MAGUI
{
	public class Status
	{		
		private Vector2 offset;
		private Unit unit = null;
		private readonly float HP_BAR_WIDTH = 128;
		private bool init = false;
		
		/// <summary>
		/// Draw this instance.
		/// </summary>
		public void Draw ()
		{
			if( unit == null ) return;
			
			// THe icon
			if( GUI.RepeatButton(    	new Rect(offset.x,offset.y,64,64), unit.icon ) )
			{
				ActivatePower();
			}
			GUI.Label( 		new Rect( offset.x + 70, offset.y + 0 , 200, 30), unit.name );
			GUI.TextArea(	new Rect( offset.x + 70, offset.y + 30, 100, 20), unit.desc, GUI.skin.customStyles[0] );
			
			if( unit.IsReady() )
			{
				float hp_pos = Mathf.Max(0, unit.CurrentHealth/unit.health * HP_BAR_WIDTH);
				GUI.Box( 		new Rect( offset.x + 70, offset.y + 50, HP_BAR_WIDTH, 12), "", GUI.skin.customStyles[1] );
				GUI.Box( 		new Rect( offset.x + 70, offset.y + 50, hp_pos, 12), "", GUI.skin.customStyles[2] );
				GUI.Label( 		new Rect( offset.x + 70 + HP_BAR_WIDTH/2, offset.y + 40 , 100, 30), unit.CurrentHealth + "/"+unit.health );
			}
			
			if( unit.IsDead() )
			{
				GUI.Box( 		new Rect(offset.x,offset.y,64,64), GameHUD.Instance.dead );
			}
		}
		
		public void SetUnit(Vector2 pos, ref Unit unit)
		{
			offset = pos;
			this.unit = unit;
		}
		
		/// <summary>
		/// Activates the power.
		/// </summary>
		private void ActivatePower()
		{
			Power power = null;

			if( unit is Defender )
				power = ((Defender)unit).power;
			
			if( power == null )
				return;
				
			// Living ready units can activate a power!
			if( Input.GetMouseButton(0) && unit.IsReady() && !unit.IsDead() )
			{
				if( !init )
				{
					power.OnActivateBegin();
					init = true;
				}
				power.OnActivateUpdate();
			}			
			
			if( Input.GetMouseButtonUp(0) )
			{
				init = false;
				power.OnActivateEnd();
			}			
		}
	}
}

