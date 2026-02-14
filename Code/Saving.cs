using LsMsgPack;
using MsgPack.Serialization;
using static Animalopoly.Code.Graphing;
using static Animalopoly.Code.PlayerClass;

namespace Animalopoly.Code
{
    public class Saving
    {
        public class GameState // As MessagePackSerializer requires the type to be public, GameState and Saving must be public and hence Graphing and Program must be public to allow Player and Grapher to be used
        {
            [MessagePackMember(0)]
            public Player[] players;
            
            [MessagePackMember(1)]
            public Grapher grapher;
            
            [MessagePackMember(2)]
            public string currentGameName;
            
            [MessagePackMember(3)]
            public int turnCount;
        }
        public static void Serialise<T>(T item, string filepath)
        {
            // Prepare the stream
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            Stream stream = File.Open(filepath, FileMode.Create);

            // Initiate serialiser
            var context = new SerializationContext { SerializationMethod = SerializationMethod.Array };
            var serialiser = MessagePackSerializer.Get<T>(context);

            // Convert the object to bytes
            serialiser.Pack(stream, item);

            stream.Close();
        }
        public static T Deserialise<T>(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException($"Cannot find {filepath}");
            }
            // Prepare the stream
            Stream stream = File.Open(filepath, FileMode.Open);

            // Initiate deserialiser
            var context = new SerializationContext { SerializationMethod = SerializationMethod.Array };
            var deserialiser = MessagePackSerializer.Get<T>(context);

            // Convert the bytes back to an object of class T
            T result = deserialiser.Unpack(stream);

            stream.Close();
            return result;
        }
    }
}

// Known bugs:
//  Names reset
//  Positions not loaded
//  Whose turn not saved
//  Duplicated labels in Grapher