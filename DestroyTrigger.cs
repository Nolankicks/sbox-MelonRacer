using System.Threading.Tasks;
using Sandbox;

public sealed class DestroyTrigger : Component, Component.ITriggerListener
{
	[Property] public GameObject melonGibs { get; set; }
	[Property] public Manager manager { get; set; }	
	protected override void OnUpdate()
	{

	}

	public void OnTriggerEnter(Collider other)
	{
			Log.Info("Triggered");
			if (other.GameObject.Tags.Has("player"))
			{
				_ = Respawn(other);
			}
	}

	public async Task Respawn(Collider other)
	{
			var triggerController = other.Components?.Get<Controller>();
			_ = triggerController.Respawn(other.GameObject);
	}
}
