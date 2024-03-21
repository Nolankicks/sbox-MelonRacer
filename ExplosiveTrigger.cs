using Sandbox;

public sealed class ExplosiveTrigger : Component, Component.ITriggerListener
{
	[Property] public GameObject Explosion { get; set; }
	[Property] public SoundEvent ExplosionSound { get; set; }
	protected override void OnUpdate()
	{

	}

	void ITriggerListener.OnTriggerEnter(Sandbox.Collider other)
	{
		other.Components.TryGet<Controller>(out var controller);
		if (other.Tags.Has("player") && controller is not null)
		{
			controller.ExplosiveKill();
			Explosion.Clone(GameObject.Transform.Position);
			Sound.Play(ExplosionSound, GameObject.Transform.Position);
		}
	}
	void ITriggerListener.OnTriggerExit(Sandbox.Collider other)
	{

	}
}
