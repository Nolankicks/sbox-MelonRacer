using System.ComponentModel;
using Sandbox;
using Sandbox.UI;

public class MelonScenePanel : ScenePanel
{
    private SceneDirectionalLight Sun { get; set; }
    public static MelonScenePanel Instance { get; set; }
    public SceneModel Model { get; set; }
	public SceneFile ShitterScene {get; set;}
	public Model model;

    public MelonScenePanel()
    {
        Instance = this;
    }




    protected override void OnParametersSet()
    {
       World = new SceneWorld();
	
	   Model = new(World, "models/shitter.vmdl", new(Vector3.Zero, Rotation.Identity));
	   
	   Sun = new(World, Rotation.Identity, Color.White);
	   Camera.Position = Model.Transform.Position - Vector3.Backward * 100;
	   Camera.Rotation = Rotation.FromAxis(Vector3.Up, 180);
	   
    }
}
