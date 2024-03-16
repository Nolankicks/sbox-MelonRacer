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
				var gibs = melonGibs.Clone(other.GameObject.Transform.Position);
				gibs.Components.TryGet<Prop>(out var prop);
				prop.Enabled = false;
				if (prop is not null)
				{
					prop.CreateGibs();
				}
				triggerController.AbleToMove = false;
				triggerController.Components.TryGet<SkinnedModelRenderer>(out var model);
				model.Enabled = false;
				await Task.DelayRealtimeSeconds(2);
				manager.Respawn(triggerController);
				triggerController.AbleToMove = true;
				model.Enabled = true;
	}
}
