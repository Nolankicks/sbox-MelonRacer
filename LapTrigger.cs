using Sandbox;

public sealed class LapTrigger : Component, Component.ITriggerListener
{
	public Manager manager { get; set; }
	protected override void OnUpdate()
	{
		manager = Scene.GetAllComponents<Manager>().FirstOrDefault( x => !x.IsProxy);
	}
	void ITriggerListener.OnTriggerEnter(Sandbox.Collider other)
	{
		other.Components.TryGet<Controller>(out var controller);
		if (other.GameObject.Tags.Has("player") && controller != null)
		{
			Log.Info("Lap Triggered");
			controller.LapCount++;
			manager.Lap(controller);
			Sandbox.Services.Stats.SetValue("laps", controller.LapCount);
			Sandbox.Services.Stats.SetValue("laptime", controller.LapTime);
		}
	}
	void ITriggerListener.OnTriggerExit(Sandbox.Collider other)
	{

	}
}
