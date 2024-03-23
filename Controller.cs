using System;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using Sandbox;
using Sandbox.ModelEditor.Nodes;
using Sandbox.Utility;

public sealed class Controller : Component
{
	[Property] public Rigidbody Rigidbody { get; set; }
	[Property] public Vector3 WishVelocity { get; set; }
	[Property] public SkinnedModelRenderer body { get; set; }
	[Property] public ModelCollider bodyCollider { get; set; }
	[Property] public Manager Manager { get; set; }
	[Property] public List<SpawnPoint> spawnPoints { get; set;} = new List<SpawnPoint>();
	public CameraComponent Camera;
	public Angles EyeAngles { get; set; }
	public TimeSince LapTime;
	public int LapCount { get; set; }
	public bool AbleToMove { get; set; } = true;
	[Property] public GameObject gibs { get; set; }
	public Model BodyModel;
	public string SteamId { get; set; }
	[Property] public Angles StartOffSetAngles { get; set; } = new Angles(0, 180, 0);
	[Property] public string MapName { get; set; }
	
	protected override void OnFixedUpdate()
	{
		if (AbleToMove)
		{
		BuildMoveAngles();
		CamRot();
		UpdateCamPos();
		}
		Rigidbody.PhysicsBody.LinearDrag = 0.5f;
		if (AbleToMove)
		{
		Rigidbody.ApplyForce(WishVelocity * 500);
		}
	}
	protected override void OnStart()
	{
		if (IsProxy) return;
		SteamId = Steam.SteamId.ToString();
		EyeAngles = StartOffSetAngles;
		spawnPoints = Scene.GetAllComponents<SpawnPoint>().ToList();
		var selectedmodel = FileSystem.Data.ReadAllText("activeModel.txt");
		if (selectedmodel is not null)
		{
			BodyModel = Model.Load(selectedmodel);
			body.Model = BodyModel;
			bodyCollider.Model = BodyModel;
		}
		if (MapName == "gm_melonrace")
		{
			Log.Info("Playing on gm_melonrace");
		}
		else if (MapName == "gm_melonracelockdown")
		{
			Log.Info("Playing on gm_melonracelockdown");
		}
	}
	void BuildMoveAngles()
	{
		//Proxy Check
		if (IsProxy) return;
		//Get the input
		WishVelocity = Input.AnalogMove;
		//Check if its not zero
		if (!WishVelocity.IsNearlyZero())
		{
			//Get the forward and right vectors and times it by the wishvelo
			WishVelocity = new Angles(0, EyeAngles.yaw, 0).ToRotation() * WishVelocity;
			//Set the z to zero
			WishVelocity.WithZ(0);
			//Clamp the length to 1
			WishVelocity.ClampLength(1);
			//Multiply the wishvelo
			WishVelocity *= 175;
		}
	}
	void CamRot()
	{
		//Proxy Check
		if (IsProxy) return;
		//Get the eyeangles
		var e = EyeAngles;
		//Add the input to the eyeangles
		e += Input.AnalogLook;
		//Clamp the eyeangles
		e.pitch = e.pitch.Clamp(-89, 89);
		//Set the eyeangles
		EyeAngles = e;
	}

void UpdateCamPos()
    {	
		//Proxy Check
		if (IsProxy) return;
		//Get the camera
		Camera = Scene.GetAllComponents<CameraComponent>().Where(x => x.IsMainCamera).FirstOrDefault();
		//Get the center of the phsyics body
		var center = Rigidbody.PhysicsBody.GetBounds().Center;
		//Get the look direction
        var lookDir = EyeAngles.ToRotation();
		//Set the camera rotation
        Camera.Transform.Rotation = lookDir;
		//Trace to see if the camera is inside a wall
		var tr = Scene.Trace.Ray(center, center - (EyeAngles.Forward * 300)).WithoutTags("player", "barrier").Run();
		if (tr.Hit)
		{
			Camera.Transform.Position = tr.EndPosition + tr.Normal * 2 + Vector3.Up * 50;
		}
		else
		{
			Camera.Transform.Position = center - (EyeAngles.Forward * 300) + Vector3.Up * 50;
		}
    }
	public void ExplosiveKill()
	{
		//Proxy Check
		if (IsProxy) return;
		//Impulse rigidbody so it looks like an explosion
		Rigidbody.ApplyImpulseAt(Rigidbody.Transform.Position * WishVelocity, Vector3.Up * 5000);
		//Respawn the player
		_ = Respawn(GameObject);
	}
	public async Task Respawn(GameObject other)
	{
				//Proxy Check
				if (IsProxy) return;
				//Get the controller from the trigger
				var triggerController = other.Components?.Get<Controller>();
				//Clones gibs
				var gibsref = gibs.Clone(other.Transform.Position);
				//Gets prop component
				gibsref.Components.TryGet<Prop>(out var prop);
				prop.Model = BodyModel;
				//Disables prop and sets position
				prop.Enabled = false;
				prop.Transform.Position = other.Transform.Position;
				//Gets model component
				other.Components.TryGet<SkinnedModelRenderer>(out var model);
				//Null checks
				if (model is not null)
				{
					model.Enabled = false;
				}
				if (prop is not null)
				{
					//Creates gibs
					prop.CreateGibs();
				}
				//Destroys prop gameobejct
				prop.GameObject.Destroy();
				//Tells the player it is not able to move
				triggerController.AbleToMove = false;
				//Waits 2 seconds
				await Task.DelayRealtimeSeconds(2);
				//If the model is not null, enable it
				if (model is not null)
				{
					model.Enabled = true;
				}
				//Reset eyeangles
				var spawnpoint = Game.Random.FromList(spawnPoints);
				Transform.Position = spawnpoint.Transform.Position + Vector3.Up * 50.0f;
				EyeAngles = spawnpoint.Transform.Rotation;
				//Log it
				Log.Info("respawn");
				//Reset laptime
				LapTime = 0;
				//Reset wishvelo
				WishVelocity = Vector3.Zero;
				//Reset velocity
				Rigidbody.Velocity = Vector3.Zero;
				//Allow the player to move
				triggerController.AbleToMove = true;
				//Log the laptime
				Log.Info(LapTime);
				//Add to the total deaths stat
				Sandbox.Services.Stats.Increment("deaths", 1);
	}
	public async Task Lap()
	{
		//Proxy Check
		if (IsProxy) return;
		//Reset eyeangles
		
		//Get all spawnpoints
		var spawnpoint = Game.Random.FromList(spawnPoints);
		Transform.Position = spawnpoint.Transform.Position + Vector3.Up * 50.0f;
		EyeAngles = spawnpoint.Transform.Rotation;
		//Increment lapcount
		LapCount++;
		//Log the laptime and lapcount
		Log.Info($"Laps:{LapCount} Laptime:{LapTime}");
		//Set stats
		if (MapName == "gm_melonrace")
		{
			Sandbox.Services.Stats.SetValue("laptime", LapTime);
		}
		else if (MapName == "gm_melonracelockdown")
		{
			Sandbox.Services.Stats.SetValue("lockdownlaps", LapTime);
		}
		
		Sandbox.Services.Stats.Increment("laps", 1);
		await Task.DelayRealtimeSeconds(0.5f);
		//Reset laptime
		LapTime = 0;
		//Reset wishvelo and velo
		WishVelocity = Vector3.Zero;
		Rigidbody.Velocity = Vector3.Zero;
		Log.Info(LapCount);
	}
}
