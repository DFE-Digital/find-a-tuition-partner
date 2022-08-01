using UI.Pages.Shared;

namespace Tests;

public class OptionsSelectModelTests
{
    [Fact]
    public void Selection_is_collapsed_when_there_are_no_items()
    {
        var sut = new OptionsSelectModel(
            "select-model", "Select Model",
             Array.Empty<(string Name, string Value, string DisplayName, bool Selected)>());
        sut.ClosedData.Should().Be("data-closed-on-load=true");
    }

    [Fact]
    public void Selection_is_collapsed_when_no_items_are_selected()
    {
        var sut = new OptionsSelectModel("select-model", "Select Model", new[]
        {
            ("item", "item", "Item", Selected : false),
       });

        sut.ClosedData.Should().Be("data-closed-on-load=true");
    }

    [Fact]
    public void Selection_is_expanded_when_items_are_selected()
    {
        var sut = new OptionsSelectModel("select-model", "Select Model", new[]
        {
            ("item", "item", "Item", Selected : false),
            ("item", "item", "Item", Selected : true),
       });

        sut.ClosedData.Should().Be("");
    }

    [Fact]
    public void Selection_is_collapsed_when_there_are_no_items_and_override_is_to_collapse()
    {
        var sut = new OptionsSelectModel(
            "select-model", "Select Model",
            Array.Empty<(string Name, string Value, string DisplayName, bool Selected)>(),
            ClosedOnLoad: true);
        sut.ClosedData.Should().Be("data-closed-on-load=true");
    }

    [Fact]
    public void Selection_is_collapsed_when_no_items_are_selected_and_override_is_to_collapse()
    {
        var sut = new OptionsSelectModel(
            "select-model", "Select Model", new[] { ("item", "item", "Item", Selected: false), },
            ClosedOnLoad: true);

        sut.ClosedData.Should().Be("data-closed-on-load=true");
    }

    [Fact]
    public void Selection_is_collapsed_when_items_are_selected_and_override_is_to_collapse()
    {
        var sut = new OptionsSelectModel(
            "select-model", "Select Model", new[]
            {
                ("item", "item", "Item", Selected : false),
                ("item", "item", "Item", Selected : true),
            },
            ClosedOnLoad: true);

        sut.ClosedData.Should().Be("data-closed-on-load=true");
    }

    [Fact]
    public void Selection_is_expanded_when_there_are_no_items_and_override_is_to_expand()
    {
        var sut = new OptionsSelectModel(
            "select-model", "Select Model",
            Array.Empty<(string Name, string Value, string DisplayName, bool Selected)>(),
            ClosedOnLoad: false);
        sut.ClosedData.Should().Be("");
    }

    [Fact]
    public void Selection_is_expanded_when_no_items_are_selected_and_override_is_to_expand()
    {
        var sut = new OptionsSelectModel(
            "select-model", "Select Model", new[] { ("item", "item", "Item", Selected: false), },
            ClosedOnLoad: false);

        sut.ClosedData.Should().Be("");
    }

    [Fact]
    public void Selection_is_expanded_when_items_are_selected_and_override_is_to_expand()
    {
        var sut = new OptionsSelectModel(
            "select-model", "Select Model", new[]
            {
                ("item", "item", "Item", Selected: false),
                ("item", "item", "Item", Selected: true),
            },
            ClosedOnLoad: false);

        sut.ClosedData.Should().Be("");
    }
}