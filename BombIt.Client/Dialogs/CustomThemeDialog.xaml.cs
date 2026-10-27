using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BombIt.Client.Adapters;
using BombIt.Shared.DTOs;

namespace BombIt.Client.Dialogs;

public partial class CustomThemeDialog : Window
{
    // Pradinės spalvos = Classic tema (kad nepaliestos spalvos nebūtų baltos)
    private static readonly string[] ClassicDefaults = { "#90EE90", "#8B4513", "#696969", "#808080" };

    // Adapter Pattern - Client: dialogas dirba TIK su Target sąsaja, apie ColorWheel/HSV nieko nežino
    private readonly IHexColorProvider _colorProvider;

    private readonly string[] _colors;
    private readonly Border[] _swatches;
    private readonly TextBlock[] _hexLabels;
    private int _selectedSlot;

    // Nustatoma paspaudus OK. Null, jei žaidėjas paspaudė Cancel.
    public MapThemeDto? Result { get; private set; }

    public CustomThemeDialog(MapThemeDto? initial = null)
    {
        InitializeComponent();

        _swatches = new[] { SwatchEmpty, SwatchDestructible, SwatchIndestructible, SwatchGrid };
        _hexLabels = new[] { HexEmpty, HexDestructible, HexIndestructible, HexGrid };

        _colors = initial is null
            ? (string[])ClassicDefaults.Clone()
            : new[] { initial.EmptyColor, initial.DestructibleColor, initial.IndestructibleColor, initial.GridLineColor };

        // Adapter Pattern: adapteris apgaubia spalvų ratą ir pateikia jį kaip IHexColorProvider
        _colorProvider = new ColorWheelHexAdapter(Wheel);
        _colorProvider.ColorChanged += (_, _) => OnWheelColorChanged();

        for (int i = 0; i < _colors.Length; i++)
            RefreshSlot(i);

        SlotEmpty.IsChecked = true;
    }

    private void Slot_Checked(object sender, RoutedEventArgs e)
    {
        _selectedSlot = int.Parse((string)((RadioButton)sender).Tag);
    }

    private void OnWheelColorChanged()
    {
        _colors[_selectedSlot] = _colorProvider.GetHexColor(); // HEX iš adapterio
        RefreshSlot(_selectedSlot);
    }

    private void RefreshSlot(int slot)
    {
        var color = (Color)ColorConverter.ConvertFromString(_colors[slot]);
        _swatches[slot].Background = new SolidColorBrush(color);
        _hexLabels[slot].Text = _colors[slot];
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        Result = new MapThemeDto
        {
            Name = "Custom",
            EmptyColor = _colors[0],
            DestructibleColor = _colors[1],
            IndestructibleColor = _colors[2],
            GridLineColor = _colors[3]
        };
        DialogResult = true;
    }
}
