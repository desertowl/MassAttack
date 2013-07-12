using System;
using UnityEngine;
using MAUnit;

namespace MAFX
{
	public class LineRendererFade : FX
	{
		public float duration = 1.0f;
		
		private float startTime;
		private LineRenderer line;
		public Material material;
		public Color BeginSourceColor;
		public Color BeginDestColor;
		
		public Color EndSourceColor;
		public Color EndDestColor;
		
		public float startWidth = 0.1f;
		public float endWidth 	= 0.2f;

		
		public override void SetTarget(Unit target)
		{
			if( target == null ) return;
			
        	line = gameObject.AddComponent<LineRenderer>();
			
        	line.material = new Material(material);
        	line.SetColors(BeginSourceColor, BeginDestColor);
        	line.SetWidth(startWidth, endWidth);
        	line.SetVertexCount(2);
			line.SetPosition(0, transform.position);
			line.SetPosition(1, target.DefaultTarget.transform.position);
			
			// Set when this should be removed
			Destroy(this.gameObject, duration);	
			
			startTime = Time.fixedTime;
		}
		
		public void Update()
		{
			if( line == null )return;
			
			// The percent of completion
			float p = (Time.fixedTime-startTime)/duration;

			//ColorA = Color.Lerp(
			line.SetColors(	Color.Lerp(BeginSourceColor, EndSourceColor, p), 
							Color.Lerp(EndSourceColor, EndDestColor, p));
		}
	}
}


