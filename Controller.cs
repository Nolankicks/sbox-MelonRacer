using System;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Sandbox;
using Sandbox.ModelEditor.Nodes;

public sealed class Controller : Component
{
	[Property] public Rigidbody Rigidbody { get; set; }
	[Property] public Vector3 WishVelocity { get; set; }
	[Property] public GameObject Body { get; set; }
	public CameraComponent Camera;
	public Angles EyeAngles { get; set; }
	public TimeSince LapTime;
	public int LapCount { get; set; }
	public bool AbleToMove { get; set; } = true;
	protected override void OnFixedUpdate()
	{
		if (AbleToMove)
		{
		BuildMoveAngles();
		CamRot();
		UpdateCamPos();
		}
		Log.Info(WishVelocity);
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
			WishVelocity *= 150;
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
		Camera = Scene.GetAllComponents<CameraComponent>().Where(x => x.IsMainCamera).FirstOrDefault();
		var center = Rigidbody.PhysicsBody.GetBounds().Center;
        var lookDir = EyeAngles.ToRotation();
        var targetpos = Transform.Position.LerpTo(center + lookDir.Backward * 300 + Vector3.Up * 50.0f, 1f);
        Camera.Transform.Position = targetpos;
        Camera.Transform.Rotation = lookDir;
    }
}
