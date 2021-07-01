using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AutoFixture.Idioms
{
    public class ReflectionScaffolding
    {
        private readonly List<FieldAccess> allFields = new List<FieldAccess>();

        private readonly Type type;

        public FieldAccess<int>[] IntegerFields => allFields.OfType<FieldAccess<int>>().ToArray();

        public FieldAccess<string>[] StringFields => allFields.OfType<FieldAccess<string>>().ToArray();

        public object Instance { get; }

        public ReflectionScaffolding(Type type, object instance) : this(type, instance, new HashSet<object>(EqualityComparer<object>.Default)) { }

        public ReflectionScaffolding(Type type, object instance, HashSet<object> cycleBreaker)
        {
            cycleBreaker.Add(instance);

            this.type = type;
            this.Instance = instance;
            var fields = type.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(int))
                {
                    AddPrimitive<int>(field, instance);
                }
                else if (field.FieldType == typeof(string))
                {
                    AddPrimitive<string>(field, instance);
                }
                else if (field.FieldType.IsPrimitive is false)
                {
                    AddNonPrimitiveFields(field, instance, cycleBreaker);              
                }
            }
        }

        private void AddPrimitive<T>(FieldInfo fieldInfo, object instance)
        {
            allFields.Add(new FieldAccess<T>(fieldInfo, instance));
        }

        private void AddNonPrimitiveFields(FieldInfo fieldInfo, object instance, HashSet<object> cycleBreaker)
        {
            var nestedInstance = fieldInfo.GetValue(instance);

            if (cycleBreaker.Contains(nestedInstance) || nestedInstance == null)
            {
                return;
            }

            allFields.AddRange(new ReflectionScaffolding(fieldInfo.FieldType, nestedInstance, cycleBreaker).allFields);
        }

        public ReflectionScaffolding Clone()
        {
            var clone = FormatterServices.GetUninitializedObject(type);
            var cloneScaffolding = new ReflectionScaffolding(type, clone);
            for (int i = 0; i < cloneScaffolding.allFields.Count; i++)
            {
                var cloneField = cloneScaffolding.allFields[i];
                var originalField = allFields[i];
                cloneField.SetObject(originalField.GetObject());
            }
            return cloneScaffolding;
        }

        public class FieldAccess<T> : FieldAccess
        {
            public FieldAccess(FieldInfo fieldInfo, object instance) : base(fieldInfo, instance)
            {
            }

            public T Get() => (T)GetObject();

            public void Set(T value) => SetObject(value);
        }

        public class FieldAccess
        {
            private readonly FieldInfo fieldInfo;

            private readonly object instance;

            public string Name => fieldInfo.Name;

            public FieldAccess(FieldInfo fieldInfo, object instance)
            {
                this.fieldInfo = fieldInfo;
                this.instance = instance;
            }

            public object GetObject() => fieldInfo.GetValue(instance);

            public void SetObject(object value)
            {
                fieldInfo.SetValue(instance, value);
            }

            public override string ToString() => $"{Name}: {GetObject()}";
        }

    }
}
