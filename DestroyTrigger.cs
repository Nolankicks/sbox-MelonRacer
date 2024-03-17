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
			var triggerController = other.Components.Get<Controller>();
			if (other.GameObject.Tags.Has("player") && triggerController is not null && triggerController.AbleToMove)
			{
				
				_ = triggerController.Respawn(other.GameObject);
			}
	}
}
