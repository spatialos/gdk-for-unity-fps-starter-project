// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:34274,y:32723,varname:node_3138,prsc:2|emission-7690-OUT;n:type:ShaderForge.SFN_TexCoord,id:6037,x:30632,y:32929,varname:node_6037,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:1164,x:30916,y:33091,varname:node_1164,prsc:2|A-6037-UVOUT,B-3489-OUT;n:type:ShaderForge.SFN_Multiply,id:568,x:30903,y:32660,varname:node_568,prsc:2|A-9853-OUT,B-6037-UVOUT;n:type:ShaderForge.SFN_Vector1,id:9853,x:30723,y:32660,varname:node_9853,prsc:2,v1:8;n:type:ShaderForge.SFN_Vector1,id:3489,x:30733,y:33109,varname:node_3489,prsc:2,v1:4;n:type:ShaderForge.SFN_Tex2d,id:9904,x:31438,y:32660,ptovrint:False,ptlb:T_Mask,ptin:_T_Mask,varname:_T_Mask,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:90518c47da7692148b1512cad7e57508,ntxv:0,isnm:False|UVIN-2850-OUT;n:type:ShaderForge.SFN_Divide,id:4855,x:31014,y:32875,varname:node_4855,prsc:2|A-5027-OUT,B-4065-OUT;n:type:ShaderForge.SFN_Vector1,id:5027,x:30834,y:32875,varname:node_5027,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:4065,x:30834,y:32937,varname:node_4065,prsc:2,v1:64;n:type:ShaderForge.SFN_Add,id:2850,x:31233,y:32659,varname:node_2850,prsc:0|A-568-OUT,B-4855-OUT;n:type:ShaderForge.SFN_Multiply,id:7414,x:31157,y:33091,varname:node_7414,prsc:2|A-1164-OUT,B-8632-OUT;n:type:ShaderForge.SFN_Vector1,id:8632,x:30964,y:33238,varname:node_8632,prsc:0,v1:64;n:type:ShaderForge.SFN_Round,id:6868,x:31349,y:33091,varname:node_6868,prsc:2|IN-7414-OUT;n:type:ShaderForge.SFN_Divide,id:4389,x:31544,y:33091,varname:node_4389,prsc:0|A-6868-OUT,B-8632-OUT;n:type:ShaderForge.SFN_Add,id:265,x:31722,y:33091,varname:node_265,prsc:0|A-4389-OUT,B-1125-OUT;n:type:ShaderForge.SFN_Time,id:3512,x:31134,y:33311,varname:node_3512,prsc:1;n:type:ShaderForge.SFN_Vector2,id:5274,x:31357,y:33257,varname:node_5274,prsc:0,v1:0.002,v2:-0.005;n:type:ShaderForge.SFN_Vector2,id:5930,x:31357,y:33376,varname:node_5930,prsc:0,v1:-0.007,v2:0.004;n:type:ShaderForge.SFN_Multiply,id:1125,x:31544,y:33257,varname:node_1125,prsc:2|A-5274-OUT,B-3512-T;n:type:ShaderForge.SFN_Multiply,id:2121,x:31544,y:33376,varname:node_2121,prsc:2|A-5930-OUT,B-3512-T;n:type:ShaderForge.SFN_Add,id:5010,x:31722,y:33257,varname:node_5010,prsc:0|A-4389-OUT,B-2121-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:8293,x:31722,y:32914,ptovrint:False,ptlb:T_Main,ptin:_T_Main,varname:_T_Main,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:80e154422909f004584cc3eecc1f320b,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:645,x:32039,y:33086,varname:_node_645,prsc:0,tex:80e154422909f004584cc3eecc1f320b,ntxv:0,isnm:False|UVIN-265-OUT,TEX-8293-TEX;n:type:ShaderForge.SFN_Tex2d,id:4044,x:32039,y:33264,varname:_node_5069,prsc:0,tex:80e154422909f004584cc3eecc1f320b,ntxv:0,isnm:False|UVIN-5010-OUT,TEX-8293-TEX;n:type:ShaderForge.SFN_Multiply,id:7592,x:32255,y:33159,varname:node_7592,prsc:2|A-645-RGB,B-4044-RGB;n:type:ShaderForge.SFN_Multiply,id:9738,x:32439,y:33159,varname:node_9738,prsc:2|A-7592-OUT,B-4856-OUT;n:type:ShaderForge.SFN_Vector3,id:4856,x:32255,y:33299,varname:node_4856,prsc:2,v1:1,v2:1,v3:3;n:type:ShaderForge.SFN_ComponentMask,id:3951,x:31657,y:32660,varname:node_3951,prsc:0,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-9904-RGB;n:type:ShaderForge.SFN_Multiply,id:3572,x:32874,y:32673,varname:node_3572,prsc:2|A-4555-OUT,B-3951-G;n:type:ShaderForge.SFN_Add,id:1735,x:33090,y:32828,varname:node_1735,prsc:2|A-3572-OUT,B-9738-OUT;n:type:ShaderForge.SFN_Multiply,id:920,x:31638,y:32483,varname:node_920,prsc:2|A-3512-T,B-1353-OUT;n:type:ShaderForge.SFN_Vector1,id:1353,x:31445,y:32505,varname:node_1353,prsc:2,v1:0.1;n:type:ShaderForge.SFN_OneMinus,id:3486,x:31929,y:32703,varname:node_3486,prsc:2|IN-3951-G;n:type:ShaderForge.SFN_Subtract,id:2769,x:31812,y:32483,varname:node_2769,prsc:2|A-920-OUT,B-3951-R;n:type:ShaderForge.SFN_Abs,id:1606,x:31975,y:32483,varname:node_1606,prsc:2|IN-2769-OUT;n:type:ShaderForge.SFN_Fmod,id:2799,x:32156,y:32483,varname:node_2799,prsc:2|A-1606-OUT,B-9764-OUT;n:type:ShaderForge.SFN_Vector1,id:9764,x:31975,y:32625,varname:node_9764,prsc:2,v1:1;n:type:ShaderForge.SFN_OneMinus,id:7655,x:32334,y:32483,varname:node_7655,prsc:2|IN-2799-OUT;n:type:ShaderForge.SFN_Power,id:7039,x:32508,y:32483,varname:node_7039,prsc:2|VAL-7655-OUT,EXP-7412-OUT;n:type:ShaderForge.SFN_Vector1,id:7412,x:32289,y:32643,varname:node_7412,prsc:2,v1:12;n:type:ShaderForge.SFN_Clamp01,id:4555,x:32691,y:32483,varname:node_4555,prsc:2|IN-7039-OUT;n:type:ShaderForge.SFN_Multiply,id:7690,x:33930,y:32821,varname:node_7690,prsc:2|A-1735-OUT,B-9633-OUT;n:type:ShaderForge.SFN_ViewPosition,id:6541,x:32867,y:33303,varname:node_6541,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:1123,x:32867,y:33142,varname:node_1123,prsc:2;n:type:ShaderForge.SFN_Subtract,id:3889,x:33026,y:33178,varname:node_3889,prsc:2|A-1123-Y,B-6541-Y;n:type:ShaderForge.SFN_RemapRange,id:7175,x:33205,y:33178,varname:node_7175,prsc:2,frmn:100,frmx:250,tomn:0,tomx:1|IN-3889-OUT;n:type:ShaderForge.SFN_Clamp01,id:6744,x:33371,y:33178,varname:node_6744,prsc:2|IN-7175-OUT;n:type:ShaderForge.SFN_Multiply,id:9633,x:33545,y:33164,varname:node_9633,prsc:2|A-6744-OUT,B-6744-OUT;proporder:8293-9904;pass:END;sub:END;*/

Shader "Spatial/Spatial_Clouds_Master" {
    Properties {
        _T_Main ("T_Main", 2D) = "white" {}
        _T_Mask ("T_Mask", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _T_Mask; uniform float4 _T_Mask_ST;
            uniform sampler2D _T_Main; uniform float4 _T_Main_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                half4 node_3512 = _Time;
                fixed2 node_2850 = ((8.0*i.uv0)+(1.0/64.0));
                fixed4 _T_Mask_var = tex2D(_T_Mask,TRANSFORM_TEX(node_2850, _T_Mask));
                fixed2 node_3951 = _T_Mask_var.rgb.rg;
                fixed node_8632 = 64.0;
                fixed2 node_4389 = (round(((i.uv0*4.0)*node_8632))/node_8632);
                fixed2 node_265 = (node_4389+(fixed2(0.002,-0.005)*node_3512.g));
                fixed4 _node_645 = tex2D(_T_Main,TRANSFORM_TEX(node_265, _T_Main));
                fixed2 node_5010 = (node_4389+(fixed2(-0.007,0.004)*node_3512.g));
                fixed4 _node_5069 = tex2D(_T_Main,TRANSFORM_TEX(node_5010, _T_Main));
                float node_6744 = saturate(((i.posWorld.g-_WorldSpaceCameraPos.g)*0.006666667+-0.6666667));
                float3 emissive = (((saturate(pow((1.0 - fmod(abs(((node_3512.g*0.1)-node_3951.r)),1.0)),12.0))*node_3951.g)+((_node_645.rgb*_node_5069.rgb)*float3(1,1,3)))*(node_6744*node_6744));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
