Shader "Sprite/Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LineWidth ("Line Width", Range(0.0, 10)) = 1
        _LineColor ("Line Color", Color) = (1, 1, 1, 1)
        _AlphaValue ("Alpha Value", Range(0.0, 1.0)) = 0.1
    }
    
    SubShader
    {
        Tags
        {
            queue = Transparent
        }
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert ( a2v v )
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _LineWidth;
            float4 _LineColor;
            float _AlphaValue;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col=tex2D(_MainTex, i.uv);

                float2 up_uv = i.uv + float2(0, _LineWidth * _MainTex_TexelSize.y);
                float2 down_uv = i.uv - float2(0, _LineWidth * _MainTex_TexelSize.y);
                float2 left_uv = i.uv - float2(_LineWidth * _MainTex_TexelSize.x, 0);
                float2 right_uv = i.uv + float2(_LineWidth * _MainTex_TexelSize.x, 0);

                float w=tex2D(_MainTex, up_uv).a+tex2D(_MainTex, down_uv).a+tex2D(_MainTex, left_uv).a+tex2D(_MainTex, right_uv).a;

                if(col.a<_AlphaValue)
                {
                    return step(_AlphaValue, w) * _LineColor;
                }
                else
                {
                    return col;
                }
            }
            
            
            ENDCG
        }
    }
}