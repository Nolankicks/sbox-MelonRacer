using System.Threading.Tasks;
using Sandbox;

public sealed class MelonController : Component
{
	public Vector3 WishVelocity;
	[Property] public CharacterController Controller { get; set; }
	public CameraComponent Camera;
	[Property] public GameObject body { get; set; }
	public int Laps = 0;
	[Sync] public Angles EyeAngles { get; set; }
	public TimeSince lastJump = 0.3f;
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
			return 900;
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
		int RotationSpeed = 5;
		body.Transform.Rotation = new Angles(RotationSpeed++, EyeAngles.yaw, 90).ToRotation();
		Camera.Transform.Rotation = EyeAngles.ToRotation();
		if (lastJump > 0.01f && Input.Pressed( "Jump" ) )
		{
			Log.Info("jump");
			Controller.Punch( Vector3.Up * 300 );
		}
	}
}

