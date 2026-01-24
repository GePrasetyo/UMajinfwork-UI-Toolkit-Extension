# 📱 Majinfwork - Unity UI Toolkit Extension

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6.3%2B-black?style=flat-square&logo=unity" alt="Unity 6.3+"/>
  <img src="https://img.shields.io/badge/License-MIT-green?style=flat-square" alt="MIT License"/>
  <img src="https://img.shields.io/badge/Version-1.0.0-blue?style=flat-square" alt="Version"/>
</p>

<p align="center">
  <b>Type-safe UI Toolkit element references for Unity</b><br/>
  Reference UI elements directly from the Inspector with filtered dropdowns - no more string queries!
</p>

---

## 🛠️ Features

- **Type-Safe References** - Each element type has its own reference class (`ButtonRef`, `LabelRef`, etc.)
- **Filtered Dropdowns** - Inspector shows only elements matching the target type
- **Two Reference Modes** - Use external UIDocument or auto-find on same GameObject
- **Code Binding API** - Assign elements via code when Inspector isn't desired
- **Auto Cache Management** - Handles UIDocument disable/enable cycles automatically
- **Convenience Methods** - Common operations like `OnClick()`, `OnValueChanged()` built-in
- **Zero Boilerplate** - No manual `Q()` calls or string constants needed

## 🚀 Getting Started

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

Each ref field shows a small icon button to toggle between two modes:

| Icon | Mode | Description |
|------|------|-------------|
| 🔗 Link | **UseReference** | Manually assign any UIDocument + select element |
| 📍 Unlink | **UseDocumentHere** | Auto-uses UIDocument on same GameObject |

**UseReference mode:**
- Drag any `UIDocument` to the document field
- Select the element from the filtered dropdown

**UseDocumentHere mode:**
- Just select the element - UIDocument is found automatically
- Shows red error if no UIDocument exists on the GameObject

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

## 💻 Assign in Code

For developers who prefer code over Inspector, use the `Bind` API:

### Bind with UIDocument

```csharp
[SerializeField] private UIDocument document;
private LabelRef scoreLabel = new();
private ButtonRef playButton = new();

void Awake() {
    // Bind to document - element will be queried on first access
    scoreLabel.Bind(document, "score-label");
    playButton.Bind(document, "play-button");
}

void Start() {
    scoreLabel.Text = "Score: 0";
    playButton.OnClick(StartGame);
}
```

### Bind with Root Element

```csharp
void OnEnable() {
    var root = document.rootVisualElement;

    // Bind and query from root element directly
    scoreLabel.Bind(root, "score-label");
    playButton.Bind(root, "play-button");
}
```

### Bind Direct Element

```csharp
void OnEnable() {
    var root = document.rootVisualElement;

    // Query manually and bind the result directly
    var label = root.Q<Label>("score-label");
    var button = root.Q<Button>("play-button");

    scoreLabel.BindDirect(label);
    playButton.BindDirect(button);
}
```

### Comparison with Standard UI Toolkit

```csharp
// Standard UI Toolkit approach
private Label _scoreLabel;
private Button _playButton;

void OnEnable() {
    var root = document.rootVisualElement;
    _scoreLabel = root.Q<Label>("score-label");
    _playButton = root.Q<Button>("play-button");
    _scoreLabel.text = "Score: 0";
}

// Majinfwork.UI approach (Inspector)
[SerializeField] private LabelRef scoreLabel;  // Assign in Inspector
[SerializeField] private ButtonRef playButton;

void Start() {
    scoreLabel.Text = "Score: 0";  // Just use it
}

// Majinfwork.UI approach (Code)
private LabelRef scoreLabel = new();
private ButtonRef playButton = new();

void Awake() {
    scoreLabel.Bind(document, "score-label");
    playButton.Bind(document, "play-button");
}

void Start() {
    scoreLabel.Text = "Score: 0";
}
```

## 📖 Available Reference Types

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

## 🔗 Direct Element Access

For advanced use cases, access the underlying element directly:

```csharp
// Access the raw Button element
playButton.Button.style.backgroundColor = Color.green;

// Multiple callbacks (use Element directly)
volumeSlider.Slider.RegisterValueChangedCallback(evt => OnVolumeA(evt.newValue));
volumeSlider.Slider.RegisterValueChangedCallback(evt => OnVolumeB(evt.newValue));
```

## 🔗 Callback Behavior

The convenience `OnValueChanged()` methods support **one callback** at a time:

```csharp
slider.OnValueChanged(OnVolumeChange);  // Registers callback
slider.OnValueChanged(OnOtherThing);    // Replaces previous callback
slider.RemoveOnValueChanged();          // Unregisters callback
```

For multiple callbacks, use `Element` directly with `RegisterValueChangedCallback()`.

## 📦 UIDocument Lifecycle

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

## ⚙️ Requirements

- Unity 6.3 or later
- UI Toolkit (com.unity.modules.uielements)

## 📜 License

MIT License - see [LICENSE](LICENSE) for details.

---
