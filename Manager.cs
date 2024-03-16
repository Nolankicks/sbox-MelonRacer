using System.Threading.Tasks;
using Sandbox;
using Sandbox.Utility;

public sealed class Manager : Component
{
	[Property] public GameObject PlayerPrefab { get; set; }
	[Property] public List<SpawnPoint> spawnPoints { get; set;} = new List<SpawnPoint>();
	protected override void OnStart()
	{

	}
	protected override void OnUpdate()
	{

	}
	public void Respawn( Controller controller )
	{
		controller.EyeAngles = new Angles(0, 180, 0);
		controller.Transform.Position = Game.Random.FromList(spawnPoints).GameObject.Transform.Position;
		controller.LapTime = 0;
		controller.WishVelocity = Vector3.Zero;
	}
	public void Lap(Controller controller)
	{
		controller.EyeAngles = new Angles(0, 180, 0);
		controller.Transform.Position = Game.Random.FromList(spawnPoints).GameObject.Transform.Position;
		controller.LapCount++;
		Log.Info($"Laps:{controller.LapCount} Laptime:{controller.LapTime}");
		Sandbox.Services.Stats.SetValue("laptime", controller.LapTime);
		Sandbox.Services.Stats.SetValue("laps", controller.LapCount);
		controller.WishVelocity = Vector3.Zero;
	}


}
