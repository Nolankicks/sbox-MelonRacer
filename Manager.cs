using System.Threading.Tasks;
using Sandbox;
using Sandbox.Utility;

public sealed class Manager : Component
{
	[Property] public GameObject PlayerPrefab { get; set; }
	[Property] public List<SpawnPoint> spawnPoints { get; set;} = new List<SpawnPoint>();
	public Sandbox.Services.Leaderboards.Board FastestLap { get; set; }
	public Sandbox.Services.Leaderboards.Board MostLaps { get; set; }
	public Sandbox.Services.Leaderboards.Board MelonLockdown { get; set; }
	public Sandbox.Services.Leaderboards.Board Deaths { get; set; }
	protected override void OnStart()
	{
		_ = FetchFastestLap();
		_ = RefreshMostLaps();
		_ = FetchMelonLockdownLeaderboard();
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
	public async Task FetchMelonLockdownLeaderboard()
	{
		if (IsProxy) return;
		MelonLockdown = Sandbox.Services.Leaderboards.Get("gm_lockdown");
		MelonLockdown.MaxEntries = 7;
		await MelonLockdown.Refresh();
	}


}
