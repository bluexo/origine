using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using ExcelDataReader;
using System.Reflection;

namespace Origine.Configuration
{
    public class ExcelConfigFilesReader : IConfigFilesReader
    {
        private readonly Dictionary<string, (string path, byte[] content)> configFiles = new Dictionary<string, (string, byte[])>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, object> configCache = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        private readonly string _filePath;
        private readonly int _fieldNameRowIndex = 0;

        public ExcelConfigFilesReader(ExcelReaderOptions options)
        {
            _filePath = options.Path;
            _fieldNameRowIndex = options.FieldNameRow;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ReadFiles();
        }

        private void ReadFiles()
        {
            var files = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, _filePath), "*.xls*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var fileName = fileInfo.Name.Split('.').FirstOrDefault();
                if (string.IsNullOrWhiteSpace(fileName)) continue;
                configFiles.Add(fileName, (file, File.ReadAllBytes(file)));
            }
        }

        public IList<T> Get<T>(string fileName, bool reload = false, Predicate<T> predicate = null) where T : new()
        {
            if (!reload && configCache.ContainsKey(fileName))
                return configCache[fileName] as IList<T>;

            if (!configFiles.ContainsKey(fileName))
                throw new NullReferenceException(fileName);

            var buffer = reload
                ? File.ReadAllBytes(configFiles[fileName].path)
                : configFiles[fileName].content;

            if (buffer == null)
                throw new NullReferenceException(fileName);

            var configs = new List<T>();
            using var stream = new MemoryStream(buffer);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var fieldNames = new List<string>();
            var currentRowIndex = 0;
            while (reader.Read())
            {
                currentRowIndex++;
                if (currentRowIndex < _fieldNameRowIndex) continue;
                if (TryGetValueFormReader(reader, currentRowIndex, fieldNames, out T obj))
                {
                    if (currentRowIndex == _fieldNameRowIndex) continue;
                    if (predicate != null && !predicate(obj)) continue;
                    configs.Add(obj);
                }
            }
            configCache[fileName] = configs;
            return configs;
        }

        private bool TryGetValueFormReader<T>(IExcelDataReader reader, int currentRowIndex, List<string> fieldNames, out T obj) where T : new()
        {
            obj = new T();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (currentRowIndex != _fieldNameRowIndex)
                {
                    var fieldName = fieldNames[i];
                    if (string.IsNullOrWhiteSpace(fieldName))
                        continue;
                    var field = obj.GetType().GetRuntimeField(fieldName);
                    if (field != null)
                    {
                        if (field.GetCustomAttribute<Ignore>() != null)
                            continue;

                        var target = GetTargetValue(reader, i, field.FieldType);
                        //var reqired = field.GetCustomAttributes<Required>();

                        //if (reqired != null && target == null) 
                        //    return false;

                        field.SetValue(obj, target);
                        continue;
                    }
                    var property = obj.GetType().GetRuntimeProperty(fieldName);
                    if (property != null)
                    {
                        if (property.GetCustomAttribute<Ignore>() != null)
                            continue;

                        var target = GetTargetValue(reader, i, property.PropertyType);
                        //var reqired = property.GetCustomAttributes<Required>();

                        //if (reqired != null && reqired.Count() > 0 && target == null) 
                        //    return false;

                        property.SetValue(obj, target);
                    }
                }
                else
                {
                    fieldNames.Add(reader.GetString(i));
                }
            }
            return true;
        }

        private object GetTargetValue(IExcelDataReader reader, int index, Type dest)
        {
            var value = reader.GetValue(index);

            if (dest == typeof(bool)) return Convert.ToBoolean(value);
            if (dest == typeof(char)) return Convert.ToChar(value);
            if (dest == typeof(byte)) return Convert.ToByte(value);
            if (dest == typeof(short)) return Convert.ToInt16(value);
            if (dest == typeof(int)) return Convert.ToInt32(value);
            if (dest == typeof(long)) return Convert.ToInt64(value);

            if (dest == typeof(sbyte)) return Convert.ToSByte(value);
            if (dest == typeof(ushort)) return Convert.ToUInt16(value);
            if (dest == typeof(uint)) return Convert.ToUInt32(value);
            if (dest == typeof(ulong)) return Convert.ToUInt64(value);

            if (dest == typeof(float)) return Convert.ToSingle(value);
            if (dest == typeof(double)) return Convert.ToDouble(value);
            if (dest == typeof(decimal)) return Convert.ToDecimal(value);

            return value == null ? string.Empty : value.ToString();
        }
    }
}
