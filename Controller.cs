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
	protected override void OnFixedUpdate()
	{
		BuildMoveAngles();
		CamRot();
		UpdateCamPos();
		Log.Info(WishVelocity);
		Rigidbody.ApplyForce(WishVelocity * 500);

		//Camera.Transform.Rotation = EyeAngles.ToRotation();
	}
	protected override void OnUpdate()
	{
		
	}
	void BuildMoveAngles()
	{
		WishVelocity = Input.AnalogMove;
		if (!WishVelocity.IsNearlyZero())
		{
			WishVelocity = new Angles(0, EyeAngles.yaw, 0).ToRotation() * WishVelocity;
			WishVelocity.WithZ(0);
			WishVelocity.ClampLength(1);
			WishVelocity *= 700;
		}
	}
	void CamRot()
	{
		var e = EyeAngles;
		e += Input.AnalogLook * 5;
		e.pitch = e.pitch.Clamp(-89, 89);
		EyeAngles = e;
	}

void UpdateCamPos()
    {	var center = Rigidbody.PhysicsBody.GetBounds().Center;
        var lookDir = EyeAngles.ToRotation();
        var targetpos = Transform.Position.LerpTo(center + lookDir.Backward * 300 + Vector3.Up * 75.0f, 1f);
        Camera.Transform.Position = targetpos;
        Camera.Transform.Rotation = lookDir;
    }
}
