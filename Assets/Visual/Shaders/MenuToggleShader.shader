Shader "Unlit/MenuButtonShader"
{
    Properties
    {
        _ColorOn("Color On", Color) = (1.0, 1.0, 1.0, 1.0)
        _HighlightColorOn("Highlight Color On", Color) = (1.0, 1.0, 1.0, 1.0)
        _ColorOff("Color Off", Color) = (1.0, 1.0, 1.0, 1.0)
        _HighlightColorOff("Highlight Color Off", Color) = (1.0, 1.0, 1.0, 1.0)
        _Highlight("Highlight", Range(0.0, 1.0)) = 0.0
        _Toggled("Toggled", Range(0.0, 1.0)) = 0.0
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

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            fixed4 _ColorOff;
            fixed4 _HighlightColorOff;
            fixed4 _ColorOn;
            fixed4 _HighlightColorOn;
            float _Highlight;
            float _Toggled;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return lerp(lerp(_ColorOff, _ColorOn, _Toggled), lerp(_HighlightColorOff, _HighlightColorOn, _Toggled), _Highlight);
            }
            ENDCG
        }
    }
}
