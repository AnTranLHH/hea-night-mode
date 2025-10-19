using UnityEngine;

namespace SoSystem
{
    [CreateAssetMenu(fileName = "StringVar", menuName = "ScriptableObjectSystem/StringVariable")]
    public class StringVariable : BaseScriptableObjectVariable<string>
    {
        protected override bool IsSetNewValue(string value)
        {
            return !_value.Equals(value);
        }
    }
}
