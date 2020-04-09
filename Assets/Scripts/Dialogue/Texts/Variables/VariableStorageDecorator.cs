﻿using Yarn;
using Yarn.Unity;

namespace Assets.Scripts.Dialogue.Texts.Variables
{
    public abstract class VariableStorageDecorator<T> : VariableStorageBehaviour where T : VariableStorageBehaviour
    {
        public T Storage { get; private set; }

        /// Reset to our default values when the game starts
        internal void Awake()
        {
            Storage = InitStorage();
            ResetToDefaults();
        }

        protected virtual T InitStorage()
            => gameObject.AddComponent<T>();

        public override Value GetValue(string variableName)
        {
            Value value = GetValueBeforeStorage(variableName);
            if (value == Value.NULL)
            {
                value = Storage.GetValue(variableName);
                if (value == Value.NULL)
                {
                    value = GetValueAfterStorage(variableName);
                }
            }
            return value;
        }

        protected virtual Value GetValueBeforeStorage(string variableName) => Value.NULL;
        protected virtual Value GetValueAfterStorage(string variableName) => Value.NULL;

        public override void ResetToDefaults()
        {
            ResetToDefaultsBeforeStorage();
            Storage.ResetToDefaults();
            ResetToDefaultsAfterStorage();
        }

        protected virtual void ResetToDefaultsBeforeStorage() { }
        protected virtual void ResetToDefaultsAfterStorage() { }

        public override void SetValue(string variableName, Value value)
        {
            if (!SetValueNoStorage(variableName, value))
            {
                Storage.SetValue(variableName, value);
            }
        }

        protected virtual bool SetValueNoStorage(string variableName, Value value) => false;
    }
}
