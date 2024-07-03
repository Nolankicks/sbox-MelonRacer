using Sandbox;

public sealed class MelonBreaker : Component
{
	protected override void OnUpdate()
	{
		var tr = Scene.Trace.Ray(Scene.Camera.ScreenPixelToRay(Mouse.Position), 10000f).Run();

		if (Input.Pressed("attack1"))
		{
			Log.Info(tr.GameObject);
		}
	}
	}
