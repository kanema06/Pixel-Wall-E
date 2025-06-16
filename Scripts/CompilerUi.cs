using PixelWallE;
using Godot;
using System;
using Grid;
using System.Collections.Generic;
using System.Numerics;

namespace codes{
public partial class CompilerUi : Control
{   
    public Panel panel2;
    ExecutionResult finalResult = new ExecutionResult();
    private FileDialog fileDialog;
    private Button saveButton;
    private Button loadButton;
	public Godot.CodeEdit codeEdit;
    private PWSyntaxHighlighter _highlighter;

    public Panel panel;
    private bool[] opcionesSeleccionadas = new bool[3];
	public override void _Ready()
	{
        
        panel2=GetNode<Panel>("Panel2");
        saveButton = GetNode<Button>("Panel2/SaveButton");
        loadButton = GetNode<Button>("Panel2/LoadButton");
        saveButton.Pressed += _on_save_button_pressed;
        loadButton.Pressed += _on_load_button_pressed;
    
    	fileDialog = GetNode<FileDialog>("FileDialog");
   		fileDialog.FileSelected += OnFileDialogFileSelected;
        panel=GetNode<Panel>("Panel");
	    codeEdit=GetNode<Godot.CodeEdit>("CodeEdit");
        _highlighter = new PWSyntaxHighlighter();
        _highlighter.SetCodeEdit(codeEdit);
        codeEdit.SyntaxHighlighter = _highlighter;
        SetupCodeEditor(codeEdit);
        var menuButton = GetNode<MenuButton>("MenuButton");
        if(Menu.abrirarchivo==true)
        {
            _on_load_button_pressed();
            Menu.abrirarchivo=false;
        }
	}

	public override void _Process(double delta)
	{
	}

	 private void _on_compile_pressed()
{
    CanvasManager.CanvasSize = 512 / GridCanvas.Instance._currentGridSize;
    GridCanvas.Instance.ClearAllTiles();
    GridCanvas.Instance.ResetStats();
    text.RichTextLabel.Instance.Text="";
    finalResult.Errors.Clear();
    finalResult.Output.Clear();
    string mycode="";
     mycode=codeEdit.Text;
    try
    {
        var lexer = new LexicalAnalyzer();
        var tokens = lexer.Tokenize(mycode); 
        var parser = new Parser(tokens);
        var (programWithErrors, parseResult) = parser.ParseWithResult();
		var interpreter = new Interpreter(programWithErrors);
        interpreter.Result.Errors.AddRange(parseResult.Errors);
        interpreter.Interpret();
        finalResult.Output.AddRange(interpreter.Result.Output);
        finalResult.Errors.AddRange(interpreter.Result.Errors);
    }
    catch (Exception ex)
    {
        finalResult.AddError($"System error: {ex.Message}");;
    }
    text.RichTextLabel.Instance.Text=string.Join(System.Environment.NewLine, finalResult.Output)+ System.Environment.NewLine + string.Join(System.Environment.NewLine, finalResult.Errors);
}



private void SetupCodeEditor(Godot.CodeEdit codeEdit)
{
    codeEdit.AddThemeColorOverride("background_color", new Color("#1E1E1E"));
    codeEdit.AddThemeColorOverride("font_color", new Color("#D4D4D4"));
    
    codeEdit.HighlightCurrentLine = true;
    codeEdit.AddThemeColorOverride("current_line_color", new Color("#2D2D2D"));
}
private void _on_save_button_pressed()
{
    fileDialog.FileMode = FileDialog.FileModeEnum.SaveFile;
    fileDialog.PopupCentered();
}

private void OnFileDialogFileSelected(string path)
{
    if (fileDialog.FileMode == FileDialog.FileModeEnum.OpenFile)
    {
        LoadFileContent(path);
    }
    else
    {
        SaveFileContent(path);
    }
}

private void SaveFileContent(string path)
{
    string contentToSave = codeEdit.Text; 
    
    using (var file = FileAccess.Open(path, FileAccess.ModeFlags.Write))
    {
        if (file != null)
        {
            file.StoreString(contentToSave);
            GD.Print("Archivo guardado correctamente en: " + path);
        }
        else
        {
            GD.PrintErr("Error al guardar el archivo: " + FileAccess.GetOpenError());
        }
    }
}
private void _on_load_button_pressed()
{
    fileDialog.FileMode = FileDialog.FileModeEnum.OpenFile;
    fileDialog.PopupCentered();
}
public void LoadFileContent(string path)
{
    using (var file = FileAccess.Open(path, FileAccess.ModeFlags.Read))
    {
        if (file != null)
        {
            string fileContent = file.GetAsText();
            GD.Print("Contenido cargado: " + fileContent);
            
            ProcessLoadedContent(fileContent);
        }
        else
        {
            GD.PrintErr("Error al cargar el archivo: " + FileAccess.GetOpenError());
        }
    }
}
public void _on_ampliar_pressed()
{
    if (panel2.Visible==true)
    {
        panel2.Visible=false;
    }
    else
    {
        panel2.Visible=true;
    }
}

public void ProcessLoadedContent(string content)
{
   codeEdit.Text=content;
    
    GD.Print("Procesando contenido cargado...");
    GD.Print(content);
}
  
}
}