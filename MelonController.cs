using Sandbox;

public sealed class MelonController : Component
{
	public Vector3 WishVelocity;
	[Property] public CharacterController Controller { get; set; }
	public CameraComponent Camera;
	[Property] public GameObject body { get; set; }
	[Sync] public Angles EyeAngles { get; set; }
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
			return 700;
		}

	}
	void Move()
	{
		var cc = Controller;
		var cam = Scene.Camera;
		Vector3 halfGrav = Scene.PhysicsWorld.Gravity * 0.5f;
		WishVelocity = Input.AnalogMove;
		if (Input.Pressed("jump") && cc.IsOnGround)
		{
			cc.Punch(Vector3.Up * 320.0f);
		}
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
		cc.ApplyFriction(6f);

		if (cc.IsOnGround)
		{
			cc.Accelerate(WishVelocity);
			cc.Velocity = Controller.Velocity.WithZ(0);
		}
		else
		{
			cc.Velocity += halfGrav;
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
		var tr = Scene.Trace.Ray(body.Transform.Position, body.Transform.Position - (EyeAngles.Forward * 600)).WithoutTags("player").Run();
		if (tr.Hit)
		{
			Camera.Transform.Position = tr.EndPosition + tr.Normal * 2 + Vector3.Up * 50;
		}
		else
		{
			Camera.Transform.Position = body.Transform.Position - (EyeAngles.Forward * 600) + Vector3.Up * 50;
		}
	}
	protected override void OnUpdate()
	{
		CamRot();
		CamMovement();
		Move();
		GameObject.Transform.Rotation = EyeAngles.ToRotation();
		Camera.Transform.Rotation = EyeAngles.ToRotation();
	}
}
