Shader "Bubble/Bubble Element"
{
    Properties  // Properties Unity Inspector�� ������ �� �ִ� ����
    {
        [Header(Sprite Normal Setting)]
        _Color ("Tint", Color) = (1,1,1,1)                          // ���� �����ϴ� ����. �⺻�� : ���
        _MainTex ("Sprite Texture", 2D) = "white" {}                // Sprite �⺻ Texture. �⺻�� : ���
        _NormalMap ("Normal Map", 2D) = "bump" {}                   // Sprite Normal Texture. �⺻�� : bump
        _NormalStrength ("Normal Strength", Range(0.0, 2.0)) = 1.0  // Normal Map�� ����. �⺻�� : 1
        _LightIntensity ("Light Intensity", Range(0.0, 2.0)) = 1.0  // Light ����. �⺻�� : 1

        [Space(10)]

        [Header(Outline Setting)]
        _OutlineColor ("Sprite Outline Color", Color) = (1,1,1,1)
        _OutlineAnimationSpeed ("Outline Speed", Range(0.1, 10)) = 1
        _OutlineMode ("Outline Mode", float) = 0.0
    }
    
    SubShader   // ���� ���� Rendering ����� ������ �� �ִ�. ���⼭�� �ϳ��� SubShader�� ����Ѵ�.
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" } // ������ object�� Rendering �� �� ���ȴ�. 

        Blend SrcAlpha OneMinusSrcAlpha // Pixel�� ������ ó���Ѵ�. Blend�� Pixel Color�� Background Color�� ���� ����� �����Ѵ�. SrcAlpha�� ���� Pixel�� Alpha ���� OneMinus�� �Ѵٴ� ���̴�.
        ZWrite Off                      // Rendering �� Z-Buffer�� Depth ������ ������� �ʴ´�.
        Cull Off                        // Polygon�� �޸��� �׸��� �ʴ� Culling�� ����.
        

        // Sprite Normal Pass
        Pass    // Rendering Pass�� �����Ѵ�.
        {
            CGPROGRAM 

            #include "UnityCG.cginc"    // Unity���� �����ϴ� �⺻ �Լ��� ���� ����
            #include "Lighting.cginc"   // Unity���� �����ϴ� Light ���� �Լ��� ���� ����

            #pragma shader_feature _IS_ENABLE_OUTLINE 
            float _OutlineAnimationSpeed;           // Animation Speed
            float4 _OutlineColor;                   // Outline Color
            float _OutlineMode;

            #pragma vertex vert         // vert��� �̸��� �Լ��� Vertex Shader ���
            #pragma fragment frag       // frag��� �̸��� �Լ��� Fragment Shader ���

            sampler2D _MainTex;         // Properties�� Sampling�ϱ� ���� ����
            float4 _MainTex_TexelSize;
            float4 _MainTex_ST;         // Offset, Scale ������ ��� �ִ� ����

            sampler2D _NormalMap;       // Normal Texture�� Sampling�ϱ� ���� ����
            float _NormalStrength;      // Normal ������ ���� ����
            float _LightIntensity;      // Light ������ ���� ����
            float4 _Color;              // Color�� ���� ����

            struct appdata                  // Model�� Vertex�κ��� �޴� Input Data�� �����ϴ� ����ü
            {
                float4 vertex : POSITION;   // Vertex ��ġ
                float2 uv : TEXCOORD0;      // Texture ��ǥ
                fixed4 color : COLOR;       // Vertex Color
                float3 normal : NORMAL;     // Vertex Normal Vector
                float4 tangent : TANGENT;   // Vertex Tangent Vector. -> Normal Map ����� ���� �ʿ��ϴ�.
            };

  
            struct v2f                              // Vertex Shader���� Fragment�� �Ѱ��ִ� Output Data�� �����ϴ� ����ü
            {
                float4 position : SV_POSITION;      // Clip Space�� Vertex Position
                float2 uv : TEXCOORD0;              // Texture ��ǥ
                fixed4 color : COLOR;               // Veretx Color

                // normal ������ ���� �ʿ��� ������.
                float3 worldNormal : TEXCOORD1;     // Normal Map ����� ���� Vertex Normal -> World Position
                float3 worldTangent : TEXCOORD2;    // Normal Map ����� ���� Vertex Tangent -> World Position
                float3 worldBinormal : TEXCOORD3;   // Normal Map ����� ���� Vertex BiNoraml -> World Position
                float3 worldPos : TEXCOORD4;        // World Position
            };

            v2f vert (appdata v)                                // Vertex Shader �Լ�
            {
                v2f o;                                                                  
                o.position = UnityObjectToClipPos(v.vertex);    // Local Space -> Clip Space
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);           // Texture Scale�� Offset�� �����Ͽ� UV ��ǥ�� ����Ѵ�.
                o.color = v.color;                              // Vertex Color�� �״�� ����

                o.worldNormal = UnityObjectToWorldNormal(v.normal);                     // Local Space Normal Vector -> World Space
                o.worldTangent = UnityObjectToWorldDir(v.tangent.xyz);                  // Local Space�� Tangent -> World Space
                o.worldBinormal = cross(o.worldNormal, o.worldTangent) * v.tangent.w;   // World Space Normal�� Tangent�� �����ؼ� BiNormal Vector �����. w�� ������ �����ϴ� ��

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;    // Local Space�� Vertex Position -> World Position
                return o;
            }
            
			float2 uvPerWorldUnit(float2 uv, float2 space) {
				float2 uvPerPixelX = abs(ddx(uv));
				float2 uvPerPixelY = abs(ddy(uv));
				float unitsPerPixelX = length(ddx(space));
				float unitsPerPixelY = length(ddy(space));
				float2 uvPerUnitX = uvPerPixelX / unitsPerPixelX;
				float2 uvPerUnitY = uvPerPixelY / unitsPerPixelY;
				return (uvPerUnitX + uvPerUnitY);
			}

            fixed4 frag (v2f i) : SV_Target                                 // Fragment Shader �Լ� i�� Vertex Shader���� Interpolate�� Data�� �޴´�.
            {
                fixed4 mainTexColor = tex2D(_MainTex, i.uv);                // Main Texture�� UV ��ǥ�� ����Ͽ� Pixel Color�� �����´�.
                fixed4 finalColor = mainTexColor * _Color * i.color;        // Texture Color * Inspector Color * v2f Color�� ���Ѵ�.

                fixed4 normalMapSample = tex2D(_NormalMap, i.uv);           // Normal Texture�� Sampling �Ѵ�.
                float3 tangentSpaceNormal = UnpackNormal(normalMapSample);  // Sampling�� Texture�� Tangent Space�� Normal Vector ��ȯ

                tangentSpaceNormal.xy *= _NormalStrength;                   // Tangent Space Normal Vector�� ������ ���Ѵ�.
                tangentSpaceNormal = normalize(tangentSpaceNormal);         // Normalize�� �Ѵ�.

                float3 worldNormalFromMap = normalize(                      
                    i.worldTangent * tangentSpaceNormal.x +
                    i.worldBinormal * tangentSpaceNormal.y +
                    i.worldNormal * tangentSpaceNormal.z
                );                                                          // Tangent Normal�� NormalVector�� World Space�� ��ȯ�Ѵ�. Tangent, BiNormal, Normal�� �̿��� Rotation Matrix�� �����.

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);          // Unity�� Direction�� ������ ������ Normalize�� �Ѵ�.
                float diffuse = saturate(dot(worldNormalFromMap, lightDir));    // World Space Normal Vector�� Light Direction Vector�� �����ؼ� ���ݻ� ���� ���� ���Ѵ�. saturate�� �̿��ؼ� ������ ������ �ʰ� �Ѵ�.

                float3 lightContribution = _LightIntensity;                                 // ���� ��ü���� ������ ��Ÿ����.
                float3 lightContribution2 = _LightIntensity * _LightColor0.rgb * diffuse;   // Diffuse�� Direction Light�� ���Ѵ�.

                finalColor.rgb *= (lightContribution + lightContribution2); 
                finalColor.a = i.color.a * _Color.a * mainTexColor.a;
                
                // Pass 1 outline

                if(_OutlineMode == 1.0f)
                {
                    #define DIV_SQRT_2 0.70710678118
				    float2 directions[8] = {float2(1, 0), float2(0, 1), float2(-1, 0), float2(0, -1),
				    float2(DIV_SQRT_2, DIV_SQRT_2), float2(-DIV_SQRT_2, DIV_SQRT_2),
				    float2(-DIV_SQRT_2, -DIV_SQRT_2), float2(DIV_SQRT_2, -DIV_SQRT_2)};
                    
                    float sinValue = sin(_Time.y * _OutlineAnimationSpeed * 10);
                    float animatedWidth = 0 + (sinValue + 1.0) * 0.5 * (10 - 0);
                    
				    float2 sampleDistance = _MainTex_TexelSize.xy * animatedWidth;
				    //float2 sampleDistance = uvPerWorldUnit(i.uv, i.worldPos.xy) * _MainTex_TexelSize.xy * _OutlineWidth;
                    float maxAlpha = 0;
				    for(uint index = 0; index<8; index++){
					    float2 sampleUV = i.uv + directions[index] * sampleDistance;
					    maxAlpha = max(maxAlpha, tex2D(_MainTex, sampleUV).a);
				    }

				    //apply border
				    finalColor.rgb = lerp(_OutlineColor.rgb, finalColor.rgb, finalColor.a);
				    finalColor.a = max(finalColor.a, maxAlpha);
                }

                return finalColor;
            }
            ENDCG
        }
    }
}
