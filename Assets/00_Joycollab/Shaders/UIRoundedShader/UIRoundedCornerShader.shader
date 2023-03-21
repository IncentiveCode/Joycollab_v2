Shader "UI/RoundedMask" {
    Properties {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}

        // --- Mask support ---
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
        [HideInInspector] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        
        _WidthHeightRadius ("WidthHeightRadius", Vector) = (100,100,30,0)
    }
    
    SubShader
    {
        Tags {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        // --- Mask support ---
        Stencil {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        Cull Off
        Lighting Off
        ZTest [unity_GUIZTestMode]
        ColorMask [_ColorMask]
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        

        Pass {
            CGPROGRAM
            
            #include "UnityCG.cginc"
            
            #pragma vertex vert
            #pragma fragment frag

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            inline fixed4 mixAlpha(fixed4 mainTexColor, fixed4 color, float sdfAlpha){
                fixed4 col = mainTexColor * color;
                col.a = min(col.a, sdfAlpha);
                return col;
            }

            float rectangle(float2 samplePosition, float2 halfSize){
                float2 distanceToEdge = abs(samplePosition) - halfSize;
                float outsideDistance = length(max(distanceToEdge, 0));
                float insideDistance = min(max(distanceToEdge.x, distanceToEdge.y), 0);
                return outsideDistance + insideDistance;
            }

            float roundedRectangle(float2 samplePosition, float absoluteRound, float2 halfSize){ 

                return rectangle(samplePosition, halfSize - absoluteRound) - absoluteRound;
            }

            float AntialiasedCutoff(float distance){
                float distanceChange = fwidth(distance) * 0.5;
                return smoothstep(distanceChange, -distanceChange, distance);
            }

            float CalcAlpha(float2 samplePosition, float2 size, float radius){
                float2 samplePositionTranslated = (samplePosition - .5) * size;
                float distToRect = roundedRectangle(samplePositionTranslated, radius * .5, size * .5);
                return AntialiasedCutoff(distToRect);
            }

            inline float2 translate(float2 samplePosition, float2 offset){
                return samplePosition - offset;
            }

            float intersect(float shape1, float shape2){
                return max(shape1, shape2);
            }

            float2 rotate(float2 samplePosition, float rotation){
                const float PI = 3.14159;
                float angle = rotation * PI * 2 * -1;
                float sine, cosine;
                sincos(angle, sine, cosine);
                return float2(cosine * samplePosition.x + sine * samplePosition.y, cosine * samplePosition.y - sine * samplePosition.x);
            }

            float circle(float2 position, float radius){
                return length(position) - radius;
            }

            float CalcAlphaForIndependentCorners(float2 samplePosition, float2 halfSize, float4 rect2props, float4 r){

                samplePosition = (samplePosition - .5) * halfSize * 2;

                float r1 = rectangle(samplePosition, halfSize);
                
                float2 r2Position = rotate(translate(samplePosition, rect2props.xy), .125);
                float r2 = rectangle(r2Position, rect2props.zw);
    
                float2 circle0Position = translate(samplePosition, float2(-halfSize.x + r.x, halfSize.y - r.x));
                float c0 = circle(circle0Position, r.x);
    
                float2 circle1Position = translate(samplePosition, float2(halfSize.x - r.y, halfSize.y - r.y));
                float c1 = circle(circle1Position, r.y);
    
                float2 circle2Position = translate(samplePosition, float2(halfSize.x - r.z, -halfSize.y + r.z));
                float c2 = circle(circle2Position, r.z);
    
                float2 circle3Position = translate(samplePosition, -halfSize + r.w);
                float c3 = circle(circle3Position, r.w);
        
                float dist = max(r1,min(min(min(min(r2, c0), c1), c2), c3));
                return AntialiasedCutoff(dist);
            }
            
            float4 _WidthHeightRadius;
            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target {
                float alpha = CalcAlpha(i.uv, _WidthHeightRadius.xy, _WidthHeightRadius.z);
                return mixAlpha(tex2D(_MainTex, i.uv), i.color, alpha);
            }
            ENDCG
        }
    }
}