using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using NeuralNet;
using MySerializer;

namespace ConnectFour
{
    /// <summary>
    /// Deserializes or Parses connect-4 8-ply database into a List of Boards then serializes as needed.
    /// </summary>
    public static class DataParser
    {
        public static string RawFileLocation = "connect-4.txt";
        public static string FileLocation = "ValidationSet.bin";
        public static List<Example> ValidationSet { get { return Store.ValidationSet; }}

        public static Storage Store = new Storage();
        public class Storage
        {
            public List<Example> ValidationSet = new List<Example>();
            public Func<Checker,double> CheckerTransform = Transform.ToValue;
            public Func<GameResult,double> GameResultTransform = Transform.ToValue;
        }

        static DataParser()
        {
            if (!File.Exists(FileLocation))
            {
                Parse();
            }
            else
            {
                try { Store = (Storage)Serializer.Deserialize(FileLocation); }
                catch { Parse(); }
                if (!Transform.Verify(Store.CheckerTransform) || !Transform.Verify(Store.GameResultTransform))
                    Parse();
            }
        }

        /// <summary>
        /// Parses the validation set from connect-4 8-ply database.
        /// </summary>
        /// <returns>Validation set</returns>
        public static List<Example> Parse()
        {
            if (!File.Exists(RawFileLocation))
                throw new Exception("Make sure " + RawFileLocation + " exists!  If not, obtain from the machine learning respository.");
                
            Store = new Storage();
            using (StreamReader reader = new StreamReader(RawFileLocation))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');

                    Example example = new Example();
                    for (int i = 0; i < values.Length - 1; ++i)
                        example.Features.Add(0);

                    for (int i = 0; i < values.Length - 1; ++i)
                    {
                        string x = values[i].ToLower().Trim();
                        Checker checker = Checker.Empty;
                        switch (x)
                        {
                            case "x": checker = Checker.Blue; break;
                            case "o": checker = Checker.Green; break;
                            case "b": checker = Checker.Empty; break;
                        }
                        // Format of linear board data in connect-4.txt is bottom to top, left to right
                        // Need to convert to our format of: Left to right, top to bottom.
                        int row = 6 - i % 6;
                        int column = i / 7;
                        example.Features[row * 7 + column] = Transform.ToValue(checker);
                    }
                    string result = values[values.Length - 1].ToLower().Trim();

                    // Current values denote next player that goes will be guaranteed to win/lose/draw given he/she plays optimally...  
                    //  We need to normalize this for our network... Ie, the label should instead denote if last player that went for given board position win/loses/ties if he/she plays optimally.
                    result = result == "win" ? "loss" : result == "loss" ? "win" : "draw";

                    example.Labels.Add(Transform.ToValue((GameResult)Enum.Parse(typeof(GameResult), result)));
                }
            }

            Serializer.Serialize(Store, FileLocation);
            return ValidationSet;
        }

        

    }
}
