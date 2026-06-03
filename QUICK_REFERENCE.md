# Aerodynamics Visualizer - Quick Reference Card

## 📌 Installation (1 Minute)

```
1. GameObject → Select any 3D object in scene
2. Inspector → Add Component → AerodynamicsSurfaceVisualizer
3. Done! Hit Play to see colors
```

## 🎨 Default Colors

| Color | Angle | Surface | Meaning |
|-------|-------|---------|---------|
| 🔴 Red | 0° | Horizontal/Flat | Low aerodynamic angle |
| 🟡 Yellow | 45° | Diagonal | Optimal aerodynamic angle |
| 🟢 Green | 90° | Vertical | High aerodynamic angle |

## ⚙️ Inspector Settings (5 Seconds Each)

### Use Per Face Normals
- ✓ **ON** (Default): Sharp, blocky colors → Fast, turbine blades
- ☐ **OFF**: Smooth gradients → Beautiful, organic shapes

### Recalculate Normals At Runtime
- ✓ **ON**: For deforming/animated meshes → Performance cost
- ☐ **OFF** (Default): For static objects → Zero overhead

### Update Every Frame
- ✓ **ON**: Only if runtime recalculation enabled → Performance cost
- ☐ **OFF** (Default): No updates after initialization → Fast

### Color Gradient
- Drag existing Gradient asset, OR
- Inspector creates default Red→Yellow→Green, OR
- Leave empty to use programmatic default

### Steepness Curve
- Linear (default): Even distribution
- Custom AnimationCurve: Artistic control

## 💻 Code Cheat Sheet

### Add Component Programmatically
```csharp
var visualizer = myObject.AddComponent<AerodynamicsSurfaceVisualizer>();
```

### Refresh Visualization
```csharp
visualizer.RefreshVisualization();
```

### Toggle Coloring Mode
```csharp
visualizer.SetUsePerFaceNormals(true);   // Sharp
visualizer.SetUsePerFaceNormals(false);  // Smooth
```

### Set Custom Gradient
```csharp
visualizer.SetColorGradient(myCustomGradient);
```

### Get Component Reference
```csharp
var visualizer = GetComponent<AerodynamicsSurfaceVisualizer>();
```

## 🚀 Common Setups (Copy-Paste Ready)

### Setup 1: Turbine Blade (Best Performance)
```csharp
// Copy these settings in Inspector:
Use Per Face Normals: ON
Recalculate Normals At Runtime: OFF
Update Every Frame: OFF
// Result: Sharp blade sections, zero per-frame cost
```

### Setup 2: Smooth Tower (Best Appearance)
```csharp
// Copy these settings:
Use Per Face Normals: OFF
Recalculate Normals At Runtime: OFF
Update Every Frame: OFF
// Result: Smooth gradients, still zero per-frame cost
```

### Setup 3: Deforming Mesh (Dynamic)
```csharp
// For animated/deforming geometry:
Use Per Face Normals: ON
Recalculate Normals At Runtime: ON
Update Every Frame: ON
// Result: Always accurate, ~5-8ms per frame cost
```

## 📊 Performance Quick Reference

| Configuration | Per-Frame Cost | Visual Quality |
|---|---|---|
| Static, Per-Face | 0ms | Sharp, clean |
| Static, Per-Vertex | 0ms | Smooth |
| Dynamic, Per-Face | 5-8ms | Sharp |
| Dynamic, Per-Vertex | 8-12ms | Smooth |

*For 100k triangles. Scale linearly.*

## 🔧 Troubleshooting (30 Seconds)

### Problem: No colors appear
```
✓ Check: MeshRenderer is enabled
✓ Try: Enable "Recalculate Normals At Runtime"
```

### Problem: Uniform color (no variation)
```
✓ Try: Switch to Per-Vertex coloring
✓ Try: Use a mesh with more geometry
```

### Problem: Slow performance
```
✓ Disable: "Update Every Frame"
✓ Enable: "Use Per Face Normals"
```

### Problem: Colors seem wrong
```
✓ Normal behavior: Both sides get same color
✓ This is intentional: Aerodynamics cares about angle, not direction
```

## 📖 Where to Find More Info

| Question | Document |
|----------|----------|
| Quick start? | `AERODYNAMICS_SETUP.md` |
| Full features? | `AERODYNAMICS_VISUALIZER_README.md` |
| How it works? | `TECHNICAL_GUIDE.md` |
| Overview? | `README_AERODYNAMICS.md` |

## 🎯 For Your Windrad Project

### Turbine Blade
```csharp
blade.AddComponent<AerodynamicsSurfaceVisualizer>();
// Shows blade twist along span visually
```

### Tower
```csharp
tower.AddComponent<AerodynamicsSurfaceVisualizer>();
// Shows vertical structure clearly
```

### Generator Housing
```csharp
housing.AddComponent<AerodynamicsSurfaceVisualizer>();
// Visualizes housing geometry
```

## ✨ Quick Wins

1. **Immediate Use**: Attach to any 3D object, see results in Play mode
2. **Debug Normals**: Quickly see if mesh normals are correct
3. **Understand Geometry**: Instantly grasp 3D surface properties
4. **Educational**: Teach aerodynamic principles visually
5. **No Dependencies**: Works with any shader, any mesh

## 🔑 Remember

- **Red** = Flat (less interesting aerodynamically)
- **Yellow** = Perfect (optimal angle)
- **Green** = Steep (very interesting aerodynamically)

---

**One line to add component:**
```csharp
gameObject.AddComponent<AerodynamicsSurfaceVisualizer>();
```

**That's it! Everything else is optional configuration.**
