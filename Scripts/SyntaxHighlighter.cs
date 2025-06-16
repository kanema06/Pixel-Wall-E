
using Godot;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class PWSyntaxHighlighter : SyntaxHighlighter
{

    private static Godot.CodeEdit _codeEdit;
    private static readonly string[] Instructions = 
    {
        "Spawn", "Color", "Size", "DrawLine", "DrawCircle", 
        "DrawRectangle", "Fill", "GoTo"
    };
    
    private static readonly string[] Functions = 
    {
        "GetActualX", "GetActualY", "GetCanvasSize", "GetColorCount",
        "IsBrushColor", "IsBrushSize", "IsCanvasColor"
    };
    
    private static readonly string[] Colors = 
    {
        "Red", "Blue", "Green", "Yellow", "Orange", 
        "Purple", "Black", "White", "Transparent"
    };

    private readonly Dictionary<string, Color> _colors = new()
    {
        ["instruction"] = new Color("#569CD6"),
        ["function"] = new Color("#4EC9B0"),
        ["color_value"] = new Color("#CE9178"),
        ["comment"] = new Color("#57A64A"),
        ["number"] = new Color("#B5CEA8"),
        ["symbol"] = new Color("#DCDCAA"),
        ["text"] = new Color("#D4D4D4")
    };
       public void SetCodeEdit(Godot.CodeEdit codeEdit)
    {
        _codeEdit = codeEdit;
    }
    public override Godot.Collections.Dictionary _GetLineSyntaxHighlighting(int line)
    {
        var highlighting = new Godot.Collections.Dictionary();
        var codeEdit = GetTextEdit();
        
        if (codeEdit == null || line < 0 || line >= codeEdit.GetLineCount())
            return highlighting;

        string text = codeEdit.GetLine(line);
        if (string.IsNullOrEmpty(text))
            return highlighting;

        AnalyzeTokens(text, highlighting);

        return highlighting;
    }

    private void AnalyzeTokens(string text, Godot.Collections.Dictionary highlighting)
    {
        int pos = 0;
        while (pos < text.Length)
        {
            if (char.IsWhiteSpace(text[pos]))
            {
                pos++;
                continue;
            }

            bool found = false;
            
            foreach (var token in Instructions)
            {
                if (pos + token.Length <= text.Length && 
                    text.Substring(pos, token.Length) == token)
                {
                    AddHighlight(highlighting, pos, token.Length, "instruction");
                    pos += token.Length;
                    found = true;
                    break;
                }
            }
            if (found) continue;

            foreach (var token in Functions)
            {
                if (pos + token.Length <= text.Length && 
                    text.Substring(pos, token.Length) == token)
                {
                    AddHighlight(highlighting, pos, token.Length, "function");
                    pos += token.Length;
                    found = true;
                    break;
                }
            }
            if (found) continue;

            if (pos < text.Length && text[pos] == '"')
            {
                int endQuote = text.IndexOf('"', pos + 1);
                if (endQuote > pos)
                {
                    string quotedText = text.Substring(pos + 1, endQuote - pos - 1);
                    if (Colors.Contains(quotedText))
                    {
                        AddHighlight(highlighting, pos, endQuote - pos + 1, "color_value");
                        pos = endQuote + 1;
                        continue;
                    }
                }
            }

            if (char.IsDigit(text[pos]))
            {
                int start = pos;
                while (pos < text.Length && char.IsDigit(text[pos])) pos++;
                AddHighlight(highlighting, start, pos - start, "number");
                continue;
            }

            if ("()[],;+-*/%".Contains(text[pos]))
            {
                AddHighlight(highlighting, pos, 1, "symbol");
                pos++;
                continue;
            }

            pos++;
        }
    }

    private void AddHighlight(Godot.Collections.Dictionary dict, int start, int length, string type)
    {
        if (length <= 0 || start < 0) return;
        
        var props = new Godot.Collections.Dictionary
        {
            ["color"] = _colors.GetValueOrDefault(type, _colors["text"])
        };
        
        dict[start] = props;
    }
}