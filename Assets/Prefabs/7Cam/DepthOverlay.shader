Shader "Unlit/DepthOverlay"
{
    Properties
    {
        _Color ("Texture", 2D) = "white" {}
        _DepthFG ("Texture", 2D) = "white" {}
        _DepthBG ("Texture", 2D) = "white" {}
    }
    SubShader
    {

        Tags {"IgnoreProjector"="True" "RenderType"="Transparent"}
        // ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        // Cull front 
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _Color;
            sampler2D_float  _DepthFG;
            sampler2D_float  _DepthBG;
            float4 _Color_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Color);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_Color, i.uv);
                float4 dFG = tex2D(_DepthFG, i.uv);
                float4 dBG = tex2D(_DepthBG, i.uv);
                //return dBG;
                if (dFG.r > dBG.r){
                    return col;
                } else {
                    return fixed4(0,0,0,0);
                }
            }
            ENDCG
        }
    }
}
