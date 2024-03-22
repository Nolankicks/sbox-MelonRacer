using Sandbox;
using System.Threading.Tasks;
using Sandbox.Network;
public sealed class NetworkSystem : Component, Component.INetworkListener
{
	[Property] public bool StartServer { get; set; } = true;

	[Property] public GameObject PlayerPrefab { get; set; }
	protected override async Task OnLoad()
	{
		if ( Scene.IsEditor )
			return;

		if ( StartServer && !GameNetworkSystem.IsActive )
		{
			LoadingScreen.Title = "Creating Lobby";
			await Task.DelayRealtimeSeconds( 0.1f );
			GameNetworkSystem.CreateLobby();
		}
	}
	public void OnActive( Connection channel )
	{
		Log.Info( $"Player '{channel.DisplayName}' has joined the game" );

		if ( PlayerPrefab is null )
			return;

		//
		// Find a spawn location for this player
		//
		var startLocation = GameObject.Transform.World;

		// Spawn this object and make the client the owner
		var player = PlayerPrefab.Clone( startLocation, name: $"Player - {channel.DisplayName}" );
		player.NetworkSpawn( channel );
	}
	public void OnDisconnected(Connection channel)
	{
		foreach (var player in Scene.GetAllComponents<Controller>())
		{
			if (player.SteamId == channel.SteamId.ToString())
			{
				player.GameObject.Destroy();
			}
		}
		Log.Info("Dissconnected");

	}
	protected override void OnUpdate()
	{

	}
}
