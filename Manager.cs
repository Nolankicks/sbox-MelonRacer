using System.Threading.Tasks;
using Sandbox;

public sealed class Manager : Component
{
	[Property] public GameObject PlayerPrefab { get; set; }
	protected override void OnStart()
	{
		var spawnPoints = Scene.GetAllComponents<SpawnPoint>().ToArray();
		PlayerPrefab.Clone(Game.Random.FromArray(spawnPoints).GameObject.Transform.Position);
	}
	protected override void OnUpdate()
	{

	}
	public void Respawn()
	{
		var spawnPoints = Scene.GetAllComponents<SpawnPoint>().ToArray();
		var player = Scene.GetAllComponents<MelonController>().FirstOrDefault();
		player.Transform.Position = Game.Random.FromArray(spawnPoints).GameObject.Transform.Position;
	}
}
