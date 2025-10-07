using LsMsgPack;

namespace Animalopoly.Code
{
    class Saving
    {
        public static void Serialise<T>(T item, string filepath)
        {
            // Convert the object to bytes
            byte[] buffer = MsgPackSerializer.Serialize(item);

            // Write the buffer to the file
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            Stream stream = File.Open(filepath, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(buffer);

            writer.Close();
            stream.Close();
        }
        public static T Deserialise<T>(string filepath) // TODO: add error handling for if filepath does not exist or cannot be parsed
        {
            // Read the buffer from the file
            Stream stream = File.Open(filepath, FileMode.Open);
            BinaryReader reader = new BinaryReader(stream);

            byte[] buffer = reader.ReadBytes((int)stream.Length);

            reader.Close();
            stream.Close();

            // Convert the bytes back to an object of class T
            T result = MsgPackSerializer.Deserialize<T>(buffer);
            return result;
        }
    }
}