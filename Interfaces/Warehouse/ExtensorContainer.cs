using ColossalFramework;
using ColossalFramework.Threading;
using ICities;
using Klyte.Commons.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Klyte.Commons.Interfaces
{
    public sealed class ExtensorContainer : Singleton<ExtensorContainer>, ISerializableDataExtension
    {
        public static event Action OnDataLoaded;

        public Dictionary<Type, IDataExtensor> Instances { get; private set; } = new Dictionary<Type, IDataExtensor>();

        #region Serialization
        public IManagers Managers => SerializableDataManager?.managers;

        public ISerializableData SerializableDataManager { get; private set; }

        public void OnCreated(ISerializableData serializableData) => SerializableDataManager = serializableData;
        public void OnLoadData()
        {
            LogUtils.DoLog($"LOADING DATA {GetType()}");
            instance.Instances = new Dictionary<Type, IDataExtensor>();
            List<Type> instancesExt = ReflectionUtils.GetInterfaceImplementations(typeof(IDataExtensor), GetType());
            var instancesLegacies = ReflectionUtils.GetSubtypesRecursive(typeof(DataExtensorLegacyBase<>), GetType()).ToDictionary(x => x.BaseType.GetGenericArguments()[0], x => x);
            LogUtils.DoLog($"SUBTYPE COUNT: {instancesExt.Count}; LEGACY COUNT: {instancesLegacies.Count}");
            foreach (Type type in instancesExt)
            {
                LogUtils.DoLog($"LOADING DATA TYPE {type}");
                if (type.IsGenericType)
                {
                    try
                    {
                        var targetParameters = type.Assembly.GetTypes().Where(x => !x.IsAbstract && !x.IsInterface && !x.IsGenericType && ReflectionUtils.CanMakeGenericTypeVia(type.GetGenericArguments()[0], x)).ToArray();
                        LogUtils.DoLog($"PARAMETER PARAMS FOR {type.GetGenericArguments()[0]} FOUND: [{string.Join(",", targetParameters.Select(x => x.ToString()).ToArray())}]");
                        foreach (var param in targetParameters)
                        {
                            var targetType = type.MakeGenericType(param);
                            ProcessExtensor(instancesLegacies, targetType);
                        }
                    }
                    catch (Exception e)
                    {
                        LogUtils.DoErrorLog($"FAILED CREATING GENERIC PARAM EXTENSOR: {e}");
                    }
                }
                else
                {
                    ProcessExtensor(instancesLegacies, type);
                }
            }

            ThreadHelper.dispatcher.Dispatch(() =>
            {
                OnDataLoaded?.Invoke();
                OnDataLoaded = null;
            });
        }

        private void ProcessExtensor(Dictionary<Type, Type> instancesLegacies, Type type)
        {
            var basicInstance = (IDataExtensor)Activator.CreateInstance(type);
            if (!SerializableDataManager.EnumerateData().Contains(basicInstance.SaveId))
            {
                LogUtils.DoLog($"SEARCHING FOR LEGACY {type}");
                if (instancesLegacies.ContainsKey(type))
                {
                    var basicInstanceLegacy = (IDataExtensorLegacy)instancesLegacies[type].GetConstructor(new Type[0]).Invoke(new Type[0]);
                    if (!SerializableDataManager.EnumerateData().Contains(basicInstanceLegacy.SaveId))
                    {
                        byte[] storage2 = MemoryStreamToArray(basicInstanceLegacy.SaveId);
                        try
                        {
                            instance.Instances[type] = basicInstanceLegacy.Deserialize(storage2) ?? basicInstance;
                            if (CommonProperties.DebugMode)
                            {
                                string content = System.Text.Encoding.UTF8.GetString(storage2);
                                LogUtils.DoLog($"{type} DATA LEGACY {storage2.Length}b => {content}");
                            }
                            return;
                        }
                        catch (Exception e)
                        {
                            string content = System.Text.Encoding.UTF8.GetString(storage2);
                            LogUtils.DoLog($"{type} CORRUPTED DATA! => \nException: {e.Message}\n{e.StackTrace}\nData  {storage2.Length}b:\n{content}");
                        }
                    }
                }
                LogUtils.DoLog($"NO DATA TYPE {type} & NO LEGACY");
                instance.Instances[type] = basicInstance;
                basicInstance.LoadDefaults();
                return;
            }
            using (var memoryStream = new MemoryStream(SerializableDataManager.LoadData(basicInstance.SaveId)))
            {
                byte[] storage = memoryStream.ToArray();
                try
                {
                    instance.Instances[type] = basicInstance.Deserialize(type, storage) ?? basicInstance;
                    if (CommonProperties.DebugMode)
                    {
                        string content = System.Text.Encoding.UTF8.GetString(storage);
                        LogUtils.DoLog($"{type} DATA {storage.Length}b => {content}");
                    }
                }
                catch (Exception e)
                {
                    string content = System.Text.Encoding.UTF8.GetString(storage);
                    LogUtils.DoLog($"{type} CORRUPTED DATA! => \nException: {e.Message}\n{e.StackTrace}\nData  {storage.Length}b:\n{content}");
                    instance.Instances[type] = basicInstance;
                }
            }
        }

        private byte[] MemoryStreamToArray(string saveId)
        {
            using (var memoryStream2 = new MemoryStream(SerializableDataManager.LoadData(saveId)))
            {
                byte[] storage2 = memoryStream2.ToArray();
                return storage2;
            }
        }

        // Token: 0x0600003B RID: 59 RVA: 0x00004020 File Offset: 0x00002220
        public void OnSaveData()
        {
            LogUtils.DoLog($"SAVING DATA {GetType()}");

            foreach (Type type in instance.Instances.Keys)
            {
                if (instance.Instances[type]?.SaveId == null || Singleton<ToolManager>.instance.m_properties.m_mode != ItemClass.Availability.Game)
                {
                    continue;
                }


                byte[] data = instance.Instances[type]?.Serialize();
                if (CommonProperties.DebugMode)
                {
                    string content = System.Text.Encoding.UTF8.GetString(data);
                    LogUtils.DoLog($"{type} DATA (L = {data.Length}) =>  {content}");
                }
                if (data.Length == 0)
                {
                    return;
                }
                try
                {
                    SerializableDataManager.SaveData(instance.Instances[type].SaveId, data);
                }
                catch (Exception e)
                {
                    LogUtils.DoErrorLog($"Exception trying to serialize {type}: {e} -  {e.Message}\n{e.StackTrace} ");
                }
            }
        }

        public void OnReleased()
        {
            foreach (IDataExtensor item in instance.Instances.Values)
            {
                item.OnReleased();
            }
        }
        #endregion
    }
}
