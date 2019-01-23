// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:33640,y:32751,varname:node_4013,prsc:2|diff-4543-OUT,spec-3356-OUT,gloss-8471-OUT,normal-7184-OUT,emission-3288-OUT,amdfl-322-RGB,amspl-322-RGB;n:type:ShaderForge.SFN_Tex2d,id:8520,x:31981,y:32940,ptovrint:False,ptlb:T_Main,ptin:_T_Main,cmnt:Unpack channels,varname:_T_Main,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:44c4899883df42548bfbcc4119521f8d,ntxv:0,isnm:False|UVIN-926-OUT;n:type:ShaderForge.SFN_Tex2d,id:3862,x:31981,y:33162,ptovrint:False,ptlb:T_Normal,ptin:_T_Normal,varname:_T_Normal,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:344e82c3f8c38814493a68ebdd13a446,ntxv:3,isnm:True|UVIN-926-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:5923,x:30437,y:32918,varname:node_5923,prsc:2;n:type:ShaderForge.SFN_Append,id:2160,x:30626,y:32937,varname:node_2160,prsc:2|A-5923-X,B-5923-Z;n:type:ShaderForge.SFN_Multiply,id:4191,x:30907,y:32937,varname:node_4191,prsc:0|A-2160-OUT,B-3181-OUT;n:type:ShaderForge.SFN_Vector1,id:3181,x:30746,y:32971,varname:node_3181,prsc:2,v1:0.25;n:type:ShaderForge.SFN_Add,id:926,x:31280,y:32938,cmnt:UVs projected down Y axis,varname:node_926,prsc:0|A-4191-OUT,B-3436-OUT;n:type:ShaderForge.SFN_Tex2d,id:9061,x:31981,y:32698,ptovrint:False,ptlb:T_Alpha,ptin:_T_Alpha,varname:_T_Alpha,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:65559dfd0c0534545bb0a56b26efd2d0,ntxv:3,isnm:False|UVIN-8489-OUT;n:type:ShaderForge.SFN_Divide,id:8489,x:31787,y:32698,varname:node_8489,prsc:0|A-3597-OUT,B-1579-OUT;n:type:ShaderForge.SFN_Vector1,id:1579,x:31613,y:32731,varname:node_1579,prsc:2,v1:9;n:type:ShaderForge.SFN_Add,id:3597,x:31468,y:32697,cmnt:UVs for pattern alpha,varname:node_3597,prsc:2|A-926-OUT,B-8631-OUT;n:type:ShaderForge.SFN_Vector2,id:8631,x:31280,y:32727,varname:node_8631,prsc:0,v1:-0.5,v2:-0.5;n:type:ShaderForge.SFN_ValueProperty,id:509,x:30825,y:33233,ptovrint:False,ptlb:Proj_Offset_X,ptin:_Proj_Offset_X,varname:_Proj_Offset_X,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_ValueProperty,id:1412,x:30825,y:33358,ptovrint:False,ptlb:Proj_Offset_Y,ptin:_Proj_Offset_Y,varname:_Proj_Offset_Y,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Append,id:3436,x:31121,y:33182,varname:node_3436,prsc:0|A-509-OUT,B-1412-OUT;n:type:ShaderForge.SFN_Lerp,id:5384,x:32261,y:32536,varname:node_5384,prsc:2|A-6783-RGB,B-3802-RGB,T-9061-R;n:type:ShaderForge.SFN_Color,id:6783,x:31981,y:32252,ptovrint:False,ptlb:Colour_Tint_Primary,ptin:_Colour_Tint_Primary,varname:_Colour_Tint_Primary,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.3,c2:0.3,c3:0.3,c4:1;n:type:ShaderForge.SFN_Color,id:3802,x:31981,y:32462,ptovrint:False,ptlb:Colour_Tint_Secondary,ptin:_Colour_Tint_Secondary,varname:_Colour_Tint_Secondary,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.45,c2:0.45,c3:0.45,c4:1;n:type:ShaderForge.SFN_Multiply,id:4543,x:33156,y:32725,varname:node_4543,prsc:2|A-5384-OUT,B-8520-R;n:type:ShaderForge.SFN_Vector1,id:3356,x:33156,y:33109,varname:node_3356,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:3288,x:33294,y:33172,varname:node_3288,prsc:2|A-9865-OUT,B-1469-OUT;n:type:ShaderForge.SFN_AmbientLight,id:322,x:33156,y:32970,varname:node_322,prsc:2;n:type:ShaderForge.SFN_ViewPosition,id:5514,x:30916,y:33661,varname:node_5514,prsc:2;n:type:ShaderForge.SFN_Append,id:8628,x:31099,y:33682,varname:node_8628,prsc:2|A-5514-X,B-5514-Z;n:type:ShaderForge.SFN_Multiply,id:1175,x:31380,y:33682,varname:node_1175,prsc:0|A-8628-OUT,B-7574-OUT;n:type:ShaderForge.SFN_Vector1,id:7574,x:31219,y:33716,varname:node_7574,prsc:2,v1:0.25;n:type:ShaderForge.SFN_Add,id:7142,x:31551,y:33666,varname:node_7142,prsc:0|A-3436-OUT,B-1175-OUT;n:type:ShaderForge.SFN_Add,id:9514,x:31551,y:33491,varname:node_9514,prsc:2|A-926-OUT,B-1129-OUT;n:type:ShaderForge.SFN_Vector1,id:1129,x:31391,y:33538,varname:node_1129,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Round,id:8950,x:31720,y:33493,varname:node_8950,prsc:2|IN-9514-OUT;n:type:ShaderForge.SFN_Multiply,id:6610,x:31982,y:33495,varname:node_6610,prsc:2|A-8950-OUT,B-479-OUT;n:type:ShaderForge.SFN_Vector1,id:479,x:31836,y:33548,varname:node_479,prsc:2,v1:4;n:type:ShaderForge.SFN_Subtract,id:5488,x:32256,y:33495,varname:node_5488,prsc:0|A-6610-OUT,B-252-OUT;n:type:ShaderForge.SFN_Vector1,id:252,x:32090,y:33548,varname:node_252,prsc:2,v1:2;n:type:ShaderForge.SFN_Distance,id:8146,x:32447,y:33651,cmnt:Rounded radial distance,varname:node_8146,prsc:2|A-5488-OUT,B-5169-OUT;n:type:ShaderForge.SFN_Multiply,id:599,x:32627,y:33488,varname:node_599,prsc:2|A-1390-OUT,B-8146-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1390,x:32466,y:33430,ptovrint:False,ptlb:Step_On_Radius,ptin:_Step_On_Radius,varname:_Step_On_Radius,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:7876,x:32977,y:33488,varname:node_7876,prsc:2|A-3173-OUT,B-3694-OUT;n:type:ShaderForge.SFN_Vector1,id:3694,x:32719,y:33660,varname:node_3694,prsc:2,v1:4;n:type:ShaderForge.SFN_OneMinus,id:3173,x:32802,y:33488,varname:node_3173,prsc:2|IN-599-OUT;n:type:ShaderForge.SFN_Clamp01,id:1469,x:33133,y:33488,varname:node_1469,prsc:2|IN-7876-OUT;n:type:ShaderForge.SFN_Vector1,id:7934,x:31836,y:33726,varname:node_7934,prsc:2,v1:4;n:type:ShaderForge.SFN_Multiply,id:5169,x:31982,y:33673,varname:node_5169,prsc:2|A-7142-OUT,B-7934-OUT;n:type:ShaderForge.SFN_Color,id:8326,x:32259,y:32724,ptovrint:False,ptlb:Emissive_Colour_Primary,ptin:_Emissive_Colour_Primary,varname:_Emissive_Colour_Primary,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:9992,x:32259,y:32932,ptovrint:False,ptlb:Emissive_Colour_Secondary,ptin:_Emissive_Colour_Secondary,varname:_Emissive_Colour_Secondary,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Lerp,id:2956,x:32506,y:32802,varname:node_2956,prsc:2|A-8326-RGB,B-9992-RGB,T-9061-R;n:type:ShaderForge.SFN_Multiply,id:9865,x:32859,y:33069,varname:node_9865,prsc:2|A-2956-OUT,B-8520-B;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:7184,x:32493,y:33163,varname:node_7184,prsc:2|IN-3862-RGB,IMIN-4593-OUT,IMAX-9674-OUT,OMIN-419-OUT,OMAX-2517-OUT;n:type:ShaderForge.SFN_Vector3,id:4593,x:32143,y:33192,varname:node_4593,prsc:0,v1:1,v2:1,v3:-1;n:type:ShaderForge.SFN_Vector3,id:9674,x:32143,y:33302,varname:node_9674,prsc:0,v1:-1,v2:-1,v3:1;n:type:ShaderForge.SFN_Vector1,id:419,x:32313,y:33283,varname:node_419,prsc:0,v1:-1;n:type:ShaderForge.SFN_Vector1,id:2517,x:32313,y:33391,varname:node_2517,prsc:2,v1:1;n:type:ShaderForge.SFN_Lerp,id:8975,x:32870,y:32871,varname:node_8975,prsc:2|A-2870-OUT,B-8228-OUT,T-9061-R;n:type:ShaderForge.SFN_Power,id:8471,x:33156,y:32842,varname:node_8471,prsc:2|VAL-8520-G,EXP-8975-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2870,x:32667,y:32871,ptovrint:False,ptlb:Gloss_Pow_Primary,ptin:_Gloss_Pow_Primary,varname:_Gloss_Pow_Primary,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:8228,x:32667,y:32980,ptovrint:False,ptlb:Gloss_Pow_Secondary,ptin:_Gloss_Pow_Secondary,varname:_Gloss_Pow_Secondary,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;proporder:8520-9061-509-1412-6783-3802-3862-1390-8326-9992-2870-8228;pass:END;sub:END;*/

Shader "Spatial/Spatial_Floor_Master" {
    Properties {
        _T_Main ("T_Main", 2D) = "white" {}
        _T_Alpha ("T_Alpha", 2D) = "bump" {}
        _Proj_Offset_X ("Proj_Offset_X", Float ) = 0.5
        _Proj_Offset_Y ("Proj_Offset_Y", Float ) = 0.5
        _Colour_Tint_Primary ("Colour_Tint_Primary", Color) = (0.3,0.3,0.3,1)
        _Colour_Tint_Secondary ("Colour_Tint_Secondary", Color) = (0.45,0.45,0.45,1)
        _T_Normal ("T_Normal", 2D) = "bump" {}
        _Step_On_Radius ("Step_On_Radius", Float ) = 1
        _Emissive_Colour_Primary ("Emissive_Colour_Primary", Color) = (0.5,0.5,0.5,1)
        _Emissive_Colour_Secondary ("Emissive_Colour_Secondary", Color) = (0.5,0.5,0.5,1)
        _Gloss_Pow_Primary ("Gloss_Pow_Primary", Float ) = 1
        _Gloss_Pow_Secondary ("Gloss_Pow_Secondary", Float ) = 2
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 
            #pragma target 3.0
            uniform sampler2D _T_Main; uniform float4 _T_Main_ST;
            uniform sampler2D _T_Normal; uniform float4 _T_Normal_ST;
            uniform sampler2D _T_Alpha; uniform float4 _T_Alpha_ST;
            uniform fixed _Proj_Offset_X;
            uniform fixed _Proj_Offset_Y;
            uniform fixed4 _Colour_Tint_Primary;
            uniform fixed4 _Colour_Tint_Secondary;
            uniform fixed _Step_On_Radius;
            uniform fixed4 _Emissive_Colour_Primary;
            uniform fixed4 _Emissive_Colour_Secondary;
            uniform fixed _Gloss_Pow_Primary;
            uniform fixed _Gloss_Pow_Secondary;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float3 tangentDir : TEXCOORD2;
                float3 bitangentDir : TEXCOORD3;
                LIGHTING_COORDS(4,5)
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                fixed2 node_3436 = float2(_Proj_Offset_X,_Proj_Offset_Y);
                fixed2 node_926 = ((float2(i.posWorld.r,i.posWorld.b)*0.25)+node_3436); // UVs projected down Y axis
                fixed3 _T_Normal_var = UnpackNormal(tex2D(_T_Normal,TRANSFORM_TEX(node_926, _T_Normal)));
                fixed3 node_4593 = fixed3(1,1,-1);
                fixed node_419 = (-1.0);
                float3 normalLocal = (node_419 + ( (_T_Normal_var.rgb - node_4593) * (1.0 - node_419) ) / (fixed3(-1,-1,1) - node_4593));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                fixed4 _T_Main_var = tex2D(_T_Main,TRANSFORM_TEX(node_926, _T_Main)); // Unpack channels
                fixed2 node_8489 = ((node_926+fixed2(-0.5,-0.5))/9.0);
                fixed4 _T_Alpha_var = tex2D(_T_Alpha,TRANSFORM_TEX(node_8489, _T_Alpha));
                float gloss = pow(_T_Main_var.g,lerp(_Gloss_Pow_Primary,_Gloss_Pow_Secondary,_T_Alpha_var.r));
                float perceptualRoughness = 1.0 - pow(_T_Main_var.g,lerp(_Gloss_Pow_Primary,_Gloss_Pow_Secondary,_T_Alpha_var.r));
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = 0.0;
                float specularMonochrome;
                float3 diffuseColor = (lerp(_Colour_Tint_Primary.rgb,_Colour_Tint_Secondary.rgb,_T_Alpha_var.r)*_T_Main_var.r); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (0 + UNITY_LIGHTMODEL_AMBIENT.rgb);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Diffuse Ambient Light
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = ((lerp(_Emissive_Colour_Primary.rgb,_Emissive_Colour_Secondary.rgb,_T_Alpha_var.r)*_T_Main_var.b)*saturate(((1.0 - (_Step_On_Radius*distance(((round((node_926+0.5))*4.0)-2.0),((node_3436+(float2(_WorldSpaceCameraPos.r,_WorldSpaceCameraPos.b)*0.25))*4.0))))+4.0)));
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 
            #pragma target 3.0
            uniform sampler2D _T_Main; uniform float4 _T_Main_ST;
            uniform sampler2D _T_Normal; uniform float4 _T_Normal_ST;
            uniform sampler2D _T_Alpha; uniform float4 _T_Alpha_ST;
            uniform fixed _Proj_Offset_X;
            uniform fixed _Proj_Offset_Y;
            uniform fixed4 _Colour_Tint_Primary;
            uniform fixed4 _Colour_Tint_Secondary;
            uniform fixed _Step_On_Radius;
            uniform fixed4 _Emissive_Colour_Primary;
            uniform fixed4 _Emissive_Colour_Secondary;
            uniform fixed _Gloss_Pow_Primary;
            uniform fixed _Gloss_Pow_Secondary;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float3 tangentDir : TEXCOORD2;
                float3 bitangentDir : TEXCOORD3;
                LIGHTING_COORDS(4,5)
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                fixed2 node_3436 = float2(_Proj_Offset_X,_Proj_Offset_Y);
                fixed2 node_926 = ((float2(i.posWorld.r,i.posWorld.b)*0.25)+node_3436); // UVs projected down Y axis
                fixed3 _T_Normal_var = UnpackNormal(tex2D(_T_Normal,TRANSFORM_TEX(node_926, _T_Normal)));
                fixed3 node_4593 = fixed3(1,1,-1);
                fixed node_419 = (-1.0);
                float3 normalLocal = (node_419 + ( (_T_Normal_var.rgb - node_4593) * (1.0 - node_419) ) / (fixed3(-1,-1,1) - node_4593));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                fixed4 _T_Main_var = tex2D(_T_Main,TRANSFORM_TEX(node_926, _T_Main)); // Unpack channels
                fixed2 node_8489 = ((node_926+fixed2(-0.5,-0.5))/9.0);
                fixed4 _T_Alpha_var = tex2D(_T_Alpha,TRANSFORM_TEX(node_8489, _T_Alpha));
                float gloss = pow(_T_Main_var.g,lerp(_Gloss_Pow_Primary,_Gloss_Pow_Secondary,_T_Alpha_var.r));
                float perceptualRoughness = 1.0 - pow(_T_Main_var.g,lerp(_Gloss_Pow_Primary,_Gloss_Pow_Secondary,_T_Alpha_var.r));
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = 0.0;
                float specularMonochrome;
                float3 diffuseColor = (lerp(_Colour_Tint_Primary.rgb,_Colour_Tint_Secondary.rgb,_T_Alpha_var.r)*_T_Main_var.r); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
