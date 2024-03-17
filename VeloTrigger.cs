using Sandbox;

public sealed class VeloTrigger : Component, Component.ITriggerListener
{
	[Property] public int Velocity { get; set; } = 350;
	protected override void OnUpdate()
	{

	}

	void ITriggerListener.OnTriggerEnter(Sandbox.Collider other)
	{
		other.Components.TryGet<Controller>(out var controller);

		if (controller is not null)
		{
			if (controller.Rigidbody.Velocity.Length >= Velocity)
			{
				_ = controller.Respawn(other.GameObject);
				Log.Info("You are going too fast!" + controller.Rigidbody.Velocity.Length);
			}
			else
			{
				Log.Info(controller.Rigidbody.Velocity.Length);
			}
		}
	}

	void ITriggerListener.OnTriggerExit(Sandbox.Collider other)
	{

	}
}
