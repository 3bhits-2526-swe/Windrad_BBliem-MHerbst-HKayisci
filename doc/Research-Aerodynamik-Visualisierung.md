# Aerodynamische Drag-Visualisierung in Unity – Kurzfassung

> **CFD** = **Computational Fluid Dynamics** (Numerische Strömungsmechanik)

---

## 1. Workflow

### Dateneinspeisung – Zwei Optionen

**Option A: Statischer CFD-Import via Vertex Colors**

- CFD-Werte normalisieren: `t = (Drag + 10) / 20` → Rot-Kanal
- FBX exportieren mit Vertex Colors
- Unity: Index Format auf 32-Bit für dichte Meshes (>65.535 Vertices)

**Option B: Echtzeit-Schrägen-Approximation**

- Drag aus Oberflächenschräge berechnen: Normal·(-WindDir)
- Keine externen Daten nötig, interaktiv justierbar

---

## 2. Implementierungsdetails

### Render-Pipeline

**URP** → Shader Graph, Performance bei CFD-Meshes, ausreichend für Visualisierung.

### Farbverlauf-Mapping (HLSL)

```hlsl
Shader "Custom/WindDragShader"
{
    Properties
    {
        _WindAngleHorizontal ("Wind Horizontal", Range(0, 360)) = 180
        _WindAngleVertical ("Wind Vertikal", Range(-90, 90)) = 0
        _Sensitivity ("Sensitivität", Range(0.5, 5.0)) = 2.0
        _ColorLow ("Blau", Color) = (0, 0, 1, 1)
        _ColorMid ("Grün", Color) = (0, 1, 0, 1)
        _ColorHigh ("Rot", Color) = (1, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; };
            struct v2f { float4 vertex : SV_POSITION; float3 worldNormal : TEXCOORD0; };

            float _WindAngleHorizontal, _WindAngleVertical, _Sensitivity;
            fixed4 _ColorLow, _ColorMid, _ColorHigh;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 normal = normalize(i.worldNormal);
                float radH = _WindAngleHorizontal * 0.0174532925;
                float radV = _WindAngleVertical * 0.0174532925;
                
                float3 windDir = float3(sin(radH) * cos(radV), sin(radV), cos(radH) * cos(radV));
                float dotProd = saturate(dot(normal, -normalize(windDir)) * _Sensitivity);
                
                fixed4 color = (dotProd < 0.5) 
                    ? lerp(_ColorLow, _ColorMid, dotProd * 2.0)
                    : lerp(_ColorMid, _ColorHigh, (dotProd - 0.5) * 2.0);
                
                return color;
            }
            ENDCG
        }
    }
}
```


---

## 3. Entscheidungsbegründungen

### Warum URP statt HDRP?

Ausreichend für analytische Visualisierung, kein Overhead durch komplexe physikalische Rendering-Features nötig. VFX Graph & Shader Graph Support vorhanden.

### Warum Echtzeit-Schrägen-Berechnung (Option B)?

**Pragmatismus:** Einfachste Implementierung — keine externen CFD-Daten nötig, keine Datenaufbereitung, keine Mesh-Konvertierung. Die Normal·WindDir-Approximation ist visuell ausreichend und sofort interaktiv justierbar. Echte CFD-Daten (Option A) nur nötig, falls exakte Messwerte erforderlich sind.

### Performance bei hochpoly CFD-Meshes

32-Bit Index Format für >65.535 Vertices zwingend erforderlich. Echtzeit-Berechnung günstiger als Vertex-Color-Import bei dynamischen Windrichtungen.