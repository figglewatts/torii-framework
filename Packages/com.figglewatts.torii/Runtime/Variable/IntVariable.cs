using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Torii/Variable/Int")]
public class IntVariable : ScriptableObject, ISerializationCallbackReceiver
{
    public int InitialValue;

    [NonSerialized] public int RuntimeValue;

    public static implicit operator int(IntVariable self) { return self.RuntimeValue; }

    public void OnBeforeSerialize() { }
    public void OnAfterDeserialize() { RuntimeValue = InitialValue; }
}
