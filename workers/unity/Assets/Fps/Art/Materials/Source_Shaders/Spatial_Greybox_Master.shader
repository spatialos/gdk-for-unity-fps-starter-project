// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.8156863,fgcg:0.9254902,fgcb:0.972549,fgca:1,fgde:0.0005,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33435,y:32219,varname:node_2865,prsc:2|diff-6331-OUT,spec-276-OUT,gloss-6073-OUT,normal-7087-OUT,emission-3352-OUT,amdfl-5123-RGB,amspl-5123-RGB,clip-4659-OUT;n:type:ShaderForge.SFN_Tex2d,id:278,x:31359,y:31990,ptovrint:False,ptlb:T_Sides,ptin:_T_Sides,varname:_T_Sides,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a9a9ff12953a10e4e98a9793d3dd3b46,ntxv:0,isnm:False|UVIN-797-OUT;n:type:ShaderForge.SFN_ObjectScale,id:707,x:27734,y:31223,varname:node_707,prsc:2,rcp:False;n:type:ShaderForge.SFN_Divide,id:8857,x:30138,y:32077,varname:node_8857,prsc:2|A-3793-OUT,B-4935-OUT;n:type:ShaderForge.SFN_Append,id:6373,x:30604,y:32281,cmnt:Z Axis,varname:node_6373,prsc:2|A-8309-R,B-8309-G;n:type:ShaderForge.SFN_Vector1,id:4935,x:29745,y:32126,varname:node_4935,prsc:2,v1:4;n:type:ShaderForge.SFN_TexCoord,id:5838,x:30967,y:32140,varname:node_5838,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:797,x:31167,y:32007,cmnt:Scaled UVs,varname:node_797,prsc:0|A-262-OUT,B-5838-UVOUT;n:type:ShaderForge.SFN_ComponentMask,id:8309,x:30372,y:32077,varname:node_8309,prsc:0,cc1:0,cc2:1,cc3:2,cc4:-1|IN-8857-OUT;n:type:ShaderForge.SFN_ComponentMask,id:4146,x:30604,y:31594,cmnt:Local axis,varname:node_4146,prsc:0,cc1:0,cc2:1,cc3:2,cc4:-1|IN-609-OUT;n:type:ShaderForge.SFN_Lerp,id:4167,x:31551,y:31990,cmnt:Blend sides and top,varname:node_4167,prsc:2|A-278-RGB,B-9689-RGB,T-4146-G;n:type:ShaderForge.SFN_Append,id:7602,x:30604,y:32057,cmnt:Y Axis,varname:node_7602,prsc:0|A-8309-R,B-8309-B;n:type:ShaderForge.SFN_Append,id:4653,x:30604,y:31855,cmnt:X Axis,varname:node_4653,prsc:0|A-8309-B,B-8309-G;n:type:ShaderForge.SFN_Lerp,id:262,x:30950,y:32007,varname:node_262,prsc:2|A-2082-OUT,B-6373-OUT,T-4146-B;n:type:ShaderForge.SFN_Tex2d,id:5139,x:31359,y:32544,ptovrint:False,ptlb:T_Normal,ptin:_T_Normal,varname:_T_Normal,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:fb45312733a26e0448d9d840bfec28ae,ntxv:3,isnm:True|UVIN-797-OUT;n:type:ShaderForge.SFN_AmbientLight,id:5123,x:33123,y:32352,varname:node_5123,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6331,x:32384,y:32160,varname:node_6331,prsc:2|A-5665-R,B-8350-OUT;n:type:ShaderForge.SFN_Tex2d,id:9689,x:31358,y:32270,ptovrint:False,ptlb:T_Top,ptin:_T_Top,varname:_T_Top,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:2823495f2a12c9640914e706d38c0230,ntxv:0,isnm:False|UVIN-1666-OUT;n:type:ShaderForge.SFN_Multiply,id:1666,x:31166,y:32270,cmnt:Top Scaled UVs,varname:node_1666,prsc:0|A-7602-OUT,B-5838-UVOUT;n:type:ShaderForge.SFN_ComponentMask,id:5665,x:31943,y:31989,cmnt:Unpack channels,varname:node_5665,prsc:0,cc1:0,cc2:1,cc3:2,cc4:-1|IN-1243-OUT;n:type:ShaderForge.SFN_Lerp,id:2082,x:30806,y:31905,varname:node_2082,prsc:2|A-4653-OUT,B-7602-OUT,T-4146-G;n:type:ShaderForge.SFN_Color,id:7615,x:31769,y:32207,ptovrint:False,ptlb:Sides_Colour,ptin:_Sides_Colour,varname:_Sides_Colour,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:4493,x:31900,y:32281,ptovrint:False,ptlb:Top_Colour,ptin:_Top_Colour,varname:_Top_Colour,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1,c2:0.1,c3:0.1,c4:1;n:type:ShaderForge.SFN_Lerp,id:8350,x:32172,y:32269,varname:node_8350,prsc:2|A-7615-RGB,B-4493-RGB,T-5665-B;n:type:ShaderForge.SFN_Subtract,id:3705,x:29934,y:31112,varname:node_3705,prsc:2|A-6426-B,B-6426-G;n:type:ShaderForge.SFN_Subtract,id:5315,x:29934,y:31228,varname:node_5315,prsc:2|A-6426-R,B-6426-B;n:type:ShaderForge.SFN_Sign,id:4691,x:30175,y:31110,varname:node_4691,prsc:2|IN-3705-OUT;n:type:ShaderForge.SFN_Clamp01,id:6002,x:30345,y:31110,varname:node_6002,prsc:2|IN-4691-OUT;n:type:ShaderForge.SFN_Sign,id:9032,x:30176,y:31237,varname:node_9032,prsc:2|IN-5315-OUT;n:type:ShaderForge.SFN_Clamp01,id:4315,x:30345,y:31237,varname:node_4315,prsc:2|IN-9032-OUT;n:type:ShaderForge.SFN_Subtract,id:5660,x:29935,y:31364,varname:node_5660,prsc:2|A-6426-R,B-6426-G;n:type:ShaderForge.SFN_Sign,id:5233,x:30176,y:31373,varname:node_5233,prsc:2|IN-5660-OUT;n:type:ShaderForge.SFN_Clamp01,id:7673,x:30345,y:31373,varname:node_7673,prsc:2|IN-5233-OUT;n:type:ShaderForge.SFN_TexCoord,id:6089,x:30121,y:30747,varname:node_6089,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:8375,x:31352,y:31281,ptovrint:False,ptlb:T_Trim,ptin:_T_Trim,varname:_T_Trim,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:08e75be191d1c864d9c4d71cd33ad202,ntxv:0,isnm:False|UVIN-8926-OUT;n:type:ShaderForge.SFN_Tex2d,id:1223,x:31350,y:31556,ptovrint:False,ptlb:T_Trim_Normal,ptin:_T_Trim_Normal,varname:_T_Trim_Normal,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0532a8e9332a89b42a84ead3b458b33e,ntxv:3,isnm:True|UVIN-8926-OUT;n:type:ShaderForge.SFN_Append,id:3691,x:30306,y:30935,cmnt:90 degree rotated UVs,varname:node_3691,prsc:0|A-6089-V,B-6089-U;n:type:ShaderForge.SFN_Lerp,id:1226,x:30613,y:30863,varname:node_1226,prsc:0|A-6089-UVOUT,B-3691-OUT,T-6002-OUT;n:type:ShaderForge.SFN_Lerp,id:9186,x:30613,y:31010,varname:node_9186,prsc:0|A-6089-UVOUT,B-3691-OUT,T-4315-OUT;n:type:ShaderForge.SFN_Lerp,id:5696,x:30613,y:31140,varname:node_5696,prsc:0|A-6089-UVOUT,B-3691-OUT,T-7673-OUT;n:type:ShaderForge.SFN_Add,id:8926,x:31003,y:30988,cmnt:Calculate Trim UVs based on object scale,varname:node_8926,prsc:0|A-6302-OUT,B-3037-OUT,C-4771-OUT;n:type:ShaderForge.SFN_Multiply,id:6302,x:30793,y:30863,varname:node_6302,prsc:2|A-1226-OUT,B-4146-R;n:type:ShaderForge.SFN_Multiply,id:3037,x:30793,y:31010,varname:node_3037,prsc:2|A-9186-OUT,B-4146-G;n:type:ShaderForge.SFN_Multiply,id:4771,x:30793,y:31140,varname:node_4771,prsc:2|A-5696-OUT,B-4146-B;n:type:ShaderForge.SFN_Lerp,id:4947,x:32190,y:32517,varname:node_4947,prsc:2|A-70-OUT,B-810-OUT,T-3882-OUT;n:type:ShaderForge.SFN_Lerp,id:1243,x:31769,y:31989,cmnt:Blend trim,varname:node_1243,prsc:2|A-8375-RGB,B-4167-OUT,T-3882-OUT;n:type:ShaderForge.SFN_Multiply,id:4124,x:30650,y:32534,varname:node_4124,prsc:2|A-1164-OUT,B-3244-OUT;n:type:ShaderForge.SFN_ComponentMask,id:1646,x:30803,y:32534,varname:node_1646,prsc:0,cc1:0,cc2:1,cc3:2,cc4:-1|IN-4124-OUT;n:type:ShaderForge.SFN_Max,id:8897,x:30984,y:32544,varname:node_8897,prsc:2|A-1646-R,B-1646-G,C-1646-B;n:type:ShaderForge.SFN_OneMinus,id:3882,x:31182,y:32469,cmnt:Trim Mask,varname:node_3882,prsc:0|IN-8897-OUT;n:type:ShaderForge.SFN_OneMinus,id:1164,x:30443,y:32418,varname:node_1164,prsc:2|IN-609-OUT;n:type:ShaderForge.SFN_RemapRange,id:7950,x:29914,y:32546,cmnt:Add trim texture to thin edges,varname:node_7950,prsc:2,frmn:0.5,frmx:0.55,tomn:1,tomx:0|IN-3793-OUT;n:type:ShaderForge.SFN_Clamp01,id:3244,x:30443,y:32554,varname:node_3244,prsc:2|IN-7950-OUT;n:type:ShaderForge.SFN_Relay,id:70,x:31582,y:32426,varname:node_70,prsc:2|IN-1223-RGB;n:type:ShaderForge.SFN_Relay,id:810,x:31582,y:32465,varname:node_810,prsc:2|IN-5139-RGB;n:type:ShaderForge.SFN_Lerp,id:276,x:32491,y:31940,varname:node_276,prsc:2|A-6805-OUT,B-9214-OUT,T-5665-B;n:type:ShaderForge.SFN_ValueProperty,id:6805,x:32230,y:31804,ptovrint:False,ptlb:Metal_Sides,ptin:_Metal_Sides,varname:_Metal_Sides,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:9214,x:32230,y:31930,ptovrint:False,ptlb:Metal_Top,ptin:_Metal_Top,varname:_Metal_Top,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Normalize,id:7087,x:32385,y:32517,varname:node_7087,prsc:2|IN-4947-OUT;n:type:ShaderForge.SFN_ComponentMask,id:6426,x:29330,y:31111,varname:node_6426,prsc:0,cc1:0,cc2:1,cc3:2,cc4:-1|IN-3793-OUT;n:type:ShaderForge.SFN_Lerp,id:3793,x:29146,y:31370,cmnt:Toggle between object scale or VTX RGB if scale is reset,varname:node_3793,prsc:0|A-707-XYZ,B-5105-OUT,T-947-OUT;n:type:ShaderForge.SFN_VertexColor,id:1210,x:27721,y:31471,varname:node_1210,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8367,x:28122,y:31482,cmnt:VTX RGB stored as 8 bit values by combine script. Reverting to original size,varname:node_8367,prsc:0|A-1210-RGB,B-7056-OUT;n:type:ShaderForge.SFN_Vector1,id:7056,x:27942,y:31568,varname:node_7056,prsc:2,v1:36;n:type:ShaderForge.SFN_Vector3,id:1211,x:27813,y:30879,varname:node_1211,prsc:0,v1:1,v2:1,v3:1;n:type:ShaderForge.SFN_Multiply,id:5274,x:28440,y:31489,varname:node_5274,prsc:2|A-8367-OUT,B-2763-OUT;n:type:ShaderForge.SFN_Vector1,id:2763,x:28196,y:31739,varname:node_2763,prsc:0,v1:4;n:type:ShaderForge.SFN_Divide,id:5105,x:28854,y:31489,cmnt:Round scale to nearest 0.25,varname:node_5105,prsc:2|A-8181-OUT,B-2763-OUT;n:type:ShaderForge.SFN_Round,id:8181,x:28663,y:31489,varname:node_8181,prsc:2|IN-5274-OUT;n:type:ShaderForge.SFN_Subtract,id:8771,x:27990,y:31024,varname:node_8771,prsc:2|A-1211-OUT,B-707-XYZ;n:type:ShaderForge.SFN_Abs,id:2019,x:28157,y:31024,varname:node_2019,prsc:2|IN-8771-OUT;n:type:ShaderForge.SFN_ComponentMask,id:5070,x:28323,y:31024,varname:node_5070,prsc:0,cc1:0,cc2:1,cc3:2,cc4:-1|IN-2019-OUT;n:type:ShaderForge.SFN_Add,id:8274,x:28492,y:31034,varname:node_8274,prsc:2|A-5070-R,B-5070-G,C-5070-B;n:type:ShaderForge.SFN_Multiply,id:8963,x:28650,y:31034,varname:node_8963,prsc:2|A-8274-OUT,B-8022-OUT;n:type:ShaderForge.SFN_Vector1,id:8022,x:28467,y:31172,varname:node_8022,prsc:2,v1:100;n:type:ShaderForge.SFN_Clamp01,id:9512,x:28816,y:31050,varname:node_9512,prsc:2|IN-8963-OUT;n:type:ShaderForge.SFN_OneMinus,id:947,x:28974,y:31196,varname:node_947,prsc:2|IN-9512-OUT;n:type:ShaderForge.SFN_Multiply,id:3646,x:28305,y:31931,varname:node_3646,prsc:2|A-9824-OUT,B-9754-OUT;n:type:ShaderForge.SFN_OneMinus,id:8645,x:28537,y:31931,varname:node_8645,prsc:2|IN-3646-OUT;n:type:ShaderForge.SFN_Vector1,id:9754,x:28125,y:31994,varname:node_9754,prsc:2,v1:100;n:type:ShaderForge.SFN_Clamp01,id:569,x:28752,y:31931,varname:node_569,prsc:2|IN-8645-OUT;n:type:ShaderForge.SFN_Multiply,id:7611,x:28537,y:32075,varname:node_7611,prsc:0|A-956-OUT,B-494-OUT;n:type:ShaderForge.SFN_Clamp01,id:2270,x:28752,y:32108,varname:node_2270,prsc:2|IN-7611-OUT;n:type:ShaderForge.SFN_Vector1,id:494,x:28372,y:32201,varname:node_494,prsc:2,v1:2;n:type:ShaderForge.SFN_Subtract,id:956,x:28203,y:32089,varname:node_956,prsc:0|A-9824-OUT,B-3152-OUT;n:type:ShaderForge.SFN_Vector1,id:3152,x:28039,y:32123,varname:node_3152,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Relay,id:9824,x:27961,y:31872,cmnt:VTX Alpha 0isX 1isZ  0.5isY,varname:node_9824,prsc:0|IN-1210-A;n:type:ShaderForge.SFN_Abs,id:1840,x:28372,y:32291,varname:node_1840,prsc:2|IN-956-OUT;n:type:ShaderForge.SFN_Multiply,id:8232,x:28554,y:32291,varname:node_8232,prsc:0|A-1840-OUT,B-385-OUT;n:type:ShaderForge.SFN_Vector1,id:385,x:28352,y:32427,varname:node_385,prsc:2,v1:100;n:type:ShaderForge.SFN_Clamp01,id:8939,x:28904,y:32288,varname:node_8939,prsc:2|IN-6304-OUT;n:type:ShaderForge.SFN_Append,id:5742,x:29047,y:32056,varname:node_5742,prsc:0|A-569-OUT,B-2270-OUT,C-8939-OUT;n:type:ShaderForge.SFN_OneMinus,id:6304,x:28726,y:32288,varname:node_6304,prsc:2|IN-8232-OUT;n:type:ShaderForge.SFN_RemapRange,id:1678,x:29247,y:32056,varname:node_1678,prsc:2,frmn:0.1,frmx:0.8,tomn:0,tomx:1|IN-5742-OUT;n:type:ShaderForge.SFN_Clamp01,id:609,x:29459,y:32056,cmnt:Define local XYZ axis from VTX Alpha,varname:node_609,prsc:0|IN-1678-OUT;n:type:ShaderForge.SFN_Relay,id:6073,x:32364,y:32282,varname:node_6073,prsc:0|IN-5665-G;n:type:ShaderForge.SFN_FragmentPosition,id:1475,x:31541,y:32575,varname:node_1475,prsc:2;n:type:ShaderForge.SFN_ViewPosition,id:4880,x:31541,y:32694,varname:node_4880,prsc:2;n:type:ShaderForge.SFN_Distance,id:9899,x:31945,y:32671,varname:node_9899,prsc:2|A-4341-OUT,B-4384-OUT;n:type:ShaderForge.SFN_Append,id:4341,x:31779,y:32585,varname:node_4341,prsc:2|A-1475-X,B-1475-Z;n:type:ShaderForge.SFN_Append,id:4384,x:31779,y:32710,varname:node_4384,prsc:2|A-4880-X,B-4880-Z;n:type:ShaderForge.SFN_RemapRange,id:9414,x:32802,y:32746,varname:node_9414,prsc:2,frmn:0.98,frmx:1,tomn:1,tomx:0|IN-9871-OUT;n:type:ShaderForge.SFN_Clamp01,id:4659,x:32954,y:32746,varname:node_4659,prsc:0|IN-9414-OUT;n:type:ShaderForge.SFN_Divide,id:3767,x:32317,y:32863,cmnt:Clip objects by radial distance,varname:node_3767,prsc:0|A-9612-OUT,B-9556-OUT;n:type:ShaderForge.SFN_RemapRange,id:9859,x:32802,y:32556,varname:node_9859,prsc:2,frmn:0.92,frmx:0.98,tomn:0,tomx:1|IN-9871-OUT;n:type:ShaderForge.SFN_Clamp01,id:6135,x:32954,y:32556,varname:node_6135,prsc:0|IN-9859-OUT;n:type:ShaderForge.SFN_Add,id:9871,x:32648,y:32734,varname:node_9871,prsc:0|A-2508-OUT,B-3767-OUT;n:type:ShaderForge.SFN_RemapRange,id:2508,x:32497,y:32683,varname:node_2508,prsc:2,frmn:0,frmx:1,tomn:0.1,tomx:-0.1|IN-6073-OUT;n:type:ShaderForge.SFN_Multiply,id:3352,x:33123,y:32498,varname:node_3352,prsc:2|A-1976-OUT,B-6135-OUT,C-6135-OUT,D-6135-OUT;n:type:ShaderForge.SFN_Vector3,id:1976,x:32932,y:32431,varname:node_1976,prsc:0,v1:0.2,v2:0.5,v3:1;n:type:ShaderForge.SFN_ToggleProperty,id:6995,x:31857,y:33211,ptovrint:False,ptlb:OverrideClipDistanceToGlobals,ptin:_OverrideClipDistanceToGlobals,varname:_OverrideClipDistanceToGlobals,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False;n:type:ShaderForge.SFN_ValueProperty,id:1805,x:31616,y:32933,ptovrint:False,ptlb:LocalClipDistance,ptin:_LocalClipDistance,varname:_LocalClipDistance,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:200;n:type:ShaderForge.SFN_ValueProperty,id:7715,x:31438,y:33105,ptovrint:False,ptlb:GlobalClipDistance,ptin:_GlobalClipDistance,varname:_GlobalClipDistance,prsc:0,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Lerp,id:9556,x:32006,y:33002,varname:node_9556,prsc:2|A-1805-OUT,B-6745-OUT,T-6995-OUT;n:type:ShaderForge.SFN_Add,id:9612,x:32139,y:32757,varname:node_9612,prsc:2|A-9899-OUT,B-4023-OUT;n:type:ShaderForge.SFN_Vector1,id:4023,x:31859,y:32853,varname:node_4023,prsc:2,v1:21;n:type:ShaderForge.SFN_Subtract,id:6745,x:31616,y:33135,cmnt:Subtracting bypasses clip if clip distance is zero,varname:node_6745,prsc:2|A-7715-OUT,B-742-OUT;n:type:ShaderForge.SFN_Vector1,id:742,x:31438,y:33220,varname:node_742,prsc:2,v1:0.01;proporder:278-5139-9689-7615-4493-8375-1223-6805-9214-1805-6995;pass:END;sub:END;*/

Shader "Spatial/Spatial_Greybox_Master" {
    Properties {
        _T_Sides ("T_Sides", 2D) = "white" {}
        _T_Normal ("T_Normal", 2D) = "bump" {}
        _T_Top ("T_Top", 2D) = "white" {}
        _Sides_Colour ("Sides_Colour", Color) = (0.5,0.5,0.5,1)
        _Top_Colour ("Top_Colour", Color) = (0.1,0.1,0.1,1)
        _T_Trim ("T_Trim", 2D) = "white" {}
        _T_Trim_Normal ("T_Trim_Normal", 2D) = "bump" {}
        _Metal_Sides ("Metal_Sides", Float ) = 0
        _Metal_Top ("Metal_Top", Float ) = 0
        _LocalClipDistance ("LocalClipDistance", Float ) = 200
        [MaterialToggle] _OverrideClipDistanceToGlobals ("OverrideClipDistanceToGlobals", Float ) = 0
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
            uniform sampler2D _T_Sides; uniform float4 _T_Sides_ST;
            uniform sampler2D _T_Normal; uniform float4 _T_Normal_ST;
            uniform sampler2D _T_Top; uniform float4 _T_Top_ST;
            uniform fixed4 _Sides_Colour;
            uniform fixed4 _Top_Colour;
            uniform sampler2D _T_Trim; uniform float4 _T_Trim_ST;
            uniform sampler2D _T_Trim_Normal; uniform float4 _T_Trim_Normal_ST;
            uniform fixed _Metal_Sides;
            uniform fixed _Metal_Top;
            uniform fixed _OverrideClipDistanceToGlobals;
            uniform fixed _LocalClipDistance;
            uniform fixed _GlobalClipDistance;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
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
                float4 vertexColor : COLOR;
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
                o.vertexColor = v.vertexColor;
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
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                fixed2 node_3691 = float2(i.uv0.g,i.uv0.r); // 90 degree rotated UVs
                fixed node_2763 = 4.0;
                fixed3 node_5070 = abs((fixed3(1,1,1)-objScale)).rgb;
                fixed3 node_3793 = lerp(objScale,(round(((i.vertexColor.rgb*36.0)*node_2763))/node_2763),(1.0 - saturate(((node_5070.r+node_5070.g+node_5070.b)*100.0)))); // Toggle between object scale or VTX RGB if scale is reset
                fixed3 node_6426 = node_3793.rgb;
                fixed node_9824 = i.vertexColor.a; // VTX Alpha 0isX 1isZ  0.5isY
                fixed node_956 = (node_9824-0.5);
                fixed3 node_609 = saturate((float3(saturate((1.0 - (node_9824*100.0))),saturate((node_956*2.0)),saturate((1.0 - (abs(node_956)*100.0))))*1.428571+-0.1428571)); // Define local XYZ axis from VTX Alpha
                fixed3 node_4146 = node_609.rgb; // Local axis
                fixed2 node_8926 = ((lerp(i.uv0,node_3691,saturate(sign((node_6426.b-node_6426.g))))*node_4146.r)+(lerp(i.uv0,node_3691,saturate(sign((node_6426.r-node_6426.b))))*node_4146.g)+(lerp(i.uv0,node_3691,saturate(sign((node_6426.r-node_6426.g))))*node_4146.b)); // Calculate Trim UVs based on object scale
                fixed3 _T_Trim_Normal_var = UnpackNormal(tex2D(_T_Trim_Normal,TRANSFORM_TEX(node_8926, _T_Trim_Normal)));
                fixed3 node_8309 = (node_3793/4.0).rgb;
                fixed2 node_7602 = float2(node_8309.r,node_8309.b); // Y Axis
                fixed2 node_797 = (lerp(lerp(float2(node_8309.b,node_8309.g),node_7602,node_4146.g),float2(node_8309.r,node_8309.g),node_4146.b)*i.uv0); // Scaled UVs
                fixed3 _T_Normal_var = UnpackNormal(tex2D(_T_Normal,TRANSFORM_TEX(node_797, _T_Normal)));
                fixed3 node_1646 = ((1.0 - node_609)*saturate((node_3793*-20.0+11.0))).rgb;
                fixed node_3882 = (1.0 - max(max(node_1646.r,node_1646.g),node_1646.b)); // Trim Mask
                float3 normalLocal = normalize(lerp(_T_Trim_Normal_var.rgb,_T_Normal_var.rgb,node_3882));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                fixed4 _T_Trim_var = tex2D(_T_Trim,TRANSFORM_TEX(node_8926, _T_Trim));
                fixed4 _T_Sides_var = tex2D(_T_Sides,TRANSFORM_TEX(node_797, _T_Sides));
                fixed2 node_1666 = (node_7602*i.uv0); // Top Scaled UVs
                fixed4 _T_Top_var = tex2D(_T_Top,TRANSFORM_TEX(node_1666, _T_Top));
                fixed3 node_5665 = lerp(_T_Trim_var.rgb,lerp(_T_Sides_var.rgb,_T_Top_var.rgb,node_4146.g),node_3882).rgb; // Unpack channels
                fixed node_6073 = node_5665.g;
                fixed node_9871 = ((node_6073*-0.2+0.1)+((distance(float2(i.posWorld.r,i.posWorld.b),float2(_WorldSpaceCameraPos.r,_WorldSpaceCameraPos.b))+21.0)/lerp(_LocalClipDistance,(_GlobalClipDistance-0.01),_OverrideClipDistanceToGlobals)));
                clip(saturate((node_9871*-50.00005+50.00005)) - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = node_6073;
                float perceptualRoughness = 1.0 - node_6073;
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
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = lerp(_Metal_Sides,_Metal_Top,node_5665.b);
                float specularMonochrome;
                float3 diffuseColor = (node_5665.r*lerp(_Sides_Colour.rgb,_Top_Colour.rgb,node_5665.b)); // Need this for specular when using metallic
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
                float3 indirectSpecular = (gi.indirect.specular + UNITY_LIGHTMODEL_AMBIENT.rgb);
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
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                fixed node_6135 = saturate((node_9871*16.66667+-15.33333));
                float3 emissive = (fixed3(0.2,0.5,1)*node_6135*node_6135*node_6135);
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
            uniform sampler2D _T_Sides; uniform float4 _T_Sides_ST;
            uniform sampler2D _T_Normal; uniform float4 _T_Normal_ST;
            uniform sampler2D _T_Top; uniform float4 _T_Top_ST;
            uniform fixed4 _Sides_Colour;
            uniform fixed4 _Top_Colour;
            uniform sampler2D _T_Trim; uniform float4 _T_Trim_ST;
            uniform sampler2D _T_Trim_Normal; uniform float4 _T_Trim_Normal_ST;
            uniform fixed _Metal_Sides;
            uniform fixed _Metal_Top;
            uniform fixed _OverrideClipDistanceToGlobals;
            uniform fixed _LocalClipDistance;
            uniform fixed _GlobalClipDistance;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
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
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                fixed2 node_3691 = float2(i.uv0.g,i.uv0.r); // 90 degree rotated UVs
                fixed node_2763 = 4.0;
                fixed3 node_5070 = abs((fixed3(1,1,1)-objScale)).rgb;
                fixed3 node_3793 = lerp(objScale,(round(((i.vertexColor.rgb*36.0)*node_2763))/node_2763),(1.0 - saturate(((node_5070.r+node_5070.g+node_5070.b)*100.0)))); // Toggle between object scale or VTX RGB if scale is reset
                fixed3 node_6426 = node_3793.rgb;
                fixed node_9824 = i.vertexColor.a; // VTX Alpha 0isX 1isZ  0.5isY
                fixed node_956 = (node_9824-0.5);
                fixed3 node_609 = saturate((float3(saturate((1.0 - (node_9824*100.0))),saturate((node_956*2.0)),saturate((1.0 - (abs(node_956)*100.0))))*1.428571+-0.1428571)); // Define local XYZ axis from VTX Alpha
                fixed3 node_4146 = node_609.rgb; // Local axis
                fixed2 node_8926 = ((lerp(i.uv0,node_3691,saturate(sign((node_6426.b-node_6426.g))))*node_4146.r)+(lerp(i.uv0,node_3691,saturate(sign((node_6426.r-node_6426.b))))*node_4146.g)+(lerp(i.uv0,node_3691,saturate(sign((node_6426.r-node_6426.g))))*node_4146.b)); // Calculate Trim UVs based on object scale
                fixed3 _T_Trim_Normal_var = UnpackNormal(tex2D(_T_Trim_Normal,TRANSFORM_TEX(node_8926, _T_Trim_Normal)));
                fixed3 node_8309 = (node_3793/4.0).rgb;
                fixed2 node_7602 = float2(node_8309.r,node_8309.b); // Y Axis
                fixed2 node_797 = (lerp(lerp(float2(node_8309.b,node_8309.g),node_7602,node_4146.g),float2(node_8309.r,node_8309.g),node_4146.b)*i.uv0); // Scaled UVs
                fixed3 _T_Normal_var = UnpackNormal(tex2D(_T_Normal,TRANSFORM_TEX(node_797, _T_Normal)));
                fixed3 node_1646 = ((1.0 - node_609)*saturate((node_3793*-20.0+11.0))).rgb;
                fixed node_3882 = (1.0 - max(max(node_1646.r,node_1646.g),node_1646.b)); // Trim Mask
                float3 normalLocal = normalize(lerp(_T_Trim_Normal_var.rgb,_T_Normal_var.rgb,node_3882));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                fixed4 _T_Trim_var = tex2D(_T_Trim,TRANSFORM_TEX(node_8926, _T_Trim));
                fixed4 _T_Sides_var = tex2D(_T_Sides,TRANSFORM_TEX(node_797, _T_Sides));
                fixed2 node_1666 = (node_7602*i.uv0); // Top Scaled UVs
                fixed4 _T_Top_var = tex2D(_T_Top,TRANSFORM_TEX(node_1666, _T_Top));
                fixed3 node_5665 = lerp(_T_Trim_var.rgb,lerp(_T_Sides_var.rgb,_T_Top_var.rgb,node_4146.g),node_3882).rgb; // Unpack channels
                fixed node_6073 = node_5665.g;
                fixed node_9871 = ((node_6073*-0.2+0.1)+((distance(float2(i.posWorld.r,i.posWorld.b),float2(_WorldSpaceCameraPos.r,_WorldSpaceCameraPos.b))+21.0)/lerp(_LocalClipDistance,(_GlobalClipDistance-0.01),_OverrideClipDistanceToGlobals)));
                clip(saturate((node_9871*-50.00005+50.00005)) - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = node_6073;
                float perceptualRoughness = 1.0 - node_6073;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = lerp(_Metal_Sides,_Metal_Top,node_5665.b);
                float specularMonochrome;
                float3 diffuseColor = (node_5665.r*lerp(_Sides_Colour.rgb,_Top_Colour.rgb,node_5665.b)); // Need this for specular when using metallic
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
            uniform sampler2D _T_Sides; uniform float4 _T_Sides_ST;
            uniform sampler2D _T_Top; uniform float4 _T_Top_ST;
            uniform sampler2D _T_Trim; uniform float4 _T_Trim_ST;
            uniform fixed _OverrideClipDistanceToGlobals;
            uniform fixed _LocalClipDistance;
            uniform fixed _GlobalClipDistance;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float2 uv1 : TEXCOORD2;
                float2 uv2 : TEXCOORD3;
                float4 posWorld : TEXCOORD4;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                fixed2 node_3691 = float2(i.uv0.g,i.uv0.r); // 90 degree rotated UVs
                fixed node_2763 = 4.0;
                fixed3 node_5070 = abs((fixed3(1,1,1)-objScale)).rgb;
                fixed3 node_3793 = lerp(objScale,(round(((i.vertexColor.rgb*36.0)*node_2763))/node_2763),(1.0 - saturate(((node_5070.r+node_5070.g+node_5070.b)*100.0)))); // Toggle between object scale or VTX RGB if scale is reset
                fixed3 node_6426 = node_3793.rgb;
                fixed node_9824 = i.vertexColor.a; // VTX Alpha 0isX 1isZ  0.5isY
                fixed node_956 = (node_9824-0.5);
                fixed3 node_609 = saturate((float3(saturate((1.0 - (node_9824*100.0))),saturate((node_956*2.0)),saturate((1.0 - (abs(node_956)*100.0))))*1.428571+-0.1428571)); // Define local XYZ axis from VTX Alpha
                fixed3 node_4146 = node_609.rgb; // Local axis
                fixed2 node_8926 = ((lerp(i.uv0,node_3691,saturate(sign((node_6426.b-node_6426.g))))*node_4146.r)+(lerp(i.uv0,node_3691,saturate(sign((node_6426.r-node_6426.b))))*node_4146.g)+(lerp(i.uv0,node_3691,saturate(sign((node_6426.r-node_6426.g))))*node_4146.b)); // Calculate Trim UVs based on object scale
                fixed4 _T_Trim_var = tex2D(_T_Trim,TRANSFORM_TEX(node_8926, _T_Trim));
                fixed3 node_8309 = (node_3793/4.0).rgb;
                fixed2 node_7602 = float2(node_8309.r,node_8309.b); // Y Axis
                fixed2 node_797 = (lerp(lerp(float2(node_8309.b,node_8309.g),node_7602,node_4146.g),float2(node_8309.r,node_8309.g),node_4146.b)*i.uv0); // Scaled UVs
                fixed4 _T_Sides_var = tex2D(_T_Sides,TRANSFORM_TEX(node_797, _T_Sides));
                fixed2 node_1666 = (node_7602*i.uv0); // Top Scaled UVs
                fixed4 _T_Top_var = tex2D(_T_Top,TRANSFORM_TEX(node_1666, _T_Top));
                fixed3 node_1646 = ((1.0 - node_609)*saturate((node_3793*-20.0+11.0))).rgb;
                fixed node_3882 = (1.0 - max(max(node_1646.r,node_1646.g),node_1646.b)); // Trim Mask
                fixed3 node_5665 = lerp(_T_Trim_var.rgb,lerp(_T_Sides_var.rgb,_T_Top_var.rgb,node_4146.g),node_3882).rgb; // Unpack channels
                fixed node_6073 = node_5665.g;
                fixed node_9871 = ((node_6073*-0.2+0.1)+((distance(float2(i.posWorld.r,i.posWorld.b),float2(_WorldSpaceCameraPos.r,_WorldSpaceCameraPos.b))+21.0)/lerp(_LocalClipDistance,(_GlobalClipDistance-0.01),_OverrideClipDistanceToGlobals)));
                clip(saturate((node_9871*-50.00005+50.00005)) - 0.5);
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
            uniform sampler2D _T_Sides; uniform float4 _T_Sides_ST;
            uniform sampler2D _T_Top; uniform float4 _T_Top_ST;
            uniform fixed4 _Sides_Colour;
            uniform fixed4 _Top_Colour;
            uniform sampler2D _T_Trim; uniform float4 _T_Trim_ST;
            uniform fixed _Metal_Sides;
            uniform fixed _Metal_Top;
            uniform fixed _OverrideClipDistanceToGlobals;
            uniform fixed _LocalClipDistance;
            uniform fixed _GlobalClipDistance;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                fixed2 node_3691 = float2(i.uv0.g,i.uv0.r); // 90 degree rotated UVs
                fixed node_2763 = 4.0;
                fixed3 node_5070 = abs((fixed3(1,1,1)-objScale)).rgb;
                fixed3 node_3793 = lerp(objScale,(round(((i.vertexColor.rgb*36.0)*node_2763))/node_2763),(1.0 - saturate(((node_5070.r+node_5070.g+node_5070.b)*100.0)))); // Toggle between object scale or VTX RGB if scale is reset
                fixed3 node_6426 = node_3793.rgb;
                fixed node_9824 = i.vertexColor.a; // VTX Alpha 0isX 1isZ  0.5isY
                fixed node_956 = (node_9824-0.5);
                fixed3 node_609 = saturate((float3(saturate((1.0 - (node_9824*100.0))),saturate((node_956*2.0)),saturate((1.0 - (abs(node_956)*100.0))))*1.428571+-0.1428571)); // Define local XYZ axis from VTX Alpha
                fixed3 node_4146 = node_609.rgb; // Local axis
                fixed2 node_8926 = ((lerp(i.uv0,node_3691,saturate(sign((node_6426.b-node_6426.g))))*node_4146.r)+(lerp(i.uv0,node_3691,saturate(sign((node_6426.r-node_6426.b))))*node_4146.g)+(lerp(i.uv0,node_3691,saturate(sign((node_6426.r-node_6426.g))))*node_4146.b)); // Calculate Trim UVs based on object scale
                fixed4 _T_Trim_var = tex2D(_T_Trim,TRANSFORM_TEX(node_8926, _T_Trim));
                fixed3 node_8309 = (node_3793/4.0).rgb;
                fixed2 node_7602 = float2(node_8309.r,node_8309.b); // Y Axis
                fixed2 node_797 = (lerp(lerp(float2(node_8309.b,node_8309.g),node_7602,node_4146.g),float2(node_8309.r,node_8309.g),node_4146.b)*i.uv0); // Scaled UVs
                fixed4 _T_Sides_var = tex2D(_T_Sides,TRANSFORM_TEX(node_797, _T_Sides));
                fixed2 node_1666 = (node_7602*i.uv0); // Top Scaled UVs
                fixed4 _T_Top_var = tex2D(_T_Top,TRANSFORM_TEX(node_1666, _T_Top));
                fixed3 node_1646 = ((1.0 - node_609)*saturate((node_3793*-20.0+11.0))).rgb;
                fixed node_3882 = (1.0 - max(max(node_1646.r,node_1646.g),node_1646.b)); // Trim Mask
                fixed3 node_5665 = lerp(_T_Trim_var.rgb,lerp(_T_Sides_var.rgb,_T_Top_var.rgb,node_4146.g),node_3882).rgb; // Unpack channels
                fixed node_6073 = node_5665.g;
                fixed node_9871 = ((node_6073*-0.2+0.1)+((distance(float2(i.posWorld.r,i.posWorld.b),float2(_WorldSpaceCameraPos.r,_WorldSpaceCameraPos.b))+21.0)/lerp(_LocalClipDistance,(_GlobalClipDistance-0.01),_OverrideClipDistanceToGlobals)));
                fixed node_6135 = saturate((node_9871*16.66667+-15.33333));
                o.Emission = (fixed3(0.2,0.5,1)*node_6135*node_6135*node_6135);
                
                float3 diffColor = (node_5665.r*lerp(_Sides_Colour.rgb,_Top_Colour.rgb,node_5665.b));
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, lerp(_Metal_Sides,_Metal_Top,node_5665.b), specColor, specularMonochrome );
                float roughness = 1.0 - node_6073;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
