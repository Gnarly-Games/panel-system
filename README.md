# **Panel System**

A robust, code-driven panel management solution for Unity, powered by PrimeTween and TriInspector.

## **🚀 Getting Started**

### **1\. Setup**

1. **Tag Main Canvas:** Ensure your primary UI Canvas in the scene is tagged as **"MainCanvas"**. The system automatically finds this canvas to spawn panels.
2. **Create Panels:**
    *   Create a clean UI Panel (e.g., using a CanvasGroup).
    *   Add the `PanelView` component.
    *   Check **Can Access Via Key** and set a unique **Key** (e.g., "SettingsPanel") if you want to access it globally.
    *   *Optional:* Check **Can Load** and place the prefab in a `Resources` folder matching the **Key** name for dynamic loading.

### **2\. Usage**

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

### **3\. Configuration**

The `PanelView` component offers several configuration options in the Inspector:

*   **Animation Type:** Choose between `Scale` or `Fade` transitions.
*   **Show/Hide Duration:** Customize the speed of animations.
*   **Is Visible On Load:** Determine if the panel starts open.
*   **Can Load:** Enables dynamic instantiation from Resources (requires `Resources/Key` match).

### **4\. Events**

Hook into `OnShow` and `OnHide` UnityEvents in the inspector, or subscribe via code (though direct method calls are preferred).