# UI Toolkit Extension

<p align="center">
  <img src="https://img.shields.io/badge/Unity-2022.3%2B-black?style=flat-square&logo=unity" alt="Unity 2022.3+"/>
  <img src="https://img.shields.io/badge/License-MIT-green?style=flat-square" alt="MIT License"/>
  <img src="https://img.shields.io/badge/Version-0.0.1--preview-blue?style=flat-square" alt="Version"/>
</p>

<p align="center">
  <b>Type-safe UI Toolkit element references for Unity</b><br/>
  Reference UI elements directly from the Inspector with filtered dropdowns - no more string queries!
</p>

---

## Features

- **Type-Safe References** - Each element type has its own reference class (`ButtonRef`, `LabelRef`, etc.)
- **Filtered Dropdowns** - Inspector shows only elements matching the target type
- **Auto Cache Management** - Handles UIDocument disable/enable cycles automatically
- **Convenience Methods** - Common operations like `OnClick()`, `OnValueChanged()` built-in
- **Zero Boilerplate** - No manual `Q()` calls or string constants needed

## Quick Start

### 1. Add fields to your MonoBehaviour

```csharp
using Majinfwork.UI;
using UnityEngine;

public class GameHUD : MonoBehaviour {
    public ButtonRef playButton;
    public ButtonRef settingsButton;
    public LabelRef scoreLabel;
    public ProgressBarRef healthBar;
    public SliderRef volumeSlider;
}
```

### 2. Assign in Inspector

- Drag your `UIDocument` to the document field
- Select the element from the filtered dropdown

### 3. Use at runtime

```csharp
void OnEnable() {
    // Button clicks
    playButton.OnClick(StartGame);
    settingsButton.OnClick(OpenSettings);

    // Set values
    scoreLabel.Text = "Score: 0";
    healthBar.Value = 100f;

    // React to changes
    volumeSlider.OnValueChanged(vol => AudioListener.volume = vol);
}

void OnDisable() {
    // Cleanup (optional but recommended)
    playButton.RemoveOnClick(StartGame);
    volumeSlider.RemoveOnValueChanged();
}
```

## Available Reference Types

| Class | UI Element | Key Properties/Methods |
|-------|------------|----------------------|
| `ButtonRef` | `Button` | `OnClick()`, `RemoveOnClick()`, `Text`, `SetEnabled()` |
| `LabelRef` | `Label` | `Text` |
| `TextFieldRef` | `TextField` | `Value`, `OnValueChanged()`, `Placeholder`, `IsReadOnly` |
| `ProgressBarRef` | `ProgressBar` | `Value`, `LowValue`, `HighValue`, `SetNormalizedValue()` |
| `ToggleRef` | `Toggle` | `Value`, `OnValueChanged()`, `Text` |
| `SliderRef` | `Slider` | `Value`, `OnValueChanged()`, `LowValue`, `HighValue` |
| `SliderIntRef` | `SliderInt` | `Value`, `OnValueChanged()`, `LowValue`, `HighValue` |
| `DropdownFieldRef` | `DropdownField` | `Value`, `Index`, `OnValueChanged()`, `SetChoices()` |
| `FoldoutRef` | `Foldout` | `Value`, `Text` |
| `VisualElementRef` | `VisualElement` | `Show()`, `Hide()`, `Visible`, `AddClass()`, `RemoveClass()` |

## Direct Element Access

For advanced use cases, access the underlying element directly:

```csharp
// Access the raw Button element
playButton.Button.style.backgroundColor = Color.green;

// Multiple callbacks (use Element directly)
volumeSlider.Slider.RegisterValueChangedCallback(evt => OnVolumeA(evt.newValue));
volumeSlider.Slider.RegisterValueChangedCallback(evt => OnVolumeB(evt.newValue));
```

## Callback Behavior

The convenience `OnValueChanged()` methods support **one callback** at a time:

```csharp
slider.OnValueChanged(OnVolumeChange);  // Registers callback
slider.OnValueChanged(OnOtherThing);    // Replaces previous callback
slider.RemoveOnValueChanged();          // Unregisters callback
```

For multiple callbacks, use `Element` directly with `RegisterValueChangedCallback()`.

## UIDocument Lifecycle

References automatically handle UIDocument disable/enable cycles:

- Cache invalidates when `rootVisualElement` changes
- Next property access re-resolves the element
- **Event callbacks must be re-registered** in `OnEnable()`

```csharp
void OnEnable() {
    // Always register callbacks here
    playButton.OnClick(OnPlayClicked);
}

void OnDisable() {
    // Optional cleanup
    playButton.RemoveOnClick(OnPlayClicked);
}
```

## Requirements

- Unity 2022.3 or later
- UI Toolkit (com.unity.modules.uielements)

## License

MIT License - see [LICENSE](LICENSE) for details.

---
