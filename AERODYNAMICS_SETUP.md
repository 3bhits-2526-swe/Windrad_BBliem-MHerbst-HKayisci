# Aerodynamics Visualizer - Quick Setup Guide

## Files Created

1. **`AerodynamicsSurfaceVisualizer.cs`** - Main component
2. **`AerodynamicsVisualizerExample.cs`** - Example usage script
3. **`AERODYNAMICS_VISUALIZER_README.md`** - Full documentation

## How to Use

### Step 1: Add Component to Object
1. In your scene, select the 3D object you want to visualize
2. In the Inspector, click **Add Component**
3. Search for **`AerodynamicsSurfaceVisualizer`**
4. Click to add it

### Step 2: Configure (Optional)
In the Inspector, you'll see these settings:

| Setting | Default | Purpose |
|---------|---------|---------|
| **Use Per Face Normals** | ✓ ON | Sharp colors per triangle (recommended for meshes) |
| **Recalculate Normals At Runtime** | OFF | For deforming geometry |
| **Update Every Frame** | OFF | Leave OFF for static meshes |
| **Color Gradient** | Red→Yellow→Green | Define custom color mapping |
| **Steepness Curve** | Linear | Adjust color distribution |

### Step 3: Play!
Hit Play in the editor. Your object will show:
- **Red** for flat/horizontal surfaces (0°)
- **Yellow** for diagonal surfaces (45°)
- **Green** for vertical surfaces (90°)

## Key Features Explained

### Steepness Calculation
```
Steepness = How perpendicular the surface is to gravity

0°  (Flat horizontal)   → Red   (steepness = 0%)
45° (Diagonal)          → Yellow (steepness = ~30%)
90° (Vertical)          → Green (steepness = 100%)
```

### Two Coloring Modes

**Per-Face Normals** (Recommended for most meshes)
- Each triangle gets ONE color
- Sharp, distinct color boundaries
- ~2x faster than per-vertex
- Best for: Hard-surface geometry, turbine blades

**Per-Vertex Normals** (Recommended for organic shapes)
- Each vertex gets its own color based on averaged normals
- Smooth color gradients across surfaces
- Better appearance for smooth geometry
- Best for: Organic objects, rounded surfaces

Toggle between them: Inspector checkbox or in code with `SetUsePerFaceNormals(bool)`

### Custom Gradients

The default gradient is Red → Yellow → Green, but you can customize:

```csharp
var visualizer = GetComponent<AerodynamicsSurfaceVisualizer>();

// Create a custom gradient (e.g., Blue to Red)
Gradient myGradient = new Gradient();
GradientColorKey[] colors = new GradientColorKey[2];
colors[0].color = Color.blue;
colors[0].time = 0f;
colors[1].color = Color.red;
colors[1].time = 1f;

GradientAlphaKey[] alphas = new GradientAlphaKey[2];
alphas[0].alpha = 1f;
alphas[0].time = 0f;
alphas[1].alpha = 1f;
alphas[1].time = 1f;

myGradient.SetKeys(colors, alphas);
visualizer.SetColorGradient(myGradient);
```

## For Your Windrad Project

### Wind Turbine Blades
```csharp
// Add to each blade
blade.AddComponent<AerodynamicsSurfaceVisualizer>();

// Use per-face for sharp blade sections
var vis = blade.GetComponent<AerodynamicsSurfaceVisualizer>();
vis.SetUsePerFaceNormals(true);
```

Results: Shows how blade angle changes across its length - efficient aerodynamic design!

### Tower/Base
```csharp
tower.AddComponent<AerodynamicsSurfaceVisualizer>();
// Use per-vertex for smooth tower geometry
vis.SetUsePerFaceNormals(false);
```

Results: Smooth gradient showing structural steepness

## Performance Tips

| Scenario | Configuration |
|----------|---------------|
| **Static mesh (turbine tower)** | Use Per-Face ON, Runtime OFF, Update Frame OFF |
| **Dynamic/deforming mesh** | Use Per-Face ON, Runtime ON, Update Frame ON |
| **High poly mesh (100k+ triangles)** | Use Per-Face ON, call `RefreshVisualization()` manually |
| **Smooth appearance priority** | Use Per-Face OFF (per-vertex mode) |

## API Reference

```csharp
// Get the component
var visualizer = GetComponent<AerodynamicsSurfaceVisualizer>();

// Manually refresh visualization
visualizer.RefreshVisualization();

// Set custom gradient
visualizer.SetColorGradient(myGradient);

// Toggle coloring mode
visualizer.SetUsePerFaceNormals(true);   // Sharp (per-face)
visualizer.SetUsePerFaceNormals(false);  // Smooth (per-vertex)
```

## Troubleshooting

### I see no colors
- Check: Is MeshRenderer enabled?
- Check: Does the mesh have valid normals?
- Try: Enable "Recalculate Normals At Runtime"

### Colors are all the same
- Your mesh might be very simple or have uniform normals
- Try: Use a more complex mesh or enable runtime recalculation

### Performance is slow
- Disable "Update Every Frame" if enabled
- Switch to "Use Per Face Normals" = ON
- Reduce mesh triangle count if possible

### Colors look wrong/inverted
- This is normal! The visualizer uses absolute angle
- Up-facing AND down-facing surfaces get the same color
- This is intentional for aerodynamics analysis

## Next Steps

1. ✅ Add the component to your windrad objects
2. ✅ Configure gradient colors in the Inspector
3. ✅ Test with different mesh objects
4. ✅ Integrate with your wind simulation for dynamic updates
5. ✅ Use `RefreshVisualization()` when mesh deforms

Enjoy your aerodynamic visualization! 🌬️💨
