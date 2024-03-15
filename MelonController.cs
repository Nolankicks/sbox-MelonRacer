using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Sandbox;

public sealed class MelonController : Component, Component.ICollisionListener
{
	public Vector3 WishVelocity;
	[Property] public CharacterController Controller { get; set; }
	public CameraComponent Camera;
	[Property] public GameObject body { get; set; }
	public int Laps = 0;
	[Sync] public Angles EyeAngles { get; set; }
	public Angles RollAngles { get; set; }
	public TimeSince lastJump = 0.3f;
	public TimeSince lastAir = 0;

	protected override void OnEnabled()
	{
		_ = Bounce();
	}
	void CamRot()
	{
		var e = EyeAngles;
		e += Input.AnalogLook * Preferences.Sensitivity;
		e.pitch = e.pitch.Clamp(-89, 89);
		e.roll = 0;
		EyeAngles = e;
	}
	public float MovementSpeed()
	{
		if (Input.Down("run"))
		{
			return 1000;
		}
		else
		{
			return 500;
		}

	}

	public float Friction()
	{
		if (Controller.IsOnGround)
		{
			return 6f;
		}
		else
		{
			return 0.2f;
		}
	}
	void Move()
	{
		var cc = Controller;
		var cam = Scene.Camera;
		Vector3 halfGrav = Scene.PhysicsWorld.Gravity * 0.5f * Time.Delta;
		WishVelocity = Input.AnalogMove;
		

		if (!WishVelocity.IsNearlyZero())
		{
			WishVelocity = new Angles(0, EyeAngles.yaw, 0).ToRotation() * WishVelocity;
			WishVelocity.WithZ(0);
			WishVelocity.ClampLength(1);
			WishVelocity *= MovementSpeed();
			if (!cc.IsOnGround)
			{
				WishVelocity.ClampLength(50);
			}
		}
		cc.ApplyFriction(Friction());

		if (cc.IsOnGround)
		{
			cc.Accelerate(WishVelocity);
			cc.Velocity = cc.Velocity.WithZ(0);
			lastAir = 0;
		}
		else
		{
			cc.Velocity += halfGrav;
			lastJump = 0;
			cc.Accelerate(WishVelocity);

		}
		cc.Move();

		if (!cc.IsOnGround)
		{
			cc.Velocity += halfGrav;
		}
		else
		{
			cc.Velocity = cc.Velocity.WithZ(0);
		}
	}
	public void CamMovement()
	{
		Camera = Scene.GetAllComponents<CameraComponent>().Where( x => x.IsMainCamera).FirstOrDefault();
		var tr = Scene.Trace.Ray(body.Transform.Position, body.Transform.Position - (EyeAngles.Forward * 300)).WithoutTags("player").Run();
		if (tr.Hit)
		{
			Camera.Transform.Position = tr.EndPosition + tr.Normal * 2 + Vector3.Up * 50;
		}
		else
		{
			Camera.Transform.Position = body.Transform.Position - (EyeAngles.Forward * 200) + Vector3.Up * 25;
		}
	}
	protected override void OnUpdate()
	{
		CamRot();
		CamMovement();
		Move();
		RollAngles += new Angles(Input.AnalogMove.x, 0, 0);
		var TargetAngles = new Angles(RollAngles.pitch, EyeAngles.yaw, RollAngles.roll).ToRotation();
		body.Transform.Rotation = Rotation.Slerp(body.Transform.Rotation, TargetAngles, Time.Delta * 5);
		Camera.Transform.Rotation = EyeAngles.ToRotation();
		body.Components.TryGet<Rigidbody>(out var rb);
		if (lastJump > 0.01f && Input.Pressed( "Jump" ) )
		{
			Log.Info("jump");
			Controller.Punch( Vector3.Up * 300 );
		}
	}
	public async Task Bounce()
	{
		while (true)
		{
			if (Controller.Velocity.Length > 200 && Controller.IsOnGround)
			{
				GameObject.Components.TryGet<Rigidbody>(out var rb);
				var tr = Controller.TraceDirection(Vector3.Down * 50);
				if (tr.Hit)
				{
					Controller.Punch(Vector3.Up * 50);
					Controller.Velocity += 300f;
				}
			}
			var randomTime = Random.Shared.Float(0f, 5f);
			Log.Info("bounce");
			await Task.DelayRealtimeSeconds(randomTime);
		}
	}
}

