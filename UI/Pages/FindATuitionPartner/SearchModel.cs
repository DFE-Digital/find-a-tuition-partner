using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace UI.Pages.FindATuitionPartner;

public record SearchModel
{
    public SearchModel(SearchModel model) : base()
    {
        Postcode = model.Postcode;
        Subjects = model.Subjects;
        TuitionType = model.TuitionType;
        KeyStages = model.KeyStages;
    }

    public string? Postcode { get; set; }

    public string[]? Subjects { get; set; }
        
    public TuitionType TuitionType { get; set; }
    
    public KeyStage[] KeyStages { get; set; } = Array.Empty<KeyStage>();
}

public static class ModelExtensions
{
    public static KeyStageSubject[] ParseKeyStageSubjects(this string[] keyStageSubjects)
        => keyStageSubjects.Select(KeyStageSubject.Parse).ToArray();
}

public record KeyStageSubject(KeyStage KeyStage, string Subject)
{
    public static KeyStageSubject Parse(string value)
    {
        var re = new Regex(@"(KeyStage[\d])-(.*)").Match(value);
        
        if (!re.Success)
            throw new ArgumentException("Subject must be of the form KS1-English");

        if (!Enum.TryParse<KeyStage>(re.Groups[1].Value, out var ks))
            throw new ArgumentException($"'{re.Groups[0].Value}' is not a valid key stage");

        return new KeyStageSubject(ks, re.Groups[2].Value);
    }

    public static bool TryParse(string value, [MaybeNullWhen(false)] out KeyStageSubject parsed)
    {
        if(value == null)
        {
            parsed = default;
            return false;
        }

        var re = new Regex(@"(KeyStage[\d])-(.*)").Match(value);

        if (!re.Success)
        {
            parsed = default;
            return false;
        }

        if (!Enum.TryParse<KeyStage>(re.Groups[1].Value, out var ks))
        {
            parsed = default;
            return false;
        }

        parsed = new KeyStageSubject(ks, re.Groups[2].Value);
        return true;
    }
}

internal class KeyStageSubjectBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var modelName = bindingContext.ModelName;

        // Try to fetch the value of the argument by name
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;

        // Check if the argument value is null or empty
        if (string.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }

        try
        {
            KeyStageSubject.Parse(value);
            bindingContext.Result = ModelBindingResult.Success(value);
        }
        catch(Exception e)
        {
            bindingContext.ModelState.TryAddModelError(
                modelName, e.Message);
        }
        //var re = new Regex(@"(KeyStage[\d])-(.*)").Match(value);
        //if(!re.Success)
        //{
        //    return Task.CompletedTask;
        //}

        //if(!Enum.TryParse<KeyStage>(re.Groups[1].Value, out var ks))
        //{
        //    bindingContext.ModelState.TryAddModelError(
        //        modelName, $"'{re.Groups[0].Value}' is not a valid key stage");
        //    return Task.CompletedTask;
        //}
        //var model = new KeyStageSubject(ks, re.Groups[2].Value);
        //bindingContext.Result = ModelBindingResult.Success(model);
        return Task.CompletedTask;
    }
}

public enum TuitionType
{
    Any = 0,
    InPerson = 1,
    Online = 2,
}

public enum KeyStage
{
    Unspecified = 0,

    [Description("Key Stage 1")]
    KeyStage1 = 1,
    
    [Description("Key stage 2")]
    KeyStage2,
    
    [Description("Key stage 3")]
    KeyStage3,
    
    [Description("Key stage 4")]
    KeyStage4,
}

public enum Subject
{
    Unspecified = 0,

    Literacy,
    Numeracy,
    Science,

    Maths,
    English,
    Humanities,
    ModernForeignLanguages,
}