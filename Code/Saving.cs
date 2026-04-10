//using LsMsgPack;
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
            private readonly Player[] players;
            
            [MessagePackMember(1)]
            private readonly Grapher grapher;
            
            [MessagePackMember(2)]
            private readonly string currentGameName;
            
            [MessagePackMember(3)]
            private readonly int turnCount;

            [MessagePackMember(4)]
            private readonly bool cheats;

            public GameState(Player[] players, Grapher grapher, string currentGameName, int turnCount, bool cheats)
            { // Contains all the important infomation needed to save and resume the game
                this.players = players;
                this.grapher = grapher;
                this.currentGameName = currentGameName;
                this.turnCount = turnCount;
                this.cheats = cheats;
            }
            public Player[] GetPlayers()
            {
                return this.players;
            }
            public Grapher GetGrapher()
            {
                return this.grapher;
            }
            public string GetCurrentGameName()
            {
                return this.currentGameName;
            }
            public int GetTurnCount()
            {
                return this.turnCount;
            }
            public bool GetCheats()
            {
                return this.cheats;
            }
        }
        static readonly SerializationContext context = new SerializationContext { SerializationMethod = SerializationMethod.Array };
        public static void Serialise<T>(T item, string filepath)
        { // General-purpose serialising function
            // Prepare the stream
            string? _parentDirectory = Path.GetDirectoryName(filepath);
            if (_parentDirectory is not string parentDirectory) // Checks that _parentDirectory isn't null and simultaneously converts it to a non-nullable string
            {
                throw new Exception("Invalid path");
            }
            Directory.CreateDirectory(parentDirectory);
            Stream stream = File.Open(filepath, FileMode.Create);

            // Initiate serialiser
            MessagePackSerializer<T> serialiser = MessagePackSerializer.Get<T>(context);

            // Convert the object to bytes
            serialiser.Pack(stream, item);

            stream.Close();
        }
        public static T Deserialise<T>(string filepath)
        { // General-purpose deserialising function
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException($"Cannot find {filepath}");
            }
            // Prepare the stream
            Stream stream = File.Open(filepath, FileMode.Open);

            // Initiate deserialiser
            MessagePackSerializer<T> deserialiser = MessagePackSerializer.Get<T>(context);

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