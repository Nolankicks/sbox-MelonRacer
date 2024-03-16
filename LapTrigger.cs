using Sandbox;

public sealed class LapTrigger : Component, Component.ITriggerListener
{
	[Property] public Manager manager { get; set; }
	protected override void OnUpdate()
	{

	}
	void ITriggerListener.OnTriggerEnter(Sandbox.Collider other)
	{
		other.Components.TryGet<Controller>(out var controller);
		if (other.GameObject.Tags.Has("player") && controller != null)
		{
			Log.Info("Lap Triggered");
			controller.LapTime = 0;
			controller.LapCount++;
			manager.Lap(controller);
		}
	}
	void ITriggerListener.OnTriggerExit(Sandbox.Collider other)
	{

	}
}
