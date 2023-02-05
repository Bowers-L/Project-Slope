#ifndef KUWAHARA_INCLUDED
#define KUWAHARA_INCLUDED

#define KERNEL 3

float vecSum(float3 vec) {
    return vec.x + vec.y + vec.z;
}

void kuwahara_float(UnityTexture2D _tex, UnitySamplerState _sampler_tex, float2 pxOffset, float2 uv, out float3 Out) {
    float3 m[] = {float3(0, 0, 0), float3(0, 0, 0), float3(0, 0, 0), float3(0, 0, 0)};
    
    // float2 pxOffset = float2(1.,1.)/(float2(textureSize(_tex, 0)));
    
    for (int i = 0; i < KERNEL; i++) {
        for (int j = 0; j < KERNEL; j++) {
            m[0] += SAMPLE_TEXTURE2D(_tex,_sampler_tex, uv.xy + pxOffset * float2(j-KERNEL+1, i-KERNEL+1)).xyz;
            m[1] += SAMPLE_TEXTURE2D(_tex,_sampler_tex, uv.xy + pxOffset * float2(j, i-KERNEL+1)).xyz;
            m[2] += SAMPLE_TEXTURE2D(_tex,_sampler_tex, uv.xy + pxOffset * float2(j-KERNEL+1, i)).xyz;
            m[3] += SAMPLE_TEXTURE2D(_tex,_sampler_tex, uv.xy + pxOffset * float2(j, i)).xyz;
        }
    }
    for (int i = 0; i < 4; i++) {
        m[i] /= float(KERNEL*KERNEL);
    }
    
    float3 sd[] = {float3(0, 0, 0), float3(0, 0, 0), float3(0, 0, 0), float3(0, 0, 0)};
    
    
    for (int i = 0; i < KERNEL; i++) {
        for (int j = 0; j < KERNEL; j++) {
            float3 v0 = SAMPLE_TEXTURE2D(_tex,_sampler_tex, uv.xy + pxOffset * float2(j-KERNEL+1, i-KERNEL+1)).xyz;
            float3 v1 = SAMPLE_TEXTURE2D(_tex,_sampler_tex, uv.xy + pxOffset * float2(j, i-KERNEL+1)).xyz;
            float3 v2 = SAMPLE_TEXTURE2D(_tex,_sampler_tex, uv.xy + pxOffset * float2(j-KERNEL+1, i)).xyz;
            float3 v3 = SAMPLE_TEXTURE2D(_tex,_sampler_tex, uv.xy + pxOffset * float2(j, i)).xyz;
            
            float3 dev0 = v0 - m[0];
            float3 dev1 = v1 - m[1];
            float3 dev2 = v2 - m[2];
            float3 dev3 = v3 - m[3];
            
            float3 sqdev0 = dev0 * dev0;
            float3 sqdev1 = dev1 * dev1;
            float3 sqdev2 = dev2 * dev2;
            float3 sqdev3 = dev3 * dev3;
            
            sd[0] += sqdev0;
            sd[1] += sqdev1;
            sd[2] += sqdev2;
            sd[3] += sqdev3;
            
            
        }
    }
    
    float sdSum[4] = {0., 0., 0., 0.};
    
    for (int i = 0; i < 4; i++) {
        sd[i] /= float(KERNEL*KERNEL);
        sd[i] = sqrt(sd[i]);
        sdSum[i] = vecSum(sd[i]);
    }
    
    int sdIndex = 0;
    for (int i = 1; i < 4; i++) {
        if (sdSum[i] < sdSum[sdIndex]) sdIndex = i;
    }
    
    Out = m[sdIndex];
}

#endif