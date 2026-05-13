//using LsMsgPack;
using MsgPack.Serialization;
using static Animalopoly.Code.Graphing;
using static Animalopoly.Code.PlayerClass;

namespace Animalopoly.Code
{
    public class Saving
    {
        public class GameState
        {
            [MessagePackMember(0)]
            private readonly Player[] players;
            
            [MessagePackMember(1)]
            private readonly Grapher grapher;
            
            [MessagePackMember(2)]
            private readonly FilePathSafeString currentGameName;
            
            [MessagePackMember(3)]
            private readonly int turnCount;

            [MessagePackMember(4)]
            private readonly bool cheats;

            public GameState(Player[] players, Grapher grapher, FilePathSafeString currentGameName, int turnCount, bool cheats)
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
            public FilePathSafeString GetCurrentGameName()
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
        private static string CleanFilePath(string saveName)
        {
            foreach (char badChar in Path.GetInvalidFileNameChars())
            {
                saveName = saveName.Replace(badChar, '-');
            }
            saveName = saveName.Trim(); // Leading or trailing whitespace aren't supported
            return saveName;
        }
        public class FilePathSafeString
        {
            [MessagePackMember(0)]
            private readonly string value;
            public FilePathSafeString(string value)
            {
                this.value = CleanFilePath(value);
            }
            public static implicit operator string(FilePathSafeString safeFilePath)
            {
                return safeFilePath.value;
            }
            public override string ToString()
            {
                return this.value;
            }
        }
        static readonly SerializationContext context = new SerializationContext { SerializationMethod = SerializationMethod.Array };
        public static void Serialise<T>(T item, string filepath)
        { // General-purpose serialising function
            // Prepare the stream
            string? _parentDirectory = Path.GetDirectoryName(filepath);
            if (_parentDirectory is not string parentDirectory) // Checks that _parentDirectory isn't null and simultaneously converts it to a non-nullable string
            {
                throw new FileNotFoundException("Invalid path");
            }
            Directory.CreateDirectory(parentDirectory); // Prevents errors if part of the filepath is missing
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

        static long DateTimeToTimestamp(DateTime time) // From: https://aske.wachs.dk/06/07/2021/c-conversion-between-unix-timestamps-and-datetime/
        {
            return ((DateTimeOffset)time).ToUnixTimeMilliseconds();
        }
        public static string GenerateInfoCSV(int version, DateTime creationTime, DateTime modificationTime)
        {
            switch (version)
            {
                case 1:
                    throw new NotImplementedException("Attempted to generate a v1 info.csv");
                case 2:
                    return $"v2\ncreatedDate,modifiedDate\n{DateTimeToTimestamp(creationTime)},{DateTimeToTimestamp(modificationTime)}";
                default:
                    throw new Exception($"Unknown version {version}");
            }
        }
        public static (DateTimeOffset, DateTimeOffset) ParseInfoCSV(string file_contents) // Returns createdDate, modifiedDate
        {
            DateTimeOffset createdDate;
            DateTimeOffset modifiedDate;
            
            int version;
            string dataLine;
            if (file_contents[0] != 'v')
            {
                version = 1;
                dataLine = file_contents.Split('\n')[1]; // First line headers, second line data
            }
            else
            {
                version = Convert.ToInt32(Convert.ToString(file_contents[1]));
                dataLine = file_contents.Split('\n')[2]; // First line version, second line headers, third line data
            }
            string[] data = dataLine.Split(',');
            switch (version)
            {
                case 1:
                    throw new NotImplementedException("Attempted to parse a v1 info.csv");
                case 2:
                    createdDate = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(data[0]));
                    modifiedDate = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(data[1]));
                    break;
                default:
                    throw new Exception($"Unknown version {version}");
            }
            return (createdDate, modifiedDate);
        }
    }
}

// Known bugs:
//  Names reset
//  Positions not loaded
//  Whose turn not saved
//  Duplicated labels in Grapher