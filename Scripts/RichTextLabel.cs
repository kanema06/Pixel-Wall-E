using Godot;
using System;
namespace text
{
public partial class RichTextLabel : Godot.RichTextLabel
{
	private static RichTextLabel _instance;
    public static RichTextLabel Instance => _instance;
	public override void _Ready()
	{
		  _instance = this;
	}
	public override void _Process(double delta)
	{
	}
}
}