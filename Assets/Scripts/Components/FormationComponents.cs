using System;
using Unity.Entities;

[Serializable]
public struct FormationGroup : ISharedComponentData { public int LeaderId; }

[Serializable]
public struct FormationLeader : IComponentData { public int Id; }

[Serializable]
public struct FormationIndex : IComponentData { public int Index; public int Count; }

[Serializable]
public struct TestudoFormation : IComponentData { }
[Serializable]
public struct OrbFormation : IComponentData { }
[Serializable]
public struct WedgeFormation : IComponentData { }

[Serializable]
public struct SelectedComponent : IComponentData { }
