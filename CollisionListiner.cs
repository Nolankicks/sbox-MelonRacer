using Sandbox;

public sealed class CollisionListiner : Component, Component.ICollisionListener
{
	protected override void OnUpdate()
	{

	}

	void ICollisionListener.OnCollisionStart(Sandbox.Collision other)
	{
		Log.Info("Collision Start");
	}

	void ICollisionListener.OnCollisionStop(Sandbox.CollisionStop other)
	{
		Log.Info("Collision Stop");
	}

	void ICollisionListener.OnCollisionUpdate(Sandbox.Collision other)
	{
		Log.Info("Collision Update");
	}
}
