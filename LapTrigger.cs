using Sandbox;

public sealed class LapTrigger : Component, Component.ITriggerListener
{
	public void OnTriggerEnter(Collider other)
	{
		if (other.GameObject.Tags.Has("player"))
		{
			var player = other.GameObject.Components.Get<MelonController>();
			player.Laps++;
			Log.Info(player.Laps);
		}
	}
	protected override void OnUpdate()
	{

	}
}
