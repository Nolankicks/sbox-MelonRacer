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
