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
				var gibs = melonGibs.Clone(other.GameObject.Transform.Position);
				gibs.Components.TryGet<Prop>(out var prop);
				prop.CreateGibs();
				manager.Respawn();
			}
	}
}
