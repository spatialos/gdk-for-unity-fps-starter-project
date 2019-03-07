// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.8156347,fgcg:0.925513,fgcb:0.9719999,fgca:1,fgde:0.0005,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33331,y:32649,varname:node_2865,prsc:2|diff-2348-OUT,spec-1151-B,gloss-1171-OUT,normal-5964-RGB,emission-4517-OUT,amdfl-5725-RGB,amspl-5725-RGB,difocc-114-OUT,spcocc-114-OUT,clip-6716-OUT;n:type:ShaderForge.SFN_Tex2d,id:7736,x:30988,y:31261,ptovrint:True,ptlb:T_Colour,ptin:_MainTex,varname:_MainTex,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:f3528a9556d869243ac93d1d727b6514,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:5964,x:31011,y:33089,ptovrint:True,ptlb:T_Normal,ptin:_BumpMap,varname:_BumpMap,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8b1cc07a59c3dd04481be3b70abc5a9a,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:9137,x:30980,y:30224,ptovrint:False,ptlb:T_Mask,ptin:_T_Mask,varname:_T_Mask,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8ecc6d4a1de26db4ba08539cc72edf83,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1151,x:30997,y:32848,ptovrint:False,ptlb:T_Orm,ptin:_T_Orm,varname:_T_Orm,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:eb21098d8df19a04088667ba93a29528,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Power,id:114,x:32611,y:32302,cmnt:AO,varname:node_114,prsc:0|VAL-9772-OUT,EXP-641-OUT;n:type:ShaderForge.SFN_AmbientLight,id:5725,x:33158,y:32813,varname:node_5725,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2348,x:32620,y:32041,cmnt:Base Colour,varname:node_2348,prsc:2|A-9629-OUT,B-7736-RGB,C-1151-R;n:type:ShaderForge.SFN_Lerp,id:2928,x:31328,y:31482,cmnt:Edge Highlight from Curvature,varname:node_2928,prsc:2|A-8761-OUT,B-1420-OUT,T-5983-OUT;n:type:ShaderForge.SFN_Vector1,id:8761,x:30998,y:31471,varname:node_8761,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:1420,x:31185,y:31601,varname:node_1420,prsc:2|A-3300-B,B-8084-OUT;n:type:ShaderForge.SFN_Vector1,id:8084,x:30998,y:31635,varname:node_8084,prsc:2,v1:2;n:type:ShaderForge.SFN_ValueProperty,id:5983,x:30998,y:31534,ptovrint:False,ptlb:Edge_Highlight,ptin:_Edge_Highlight,varname:_Edge_Highlight,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.4;n:type:ShaderForge.SFN_Color,id:967,x:30979,y:30449,ptovrint:False,ptlb:Base_Colour,ptin:_Base_Colour,varname:_Base_Colour,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:5276,x:30979,y:30646,ptovrint:False,ptlb:Main_Colour,ptin:_Main_Colour,varname:_Main_Colour,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.7,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:291,x:30979,y:30860,ptovrint:False,ptlb:Secondary_Colour,ptin:_Secondary_Colour,varname:_Secondary_Colour,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Lerp,id:8207,x:31316,y:30357,varname:node_8207,prsc:2|A-967-RGB,B-5276-RGB,T-9137-R;n:type:ShaderForge.SFN_Lerp,id:1269,x:31510,y:30544,varname:node_1269,prsc:2|A-8207-OUT,B-291-RGB,T-9137-G;n:type:ShaderForge.SFN_OneMinus,id:1171,x:32021,y:32831,cmnt:Gloss,varname:node_1171,prsc:0|IN-2284-OUT;n:type:ShaderForge.SFN_Color,id:7954,x:30986,y:31860,ptovrint:False,ptlb:Main_Light,ptin:_Main_Light,varname:_Main_Light,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.2,c3:0.1,c4:1;n:type:ShaderForge.SFN_Color,id:1475,x:30986,y:32209,ptovrint:False,ptlb:Secondary_Light,ptin:_Secondary_Light,varname:_Secondary_Light,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.2,c2:0.5,c3:1,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:6934,x:30986,y:32453,ptovrint:False,ptlb:Main_Light_Pow,ptin:_Main_Light_Pow,varname:_Main_Light_Pow,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_ValueProperty,id:4306,x:30986,y:32580,ptovrint:False,ptlb:Secondary_Light_Pow,ptin:_Secondary_Light_Pow,varname:_Secondary_Light_Pow,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_Power,id:8746,x:31234,y:32434,varname:node_8746,prsc:2|VAL-3300-R,EXP-6934-OUT;n:type:ShaderForge.SFN_Power,id:3837,x:31234,y:32566,varname:node_3837,prsc:2|VAL-3300-G,EXP-4306-OUT;n:type:ShaderForge.SFN_ValueProperty,id:50,x:30998,y:31729,ptovrint:False,ptlb:Main_Light_Brightness,ptin:_Main_Light_Brightness,varname:_Main_Light_Brightness,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_ValueProperty,id:6694,x:30986,y:32100,ptovrint:False,ptlb:Secondary_Light_Brightness,ptin:_Secondary_Light_Brightness,varname:_Secondary_Light_Brightness,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_Multiply,id:7669,x:31513,y:31875,varname:node_7669,prsc:2|A-50-OUT,B-7954-RGB,C-8746-OUT;n:type:ShaderForge.SFN_Multiply,id:4029,x:31522,y:32193,varname:node_4029,prsc:2|A-6694-OUT,B-1475-RGB,C-3837-OUT;n:type:ShaderForge.SFN_Add,id:7434,x:31641,y:32397,cmnt:Emissive,varname:node_7434,prsc:2|A-7669-OUT,B-4029-OUT;n:type:ShaderForge.SFN_ValueProperty,id:641,x:32373,y:32404,ptovrint:False,ptlb:AO_Power,ptin:_AO_Power,varname:_AO_Power,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_FragmentPosition,id:5003,x:31062,y:33345,varname:node_5003,prsc:2;n:type:ShaderForge.SFN_ViewPosition,id:3855,x:31062,y:33484,varname:node_3855,prsc:2;n:type:ShaderForge.SFN_Distance,id:8025,x:31475,y:33421,varname:node_8025,prsc:2|A-6299-OUT,B-6068-OUT;n:type:ShaderForge.SFN_Append,id:6299,x:31294,y:33372,varname:node_6299,prsc:2|A-5003-X,B-5003-Z;n:type:ShaderForge.SFN_Append,id:6068,x:31294,y:33504,varname:node_6068,prsc:2|A-3855-X,B-3855-Z;n:type:ShaderForge.SFN_RemapRange,id:8159,x:32444,y:33417,varname:node_8159,prsc:2,frmn:0.98,frmx:1,tomn:1,tomx:0|IN-3445-OUT;n:type:ShaderForge.SFN_Clamp01,id:6716,x:32596,y:33417,varname:node_6716,prsc:0|IN-8159-OUT;n:type:ShaderForge.SFN_Divide,id:7308,x:31920,y:33534,cmnt:Clip objects by radial distance,varname:node_7308,prsc:0|A-4624-OUT,B-7657-OUT;n:type:ShaderForge.SFN_Add,id:3445,x:32290,y:33405,varname:node_3445,prsc:0|A-188-OUT,B-7308-OUT;n:type:ShaderForge.SFN_RemapRange,id:188,x:32139,y:33354,varname:node_188,prsc:2,frmn:0,frmx:1,tomn:0.1,tomx:-0.1|IN-1171-OUT;n:type:ShaderForge.SFN_RemapRange,id:3176,x:32444,y:33227,varname:node_3176,prsc:2,frmn:0.92,frmx:0.98,tomn:0,tomx:1|IN-3445-OUT;n:type:ShaderForge.SFN_Clamp01,id:8197,x:32596,y:33227,varname:node_8197,prsc:0|IN-3176-OUT;n:type:ShaderForge.SFN_Vector3,id:4670,x:32574,y:33102,varname:node_4670,prsc:0,v1:0.2,v2:0.5,v3:1;n:type:ShaderForge.SFN_Multiply,id:8096,x:32765,y:33169,varname:node_8096,prsc:2|A-4670-OUT,B-8197-OUT,C-8197-OUT,D-8197-OUT;n:type:ShaderForge.SFN_Add,id:4517,x:32950,y:32768,varname:node_4517,prsc:2|A-7434-OUT,B-8096-OUT;n:type:ShaderForge.SFN_ToggleProperty,id:8209,x:31433,y:34154,ptovrint:False,ptlb:OverrideClipDistanceToGlobals,ptin:_OverrideClipDistanceToGlobals,varname:_OverrideClipDistanceToGlobals,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False;n:type:ShaderForge.SFN_ValueProperty,id:8932,x:31159,y:33845,ptovrint:False,ptlb:LocalClipDistance,ptin:_LocalClipDistance,varname:_LocalClipDistance,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:200;n:type:ShaderForge.SFN_ValueProperty,id:1323,x:31159,y:33982,ptovrint:False,ptlb:GlobalClipDistance,ptin:_GlobalClipDistance,varname:_GlobalClipDistance,prsc:0,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Lerp,id:7657,x:31626,y:33850,varname:node_7657,prsc:2|A-8932-OUT,B-780-OUT,T-8209-OUT;n:type:ShaderForge.SFN_Add,id:4624,x:31722,y:33486,varname:node_4624,prsc:2|A-8025-OUT,B-4299-OUT;n:type:ShaderForge.SFN_Vector1,id:4299,x:31475,y:33588,varname:node_4299,prsc:2,v1:21;n:type:ShaderForge.SFN_Subtract,id:780,x:31337,y:34000,cmnt:Subtracting bypasses clip if clip distance is zero,varname:node_780,prsc:2|A-1323-OUT,B-3041-OUT;n:type:ShaderForge.SFN_Vector1,id:3041,x:31159,y:34080,varname:node_3041,prsc:2,v1:0.01;n:type:ShaderForge.SFN_Color,id:2346,x:30979,y:31064,ptovrint:False,ptlb:Tertiary_Colour,ptin:_Tertiary_Colour,varname:_Tertiary_Colour,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Lerp,id:9629,x:31736,y:30730,varname:node_9629,prsc:2|A-1269-OUT,B-2346-RGB,T-9137-B;n:type:ShaderForge.SFN_Tex2d,id:3300,x:30657,y:32210,ptovrint:False,ptlb:T_Light,ptin:_T_Light,varname:_T_Light,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ae5e0bea1137cb0478822dbafd990b0f,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:9772,x:32282,y:32227,varname:node_9772,prsc:2|A-2928-OUT,B-1151-R;n:type:ShaderForge.SFN_Power,id:2284,x:31845,y:32831,cmnt:Roughness,varname:node_2284,prsc:0|VAL-1151-G,EXP-6450-OUT;n:type:ShaderForge.SFN_Lerp,id:2668,x:31346,y:30748,varname:node_2668,prsc:2|A-964-OUT,B-1109-OUT,T-9137-R;n:type:ShaderForge.SFN_Lerp,id:620,x:31512,y:30920,varname:node_620,prsc:2|A-2668-OUT,B-4682-OUT,T-9137-G;n:type:ShaderForge.SFN_Lerp,id:6450,x:31665,y:31124,varname:node_6450,prsc:2|A-620-OUT,B-9976-OUT,T-9137-B;n:type:ShaderForge.SFN_ValueProperty,id:964,x:30571,y:30731,ptovrint:False,ptlb:Rgh_Pow_Base,ptin:_Rgh_Pow_Base,varname:_Rgh_Pow_Base,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:1109,x:30571,y:30855,ptovrint:False,ptlb:Rgh_Pow_Primary,ptin:_Rgh_Pow_Primary,varname:_Rgh_Pow_Primary,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:4682,x:30582,y:31009,ptovrint:False,ptlb:Rgh_Pow_Secondary,ptin:_Rgh_Pow_Secondary,varname:_Rgh_Pow_Secondary,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:9976,x:30582,y:31133,ptovrint:False,ptlb:Rgh_Pow_Tertiary,ptin:_Rgh_Pow_Tertiary,varname:_Rgh_Pow_Tertiary,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:5964-7736-1151-9137-967-5276-291-5983-7954-6934-50-1475-4306-6694-641-8209-8932-2346-3300-964-1109-4682-9976;pass:END;sub:END;*/

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
        _AO_Power ("AO_Power", Float ) = 3
        [MaterialToggle] _OverrideClipDistanceToGlobals ("OverrideClipDistanceToGlobals", Float ) = 0
        _LocalClipDistance ("LocalClipDistance", Float ) = 200
        _Tertiary_Colour ("Tertiary_Colour", Color) = (1,1,1,1)
        _T_Light ("T_Light", 2D) = "white" {}
        _Rgh_Pow_Base ("Rgh_Pow_Base", Float ) = 1
        _Rgh_Pow_Primary ("Rgh_Pow_Primary", Float ) = 1
        _Rgh_Pow_Secondary ("Rgh_Pow_Secondary", Float ) = 1
        _Rgh_Pow_Tertiary ("Rgh_Pow_Tertiary", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
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
            uniform fixed _Edge_Highlight;
            uniform fixed4 _Base_Colour;
            uniform fixed4 _Main_Colour;
            uniform fixed4 _Secondary_Colour;
            uniform fixed4 _Main_Light;
            uniform fixed4 _Secondary_Light;
            uniform fixed _Main_Light_Pow;
            uniform fixed _Secondary_Light_Pow;
            uniform fixed _Main_Light_Brightness;
            uniform fixed _Secondary_Light_Brightness;
            uniform fixed _AO_Power;
            uniform fixed _OverrideClipDistanceToGlobals;
            uniform fixed _LocalClipDistance;
            uniform fixed _GlobalClipDistance;
            uniform fixed4 _Tertiary_Colour;
            uniform sampler2D _T_Light; uniform float4 _T_Light_ST;
            uniform fixed _Rgh_Pow_Base;
            uniform fixed _Rgh_Pow_Primary;
            uniform fixed _Rgh_Pow_Secondary;
            uniform fixed _Rgh_Pow_Tertiary;
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
                fixed4 _T_Orm_var = tex2D(_T_Orm,TRANSFORM_TEX(i.uv0, _T_Orm));
                fixed4 _T_Mask_var = tex2D(_T_Mask,TRANSFORM_TEX(i.uv0, _T_Mask));
                fixed node_1171 = (1.0 - pow(_T_Orm_var.g,lerp(lerp(lerp(_Rgh_Pow_Base,_Rgh_Pow_Primary,_T_Mask_var.r),_Rgh_Pow_Secondary,_T_Mask_var.g),_Rgh_Pow_Tertiary,_T_Mask_var.b))); // Gloss
                fixed node_3445 = ((node_1171*-0.2+0.1)+((distance(float2(i.posWorld.r,i.posWorld.b),float2(_WorldSpaceCameraPos.r,_WorldSpaceCameraPos.b))+21.0)/lerp(_LocalClipDistance,(_GlobalClipDistance-0.01),_OverrideClipDistanceToGlobals)));
                clip(saturate((node_3445*-50.00005+50.00005)) - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = node_1171;
                float perceptualRoughness = 1.0 - node_1171;
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
                fixed4 _T_Light_var = tex2D(_T_Light,TRANSFORM_TEX(i.uv0, _T_Light));
                fixed node_114 = pow((lerp(1.0,(_T_Light_var.b*2.0),_Edge_Highlight)*_T_Orm_var.r),_AO_Power); // AO
                float3 specularAO = node_114;
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _T_Orm_var.b;
                float specularMonochrome;
                fixed4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffuseColor = (lerp(lerp(lerp(_Base_Colour.rgb,_Main_Colour.rgb,_T_Mask_var.r),_Secondary_Colour.rgb,_T_Mask_var.g),_Tertiary_Colour.rgb,_T_Mask_var.b)*_MainTex_var.rgb*_T_Orm_var.r); // Need this for specular when using metallic
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
                fixed node_8197 = saturate((node_3445*16.66667+-15.33333));
                float3 emissive = (((_Main_Light_Brightness*_Main_Light.rgb*pow(_T_Light_var.r,_Main_Light_Pow))+(_Secondary_Light_Brightness*_Secondary_Light.rgb*pow(_T_Light_var.g,_Secondary_Light_Pow)))+(fixed3(0.2,0.5,1)*node_8197*node_8197*node_8197));
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
            uniform fixed4 _Base_Colour;
            uniform fixed4 _Main_Colour;
            uniform fixed4 _Secondary_Colour;
            uniform fixed4 _Main_Light;
            uniform fixed4 _Secondary_Light;
            uniform fixed _Main_Light_Pow;
            uniform fixed _Secondary_Light_Pow;
            uniform fixed _Main_Light_Brightness;
            uniform fixed _Secondary_Light_Brightness;
            uniform fixed _OverrideClipDistanceToGlobals;
            uniform fixed _LocalClipDistance;
            uniform fixed _GlobalClipDistance;
            uniform fixed4 _Tertiary_Colour;
            uniform sampler2D _T_Light; uniform float4 _T_Light_ST;
            uniform fixed _Rgh_Pow_Base;
            uniform fixed _Rgh_Pow_Primary;
            uniform fixed _Rgh_Pow_Secondary;
            uniform fixed _Rgh_Pow_Tertiary;
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
                fixed4 _T_Orm_var = tex2D(_T_Orm,TRANSFORM_TEX(i.uv0, _T_Orm));
                fixed4 _T_Mask_var = tex2D(_T_Mask,TRANSFORM_TEX(i.uv0, _T_Mask));
                fixed node_1171 = (1.0 - pow(_T_Orm_var.g,lerp(lerp(lerp(_Rgh_Pow_Base,_Rgh_Pow_Primary,_T_Mask_var.r),_Rgh_Pow_Secondary,_T_Mask_var.g),_Rgh_Pow_Tertiary,_T_Mask_var.b))); // Gloss
                fixed node_3445 = ((node_1171*-0.2+0.1)+((distance(float2(i.posWorld.r,i.posWorld.b),float2(_WorldSpaceCameraPos.r,_WorldSpaceCameraPos.b))+21.0)/lerp(_LocalClipDistance,(_GlobalClipDistance-0.01),_OverrideClipDistanceToGlobals)));
                clip(saturate((node_3445*-50.00005+50.00005)) - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = node_1171;
                float perceptualRoughness = 1.0 - node_1171;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _T_Orm_var.b;
                float specularMonochrome;
                fixed4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffuseColor = (lerp(lerp(lerp(_Base_Colour.rgb,_Main_Colour.rgb,_T_Mask_var.r),_Secondary_Colour.rgb,_T_Mask_var.g),_Tertiary_Colour.rgb,_T_Mask_var.b)*_MainTex_var.rgb*_T_Orm_var.r); // Need this for specular when using metallic
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
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _T_Mask; uniform float4 _T_Mask_ST;
            uniform sampler2D _T_Orm; uniform float4 _T_Orm_ST;
            uniform fixed _OverrideClipDistanceToGlobals;
            uniform fixed _LocalClipDistance;
            uniform fixed _GlobalClipDistance;
            uniform fixed _Rgh_Pow_Base;
            uniform fixed _Rgh_Pow_Primary;
            uniform fixed _Rgh_Pow_Secondary;
            uniform fixed _Rgh_Pow_Tertiary;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float2 uv1 : TEXCOORD2;
                float2 uv2 : TEXCOORD3;
                float4 posWorld : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                fixed4 _T_Orm_var = tex2D(_T_Orm,TRANSFORM_TEX(i.uv0, _T_Orm));
                fixed4 _T_Mask_var = tex2D(_T_Mask,TRANSFORM_TEX(i.uv0, _T_Mask));
                fixed node_1171 = (1.0 - pow(_T_Orm_var.g,lerp(lerp(lerp(_Rgh_Pow_Base,_Rgh_Pow_Primary,_T_Mask_var.r),_Rgh_Pow_Secondary,_T_Mask_var.g),_Rgh_Pow_Tertiary,_T_Mask_var.b))); // Gloss
                fixed node_3445 = ((node_1171*-0.2+0.1)+((distance(float2(i.posWorld.r,i.posWorld.b),float2(_WorldSpaceCameraPos.r,_WorldSpaceCameraPos.b))+21.0)/lerp(_LocalClipDistance,(_GlobalClipDistance-0.01),_OverrideClipDistanceToGlobals)));
                clip(saturate((node_3445*-50.00005+50.00005)) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
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
            uniform fixed4 _Base_Colour;
            uniform fixed4 _Main_Colour;
            uniform fixed4 _Secondary_Colour;
            uniform fixed4 _Main_Light;
            uniform fixed4 _Secondary_Light;
            uniform fixed _Main_Light_Pow;
            uniform fixed _Secondary_Light_Pow;
            uniform fixed _Main_Light_Brightness;
            uniform fixed _Secondary_Light_Brightness;
            uniform fixed _OverrideClipDistanceToGlobals;
            uniform fixed _LocalClipDistance;
            uniform fixed _GlobalClipDistance;
            uniform fixed4 _Tertiary_Colour;
            uniform sampler2D _T_Light; uniform float4 _T_Light_ST;
            uniform fixed _Rgh_Pow_Base;
            uniform fixed _Rgh_Pow_Primary;
            uniform fixed _Rgh_Pow_Secondary;
            uniform fixed _Rgh_Pow_Tertiary;
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
                
                fixed4 _T_Light_var = tex2D(_T_Light,TRANSFORM_TEX(i.uv0, _T_Light));
                fixed4 _T_Orm_var = tex2D(_T_Orm,TRANSFORM_TEX(i.uv0, _T_Orm));
                fixed4 _T_Mask_var = tex2D(_T_Mask,TRANSFORM_TEX(i.uv0, _T_Mask));
                fixed node_1171 = (1.0 - pow(_T_Orm_var.g,lerp(lerp(lerp(_Rgh_Pow_Base,_Rgh_Pow_Primary,_T_Mask_var.r),_Rgh_Pow_Secondary,_T_Mask_var.g),_Rgh_Pow_Tertiary,_T_Mask_var.b))); // Gloss
                fixed node_3445 = ((node_1171*-0.2+0.1)+((distance(float2(i.posWorld.r,i.posWorld.b),float2(_WorldSpaceCameraPos.r,_WorldSpaceCameraPos.b))+21.0)/lerp(_LocalClipDistance,(_GlobalClipDistance-0.01),_OverrideClipDistanceToGlobals)));
                fixed node_8197 = saturate((node_3445*16.66667+-15.33333));
                o.Emission = (((_Main_Light_Brightness*_Main_Light.rgb*pow(_T_Light_var.r,_Main_Light_Pow))+(_Secondary_Light_Brightness*_Secondary_Light.rgb*pow(_T_Light_var.g,_Secondary_Light_Pow)))+(fixed3(0.2,0.5,1)*node_8197*node_8197*node_8197));
                
                fixed4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffColor = (lerp(lerp(lerp(_Base_Colour.rgb,_Main_Colour.rgb,_T_Mask_var.r),_Secondary_Colour.rgb,_T_Mask_var.g),_Tertiary_Colour.rgb,_T_Mask_var.b)*_MainTex_var.rgb*_T_Orm_var.r);
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _T_Orm_var.b, specColor, specularMonochrome );
                float roughness = 1.0 - node_1171;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
