Shader "Unlit/AR (Padding)"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainCol ("Color", Color) = (1,1,1,1)
        _Padding ("Padding", Vector) = (0,0,1,1)
    }
    SubShader
    {
        LOD 100
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }


        Pass
        {
            Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 screen_pos: TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _MainCol;
            fixed4 _Padding;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screen_pos = ComputeScreenPos(o.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float2 suv = (i.screen_pos.xy / i.screen_pos.w);
                return (suv.x>_Padding.x && suv.x <_Padding.y && suv.y > _Padding.z && suv.y < _Padding.w) * tex2D(_MainTex, i.uv) * _MainCol;///_ScreenParams;
                // if (pos.x > .25 && pos.x < .5 && pos.y > .25 && pos.y < .75){
                    //     return tex2D(_MainTex, i.uv) * _MainCol;
                    // } else {
                    //     return fixed4(0,0,0,0);
                // }
                // ;
                // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);


                // return col;
            }
            ENDCG
        }
    }
}
