using Godot;
using System;

public partial class WarpRing : Area2D
{
	private SubViewport ringView;
	private Sprite2D renderedSprite;
	private MeshInstance3D torus;

	public override void _Ready()
	{
		ringView = GetNode<SubViewport>("SubViewport");
		renderedSprite = GetNode<Sprite2D>("Sprite2D");
		torus = GetNode<MeshInstance3D>("SubViewport/WarpRingRenderer/Torus");
	}

	public override void _Process(double delta)
	{
		SpinWarpRing(delta);
		renderedSprite.Texture = ringView.GetTexture();
	}

	private void SpinWarpRing(double delta) {
		torus.RotateX(0.01f);
		torus.RotateY(0.005f);
		torus.RotateZ(0.02f);
	}

	private void OnWarpEntered(Node Collider) {
		Mesh torusMesh = torus.Mesh;
		Vector3[] vertices = torusMesh.GetFaces();

		ImmediateMesh immediateMesh = new ImmediateMesh();
		immediateMesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
		immediateMesh.SurfaceSetColor(Colors.Black);

		for(int i = 0; i <  vertices.Length; i += 3) {
			immediateMesh.SurfaceAddVertex(vertices[i]);
			immediateMesh.SurfaceAddVertex(vertices[i + 1]);

			immediateMesh.SurfaceAddVertex(vertices[i + 1]);
			immediateMesh.SurfaceAddVertex(vertices[i + 2]);

			immediateMesh.SurfaceAddVertex(vertices[i + 2]);
			immediateMesh.SurfaceAddVertex(vertices[i]);
		}

		immediateMesh.SurfaceEnd();
		torus.Mesh = immediateMesh;
		
		this.CollisionMask = 0;
	}
}
