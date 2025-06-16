using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PixelAutocompletion : CodeEdit
{
    private readonly string[] _keywords = {
        "Spawn", "Color", "Size", "DrawLine", "DrawCircle", 
        "DrawRectangle", "Fill", "GoTo", "GetActualX", "GetActualY",
        "GetCanvasSize", "GetColorCount", "IsBrushColor", "IsBrushSize", "IsCanvasColor"
    };

    private readonly string[] _colors = {
        "Red", "Blue", "Green", "Yellow", "Orange", 
        "Purple", "Black", "White", "Transparent"
    };

    public override void _Ready()
    {
        CodeCompletionEnabled = true;
        TextChanged += OnTextChanged;
    }

    private void OnTextChanged()
    {
        var prefix = GetTextForCodeCompletion();
        var suggestions = GetCompletionSuggestions(prefix);
        foreach (var suggestion in suggestions)
        {
            AddCodeCompletionOption(CodeCompletionKind.Function, suggestion, suggestion);
        }
        UpdateCodeCompletionOptions(true);
    }

    private List<string> GetCompletionSuggestions(string prefix)
    {
        var suggestions = new List<string>();

        suggestions.AddRange(_keywords.Where(k =>
            k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)));
        suggestions.AddRange(_colors.Where(c =>
            c.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)));

        suggestions.AddRange(FindUserVariables(prefix));

        return suggestions;
    }
    private List<string> FindUserVariables(string prefix)
    {
        var variables = new List<string>();
        string fullText = Text;
        int startIndex = 0;

        while (startIndex < fullText.Length)
        {
            int assignIndex = fullText.IndexOf("<-", startIndex);
            if (assignIndex == -1)
                break;

            int varStart = fullText.LastIndexOfAny(new char[] { '\n', ';' }, assignIndex) + 1;
            if (varStart < 0)
                varStart = 0;

            string varName = fullText.Substring(varStart, assignIndex - varStart).Trim();

            if (!string.IsNullOrEmpty(varName) &&
                !variables.Contains(varName) &&
                varName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                variables.Add(varName);
            }

            startIndex = assignIndex + 2;
        }

        return variables;
    }

    private string GetTextForCodeCompletion()
    {
        int caretLine = GetCaretLine();
        int caretColumn = GetCaretColumn();
        string lineText = GetLine(caretLine);

        int startPos = caretColumn;
        while (startPos > 0 && IsIdentifierChar(lineText[startPos - 1]))
        {
            startPos--;
        }

        return lineText.Substring(startPos, caretColumn - startPos);
    }

    private bool IsIdentifierChar(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_' || c == '-';
    }
}
