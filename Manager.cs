using System.Threading.Tasks;
using Sandbox;
using Sandbox.Utility;

public sealed class Manager : Component
{
	[Property] public GameObject PlayerPrefab { get; set; }
	protected override void OnStart()
	{

	}
	protected override void OnUpdate()
	{

	}
	public void Respawn( Controller controller )
	{
		var spawnPoints = Scene.GetAllComponents<SpawnPoint>().ToArray();
		controller.Transform.Position = Game.Random.FromArray(spawnPoints).GameObject.Transform.Position;
	}
}
