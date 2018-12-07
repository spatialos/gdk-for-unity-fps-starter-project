// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:32719,y:32712,varname:node_2865,prsc:2|diff-2348-OUT,spec-1151-B,gloss-1171-OUT,normal-5964-RGB,emission-7434-OUT,amdfl-5725-RGB,amspl-5725-RGB,difocc-114-OUT,spcocc-114-OUT;n:type:ShaderForge.SFN_Tex2d,id:7736,x:31132,y:32313,ptovrint:True,ptlb:T_Colour,ptin:_MainTex,varname:_MainTex,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:f3528a9556d869243ac93d1d727b6514,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:5964,x:31140,y:33136,ptovrint:True,ptlb:T_Normal,ptin:_BumpMap,varname:_BumpMap,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8b1cc07a59c3dd04481be3b70abc5a9a,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:9137,x:31133,y:31469,ptovrint:False,ptlb:T_Mask,ptin:_T_Mask,varname:_T_Mask,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6c09dd94400846d40a06122e8b936deb,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1151,x:31131,y:32862,ptovrint:False,ptlb:T_Orm,ptin:_T_Orm,varname:_T_Orm,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:c7b958f0fcad311458480fe9b600928a,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Power,id:114,x:32153,y:32688,varname:node_114,prsc:2|VAL-1151-R,EXP-7107-OUT;n:type:ShaderForge.SFN_Vector1,id:7107,x:31956,y:32793,varname:node_7107,prsc:2,v1:3;n:type:ShaderForge.SFN_AmbientLight,id:5725,x:32387,y:32891,varname:node_5725,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2348,x:32009,y:32311,varname:node_2348,prsc:2|A-1269-OUT,B-7736-RGB,C-2928-OUT,D-1151-R;n:type:ShaderForge.SFN_Lerp,id:2928,x:31472,y:32534,varname:node_2928,prsc:2|A-8761-OUT,B-1420-OUT,T-5983-OUT;n:type:ShaderForge.SFN_Vector1,id:8761,x:31142,y:32523,varname:node_8761,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:1420,x:31321,y:32653,varname:node_1420,prsc:2|A-1151-A,B-8084-OUT;n:type:ShaderForge.SFN_Vector1,id:8084,x:31142,y:32687,varname:node_8084,prsc:2,v1:2;n:type:ShaderForge.SFN_ValueProperty,id:5983,x:31142,y:32586,ptovrint:False,ptlb:Edge_Highlight,ptin:_Edge_Highlight,varname:_Edge_Highlight,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.4;n:type:ShaderForge.SFN_Color,id:967,x:31132,y:31694,ptovrint:False,ptlb:Base_Colour,ptin:_Base_Colour,varname:_Base_Colour,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:5276,x:31132,y:31891,ptovrint:False,ptlb:Main_Colour,ptin:_Main_Colour,varname:_Main_Colour,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.7,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:291,x:31132,y:32105,ptovrint:False,ptlb:Secondary_Colour,ptin:_Secondary_Colour,varname:_Secondary_Colour,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Lerp,id:8207,x:31469,y:31602,varname:node_8207,prsc:2|A-967-RGB,B-5276-RGB,T-9137-B;n:type:ShaderForge.SFN_Lerp,id:1269,x:31663,y:31789,varname:node_1269,prsc:2|A-8207-OUT,B-291-RGB,T-9137-A;n:type:ShaderForge.SFN_OneMinus,id:1171,x:32153,y:32830,varname:node_1171,prsc:2|IN-1151-G;n:type:ShaderForge.SFN_Color,id:7954,x:31133,y:30580,ptovrint:False,ptlb:Main_Light,ptin:_Main_Light,varname:_Main_Light,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.2,c3:0.1,c4:1;n:type:ShaderForge.SFN_Color,id:1475,x:31133,y:30929,ptovrint:False,ptlb:Secondary_Light,ptin:_Secondary_Light,varname:_Secondary_Light,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.2,c2:0.5,c3:1,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:6934,x:31133,y:31173,ptovrint:False,ptlb:Main_Light_Pow,ptin:_Main_Light_Pow,varname:_Main_Light_Pow,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_ValueProperty,id:4306,x:31133,y:31300,ptovrint:False,ptlb:Secondary_Light_Pow,ptin:_Secondary_Light_Pow,varname:_Secondary_Light_Pow,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_Power,id:8746,x:31381,y:31154,varname:node_8746,prsc:2|VAL-9137-R,EXP-6934-OUT;n:type:ShaderForge.SFN_Power,id:3837,x:31381,y:31286,varname:node_3837,prsc:2|VAL-9137-G,EXP-4306-OUT;n:type:ShaderForge.SFN_ValueProperty,id:50,x:31133,y:30458,ptovrint:False,ptlb:Main_Light_Brightness,ptin:_Main_Light_Brightness,varname:_Main_Light_Brightness,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_ValueProperty,id:6694,x:31133,y:30820,ptovrint:False,ptlb:Secondary_Light_Brightness,ptin:_Secondary_Light_Brightness,varname:_Secondary_Light_Brightness,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_Multiply,id:7669,x:31660,y:30595,varname:node_7669,prsc:2|A-50-OUT,B-7954-RGB,C-8746-OUT;n:type:ShaderForge.SFN_Multiply,id:4029,x:31669,y:30913,varname:node_4029,prsc:2|A-6694-OUT,B-1475-RGB,C-3837-OUT;n:type:ShaderForge.SFN_Add,id:7434,x:32063,y:31003,varname:node_7434,prsc:2|A-7669-OUT,B-4029-OUT;proporder:5964-7736-1151-9137-967-5276-291-5983-7954-6934-50-1475-4306-6694;pass:END;sub:END;*/

Shader "Spatial/Spatial_Asset_Master" {
    Properties {
        _BumpMap ("T_Normal", 2D) = "bump" {}
        _MainTex ("T_Colour", 2D) = "white" {}
        _T_Orm ("T_Orm", 2D) = "white" {}
        _T_Mask ("T_Mask", 2D) = "white" {}
        _Base_Colour ("Base_Colour", Color) = (1,1,1,1)
        _Main_Colour ("Main_Colour", Color) = (1,0.7,0,1)
        _Secondary_Colour ("Secondary_Colour", Color) = (1,1,1,1)
        _Edge_Highlight ("Edge_Highlight", Float ) = 0.4
        _Main_Light ("Main_Light", Color) = (1,0.2,0.1,1)
        _Main_Light_Pow ("Main_Light_Pow", Float ) = 4
        _Main_Light_Brightness ("Main_Light_Brightness", Float ) = 4
        _Secondary_Light ("Secondary_Light", Color) = (0.2,0.5,1,1)
        _Secondary_Light_Pow ("Secondary_Light_Pow", Float ) = 4
        _Secondary_Light_Brightness ("Secondary_Light_Brightness", Float ) = 4
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
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform sampler2D _T_Mask; uniform float4 _T_Mask_ST;
            uniform sampler2D _T_Orm; uniform float4 _T_Orm_ST;
            uniform float _Edge_Highlight;
            uniform fixed4 _Base_Colour;
            uniform fixed4 _Main_Colour;
            uniform fixed4 _Secondary_Colour;
            uniform fixed4 _Main_Light;
            uniform fixed4 _Secondary_Light;
            uniform fixed _Main_Light_Pow;
            uniform fixed _Secondary_Light_Pow;
            uniform fixed _Main_Light_Brightness;
            uniform fixed _Secondary_Light_Brightness;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
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
                fixed3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
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
                fixed4 _T_Orm_var = tex2D(_T_Orm,TRANSFORM_TEX(i.uv0, _T_Orm));
                float gloss = (1.0 - _T_Orm_var.g);
                float perceptualRoughness = 1.0 - (1.0 - _T_Orm_var.g);
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
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float node_114 = pow(_T_Orm_var.r,3.0);
                float3 specularAO = node_114;
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _T_Orm_var.b;
                float specularMonochrome;
                fixed4 _T_Mask_var = tex2D(_T_Mask,TRANSFORM_TEX(i.uv0, _T_Mask));
                fixed4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffuseColor = (lerp(lerp(_Base_Colour.rgb,_Main_Colour.rgb,_T_Mask_var.b),_Secondary_Colour.rgb,_T_Mask_var.a)*_MainTex_var.rgb*lerp(1.0,(_T_Orm_var.a*2.0),_Edge_Highlight)*_T_Orm_var.r); // Need this for specular when using metallic
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
                float3 indirectSpecular = (gi.indirect.specular + UNITY_LIGHTMODEL_AMBIENT.rgb) * specularAO;
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
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Diffuse Ambient Light
                indirectDiffuse += gi.indirect.diffuse;
                indirectDiffuse *= node_114; // Diffuse AO
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = ((_Main_Light_Brightness*_Main_Light.rgb*pow(_T_Mask_var.r,_Main_Light_Pow))+(_Secondary_Light_Brightness*_Secondary_Light.rgb*pow(_T_Mask_var.g,_Secondary_Light_Pow)));
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
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform sampler2D _T_Mask; uniform float4 _T_Mask_ST;
            uniform sampler2D _T_Orm; uniform float4 _T_Orm_ST;
            uniform float _Edge_Highlight;
            uniform fixed4 _Base_Colour;
            uniform fixed4 _Main_Colour;
            uniform fixed4 _Secondary_Colour;
            uniform fixed4 _Main_Light;
            uniform fixed4 _Secondary_Light;
            uniform fixed _Main_Light_Pow;
            uniform fixed _Secondary_Light_Pow;
            uniform fixed _Main_Light_Brightness;
            uniform fixed _Secondary_Light_Brightness;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
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
                fixed3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
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
                fixed4 _T_Orm_var = tex2D(_T_Orm,TRANSFORM_TEX(i.uv0, _T_Orm));
                float gloss = (1.0 - _T_Orm_var.g);
                float perceptualRoughness = 1.0 - (1.0 - _T_Orm_var.g);
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _T_Orm_var.b;
                float specularMonochrome;
                fixed4 _T_Mask_var = tex2D(_T_Mask,TRANSFORM_TEX(i.uv0, _T_Mask));
                fixed4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffuseColor = (lerp(lerp(_Base_Colour.rgb,_Main_Colour.rgb,_T_Mask_var.b),_Secondary_Colour.rgb,_T_Mask_var.a)*_MainTex_var.rgb*lerp(1.0,(_T_Orm_var.a*2.0),_Edge_Highlight)*_T_Orm_var.r); // Need this for specular when using metallic
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
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _T_Mask; uniform float4 _T_Mask_ST;
            uniform sampler2D _T_Orm; uniform float4 _T_Orm_ST;
            uniform float _Edge_Highlight;
            uniform fixed4 _Base_Colour;
            uniform fixed4 _Main_Colour;
            uniform fixed4 _Secondary_Colour;
            uniform fixed4 _Main_Light;
            uniform fixed4 _Secondary_Light;
            uniform fixed _Main_Light_Pow;
            uniform fixed _Secondary_Light_Pow;
            uniform fixed _Main_Light_Brightness;
            uniform fixed _Secondary_Light_Brightness;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                fixed4 _T_Mask_var = tex2D(_T_Mask,TRANSFORM_TEX(i.uv0, _T_Mask));
                o.Emission = ((_Main_Light_Brightness*_Main_Light.rgb*pow(_T_Mask_var.r,_Main_Light_Pow))+(_Secondary_Light_Brightness*_Secondary_Light.rgb*pow(_T_Mask_var.g,_Secondary_Light_Pow)));
                
                fixed4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                fixed4 _T_Orm_var = tex2D(_T_Orm,TRANSFORM_TEX(i.uv0, _T_Orm));
                float3 diffColor = (lerp(lerp(_Base_Colour.rgb,_Main_Colour.rgb,_T_Mask_var.b),_Secondary_Colour.rgb,_T_Mask_var.a)*_MainTex_var.rgb*lerp(1.0,(_T_Orm_var.a*2.0),_Edge_Highlight)*_T_Orm_var.r);
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _T_Orm_var.b, specColor, specularMonochrome );
                float roughness = 1.0 - (1.0 - _T_Orm_var.g);
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
