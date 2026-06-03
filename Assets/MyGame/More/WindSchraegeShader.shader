Shader "Custom/WindSchraegeShader_Interaktiv"
{
    Properties
    {
        // Schieberegler für die Rotation des Windes in Grad
        _WindAngleHorizontal ("Wind Drehung Horizontal (Y-Achse)", Range(0, 360)) = 180
        _WindAngleVertical ("Wind Drehung Vertikal (X-Achse)", Range(-90, 90)) = 0

        _Sensitivity ("Farb-Sensitivität (Kontrast)", Range(0.5, 5.0)) = 2.0

        _ColorLow ("Wenig Drag (Blau)", Color) = (0, 0, 1, 1)
        _ColorMid ("Mittlerer Drag (Gruen)", Color) = (0, 1, 0, 1)
        _ColorHigh ("Hoher Drag (Rot)", Color) = (1, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
            };

            float _WindAngleHorizontal;
            float _WindAngleVertical;
            float _Sensitivity;
            fixed4 _ColorLow;
            fixed4 _ColorMid;
            fixed4 _ColorHigh;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 normal = normalize(i.worldNormal);

                // --- WINDVEKTOR AUS GRADZAHLEN BERECHNEN ---
                // Umrechnung von Grad in Bogenmaß (Radians)
                float radH = _WindAngleHorizontal * 0.0174532925; // DEG2RAD
                float radV = _WindAngleVertical * 0.0174532925;

                // Mathematische Rotation im 3D-Raum (Kugelkoordinaten)
                float3 windDir;
                windDir.x = sin(radH) * cos(radV);
                windDir.y = sin(radV);
                windDir.z = cos(radH) * cos(radV);
                windDir = normalize(windDir);

                // Berechnung des Prall-Winkels (Dot Product)
                // Da wir sehen wollen, WO der Wind auftrifft, invertieren wir die Windrichtung
                float dotProduct = dot(normal, -windDir);

                // Normalisierung des Werts für die Farbrampe
                float t = saturate(dotProduct * _Sensitivity);

                // Farbverlauf berechnen (Blau -> Grün -> Rot)
                fixed4 finalColor;
                if (t < 0.5)
                {
                    finalColor = lerp(_ColorLow, _ColorMid, t * 2.0);
                }
                else
                {
                    finalColor = lerp(_ColorMid, _ColorHigh, (t - 0.5) * 2.0);
                }

                return finalColor;
            }
            ENDCG
        }
    }
}
