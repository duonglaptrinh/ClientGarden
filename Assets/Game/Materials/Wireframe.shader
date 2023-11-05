// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// https://answers.unity.com/questions/727376/modifying-wireframe-vertex-shader-to-include-light.html
Shader "Custom/Wireframe"
{
    Properties
    {
        _LineColor("Line Color", Color) = (1,1,1,1)
        _GridColor("Grid Color", Color) = (1,1,1,0)
        _LineWidth("Line Width", float) = 0.2
        //_FillRate("Fill Grid Rate", Range(0.0, 1.0)) = 0.0
    }

        SubShader
    {
        Tags { "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        AlphaTest Greater 0.5

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float4 _LineColor;
            uniform float4 _GridColor;
            uniform float _LineWidth;
            //uniform float _FillRate;

            // vertex input: position, uv1, uv2
            struct appdata
            {
                float4 vertex : POSITION;
                float4 texcoord1 : TEXCOORD0;
                float4 color : COLOR;
                float3 normal: NORMAL;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 texcoord1 : TEXCOORD0;
                float4 color : COLOR;
                half3 worldRefl : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldRefl = reflect(-worldViewDir, worldNormal);
                o.texcoord1 = v.texcoord1;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                // sample the default reflection cubemap, using the reflection vector
                half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, i.worldRefl);
                // decode cubemap data into actual color
                half3 skyColor = DecodeHDR(skyData, unity_SpecCube0_HDR);

                fixed4 answer;
                float lx = step(_LineWidth, i.texcoord1.x);
                float ly = step(_LineWidth, i.texcoord1.y);
                float hx = step(i.texcoord1.x, 1.0 - _LineWidth);
                float hy = step(i.texcoord1.y, 1.0 - _LineWidth);
                answer = lerp(_LineColor, _GridColor, lx * ly * hx * hy);

                answer *= half4(skyColor, 1);

                return answer;
            }
            ENDCG
        }

        //Pass 
        //{
        //    CGPROGRAM
        //    #pragma vertex vert
        //    #pragma fragment frag

        //    struct v2f
        //    {
        //        float4 texcoord : TEXCOORD0;
        //        float4 pos : POSITION;
        //    };

        //    v2f vert(float4 vertex : POSITION, float4 texcoord : TEXCOORD0)
        //    {
        //        v2f o;
        //        o.pos = UnityObjectToClipPos(vertex);
        //        o.texcoord = texcoord;
        //        return o;
        //    }

        //    fixed4 frag(v2f i) : COLOR
        //    {
        //        
        //        return half4(0, 0, .5, 1);
        //    }
        //    
        //    ENDCG
        //}
    }
        Fallback "Vertex Colored", 1
}