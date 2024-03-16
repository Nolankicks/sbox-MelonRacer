using Sandbox;

public sealed class StartTrigger : Component, Component.ITriggerListener
{
	protected override void OnUpdate()
	{

	}
	void ITriggerListener.OnTriggerEnter(Sandbox.Collider other)
	{
		other.Components.TryGet<Controller>(out var player);
		if (player != null)
		{
			player.LapTime = 0;
		}
	}
	void ITriggerListener.OnTriggerExit(Sandbox.Collider other)
	{

	}
}
