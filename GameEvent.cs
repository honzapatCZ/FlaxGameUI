using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using FlaxEngine;

namespace FlaxGameUI
{
    public delegate void GameAction();
    public delegate void GameAction<T0>(T0 value);

    public class GameEventBase
    {
        [Serializable]
        public struct SerializedMethod
        {
            public enum SerializedActorType{
                Actor, Script
            }
            public enum SerializedActorValueType{
                None, Method, Property, Field
            }
            public struct SerInfos
            {
                public MethodInfo minfo;
                public FieldInfo finfo;
                public PropertyInfo pinfo;

                public SerializedActorValueType GetValueType()
                {
                    if (minfo != null)
                        return SerializedActorValueType.Method;
                    else if (finfo != null)
                        return SerializedActorValueType.Field;
                    else if (pinfo != null)
                        return SerializedActorValueType.Property;
                    else
                        return SerializedActorValueType.None;
                }
                public string GetValueName()
                {
                    switch (GetValueType())
                    {
                        case SerializedActorValueType.Method:
                            return minfo.Name;
                        case SerializedActorValueType.Field:
                            return finfo.Name;
                        case SerializedActorValueType.Property:
                            return pinfo.Name;
                        default:
                            return null;
                    }
                }
                public Type GetParOrValueType()
                {
                    switch (GetValueType())
                    {
                        case SerializedActorValueType.Method:
                            {
                                List<ParameterInfo> pinfos = minfo?.GetParameters()?.ToList();
                                if (pinfos == null || pinfos.Count == 0)
                                    return null;
                                return pinfos[0].ParameterType;
                            }                            
                        case SerializedActorValueType.Field:
                            return finfo?.FieldType;
                        case SerializedActorValueType.Property:
                            return pinfo?.PropertyType;
                        default:
                            return null;
                    }
                }
            }
            public void ApplySerInfo(SerInfos info)
            {
                ValueName = info.GetValueName();
                valueType = info.GetValueType();
                if(SavedValue?.GetType() != info.GetParOrValueType())
                {
                    Type theType = info.GetParOrValueType();
                    if (theType == null || !theType.IsValueType)
                        SavedValue = null;
                    else
                        SavedValue = Activator.CreateInstance(theType);
                }
                if(info.GetParOrValueType() != allowedDynamicType)
                {
                    IsDynamic = false;
                }
            }
            public SerInfos GetSerInfos()
            {
                SerInfos infos = new SerInfos();

                Type finalType = GetInvoker()?.GetType();

                string valName = ValueName;

                switch (valueType)
                {
                    case SerializedActorValueType.Method:
                        {
                            List< MethodInfo> methodInfos = finalType?.GetMethods().Where(mi=>mi.Name.Equals(valName)).ToList();
                            if (methodInfos.Count > 1)
                                methodInfos = methodInfos.Where((mi) => mi.GetParameters().Count() < 2).ToList();

                            infos.minfo = methodInfos.FirstOrDefault();
                            break;
                        }
                    case SerializedActorValueType.Field:
                        {
                            infos.finfo = finalType?.GetFields().SingleOrDefault(mi => mi.Name.Equals(valName));
                            break;
                        }
                    case SerializedActorValueType.Property:
                        {
                            infos.pinfo = finalType?.GetProperties().SingleOrDefault(mi => mi.Name.Equals(valName));
                            break;
                        }
                }
                return infos;
            }

            public object SavedValue;
            public bool IsDynamic;

            public string ValueName;

            public SerializedActorType type;
            public SerializedActorValueType valueType;
            public string GetDisplayValueName()
            {
                switch (valueType)
                {
                    case SerializedActorValueType.Method:
                        return GetInvoker().GetType().Name + "." + ValueName + "()";
                    case SerializedActorValueType.Field:
                    case SerializedActorValueType.Property:
                        return GetInvoker().GetType().Name  + "." + ValueName;
                    default:
                        return "Not set";
                }
            }
            public object GetInvoker()
            {
                if (type == SerializedActorType.Actor)
                    return Actor;
                else if (type == SerializedActorType.Script)
                    return script;
                else
                    return null;
            }

            public Actor Actor;
            public Script script;

            public Actor GetActor()
            {
                if (type == SerializedActorType.Actor)
                    return Actor;
                else if (type == SerializedActorType.Script)
                    return script.Actor;
                else
                    return null;
            }
            public object GetSavedValueOrDynamic(object dynamic, Type targetType)
            {
                if (IsDynamic)
                    return dynamic;
                else
                {
                    if (SavedValue == null)
                        return null;
                    if (targetType != null && SavedValue.GetType() == typeof(long) && targetType != typeof(long))
                        SavedValue = (int)(long)SavedValue;
                    return SavedValue;
                }                    
            }

            public override bool Equals(object obj)
            {
                if(obj is SerializedMethod otherSerMethod)
                {
                    bool valEq = SavedValue == otherSerMethod.SavedValue && ValueName == otherSerMethod.ValueName;
                    bool typesQr = type == otherSerMethod.type && valueType == otherSerMethod.valueType;
                    bool invokersEq = Actor == otherSerMethod.Actor && script == otherSerMethod.script;
                    bool dynamicEq = IsDynamic == otherSerMethod.IsDynamic;
                    return valEq && typesQr && invokersEq && dynamicEq;
                }
                else
                    return false;
            }
            public Type allowedDynamicType;
            public void Invoke(object dynamicValue = null)
            {
                object invoker = GetInvoker();
                SerializedMethod.SerInfos info = GetSerInfos();

                switch (info.GetValueType())
                {
                    case SerializedMethod.SerializedActorValueType.Method:
                        {
                            object[] parameters = new object[] { GetSavedValueOrDynamic(dynamicValue, info.GetParOrValueType()) };
                            if (parameters[0] == null || info.minfo.GetParameters().Count() == 0)
                                parameters = null;
                            try
                            {
                                info.minfo.Invoke(invoker, parameters);
                            }
                            catch (Exception e)
                            {
                                Debug.LogWarning("Failed to invoke method of GameEvent on " + GetActor() + " with ValueName: " + GetDisplayValueName() + ", reason: " + e.Message);
                                Debug.LogWarning(e.StackTrace);
                            }
                            break;
                        }
                    case SerializedMethod.SerializedActorValueType.Property:
                        {
                            try
                            {
                                info.pinfo.SetValue(invoker, GetSavedValueOrDynamic(dynamicValue, info.GetParOrValueType()));
                            }
                            catch (Exception e)
                            {
                                Debug.LogWarning("Failed to set property of GameEvent on " + GetActor() + " with ValueName: " + GetDisplayValueName() + ", reason: " + e.Message);
                                Debug.LogWarning(e.StackTrace);
                            }
                            break;
                        }
                    case SerializedMethod.SerializedActorValueType.Field:
                        {
                            try
                            {
                                info.finfo.SetValue(invoker, GetSavedValueOrDynamic(dynamicValue, info.GetParOrValueType()));
                            }
                            catch (Exception e)
                            {
                                Debug.LogWarning("Failed to set field of GameEvent on " + GetActor() + " with ValueName: " + GetDisplayValueName() + ", reason: " + e.Message);
                                Debug.LogWarning(e.StackTrace);
                            }
                            break;
                        }
                    default:
                        {
                            Debug.LogWarning("Could not deserialize GameEvent on  " + GetActor() + " with valueName " + ValueName);
                            break;
                        }
                }
            }
        }
    }
    public class GameEvent : GameEventBase
    {
        List<GameAction> actions = new List<GameAction>();
        public void AddListener(GameAction action)
        {
            actions.Add(action);
        }
        public void ClearAllListeners()
        {
            actions.Clear();
        }
        public class GameEventVoid { }

        public SerMethList<GameEventVoid> actorMethods = new SerMethList<GameEventVoid>();

        public void Invoke()
        {
            foreach (SerializedMethod actorMethod in actorMethods)
            {
                actorMethod.Invoke();
            }
            foreach (GameAction action in actions)
            {
                action?.Invoke();
            }
        }    
    }
    public class SerMethList<T0> : List<GameEventBase.SerializedMethod>
    {

    }

    public class GameEvent<T0> : GameEventBase
    {
        List<GameAction<T0>> actions = new List<GameAction<T0>>();

        public void AddListener(GameAction<T0> action)
        {
            actions.Add(action);
        }
        public void ClearAllListeners()
        {
            actions.Clear();
        }

        public SerMethList<T0> actorMethods = new SerMethList<T0>();

        public void Invoke(T0 val)
        {
            foreach (SerializedMethod actorMethod in actorMethods)
            {
                actorMethod.Invoke(val);
            }
            foreach (GameAction<T0> action in actions)
            {
                action?.Invoke(val);
            }
        }
    }
}
