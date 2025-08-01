Shader "Bubble/Sprite Normal"
{
    Properties
    {
        _Color ("Tint", Color) = (1,1,1,1) // 기본 색상을 흰색으로 변경
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {} // 노멀 맵 텍스처
        _NormalStrength ("Normal Strength", Range(0.0, 2.0)) = 1.0 // 노멀 맵 강도 조절
        _LightIntensity ("Light Intensity", Range(0.0, 2.0)) = 1.0 // 전체적인 빛 강도 조절
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" } // 투명 렌더링 설정

        Blend SrcAlpha OneMinusSrcAlpha 
        ZWrite Off                     
        Cull Off                       

        Pass
        {
            CGPROGRAM

            #include "UnityCG.cginc" // Unity 기본 유틸리티 함수 포함
            #include "Lighting.cginc" // 광원 관련 유틸리티 함수 포함 (예: _LightColor0)

            #pragma vertex vert
            #pragma fragment frag

            // 쉐이더 프로퍼티 변수 선언
            sampler2D _MainTex;
            float4 _MainTex_ST; 

            sampler2D _NormalMap;
            float _NormalStrength;
            float _LightIntensity;
            float4 _Color;

            // 앱 데이터 (메쉬에서 버텍스 쉐이더로 입력)
            struct appdata
            {
                float4 vertex : POSITION;   // 버텍스 위치
                float2 uv : TEXCOORD0;      // UV 좌표
                float3 normal : NORMAL;     // 버텍스 노멀
                float4 tangent : TANGENT;   // 버텍스 탄젠트 (노멀 맵에 필수)
                fixed4 color : COLOR;      // 버텍스 색상
            };

            // 버텍스 쉐이더에서 프래그먼트 쉐이더로 출력
            struct v2f
            {
                float4 position : SV_POSITION;  // 클립 공간 위치
                float2 uv : TEXCOORD0;          // UV 좌표
                fixed4 color : COLOR;           // 버텍스 색상
                float3 worldNormal : TEXCOORD1; // 월드 공간 노멀 (베이시스 변환용)
                float3 worldTangent : TEXCOORD2; // 월드 공간 탄젠트 (베이시스 변환용)
                float3 worldBinormal : TEXCOORD3; // 월드 공간 바이노멀 (베이시스 변환용)
                float3 worldPos : TEXCOORD4;    // 월드 공간 위치 (광원 계산에 사용)
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex); // 오브젝트 공간 -> 클립 공간 변환
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);       // UV 변환

                o.color = v.color; // 버텍스 컬러 전달

                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                o.worldBinormal = cross(o.worldNormal, o.worldTangent) * v.tangent.w;

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // 월드 공간 위치
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 1. 메인 텍스처 및 색상 적용
                fixed4 mainTexColor = tex2D(_MainTex, i.uv);
                fixed4 finalColor = mainTexColor * _Color * i.color;

                // 2. 노멀 맵 샘플링 및 언팩 (탄젠트 공간 노멀 얻기)
                fixed4 normalMapSample = tex2D(_NormalMap, i.uv);
                float3 tangentSpaceNormal = UnpackNormal(normalMapSample);

                // 3. 노멀 맵 강도 적용 및 정규화
                tangentSpaceNormal.xy *= _NormalStrength;
                tangentSpaceNormal = normalize(tangentSpaceNormal);

                // 4. 탄젠트 공간 노멀을 월드 공간 노멀로 변환
                // 버텍스 쉐이더에서 계산한 베이시스 벡터(월드 노멀, 탄젠트, 바이노멀)를 사용
                float3 worldNormalFromMap = normalize(
                    i.worldTangent * tangentSpaceNormal.x +
                    i.worldBinormal * tangentSpaceNormal.y +
                    i.worldNormal * tangentSpaceNormal.z
                );

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz); // 방향성 광원의 방향
                
                // saturate는 값을 0~1 사이로 제한하여 음수 값이 되지 않도록 합니다.
                float diffuse = saturate(dot(worldNormalFromMap, lightDir));

                // 빛의 색상과 강도를 적용
                float3 lightContribution = _LightIntensity;
                float3 lightContribution2 = _LightIntensity * _LightColor0.rgb * diffuse;

                // 6. 최종 색상에 빛의 영향 적용
                finalColor.rgb *= (lightContribution + lightContribution2); // 메인 색상에 빛의 영향을 곱함
                finalColor.a = i.color.a * _Color.a * mainTexColor.a;
                return finalColor;
            }
            ENDCG
        }
    }
}
