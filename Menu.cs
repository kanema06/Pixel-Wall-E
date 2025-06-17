using codes;
using Godot;
using Grid;
using System;
using System.IO;

public partial class Menu : Control
{
	public static bool abrirarchivo=false;
	private Panel panel;
	private FileDialog fileDialogmenu;

	public void _on_acerca_de_pressed()
	{
		panel.Visible=true;
	}
	public void _on_volver_pressed()
	{
		panel.Visible=false;
	}
	public void _on_abrir_archivo_pressed()
	{
		abrirarchivo=true;
		GetTree().ChangeSceneToPacked((PackedScene)ResourceLoader.Load("res://Scenes/CompilerUI.tscn"));
	}
	
	public void _on_salir_pressed()
	{
	     GetTree().Quit();
	}
	public override void _Ready()
	{

		panel=GetNode<Panel>("PanelAcercaDe");
	}
	public void _on_nuevo_documento_pressed()
	{
		
		GetTree().ChangeSceneToPacked((PackedScene)ResourceLoader.Load("res://Scenes/CompilerUI.tscn"));
	}

	public override void _Process(double delta)
	{
	}
}
