using SerializingUtils.Classes;

namespace SerializingUtils.Interfaces
{
    public interface ISerializer
    {
        public ConverterResult<T> DesirializeObjectFromXML<T>(string absoluteFolderPath, string fileName);

        public ConverterResult<T> SerializeOjectToXML<T>(string absoluteFolderPath, string fileName, T objectToSerialize);
    }
}