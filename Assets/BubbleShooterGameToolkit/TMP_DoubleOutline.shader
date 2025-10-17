Shader "TextMeshPro/Distance Field Double Outline Inward"
{
    Properties
    {
        _MainTex("Font Atlas", 2D) = "white" {}
        _FaceColor("Face Color", Color) = (1,1,1,1)

        _InnerOutlineColor("Inner Outline Color", Color) = (0,0,0,1)
        _InnerOutlineWidth("Inner Outline Width", Range(0, 1)) = 0.05

        _OuterOutlineColor("Outer Outline Color", Color) = (1,0,0,1)
        _OuterOutlineWidth("Outer Outline Width", Range(0, 1)) = 0.15

        _Softness("Softness", Range(0, 0.1)) = 0.01
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _FaceColor;
            float4 _InnerOutlineColor;
            float _InnerOutlineWidth;
            float4 _OuterOutlineColor;
            float _OuterOutlineWidth;
            float _Softness;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR; // TMP color
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float sdf = tex2D(_MainTex, i.uv).a;
                float edge = 0.5;
                float smoothing = _Softness;

                float outerStart = edge - _OuterOutlineWidth;
                float alphaOuter = smoothstep(outerStart - smoothing, outerStart + smoothing, sdf);

                float innerEnd = edge + _InnerOutlineWidth;
                float alphaInner = 1.0 - smoothstep(innerEnd - smoothing, innerEnd + smoothing, sdf);

                float alphaFace = smoothstep(edge - smoothing, edge + smoothing, sdf);

                float3 color;
                float alpha;

                float4 finalFaceColor = _FaceColor * i.color;

                if (sdf < edge - _OuterOutlineWidth)
                {
                    return float4(0, 0, 0, 0);
                }
                else if (sdf < edge)
                {
                    color = _OuterOutlineColor.rgb;
                    alpha = alphaOuter;
                }
                else if (sdf < edge + _InnerOutlineWidth)
                {
                    color = _InnerOutlineColor.rgb;
                    alpha = alphaInner;
                }
                else
                {
                    color = finalFaceColor.rgb;
                    alpha = alphaFace * finalFaceColor.a;
                }

                return float4(color, alpha);
            }
            ENDCG
        }
    }
}
