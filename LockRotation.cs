using Sandbox;

public sealed class LockRotation : Component
{
	protected override void OnStart()
	{
		Transform.OnTransformChanged += RotationLock;

	}
	void RotationLock()
	{
		GameObject.Transform.World = GameObject.Parent.Transform.World;
	}
}
