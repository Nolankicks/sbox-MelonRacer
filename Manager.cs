using System.Threading.Tasks;
using Sandbox;
using Sandbox.Utility;

public sealed class Manager : Component
{
	[Property] public GameObject PlayerPrefab { get; set; }
	[Property] public List<SpawnPoint> spawnPoints { get; set;} = new List<SpawnPoint>();
	public Sandbox.Services.Leaderboards.Board FastestLap { get; set; }
	public Sandbox.Services.Leaderboards.Board MostLaps { get; set; }
	public Sandbox.Services.Leaderboards.Board Deaths { get; set; }
	protected override void OnStart()
	{
		_ = FetchFastestLap();
		_ = RefreshMostLaps();
		_ = FetchDeathsLeaderboard();
	}
	protected override void OnUpdate()
	{
		
	}
	public void Respawn( Controller controller )
	{
		if (IsProxy) return;
		controller.EyeAngles = new Angles(0, 180, 0);
		controller.Transform.Position = Game.Random.FromList(spawnPoints).GameObject.Transform.Position;
		Log.Info("respawn");
		controller.GameObject.Components.TryGet<SkinnedModelRenderer>(out var model);
	}
	public void Lap(Controller controller)
	{
		if (IsProxy) return;
		controller.EyeAngles = new Angles(0, 180, 0);
		controller.Transform.Position = Game.Random.FromList(spawnPoints).GameObject.Transform.Position;
		controller.LapCount++;
		Log.Info($"Laps:{controller.LapCount} Laptime:{controller.LapTime}");
		Sandbox.Services.Stats.SetValue("laptime", controller.LapTime);
		Sandbox.Services.Stats.SetValue("laps", controller.LapCount);
		controller.WishVelocity = Vector3.Zero;
		controller.Rigidbody.Velocity = Vector3.Zero;
	}

	public async Task FetchFastestLap()
	{
		if (IsProxy) return;
		FastestLap = Sandbox.Services.Leaderboards.Get("fastestlap");
		FastestLap.MaxEntries = 7;
		await FastestLap.Refresh();
	}
	public async Task RefreshMostLaps()
	{
		if (IsProxy) return;
		MostLaps = Sandbox.Services.Leaderboards.Get("mostlaps");
		MostLaps.MaxEntries = 7;
		await MostLaps.Refresh();
	}
	public async Task FetchDeathsLeaderboard()
	{
		if (IsProxy) return;
		Deaths = Sandbox.Services.Leaderboards.Get("mostdeaths");
		Deaths.MaxEntries = 7;
		await Deaths.Refresh();
	}


}
