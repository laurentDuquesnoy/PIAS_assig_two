using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using SerializingUtils.Interfaces;
using SerializingUtils.enums;

namespace SerializingUtils.Classes
{
    public class Serializer : ISerializer
    {
        public ConverterResult<T> DesirializeObjectFromXML<T>(string absoluteFolderPath, string fileName)
        {
            var result = new ConverterResult<T>() {Status = ConverterStatus.Ok};
            var fullFilePath = Path.Combine(absoluteFolderPath, fileName);
            {
                if (File.Exists(fullFilePath))
                {
                    XmlSerializer desirializer = new XmlSerializer(typeof(T));
                    TextReader reader = new StreamReader(fullFilePath);

                    object obj = desirializer.Deserialize(reader);

                    result.ReturnValue = (T)obj;
                }
                else
                {
                    result.Status = ConverterStatus.HasError;
                    result.Error = new Exception("FIle does not exist");
                }
            }

            return result;
        }

       

        public ConverterResult<T> SerializeOjectToXML<T>(string absoluteFolderPath,  string fileName ,T objectToSerialize)
        {
            var result = new ConverterResult<T>() {Status = ConverterStatus.Ok};
            try
            {
                string fullFilePath; 
                
                if (!string.IsNullOrWhiteSpace(fileName))
                { 
                    fullFilePath = Path.Combine(absoluteFolderPath, fileName);
                }
                else
                { 
                    fullFilePath = Path.Combine(absoluteFolderPath, typeof(T).ToString());
                }
               
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                using (TextWriter writer = new StreamWriter(fullFilePath))
                {
                    serializer.Serialize(writer, objectToSerialize);

                }
            }
            catch (Exception e)
            {
                result.Error = e;
                result.Status = ConverterStatus.HasError;
            }

            return result;
        }
        
        public ConverterResult<TOts> SerializeObjectToJSONFile<TOts>(string absoluteFolderPath, string fileName, TOts objectToSerialize)
        {
            var result = new ConverterResult<TOts>() { Status = ConverterStatus.Ok };
            try
            {
                var fullFilePath = Path.Combine(absoluteFolderPath, fileName);
                var jsonString = System.Text.Json.JsonSerializer.Serialize<TOts>(objectToSerialize);
                File.WriteAllText(fullFilePath, jsonString);
            }
            catch (Exception e)
            {
                result.Error = e;
                result.Status = ConverterStatus.HasError;
            }
            return result;
        }
        
        public ConverterResult<TOtds> DeserializeObjectFromJSONFile<TOtds>(string absoluteFolderPath, string fileName)
        {
            var result = new ConverterResult<TOtds>() { Status = ConverterStatus.Ok };
            var fullFilePath = Path.Combine(absoluteFolderPath, fileName);
            if (File.Exists(fullFilePath))
            {
                string JSONString = File.ReadAllText(fullFilePath);
                result.ReturnValue = System.Text.Json.JsonSerializer.Deserialize<TOtds>(JSONString);
            }
            else
            {
                result.Status = ConverterStatus.HasError;
                result.Error = new Exception("File does not exist");
            }
            return result;
        }

        public ConverterResult<T> Serialize<T>(string absoluteFolderPath, string fileName, T objectToSerialize, FileType f)
        {
            ConverterResult<T> result;
            result = new ConverterResult<T>() { Status = ConverterStatus.Ok };

            if (f == FileType.JSON)
            {
                absoluteFolderPath += "/JSON/";
                if (!Directory.Exists(absoluteFolderPath))
                {
                    Directory.CreateDirectory(absoluteFolderPath);
                }
                result = SerializeObjectToJSONFile(absoluteFolderPath, fileName, objectToSerialize);
            }else if (f == FileType.XML)
            {
                absoluteFolderPath += "/XML/";
                if (!Directory.Exists(absoluteFolderPath))
                {
                    Directory.CreateDirectory(absoluteFolderPath);
                }
                result = SerializeOjectToXML(absoluteFolderPath, fileName, objectToSerialize);
            }

            return result;
        }
    }
}