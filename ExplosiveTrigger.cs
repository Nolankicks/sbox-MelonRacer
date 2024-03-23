using Sandbox;

public sealed class ExplosiveTrigger : Component, Component.ITriggerListener
{
	[Property] public GameObject Explosion { get; set; }
	[Property] public SoundEvent ExplosionSound { get; set; }
	[Property] public ModelRenderer ModelRender { get; set; }
	[Property] public List<Model> models { get; set; } = new();
	[Property] public ModelCollider modelCollider { get; set; }
	[Property] public ModelCollider triggerCollider { get; set; }
	protected override void OnStart()
	{
		if (ModelRender is not null && models is not null && modelCollider is not null)
		{
			var selectedModel = Game.Random.FromList(models);
			ModelRender.Model = selectedModel;
			modelCollider.Model = selectedModel;
			triggerCollider.Model = selectedModel;
		}
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
