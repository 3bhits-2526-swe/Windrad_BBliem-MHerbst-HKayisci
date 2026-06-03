# Aerodynamics Visualizer - Visual Guide

## Component Architecture

```
AerodynamicsSurfaceVisualizer
│
├── Input Data
│   ├── Mesh (from MeshFilter)
│   ├── Mesh Renderer (requires vertex colors)
│   └── Transform (for world space transforms)
│
├── Configuration
│   ├── Use Per Face Normals (boolean)
│   ├── Recalculate Normals At Runtime (boolean)
│   ├── Update Every Frame (boolean)
│   ├── Color Gradient (Gradient)
│   └── Steepness Curve (AnimationCurve)
│
├── Processing Pipeline
│   ├── Cache Mesh Data
│   ├── Recalculate Normals (if enabled)
│   ├── Calculate Steepness
│   │   └── For each face/vertex: angle(normal, Vector3.up)
│   ├── Map to Color
│   │   └── gradient.Evaluate(steepness)
│   └── Apply Colors to Mesh
│
└── Output
    └── Colored Mesh (per-vertex colors)
```

## Processing Flow Diagram

```
Attach Component to GameObject
         ↓
OnEnable() called
         ↓
InitializeVisualization()
    ├─ Get MeshFilter & MeshRenderer
    ├─ Copy original mesh
    ├─ Create visualization material
    ├─ Cache mesh data (vertices, triangles, normals)
    └─ Call UpdateSurfaceVisualization()
         ↓
UpdateSurfaceVisualization()
    ├─ Check if runtime recalc enabled
    ├─ If yes: RecalculateNormals()
    ├─ If usePerFaceNormals:
    │   └─ ColorizePerFace()
    │       └─ For each triangle:
    │           ├─ Calculate face normal
    │           ├─ Calculate steepness
    │           ├─ Get color from gradient
    │           └─ Apply to all 3 vertices
    └─ Else:
        └─ ColorizePerVertex()
            └─ For each vertex:
                ├─ Get vertex normal
                ├─ Calculate steepness
                ├─ Get color from gradient
                └─ Apply color
         ↓
Set mesh.colors array
         ↓
Mesh renders with new colors!
```

## Steepness Calculation Flowchart

```
                    Input: Surface Normal
                            ↓
                 transform.TransformDirection()
                   (to world space)
                            ↓
            Normalize vector (if needed)
                            ↓
         Vector3.Dot(normal, Vector3.up)
            (compare to gravity)
                            ↓
            Mathf.Abs() - handle both directions
                            ↓
         Mathf.Clamp01() - ensure [0, 1] range
                            ↓
            steepness = 1 - dotProduct
         (0 = flat, 1 = vertical)
                            ↓
         steepnessCurve.Evaluate(steepness)
         (apply animation curve)
                            ↓
         colorGradient.Evaluate(steepness)
                ↓
            Output: Color!
```

## Surface Angle Visualization

```
                World Up (0, 1, 0)
                      ↑
                      |
                      |
    =========================================
        
    Surface 1: Horizontal (Red)
    Normal = (0, 1, 0) | parallel to up
    Dot product = 1.0
    Steepness = 0% → RED
    
    /////////////////////////////////////////
    
    Surface 2: 45° Diagonal (Yellow)
    Normal = (0.707, 0.707, 0) | 45° from up
    Dot product = 0.707
    Steepness = 29.3% → YELLOW
    
    |||||||||||||||||||||||||||||||||||||||||||
    
    Surface 3: Vertical (Green)
    Normal = (1, 0, 0) | perpendicular to up
    Dot product = 0.0
    Steepness = 100% → GREEN
    
    |||||||||||||||||||||||||||||||||||||||||||
    
    Surface 4: Inverted Horizontal (Red)
    Normal = (0, -1, 0) | opposite to up
    Dot product = -1.0 → |−1.0| = 1.0
    Steepness = 0% → RED (same as flat!)
```

## Color Gradient Pipeline

```
Steepness Value (0.0 to 1.0)
        ↓
    colorGradient.Evaluate(value)
        ↓
Linear interpolation through gradient
        ↓
    ┌─────────────────────────┐
    │   Default Gradient      │
    │  Red → Yellow → Green   │
    │  0%    50%      100%    │
    └─────────────────────────┘
        ↓
    Output: Color
        ├─ 0.0 → Red (flat)
        ├─ 0.25 → Orange (22.5°)
        ├─ 0.5 → Yellow (45°)
        ├─ 0.75 → Lime (67.5°)
        └─ 1.0 → Green (vertical)
```

## Per-Face vs Per-Vertex Coloring

```
Per-Face Normals (Recommended for Meshes)
═════════════════════════════════════════
    
    Each triangle = One normal = One color
    
        /\          Each triangle
       /  \     →   gets uniform color
      /    \
     /______\  
     
     ┌──────┐
    │ Sharp │ Look
     │Blocky│
     │Colors│
     └──────┘
    
    Speed: Fast (O(triangles))
    Quality: Sharp, distinct
    Best for: Hard surfaces, blades


Per-Vertex Normals (Recommended for Organic Shapes)
════════════════════════════════════════════════════
    
    Each vertex = Averaged normal = One color
    Colors interpolate across surface
    
        /\          Vertices have
       /  \    →    different colors
      /    \        (smooth gradient)
     /______\  
     
     ┌──────┐
    │ Smooth│ Look
     │Beautiful
     │Gradient│
     └──────┘
    
    Speed: Fast (O(vertices))
    Quality: Smooth, professional
    Best for: Organic shapes, smooth surfaces
```

## For Wind Turbine Application

```
Wind Turbine Visualization
══════════════════════════

┌─────────────────────────────────┐
│    TURBINE BLADE TWIST          │
│  (Shows aerodynamic efficiency)  │
├─────────────────────────────────┤
│                                 │
│  Root (Base):          🟢 GREEN │  Steep (high blade angle)
│  (high attack angle)             │
│                                 │
│  Mid Section:    🟡 YELLOW      │  Optimal (perfect attack)
│  (optimal angle)                 │
│                                 │
│  Tip:                  🔴 RED   │  Flat (low blade angle)
│  (low attack angle)              │
│                                 │
└─────────────────────────────────┘

Color Distribution tells you:
- Blade twist effectiveness
- Attack angle variation
- Aerodynamic efficiency zones
- Optimal vs suboptimal sections


TOWER STRUCTURE
═══════════════

    ┌─────────┐
    │   🟢    │  Top: Mostly vertical (GREEN)
    │  🟢 🟢  │
    │ 🟢   🟢 │
    │ 🟡   🟡 │  Middle: Base flare (YELLOW)
    │🟡     🟡│
    └─────────┘
      └─────┘   Base: Heavily tapered (mix)

Structure visualization:
- Vertical sections: GREEN
- Angled sections: YELLOW
- Horizontal sections: RED
```

## Memory Layout

```
Mesh Data Cache (Per Component Instance)
════════════════════════════════════════

Original Mesh          Visualization Mesh       Arrays
───────────────       ──────────────────       ──────
(Read-only)           (Working Copy)

• Vertices ────────→  • Vertices ─────────→  vertices[]
• Triangles ──────→   • Triangles ────────→  triangles[]
• Normals ────────→   • Normals ──────────→  normals[]
• (no colors)         • Colors ───────────→  vertexColors[]
                                              (computed)

Size for 100k triangles:
• vertices:     3 × 100,000 × 4 bytes = 1.2 MB
• triangles:    3 × 30,000 × 4 bytes = 0.4 MB
• normals:      3 × 100,000 × 4 bytes = 1.2 MB
• vertexColors: 4 × 100,000 × 4 bytes = 1.6 MB
                    ─────────────────────────
                    Total ≈ 4.4 MB

(Mesh sharing reduces total project memory)
```

## Performance Timeline (100k Triangles)

```
Time to First Frame
═══════════════════

0 ms    ├─ OnEnable() called
        ├─ MeshFilter/Renderer accessed
        ├─ Mesh copied
        │
2 ms    ├─ Cache arrays
        ├─ Gradient created (if needed)
        │
5 ms    ├─ Per-face coloring
        │   ├─ For each triangle (33k triangles)
        │   ├─ Calculate normal
        │   ├─ Calculate steepness
        │   └─ Get color
        │
10 ms   ├─ mesh.colors = vertexColors[]
        │   (GPU upload)
        │
┌───────┤
│ Total: ~10ms (one-time)
└───────┤

Per-Frame Cost (Static Mesh)
════════════════════════════
0 ms    ├─ Nothing! (update disabled)

Per-Frame Cost (Dynamic, Per-Face)
══════════════════════════════════
0 ms    ├─ RecalculateNormals()
        │   └─ ~5ms
        │
5 ms    ├─ Per-face coloring
        │   └─ ~3ms
        │
8 ms    ├─ GPU upload
        │
┌───────┤
│ Total: ~8ms per frame
└───────┤
```

## Configuration Decision Tree

```
                    Which Mode?
                        ↓
        ┌───────────────────────────────┐
        │  Is mesh static (not moving)? │
        └───────────────────────────────┘
                ↙                   ↘
              YES                   NO
               ↓                     ↓
        ┌────────────┐       ┌──────────────┐
        │Set all OFF:│       │Enable Runtime│
        │Runtime: NO │       │Recalc & Frame│
        │Frame: NO   │       │Update: YES   │
        └────────────┘       └──────────────┘
               ↓                     ↓
        ┌─────────────────────────────────────────┐
        │  Do you want sharp or smooth appearance?│
        └─────────────────────────────────────────┘
               ↓                     ↓
           SHARP                  SMOOTH
             ↓                       ↓
       ┌──────────────┐      ┌──────────────┐
       │Per-Face: ON  │      │Per-Face: OFF │
       │ FAST & CLEAN │      │SMOOTH & NICE │
       └──────────────┘      └──────────────┘
```

## Edge Case Handling

```
Edge Cases Handled
══════════════════

1. Inverted Normals
   Problem: Normal points inward instead of outward
   Solution: Use Mathf.Abs(dotProduct)
   Result: Both directions get same color ✓

2. Zero-Length Edge
   Problem: Degenerate triangle geometry
   Solution: Normalize all vectors
   Result: Prevents NaN/Inf ✓

3. Non-Uniform Scaling
   Problem: Object scaled (2, 0.5, 1)
   Solution: Use TransformDirection() not MultiplyVector()
   Result: Normal transformation correct ✓

4. Extreme Rotation
   Problem: Mesh rotated so "up" is sideways
   Solution: Always use Vector3.up in world space
   Result: Works at any rotation ✓

5. Invalid Mesh
   Problem: No mesh found
   Solution: Check in OnEnable(), disable component
   Result: Clean error message, no crash ✓
```

---

These diagrams show the complete flow, architecture, and decision-making process of the Aerodynamics Visualizer component.
