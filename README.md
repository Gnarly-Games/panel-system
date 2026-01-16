# **Panel System**

A robust, code-driven panel management solution for Unity.

## **🚀 Getting Started**

### **1\. Setup**

1. **Tag Main Canvas:** Ensure your primary UI Canvas in the scene is tagged as **"MainCanvas"**. The system automatically finds this canvas to spawn panels.
2. **Create Panels:**
    *   Create a clean UI Panel (e.g., using a CanvasGroup).
    *   Add the `PanelView` component.
    *   Add a **Panel Animation** component (see below) and assign it to the `PanelView`'s **Panel Animation** field.
    *   Check **Can Access Via Key** and set a unique **Key** (e.g., "SettingsPanel") if you want to access it globally.
    *   *Optional:* Check **Can Load** and place the prefab in a `Resources` folder matching the **Key** name for dynamic loading.

### **2\. Animation System**

The system uses a generic `PanelAnimation` class to handle transitions. You can use the provided samples or create your own.

#### **Using Samples**
Import the **Starting Animations** sample from the Package Manager to get access to:
*   `PanelScaleAnimation`: Scales the panel in/out.
*   `PanelFadeAnimation`: Fades the canvas group alpha.
*   `PanelDelayAnimation`: Simple delay with no visual changes (useful for testing).

#### **Custom Animations**
Create a new class inheriting from `PanelAnimation` and implement the abstract methods:
*   `SetInitialState()`: Reset the panel to its closed state (e.g., alpha 0).
*   `MakeVisible()`: Return a `PrimeTween` Sequence for showing the panel.
*   `MakeHidden()`: Return a `PrimeTween` Sequence for hiding the panel.

#### **API Reference**

```csharp
public abstract class PanelAnimation : MonoBehaviour
{
    // The PanelView this animation is attached to
    public PanelView TargetPanel { get; }

    // Called on Awake to set the initial state (immediate, no animation)
    public abstract void SetInitialState();

    // Returns a Sequence that handles the "Show" animation
    public abstract Sequence MakeVisible();

    // Returns a Sequence that handles the "Hide" animation
    // IMPORTANT: Must call TargetPanel.OnHide(shouldDestroyAfterHide) on complete
    public abstract Sequence MakeHidden(bool shouldDestroyAfterHide);
}
```

#### **Example: Slide Animation**

Here is a complete example of a custom animation that slides and fades a panel.

```csharp
using PrimeTween;
using UnityEngine;
using Gnarly.PanelSystem;

public class PanelSlideAnimation : PanelAnimation
{
    public Vector2 startOffset = new Vector2(0, -500);

    public override void SetInitialState()
    {
        var rect = TargetPanel.GetComponent<RectTransform>();
        
        if (TargetPanel.isVisibleOnLoad)
        {
            TargetPanel.Group.alpha = 1f;
            rect.anchoredPosition = Vector2.zero;
            TargetPanel.Group.blocksRaycasts = true;
        }
        else
        {
            TargetPanel.Group.alpha = 0f;
            rect.anchoredPosition = startOffset;
            TargetPanel.Group.blocksRaycasts = false;
        }
    }

    public override Sequence MakeVisible()
    {
        var rect = TargetPanel.GetComponent<RectTransform>();
        var seq = Sequence.Create(useUnscaledTime: true);
        seq.Group(Tween.UIAnchoredPosition(rect, Vector2.zero, TargetPanel.showDuration, Ease.OutBack));
        seq.Group(Tween.Alpha(TargetPanel.Group, 1f, TargetPanel.showDuration));
        seq.OnComplete(() => TargetPanel.onShow?.Invoke());
        
        return seq;
    }

    public override Sequence MakeHidden(bool shouldDestroyAfterHide)
    {
        var rect = TargetPanel.GetComponent<RectTransform>();
        var seq = Sequence.Create(useUnscaledTime: true);
        seq.Group(Tween.UIAnchoredPosition(rect, startOffset, TargetPanel.hideDuration, Ease.InBack));
        seq.Group(Tween.Alpha(TargetPanel.Group, 0f, TargetPanel.hideDuration));
        seq.OnComplete(() => TargetPanel.OnHide(shouldDestroyAfterHide));
        
        return seq;
    }
}
```

### **3\. Usage**

The system provides a static `PanelManager` to easily access and control panels from anywhere in your codebase.

#### **Access & Control**

Retrieve a panel by its Key or Component type. If it doesn't exist in the scene, the system attempts to load it from Resources.

```csharp
using Gnarly.PanelSystem;

public void OpenSettings()
{
    // Get by Type (Key is inferred from Class Name if not specified)
    var settingsPanel = PanelManager.GetOrCreate<SettingsPanel>();
    
    // OR Get by Key
    // var settingsPanel = PanelManager.GetOrCreate("SettingsPanel");

    if (settingsPanel)
    {
        settingsPanel.Show();
    }
}
```

#### **Hiding Panels**

```csharp
public void ClosePanel(PanelView panel)
{
    // Hide and optionally destroy if it was dynamically loaded
    panel.Hide(shouldDestroyAfterFinish: true);
}
```

#### **Stack Management**

Easily manage UI layering (Z-order) relative to other objects.

```csharp
// Bring to front
myPanel.SetAsLastSibling();

// Place behind another specific element
myPanel.SetBehindOf(someOtherTransform);
```

### **4\. Configuration**

The `PanelView` component offers several configuration options in the Inspector:

*   **Panel Animation:** Reference to the generic `PanelAnimation` component.
*   **Show/Hide Duration:** Customize the speed of animations.
*   **Is Visible On Load:** Determine if the panel starts open.
*   **Can Load:** Enables dynamic instantiation from Resources (requires `Resources/Key` match).

### **5\. Events**

Hook into `OnShow` and `OnHide` UnityEvents in the inspector, or subscribe via code (though direct method calls are preferred).