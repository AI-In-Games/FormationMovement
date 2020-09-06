using System;
using Unity.Entities;

[Flags]
public enum ThreatType : int
{
    None = 0,
    Vision = 1,
    Hearing = 2
}

[Serializable]
public struct PerceptionThreatComponent : IComponentData { public int ThreatType; }

[Serializable]
public struct AlertComponent : IComponentData { public int AlertType; }

[Serializable]
public struct VisionPerceptionComponent : IComponentData
{
    public float Distance;
    public float Angle;
    //public float3 PositionOffset;
}

[Serializable]
public struct HearingPerceptionComponent : IComponentData
{
    public float Distance;
    //public float3 PositionOffset;
}
