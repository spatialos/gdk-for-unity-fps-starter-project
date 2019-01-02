// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.4980392,fgcg:0.598756,fgcb:0.7960785,fgca:1,fgde:0.005,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33684,y:33612,varname:node_2865,prsc:2|emission-2296-OUT;n:type:ShaderForge.SFN_TexCoord,id:4219,x:30293,y:33486,cmnt:Default coordinates,varname:node_4219,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Relay,id:8397,x:31861,y:33341,varname:node_8397,prsc:0|IN-4219-UVOUT;n:type:ShaderForge.SFN_Tex2dAsset,id:4430,x:31690,y:33035,ptovrint:False,ptlb:MainTex,ptin:_MainTex,cmnt:MainTex contains the color of the scene,varname:_MainTex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7542,x:32185,y:33215,varname:node_1672,prsc:0,ntxv:0,isnm:False|UVIN-8397-OUT,TEX-4430-TEX;n:type:ShaderForge.SFN_ValueProperty,id:900,x:30256,y:33863,ptovrint:False,ptlb:DAMAGE_YAW,ptin:_DAMAGE_YAW,varname:_DAMAGE_YAW,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Fmod,id:9514,x:30463,y:33852,varname:node_9514,prsc:2|A-900-OUT,B-217-OUT;n:type:ShaderForge.SFN_Divide,id:88,x:30652,y:33852,varname:node_88,prsc:0|A-9514-OUT,B-217-OUT;n:type:ShaderForge.SFN_Vector1,id:217,x:30270,y:33981,varname:node_217,prsc:0,v1:360;n:type:ShaderForge.SFN_Subtract,id:2872,x:31163,y:33818,varname:node_2872,prsc:2|A-2118-G,B-88-OUT;n:type:ShaderForge.SFN_Subtract,id:865,x:31163,y:33981,varname:node_865,prsc:2|A-2118-G,B-1954-OUT;n:type:ShaderForge.SFN_Subtract,id:8537,x:31163,y:34142,varname:node_8537,prsc:2|A-2118-G,B-3028-OUT;n:type:ShaderForge.SFN_Add,id:3028,x:30928,y:34091,varname:node_3028,prsc:2|A-88-OUT,B-391-OUT;n:type:ShaderForge.SFN_Add,id:1954,x:30944,y:33934,varname:node_1954,prsc:2|A-88-OUT,B-7524-OUT;n:type:ShaderForge.SFN_Vector1,id:391,x:30728,y:34115,varname:node_391,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:7524,x:30756,y:33955,varname:node_7524,prsc:2,v1:-1;n:type:ShaderForge.SFN_Abs,id:7244,x:31337,y:33818,varname:node_7244,prsc:2|IN-2872-OUT;n:type:ShaderForge.SFN_Abs,id:6335,x:31337,y:33981,varname:node_6335,prsc:2|IN-865-OUT;n:type:ShaderForge.SFN_Abs,id:9140,x:31337,y:34142,varname:node_9140,prsc:2|IN-8537-OUT;n:type:ShaderForge.SFN_Min,id:5284,x:31699,y:33868,varname:node_5284,prsc:2|A-9174-OUT,B-9424-OUT;n:type:ShaderForge.SFN_Min,id:7151,x:31835,y:34016,varname:node_7151,prsc:2|A-5284-OUT,B-5944-OUT;n:type:ShaderForge.SFN_Tex2d,id:2118,x:30762,y:33496,ptovrint:False,ptlb:Vignette Mask,ptin:_VignetteMask,varname:_VignetteMask,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-4219-UVOUT;n:type:ShaderForge.SFN_Multiply,id:659,x:32017,y:33993,varname:node_659,prsc:2|A-9518-OUT,B-7151-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9518,x:31872,y:33891,ptovrint:False,ptlb:DAMAGE_FOCUS,ptin:_DAMAGE_FOCUS,varname:_DAMAGE_FOCUS,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:16;n:type:ShaderForge.SFN_OneMinus,id:6763,x:32338,y:33993,varname:node_6763,prsc:2|IN-4995-OUT;n:type:ShaderForge.SFN_Clamp01,id:4995,x:32187,y:33993,varname:node_4995,prsc:2|IN-659-OUT;n:type:ShaderForge.SFN_Multiply,id:8155,x:32502,y:33977,varname:node_8155,prsc:2|A-3705-OUT,B-6763-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3705,x:32147,y:33733,ptovrint:False,ptlb:DAMAGE_INTENSITY,ptin:_DAMAGE_INTENSITY,varname:_DAMAGE_INTENSITY,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Power,id:5442,x:32933,y:33939,varname:node_5442,prsc:2|VAL-3485-OUT,EXP-2729-OUT;n:type:ShaderForge.SFN_Vector1,id:2729,x:32719,y:34091,varname:node_2729,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Clamp01,id:9174,x:31518,y:33818,varname:node_9174,prsc:2|IN-7244-OUT;n:type:ShaderForge.SFN_Clamp01,id:9424,x:31518,y:33981,varname:node_9424,prsc:2|IN-6335-OUT;n:type:ShaderForge.SFN_Clamp01,id:5944,x:31518,y:34142,varname:node_5944,prsc:2|IN-9140-OUT;n:type:ShaderForge.SFN_Lerp,id:324,x:33283,y:33463,varname:node_324,prsc:2|A-9333-OUT,B-1861-OUT,T-2586-OUT;n:type:ShaderForge.SFN_Lerp,id:2296,x:33421,y:33580,varname:node_2296,prsc:2|A-324-OUT,B-1861-OUT,T-7113-OUT;n:type:ShaderForge.SFN_Vector3,id:1861,x:32688,y:33603,varname:node_1861,prsc:0,v1:1,v2:0,v3:0;n:type:ShaderForge.SFN_Multiply,id:303,x:33094,y:33721,varname:node_303,prsc:2|A-6794-OUT,B-5442-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2307,x:30834,y:33249,ptovrint:False,ptlb:HEALTH_VALUE,ptin:_HEALTH_VALUE,varname:_HEALTH_VALUE,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Clamp01,id:7587,x:31002,y:33205,varname:node_7587,prsc:2|IN-2307-OUT;n:type:ShaderForge.SFN_RemapRange,id:4751,x:31377,y:33226,varname:node_4751,prsc:2,frmn:0,frmx:1,tomn:0.75,tomx:20|IN-7088-OUT;n:type:ShaderForge.SFN_Power,id:7088,x:31183,y:33205,varname:node_7088,prsc:0|VAL-7587-OUT,EXP-6911-OUT;n:type:ShaderForge.SFN_Power,id:2586,x:31684,y:33460,varname:node_2586,prsc:2|VAL-3506-OUT,EXP-4751-OUT;n:type:ShaderForge.SFN_Vector1,id:6911,x:31037,y:33340,varname:node_6911,prsc:2,v1:4;n:type:ShaderForge.SFN_Vector1,id:6384,x:32425,y:33730,varname:node_6384,prsc:2,v1:8;n:type:ShaderForge.SFN_Vector1,id:2282,x:30774,y:33109,varname:node_2282,prsc:2,v1:1;n:type:ShaderForge.SFN_Power,id:3506,x:31210,y:33452,varname:node_3506,prsc:2|VAL-2118-R,EXP-3920-OUT;n:type:ShaderForge.SFN_Vector1,id:3920,x:31002,y:33496,varname:node_3920,prsc:2,v1:2;n:type:ShaderForge.SFN_Power,id:6794,x:31599,y:33631,varname:node_6794,prsc:2|VAL-2118-B,EXP-7657-OUT;n:type:ShaderForge.SFN_Vector1,id:7657,x:31086,y:33635,varname:node_7657,prsc:2,v1:1;n:type:ShaderForge.SFN_Power,id:7113,x:33248,y:33744,varname:node_7113,prsc:2|VAL-303-OUT,EXP-3935-OUT;n:type:ShaderForge.SFN_Vector1,id:3935,x:33094,y:33886,varname:node_3935,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Add,id:263,x:32230,y:32871,varname:node_263,prsc:2|A-7542-R,B-7542-G;n:type:ShaderForge.SFN_Add,id:7238,x:32400,y:32871,varname:node_7238,prsc:2|A-263-OUT,B-7542-B;n:type:ShaderForge.SFN_Multiply,id:4436,x:32543,y:32871,varname:node_4436,prsc:2|A-7238-OUT,B-3744-OUT;n:type:ShaderForge.SFN_Vector1,id:3744,x:32298,y:33003,varname:node_3744,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Vector1,id:1310,x:32460,y:33103,varname:node_1310,prsc:2,v1:1.25;n:type:ShaderForge.SFN_Power,id:2322,x:32643,y:33029,varname:node_2322,prsc:0|VAL-4436-OUT,EXP-1310-OUT;n:type:ShaderForge.SFN_Lerp,id:9333,x:33112,y:33336,varname:node_9333,prsc:2|A-7943-OUT,B-7542-RGB,T-4422-OUT;n:type:ShaderForge.SFN_Append,id:1759,x:32812,y:33029,varname:node_1759,prsc:2|A-2322-OUT,B-2322-OUT;n:type:ShaderForge.SFN_Append,id:7943,x:32958,y:33208,varname:node_7943,prsc:2|A-1759-OUT,B-2322-OUT;n:type:ShaderForge.SFN_Power,id:4422,x:32688,y:33363,varname:node_4422,prsc:2|VAL-7088-OUT,EXP-9221-OUT;n:type:ShaderForge.SFN_Vector1,id:9221,x:32516,y:33433,varname:node_9221,prsc:2,v1:0.15;n:type:ShaderForge.SFN_Clamp01,id:3485,x:32778,y:33949,varname:node_3485,prsc:2|IN-268-OUT;n:type:ShaderForge.SFN_Multiply,id:268,x:32640,y:33977,varname:node_268,prsc:2|A-6384-OUT,B-8155-OUT;proporder:4430-2118-900-9518-3705-2307;pass:END;sub:END;*/

Shader "Spatial/Spatial_PostVignette" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _VignetteMask ("Vignette Mask", 2D) = "white" {}
        _DAMAGE_YAW ("DAMAGE_YAW", Float ) = 0
        _DAMAGE_FOCUS ("DAMAGE_FOCUS", Float ) = 16
        _DAMAGE_INTENSITY ("DAMAGE_INTENSITY", Float ) = 1
        _HEALTH_VALUE ("HEALTH_VALUE", Float ) = 1
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Overlay+1"
            "RenderType"="Overlay"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZTest Always
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform fixed _DAMAGE_YAW;
            uniform sampler2D _VignetteMask; uniform float4 _VignetteMask_ST;
            uniform fixed _DAMAGE_FOCUS;
            uniform fixed _DAMAGE_INTENSITY;
            uniform fixed _HEALTH_VALUE;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                fixed2 node_8397 = i.uv0;
                fixed4 node_1672 = tex2D(_MainTex,TRANSFORM_TEX(node_8397, _MainTex));
                fixed node_2322 = pow((((node_1672.r+node_1672.g)+node_1672.b)*0.3),1.25);
                fixed node_7088 = pow(saturate(_HEALTH_VALUE),4.0);
                fixed3 node_1861 = fixed3(1,0,0);
                fixed4 _VignetteMask_var = tex2D(_VignetteMask,TRANSFORM_TEX(i.uv0, _VignetteMask));
                fixed node_217 = 360.0;
                fixed node_88 = (fmod(_DAMAGE_YAW,node_217)/node_217);
                float3 emissive = lerp(lerp(lerp(float3(float2(node_2322,node_2322),node_2322),node_1672.rgb,pow(node_7088,0.15)),node_1861,pow(pow(_VignetteMask_var.r,2.0),(node_7088*19.25+0.75))),node_1861,pow((pow(_VignetteMask_var.b,1.0)*pow(saturate((8.0*(_DAMAGE_INTENSITY*(1.0 - saturate((_DAMAGE_FOCUS*min(min(saturate(abs((_VignetteMask_var.g-node_88))),saturate(abs((_VignetteMask_var.g-(node_88+(-1.0)))))),saturate(abs((_VignetteMask_var.g-(node_88+1.0))))))))))),0.5)),1.5));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
