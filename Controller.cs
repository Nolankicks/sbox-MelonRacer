using System;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using Sandbox;
using Sandbox.ModelEditor.Nodes;

public sealed class Controller : Component
{
	[Property] public Rigidbody Rigidbody { get; set; }
	[Property] public Vector3 WishVelocity { get; set; }
	[Property] public Manager Manager { get; set; }
		[Property] public List<SpawnPoint> spawnPoints { get; set;} = new List<SpawnPoint>();
	public CameraComponent Camera;
	public Angles EyeAngles { get; set; }
	public TimeSince LapTime;
	public int LapCount { get; set; }
	public bool AbleToMove { get; set; } = true;
	[Property] public GameObject gibs { get; set; }
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
		//Camera.Transform.Rotation = EyeAngles.ToRotation();
	}
	protected override void OnStart()
	{
		EyeAngles = new Angles(0, 180, 0);
		spawnPoints = Scene.GetAllComponents<SpawnPoint>().ToList();
	}
	protected override void OnUpdate()
	{
		
	}
	void BuildMoveAngles()
	{
		if (IsProxy) return;
		WishVelocity = Input.AnalogMove;
		if (!WishVelocity.IsNearlyZero())
		{
			WishVelocity = new Angles(0, EyeAngles.yaw, 0).ToRotation() * WishVelocity;
			WishVelocity.WithZ(0);
			WishVelocity.ClampLength(1);
			WishVelocity *= 175;
		}
	}
	void CamRot()
	{
		if (IsProxy) return;
		var e = EyeAngles;
		e += Input.AnalogLook;
		e.pitch = e.pitch.Clamp(-89, 89);
		EyeAngles = e;
	}

void UpdateCamPos()
    {	
		if (IsProxy) return;
		Camera = Scene.GetAllComponents<CameraComponent>().Where(x => x.IsMainCamera).FirstOrDefault();
		var center = Rigidbody.PhysicsBody.GetBounds().Center;
        var lookDir = EyeAngles.ToRotation();
        //var targetpos = Transform.Position.LerpTo(center + lookDir.Backward * 300 + Vector3.Up * 50.0f, 1f);
        //Camera.Transform.Position = targetpos;
        Camera.Transform.Rotation = lookDir;
		var tr = Scene.Trace.Ray(center, center - (EyeAngles.Forward * 300)).WithoutTags("player").Run();
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
		if (IsProxy) return;
		Rigidbody.ApplyImpulseAt(Rigidbody.Transform.Position * WishVelocity, Vector3.Up * 5000);
		_ = Respawn(GameObject);
	}
	public async Task Respawn(GameObject other)
	{
				if (IsProxy) return;
				var triggerController = other.Components?.Get<Controller>();
				var gibsref = gibs.Clone(other.Transform.Position);
				gibsref.Components.TryGet<Prop>(out var prop);
				prop.Enabled = false;
				prop.Transform.Position = other.Transform.Position;
				other.Components.TryGet<SkinnedModelRenderer>(out var model);
				if (model is not null)
				{
					model.Enabled = false;
				}
				if (prop is not null)
				{
					prop.CreateGibs();
				}
				triggerController.AbleToMove = false;
				await Task.DelayRealtimeSeconds(2);
				if (model is not null)
				{
					model.Enabled = true;
				}
				EyeAngles = new Angles(0, 180, 0);
				Transform.Position = Game.Random.FromList(spawnPoints).GameObject.Transform.Position + Vector3.Up * 50.0f;
				Log.Info("respawn");
				LapTime = 0;
				WishVelocity = Vector3.Zero;
				Rigidbody.Velocity = Vector3.Zero;
				triggerController.AbleToMove = true;
				Log.Info(LapTime);
				Sandbox.Services.Stats.Increment("deaths", 1);
	}
	public void Lap()
	{
		if (IsProxy) return;
		EyeAngles = new Angles(0, 180, 0);
		Transform.Position = Game.Random.FromList(spawnPoints).GameObject.Transform.Position + Vector3.Up * 50.0f;
		LapCount++;
		Log.Info($"Laps:{LapCount} Laptime:{LapTime}");
		Sandbox.Services.Stats.SetValue("laptime", LapTime);
		Sandbox.Services.Stats.Increment("laps", 1);
		WishVelocity = Vector3.Zero;
		Rigidbody.Velocity = Vector3.Zero;
	}
}
