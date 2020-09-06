using System;
using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;

[Serializable]
[MaterialProperty("_BaseColor", MaterialPropertyFormat.Float4)]
public struct CustomColor : IComponentData 
{
    public float4 Value;
}
