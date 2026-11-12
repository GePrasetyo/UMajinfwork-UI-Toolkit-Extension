# Changelog

## [1.1.0] - 2026-11-12

### Added
- `TemplateRefs`: refs into a UXML template (list rows, items, a template placed with `<ui:Instance>`), with the template on the class; the Inspector lists the template's elements in each ref's dropdown.
- `UITElementRef<T>.In(instance)`: the ref's element inside one instance of its template.
- `ElementRef<T>`: a ref to any element type, such as your own controls.

### Changed
- Outside Play mode, with the prefab closed, a document ref's dropdown lists the elements of the document's source UXML instead of asking to open the prefab or enter Play mode.

## [1.0.0]

- Type-safe element refs (`ButtonRef`, `LabelRef`, `TextFieldRef`, `ProgressBarRef`, `ToggleRef`, `SliderRef`, `SliderIntRef`, `DropdownFieldRef`, `FoldoutRef`, `VisualElementRef`) with filtered Inspector dropdowns, code binding and optional localization.
