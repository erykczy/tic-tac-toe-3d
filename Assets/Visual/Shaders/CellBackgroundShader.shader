Shader "Unlit/CellBackgroundShader"
{
    Properties
    {
        _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _HighlightColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Highlight("Highlight", Range(0.0, 1.0)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Front
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
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color;
            fixed4 _HighlightColor;
            float _Highlight;
            
            float4 vert (appdata v) : SV_POSITION
            {
                return UnityObjectToClipPos(v.vertex);
            }

            fixed4 frag () : SV_Target
            {
                return lerp(_Color, _HighlightColor, _Highlight);
            }
            ENDCG
        }
    }
}
