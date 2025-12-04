Shader "Unlit/SkyboxShader"
{
    Properties
    {
        _ColorUpTop("Color Up Top", Color) = (0.0, 1.0, 0.0, 1.0)
        _ColorUpBottom("Color Up Bottom", Color) = (0.0, 1.0, 1.0, 1.0)
        _ColorDownTop("Color Down Top", Color) = (0.0, 0.0, 0.0, 1.0)
        _ColorDownBottom("Color Down Bottom", Color) = (0.0, 0.0, 0.0, 1.0)
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
                float3 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 texcoord : TEXCOORD0;
            };

            fixed4 _ColorUpTop;
            fixed4 _ColorUpBottom;
            fixed4 _ColorDownTop;
            fixed4 _ColorDownBottom;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 cameraDir = unity_WorldToCamera[2];
                float cameraFactor = (cameraDir.y + 1.0) / 2.0;
                fixed4 bottom = lerp(_ColorDownBottom, _ColorUpBottom, cameraFactor);
                fixed4 top = lerp(_ColorDownTop, _ColorUpTop, cameraFactor);
                return lerp(bottom, top, i.texcoord.y);
            }
            ENDCG
        }
    }
}
