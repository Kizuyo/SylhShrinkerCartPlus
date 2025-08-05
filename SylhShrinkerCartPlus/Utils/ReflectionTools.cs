using System.Collections.Concurrent;
using System.Reflection;

namespace SylhShrinkerCartPlus.Utils
{
    /// <summary>
    /// Helper typé pour accéder rapidement aux champs/propriétés/méthodes d'une instance via reflection (avec cache).
    /// </summary>
    public class FastReflectionHelper<T>
    {
        private readonly T _instance;
        private static readonly Type _type = typeof(T);

        private static readonly ConcurrentDictionary<string, FieldInfo> _fieldCache = new();
        private static readonly ConcurrentDictionary<string, PropertyInfo> _propertyCache = new();
        private static readonly ConcurrentDictionary<string, MethodInfo> _methodCache = new();

        public FastReflectionHelper(T instance)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        public TField GetField<TField>(string fieldName, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var field = _fieldCache.GetOrAdd(fieldName, name =>
                _type.GetField(name, flags)
                ?? throw new MissingFieldException(_type.FullName, name));

            return (TField)field.GetValue(_instance);
        }
        
        public bool TryGetField<TField>(string fieldName, out TField value, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            if (_fieldCache.TryGetValue(fieldName, out var field) || 
                (field = _type.GetField(fieldName, flags)) != null)
            {
                _fieldCache[fieldName] = field;
                value = (TField)field.GetValue(_instance);
                return true;
            }

            value = default;
            return false;
        }

        public void SetField<TField>(string fieldName, TField value, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var field = _fieldCache.GetOrAdd(fieldName, name =>
                _type.GetField(name, flags)
                ?? throw new MissingFieldException(_type.FullName, name));

            field.SetValue(_instance, value);
        }

        public TProperty GetProperty<TProperty>(string propertyName, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var prop = _propertyCache.GetOrAdd(propertyName, name =>
                _type.GetProperty(name, flags)
                ?? throw new MissingMemberException(_type.FullName, name));

            return (TProperty)prop.GetValue(_instance);
        }
        
        public bool TryGetProperty<TProperty>(string propertyName, out TProperty value, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            if (_propertyCache.TryGetValue(propertyName, out var prop) ||
                (prop = _type.GetProperty(propertyName, flags)) != null)
            {
                _propertyCache[propertyName] = prop;
                value = (TProperty)prop.GetValue(_instance);
                return true;
            }

            value = default;
            return false;
        }

        public void SetProperty<TProperty>(string propertyName, TProperty value, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var prop = _propertyCache.GetOrAdd(propertyName, name =>
                _type.GetProperty(name, flags)
                ?? throw new MissingMemberException(_type.FullName, name));

            prop.SetValue(_instance, value);
        }

        public TResult CallMethod<TResult>(string methodName, object[] parameters = null, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var method = _methodCache.GetOrAdd(methodName, name =>
                _type.GetMethod(name, flags)
                ?? throw new MissingMethodException(_type.FullName, name));

            return (TResult)method.Invoke(_instance, parameters);
        }

        public void CallMethod(string methodName, object[] parameters = null, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var method = _methodCache.GetOrAdd(methodName, name =>
                _type.GetMethod(name, flags)
                ?? throw new MissingMethodException(_type.FullName, name));

            method.Invoke(_instance, parameters);
        }
        
        public bool TryCallMethod<TResult>(
            string methodName,
            out TResult result,
            object[] parameters = null,
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            if (_methodCache.TryGetValue(methodName, out var method) ||
                (method = _type.GetMethod(methodName, flags)) != null)
            {
                _methodCache[methodName] = method;
                result = (TResult)method.Invoke(_instance, parameters);
                return true;
            }

            result = default;
            return false;
        }

        public bool TryCallMethod(
            string methodName,
            object[] parameters = null,
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            if (_methodCache.TryGetValue(methodName, out var method) ||
                (method = _type.GetMethod(methodName, flags)) != null)
            {
                _methodCache[methodName] = method;
                method.Invoke(_instance, parameters);
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Version statique de FastReflection : permet l'accès aux champs/méthodes/propriétés à partir d'un Type.
    /// Parfait pour les cas dynamiques ou les static fields.
    /// </summary>
    public static class FastReflection
    {
        private static readonly ConcurrentDictionary<(Type, string), FieldInfo> _fieldCache = new();
        private static readonly ConcurrentDictionary<(Type, string), PropertyInfo> _propertyCache = new();
        private static readonly ConcurrentDictionary<(Type, string), MethodInfo> _methodCache = new();

        public static T GetField<T>(
            Type type, 
            object instance, 
            string fieldName, 
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance
            )
        {
            var key = (type, fieldName);
            var field = _fieldCache.GetOrAdd(key, _ =>
                type.GetField(fieldName, flags)
                ?? throw new MissingFieldException(type.FullName, fieldName));

            return (T)field.GetValue(instance);
        }
        
        public static bool TryGetField<T>(Type type, object instance, string fieldName, out T value, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var key = (type, fieldName);

            if (_fieldCache.TryGetValue(key, out var field) || 
                (field = type.GetField(fieldName, flags)) != null)
            {
                _fieldCache[key] = field;
                value = (T)field.GetValue(instance);
                return true;
            }

            value = default;
            return false;
        }

        public static void SetField<T>(Type type, object instance, string fieldName, T value, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var key = (type, fieldName);
            var field = _fieldCache.GetOrAdd(key, _ =>
                type.GetField(fieldName, flags)
                ?? throw new MissingFieldException(type.FullName, fieldName));

            field.SetValue(instance, value);
        }

        public static T GetProperty<T>(Type type, object instance, string propertyName, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var key = (type, propertyName);
            var prop = _propertyCache.GetOrAdd(key, _ =>
                type.GetProperty(propertyName, flags)
                ?? throw new MissingMemberException(type.FullName, propertyName));

            return (T)prop.GetValue(instance);
        }
        
        public static bool TryGetProperty<T>(Type type, object instance, string propertyName, out T value, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var key = (type, propertyName);

            if (_propertyCache.TryGetValue(key, out var prop) ||
                (prop = type.GetProperty(propertyName, flags)) != null)
            {
                _propertyCache[key] = prop;
                value = (T)prop.GetValue(instance);
                return true;
            }

            value = default;
            return false;
        }

        public static void SetProperty<T>(Type type, object instance, string propertyName, T value, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var key = (type, propertyName);
            var prop = _propertyCache.GetOrAdd(key, _ =>
                type.GetProperty(propertyName, flags)
                ?? throw new MissingMemberException(type.FullName, propertyName));

            prop.SetValue(instance, value);
        }

        public static T InvokeMethod<T>(Type type, object instance, string methodName, object[] parameters = null, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var key = (type, methodName);
            var method = _methodCache.GetOrAdd(key, _ =>
                type.GetMethod(methodName, flags)
                ?? throw new MissingMethodException(type.FullName, methodName));

            return (T)method.Invoke(instance, parameters);
        }

        public static void InvokeMethod(Type type, object instance, string methodName, object[] parameters = null, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var key = (type, methodName);
            var method = _methodCache.GetOrAdd(key, _ =>
                type.GetMethod(methodName, flags)
                ?? throw new MissingMethodException(type.FullName, methodName));

            method.Invoke(instance, parameters);
        }
        
        public static bool TryInvokeMethod<TResult>(
            Type type,
            object instance,
            string methodName,
            out TResult result,
            object[] parameters = null,
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var key = (type, methodName);

            if (_methodCache.TryGetValue(key, out var method) ||
                (method = type.GetMethod(methodName, flags)) != null)
            {
                _methodCache[key] = method;
                result = (TResult)method.Invoke(instance, parameters);
                return true;
            }

            result = default;
            return false;
        }

        public static bool TryInvokeMethod(
            Type type,
            object instance,
            string methodName,
            object[] parameters = null,
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var key = (type, methodName);

            if (_methodCache.TryGetValue(key, out var method) ||
                (method = type.GetMethod(methodName, flags)) != null)
            {
                _methodCache[key] = method;
                method.Invoke(instance, parameters);
                return true;
            }

            return false;
        }
    }
}
