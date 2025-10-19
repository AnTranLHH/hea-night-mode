using UnityEngine;

namespace SoSystem
{
    [CreateAssetMenu(fileName = "FloatVar", menuName = "ScriptableObjectSystem/FloatVariable")]
    public class FloatVariable : BaseScriptableObjectVariable<float>
    {
        private float _change;

        protected override bool IsSetNewValue(float value)
        {
            _change = value - _value;
            return value != _value;
        }

        public float LastChange
        {
            get
            {
                return _change;
            }
        }

        public void Increase(float increment)
        {
            Value += increment;
        }
    }

}
