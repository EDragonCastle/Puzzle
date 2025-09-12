Shader "Bubble/Sprite Normal"
{
    
    Properties  // Properties Unity Inspector에 조절할 수 있는 변수
    {
        
        _Color ("Tint", Color) = (1,1,1,1)                          // 색상 조절하는 변수. 기본값 : 흰색
        _MainTex ("Sprite Texture", 2D) = "white" {}                // Sprite 기본 Texture. 기본값 : 흰색
        _NormalMap ("Normal Map", 2D) = "bump" {}                   // Sprite Normal Texture. 기본값 : bump
        _NormalStrength ("Normal Strength", Range(0.0, 2.0)) = 1.0  // Normal Map의 강도. 기본값 : 1
        _LightIntensity ("Light Intensity", Range(0.0, 2.0)) = 1.0  // Light 강도. 기본값 : 1
    }

    
    SubShader   // 여러 개의 Rendering 방법을 정의할 수 있다. 여기서는 하나의 SubShader만 사용한다.
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" } // 투명한 object를 Rendering 할 때 사용된다. 

        Blend SrcAlpha OneMinusSrcAlpha // Pixel의 투명도를 처리한다. Blend는 Pixel Color와 Background Color를 섞는 방법을 정의한다. SrcAlpha는 현재 Pixel의 Alpha 값을 OneMinus를 한다는 것이다.
        ZWrite Off                      // Rendering 시 Z-Buffer에 Depth 정보를 기록하지 않는다.
        Cull Off                        // Polygon의 뒷면을 그리지 않는 Culling을 끈다.

        Pass    // Rendering Pass를 정의한다.
        {
            CGPROGRAM 

            #include "UnityCG.cginc"    // Unity에서 제공하는 기본 함수와 변수 모음
            #include "Lighting.cginc"   // Unity에서 제공하는 Light 관련 함수와 변수 모음

            #pragma vertex vert         // vert라는 이름의 함수를 Vertex Shader 사용
            #pragma fragment frag       // frag라는 이름의 함수를 Fragment Shader 사용

            sampler2D _MainTex;         // Properties에 Sampling하기 위한 변수
            float4 _MainTex_ST;         // Offset, Scale 정보를 담고 있는 변수

            sampler2D _NormalMap;       // Normal Texture를 Sampling하기 위한 변수
            float _NormalStrength;      // Normal 강도를 위한 변수
            float _LightIntensity;      // Light 강도를 위한 변수
            float4 _Color;              // Color를 위한 변수

            struct appdata                  // Model의 Vertex로부터 받는 Input Data를 정의하는 구조체
            {
                float4 vertex : POSITION;   // Vertex 위치
                float2 uv : TEXCOORD0;      // Texture 좌표
                fixed4 color : COLOR;       // Vertex Color
                float3 normal : NORMAL;     // Vertex Normal Vector
                float4 tangent : TANGENT;   // Vertex Tangent Vector. -> Normal Map 계산을 위해 필요하다.
            };

  
            struct v2f                              // Vertex Shader에서 Fragment로 넘겨주는 Output Data를 정의하는 구조체
            {
                float4 position : SV_POSITION;      // Clip Space의 Vertex Position
                float2 uv : TEXCOORD0;              // Texture 좌표
                fixed4 color : COLOR;               // Veretx Color

                // normal 정보를 위해 필요한 정보다.
                float3 worldNormal : TEXCOORD1;     // Normal Map 계산을 위해 Vertex Normal -> World Position
                float3 worldTangent : TEXCOORD2;    // Normal Map 계산을 위해 Vertex Tangent -> World Position
                float3 worldBinormal : TEXCOORD3;   // Normal Map 계산을 위해 Vertex BiNoraml -> World Position
                float3 worldPos : TEXCOORD4;        // World Position
            };

            v2f vert (appdata v)                                // Vertex Shader 함수
            {
                v2f o;                                                                  
                o.position = UnityObjectToClipPos(v.vertex);    // Local Space -> Clip Space
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);           // Texture Scale과 Offset을 적용하여 UV 좌표를 계산한다.
                o.color = v.color;                              // Vertex Color를 그대로 전달

                o.worldNormal = UnityObjectToWorldNormal(v.normal);                     // Local Space Normal Vector -> World Space
                o.worldTangent = UnityObjectToWorldDir(v.tangent.xyz);                  // Local Space의 Tangent -> World Space
                o.worldBinormal = cross(o.worldNormal, o.worldTangent) * v.tangent.w;   // World Space Normal과 Tangent를 외적해서 BiNormal Vector 만든다. w는 방향을 보정하는 값

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;    // Local Space의 Vertex Position -> World Position
                return o;
            }

            fixed4 frag (v2f i) : SV_Target                                 // Fragment Shader 함수 i는 Vertex Shader에서 Interpolate된 Data를 받는다.
            {
                fixed4 mainTexColor = tex2D(_MainTex, i.uv);                // Main Texture의 UV 좌표를 사용하여 Pixel Color를 가져온다.
                fixed4 finalColor = mainTexColor * _Color * i.color;        // Texture Color * Inspector Color * v2f Color를 곱한다.

                fixed4 normalMapSample = tex2D(_NormalMap, i.uv);           // Normal Texture를 Sampling 한다.
                float3 tangentSpaceNormal = UnpackNormal(normalMapSample);  // Sampling한 Texture를 Tangent Space의 Normal Vector 변환

                tangentSpaceNormal.xy *= _NormalStrength;                   // Tangent Space Normal Vector에 강도를 곱한다.
                tangentSpaceNormal = normalize(tangentSpaceNormal);         // Normalize를 한다.

                float3 worldNormalFromMap = normalize(                      
                    i.worldTangent * tangentSpaceNormal.x +
                    i.worldBinormal * tangentSpaceNormal.y +
                    i.worldNormal * tangentSpaceNormal.z
                );                                                          // Tangent Normal의 NormalVector를 World Space로 변환한다. Tangent, BiNormal, Normal을 이용해 Rotation Matrix를 만든다.

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);          // Unity의 Direction의 방향을 가져와 Normalize를 한다.
                float diffuse = saturate(dot(worldNormalFromMap, lightDir));    // World Space Normal Vector와 Light Direction Vector를 내적해서 난반사 조명 값을 구한다. saturate를 이용해서 음수가 나오지 않게 한다.

                float3 lightContribution = _LightIntensity;                                 // 빛의 전체적인 강도를 나타낸다.
                float3 lightContribution2 = _LightIntensity * _LightColor0.rgb * diffuse;   // Diffuse와 Direction Light를 곱한다.

                finalColor.rgb *= (lightContribution + lightContribution2); 
                finalColor.a = i.color.a * _Color.a * mainTexColor.a;

                return finalColor;
            }
            ENDCG
        }
    }
}
