using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using MAPlayer;
using MAUnit;
using MAGUI;

namespace MACore
{	
	public class Game : MonoBehaviour
	{
		// Class Variables
		private static Game instance;
		public static Game Instance { get {return instance;} }		
		
		// Memeber varaibles
		public Level level;
		public Player player;
		private Defender _avatar;
		public Defender Avatar
		{
			get { return _avatar; }
			set
			{	
				if( _avatar != null )
					_avatar.playerControlled = false;
				_avatar = value;		
				_avatar.playerControlled = true;
			}
		}	
		
		public ParticleSystem selectionTemplate;
		private ParticleSystem selection = null;
		private Monster _target;
		public Monster Target
		{
			get { return _target; }
			set
			{	
				if( value != null && value.IsDead() )
					return;
				
				if( _target != null )
					_target.selected = false;
				_target = value;		
				
				if( selection != null )
					Destroy(selection.gameObject);
				
				_avatar.Target 	= _target;				
				if( _target == null )
					return;
				
				selection = GameObject.Instantiate(selectionTemplate) as ParticleSystem;
				
				selection.transform.parent 	= _target.SelectionParent.transform;
				selection.transform.localPosition = Vector3.zero;
				selection.transform.Translate(0, 0.5f, 0, Space.World );
				
				_avatar.Target 	= _target;
				_target.selected = true;
			}
		}			
		
		
		private int goldEarned;
		
		private EGameState _state;
		public EGameState State
		{
			get { return _state; }
			set
			{	
				
				_state = value;		
			}
		}
		
		private List<Defender> defenders;
		private List<Monster> monsters;
		private List<SpawnPoint> spawns;
		private SpawnPoint defenderSpawn;
		private Power active = null;
		
		// Accessors
		public List<Defender> Defenders { get {return defenders;} }
		public List<Monster> Monsters { get {return monsters;} }
		
		// Member variables, unsettable
		private float start;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MACore.Game"/> class.
		/// </summary>
		public Game ()
		{
			defenders = new List<Defender>();
			monsters  = new List<Monster>();
			spawns	  = new List<SpawnPoint>();
			instance  = this;
			State	  = EGameState.Loading;
		}
		
		/// <summary>
		/// Awake this instance.
		/// </summary>
		public void Awake()
		{
			if( Session.Instance != null )
			{
				player = Session.Instance.Player;
				level  = Instantiate(Session.Instance.TargetLevel, new Vector3(0,0,0), Quaternion.identity) as Level;
			}			
			
			Time.timeScale = 1.0f;
			start = Time.fixedTime;
			goldEarned = 0;
			
			
			// Preload all the spawn points
			GameObject [] spawnParents = GameObject.FindGameObjectsWithTag("Spawn");
			foreach( GameObject parent in spawnParents )
			{
				SpawnPoint spawn = parent.GetComponent<SpawnPoint>();
				if( spawn != null )
					spawns.Add(spawn);
			}			
			
			// Get the defender spawn point
			spawnParents = GameObject.FindGameObjectsWithTag("Defender Spawn");
			defenderSpawn = spawnParents[0].GetComponent<SpawnPoint>();
			
			// Spawn the friendlies
			player = new Player(0);
			
			SpawnDefenders();
			
			// Now we are playing!
			State	  = EGameState.Playing;
		}
		
		/// <summary>
		/// Spawns the defenders.
		/// </summary>
		public void SpawnDefenders()
		{	
			GetComponent<GameHUD>().Clear();
			
			// Iterate over the defenders to spawn
			List<DefenderData> list = Session.Instance.GameData.GetDefenderData();
			
			foreach( DefenderData data in list )
				Spawn (data);
		}
		
		/// <summary>
		/// Spawn the specified data.
		/// </summary>
		/// <param name='data'>
		/// Data.
		/// </param>
		public Defender Spawn(DefenderData data)
		{
			// Spawn the game object
			Defender def = Instantiate(data.GetDefender()) as Defender;
			
			if( def == null )
				Debug.LogError("UNABLE TO SPAWN DEFENDER " + data.GetDefenderType() );			

			// Register them with the HUD
			GetComponent<GameHUD>().Add(def);
			
			// Register them here!
			defenders.Add(def);					
			
			// Set their location
			def.Begin( def.type == EDefender.Sniper ? GetDefenderSpawnCenter() : defenderSpawn.GetNextSpawn());

			// Apply all the talents
			Talent [] talents 	= def.GetComponents<Talent>();
			
			foreach( Talent talent in talents )
				for( int c=0;c<talent.GetUnlocked();c++ )
					talent.Apply(def);
			
			return def;
		}
		
		/// <summary>
		/// Gets the defender spawn center.
		/// </summary>
		/// <returns>
		/// The defender spawn center.
		/// </returns>
		public Vector3 GetDefenderSpawnCenter()
		{
			if( defenderSpawn == null )
				return Vector3.zero;
			return defenderSpawn.transform.position;
		}
		
		/// <summary>
		/// Update this instance.
		/// </summary>
		public void Update()
		{
			// Dont pump anymore unless you need to
			if( State != EGameState.Playing )
				return;
			
			// Get the current time
			float now = Time.fixedTime - start;
			
			if( defenders.Count == 0 )
				State = EGameState.Defeat;
			
			if( level.AllWavesSent() && monsters.Count == 0 )
			{
				State = EGameState.Victory;
			}
			
			// Get whatever waves should be spawned now
			List<Wave> waves = level.GetNextWave(now);
			
			// Spawn those waves
			foreach( Wave wave in waves )
				SpawnWave(wave);
			
			
			if( Input.GetMouseButtonDown(0) )
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				
				RaycastHit hit;
				if( Physics.Raycast(ray, out hit, 1000.0f) )
				{
					Defender defender = hit.collider.gameObject.GetComponent<Defender>();
					
					if( defender != null )
					{
						ActivatePower(defender.power);
					}
					else
					{
						Target = hit.collider.gameObject.GetComponent<Monster>();
					}
				}
			}			
			
			if( active != null && active.Activating && Input.GetMouseButtonUp(0))
			{
				active.OnActivateEnd();
				active.Activating = false;
				active = null;
			}
		}
		
		/// <summary>
		/// Spawns the wave.
		/// </summary>
		/// <param name='wave'>
		/// Wave.
		/// </param>
		private void SpawnWave(Wave wave)
		{
			for( int i=0;i<wave.count;i ++ )
			{
				// SPawna  monster
				monsters.Add (Spawn(wave.type));
			}
		}
		
		/// <summary>
		/// Spawn the specified type.
		/// </summary>
		/// <param name='type'>
		/// Type.
		/// </param>
		private Monster Spawn(EMonsterType type)
		{
			Monster monster = null;
			
			GameObject obj = Instantiate(Resources.Load ("Monsters/"+type)) as GameObject;
			
			if( obj == null )
			{
				Debug.LogError("Unable to load monster of type " + type );
				return null;
			}			
			
			monster = obj.GetComponent<Monster>();
			
			// Find a spawn point
			SpawnPoint spawn = GetRandomSpawnPoint();
			
			if( spawn == null )
			{
				Debug.LogError("No spawn point found from spawns: " + spawns );
				return null;
			}
			
			
			if( monster.showStatus )
				GetComponent<GameHUD>().Add(monster);
			
			// Begin the "AI"
			monster.Begin(spawn.GetNextSpawn());
			
			return monster;
		}
		
		private SpawnPoint GetRandomSpawnPoint()
		{
			return spawns[(int)(UnityEngine.Random.value * spawns.Count)];
		}	
		
		
		/// <summary>
		/// Dos the damage.
		/// </summary>
		/// <returns>
		/// The damage.
		/// </returns>
		/// <param name='source'>
		/// If set to <c>true</c> source.
		/// </param>
		/// <param name='weapon'>
		/// If set to <c>true</c> weapon.
		/// </param>
		/// <param name='target'>
		/// If set to <c>true</c> target.
		/// </param>
		public bool DoDamage(Unit source, Attack attack, Unit target)
		{
			// If the attack is not active, do nothing!
			if( !attack.Active )
				return false;
			
			// Get the damage to be done
			float damage = attack.damage - target.armor;
			if( damage < 0 )
				return false;
			
			target.CurrentHealth -= (int)Mathf.Round(damage);
			target.OnHurt(source);
			
			
			// Get the force & direction of the murder!
			Vector3 force = target.DefaultTarget.transform.position - source.transform.position;
			force.Normalize();
			force *= UnityEngine.Random.value * (attack.force/2) + attack.force;
			
			if( target.CurrentHealth <= 0 && !target.IsDead() )
			{
				if( target is Monster )
					Kill((Monster)target, force);
				else
					Kill((Defender)target, force);
			}
			else if( attack.alwaysApplyForce )
			{
				target.ApplyForce(force);	
			}
			return true;	
		}
		
		public void Kill(Monster target, Vector3 force)
		{
			if( target.selected )
				Target = null;
			
			// Remove this from the target list
			monsters.Remove(target);	
			target.Kill();
			target.ApplyForce(force);
			
			GainGold(target.gold);
			
			Destroy(target.gameObject, 10.0f);
		}
		public void Kill(Defender target, Vector3 force)
		{
			// Remove this from the target list
			defenders.Remove(target);	
			target.Kill();
			target.ApplyForce(force);
		}		
		
		
		protected void GainGold(int amount)
		{
			goldEarned += amount;
		}
		public int GetGoldEarned()
		{
			return goldEarned;
		}
		
		/// <summary>
		/// Determines whether this instance is within combat area the specified subject.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance is within combat area the specified subject; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='subject'>
		/// If set to <c>true</c> subject.
		/// </param>
		public bool IsWithinCombatArea(Unit subject)
		{
			return level.area.IsWithin(subject.transform.position);
		}		
		
		
		/// <summary>
		/// Gets the monsters in range.
		/// </summary>
		/// <returns>
		/// The monsters in range.
		/// </returns>
		/// <param name='point'>
		/// Point.
		/// </param>
		/// <param name='radius'>
		/// Radius.
		/// </param>
		public List<Monster> GetMonstersInRange(Vector3 point, float radius)
		{
			List<Monster> result = new List<Monster>();
			
			// Get the distance
			foreach( Monster monster in monsters )
			{
				float dist = Vector3.Distance(monster.transform.position, point);
				if( dist <= radius )
					result.Add(monster);
			}
			return result;
		}
		
		public List<Defender> GetDefendersInRange(Vector3 point, float radius)
		{
			List<Defender> result = new List<Defender>();
			
			// Get the distance
			foreach( Defender defender in defenders )
			{
				float dist = Vector3.Distance(defender.transform.position, point);
				if( dist <= radius )
					result.Add(defender);
			}
			return result;
		}		
		
		/// <summary>
		/// Activates the power.
		/// </summary>
		/// <param name='power'>
		/// Power.
		/// </param>
		public void ActivatePower(Power power)
		{
			// cant begin the activate of two powers at the same time
			if( active != null )
				return;
			
			if( power == null || power.GetCooldown()>0 ) 
				return;
			
			// Get the unit
			Unit unit = power.GetComponent<Unit>();

			// Living ready units can activate a power!
			if( !unit.IsReady() || unit.IsDead() )
				return;
			
			Debug.Log("power is ready?: " + power.Activating );
			
			if( !power.Activating )
			{
				power.Activating = true;
				power.OnActivateBegin();
			}
			active = power;
		}
	}
	
	public enum EGameState
	{
		Loading,
		Playing,
		Victory,
		Defeat
	}	
}

