using Godot;
using System;

[Tool]
public class ParentNode : Node3D
{
	private float _globalSeed;

	public float GlobalSeed
	{
		get { return _globalSeed; }
		set
		{
			_globalSeed = value;
			UpdateChildrenGlobalSeed();
		}
	}

	public override void _Ready()
	{
		UpdateChildrenGlobalSeed();
	}

	public void UpdateChildrenGlobalSeed()
	{
		foreach (Node child in GetChildren())
		{
			var childType = child.GetType();
			var fieldInfo = childType.GetField("global_seed");
			if (fieldInfo != null)
			{
				fieldInfo.SetValue(child, _globalSeed);
			}
		}
	}
}
