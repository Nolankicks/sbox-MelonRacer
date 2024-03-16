using System.Runtime.CompilerServices;
using Sandbox;
using Sandbox.ModelEditor.Nodes;

public sealed class Controller : Component
{
	[Property] public Rigidbody Rigidbody { get; set; }
	[Property] public Vector3 WishVelocity { get; set; }
	[Property] public GameObject Body { get; set; }
	[Property] public CameraComponent Camera;
	public Angles EyeAngles { get; set; }
	protected override void OnUpdate()
	{
		BuildMoveAngles();
		CamRot();
		UpdateCamPos();
		Log.Info(WishVelocity);
		Rigidbody.Velocity += Vector3.Zero.WithAcceleration(WishVelocity, 10f);

		//Camera.Transform.Rotation = EyeAngles.ToRotation();
	}
	void BuildMoveAngles()
	{
		WishVelocity = Input.AnalogMove;
		if (!WishVelocity.IsNearlyZero())
		{
			WishVelocity = new Angles(0, EyeAngles.yaw, 0).ToRotation() * WishVelocity;
			WishVelocity.WithZ(0);
			WishVelocity.ClampLength(1);
			WishVelocity *= 5;
		}
	}
	void CamRot()
	{
		var e = EyeAngles;
		e += Input.AnalogLook * Preferences.Sensitivity;
		e.pitch = e.pitch.Clamp(-89, 89);
		EyeAngles = e;
	}

	void UpdateCamPos()
	{
		var lookDir = EyeAngles.ToRotation();
		var targetpos = Transform.Position.LerpTo(Body.Transform.Position + lookDir.Backward * 300 + Vector3.Up * 75.0f, 25f);
		Camera.Transform.Position = targetpos;
		targetpos.z = Camera.Transform.Position.z.LerpTo(targetpos.z, RealTime.Delta * 25.0f);
		Camera.Transform.Rotation = lookDir;
	}
}
