using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Torii/Variable/Float")]
public class FloatVariable : ScriptableObject, ISerializationCallbackReceiver
{
    public float InitialValue;

    [NonSerialized] public float RuntimeValue;

    public static implicit operator float(FloatVariable self) { return self.RuntimeValue; }

    public void OnBeforeSerialize() { }
    public void OnAfterDeserialize() { RuntimeValue = InitialValue; }
}
