using System;
using JsonFx.Json;

namespace MACore
{
	public class MAUtil
	{	
	
		
		
		
		/// <summary>
		/// Jsons the encode.
		/// </summary>
		/// <returns>
		/// The encode.
		/// </returns>
		/// <param name='obj'>
		/// Object.
		/// </param>
		public static string JsonEncode(Object obj)
		{
			System.Text.StringBuilder output 	= new System.Text.StringBuilder();
			JsonWriter writer 					= new JsonWriter (output);
			writer.Write (obj);
			return output.ToString();
		}		
		
		/// <summary>
		/// Jsons the decode.
		/// </summary>
		/// <returns>
		/// The decode.
		/// </returns>
		/// <param name='data'>
		/// Data.
		/// </param>
		/// <typeparam name='T'>
		/// The 1st type parameter.
		/// </typeparam>
		public static T JsonDecode<T> ( String data )
		{
			JsonReader reader = new JsonReader(data);
			
			T deserializedObject = (T)reader.Deserialize(typeof(T));
			
			return deserializedObject;
		}	
		
		/// <summary>
		/// Sets the layer.
		/// </summary>
		/// <param name='obj'>
		/// Object.
		/// </param>
		/// <param name='layer'>
		/// Layer.
		/// </param>
		/*
		private void SetLayer(GameObject obj, int layer)
		{
			obj.layer = layer;
			
			foreach( Transform child in obj.transform )
				SetLayer(child.gameObject, layer);
		}		
		*/
	}
}

