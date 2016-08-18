using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    [Serializable]
    public class Crossword
    {
        public char[,] Grid { get; }

        public readonly List<Word> Words = new List<Word>();

        [Serializable]
        public struct Word
        {
            public enum Orientation { Down, Across }
            public readonly int X, Y;
            public char[] Letters;
            public int Length => Letters.Length;
            public Orientation Direction;
            
            public Word(int xPosition, int yPosition, int length, Orientation direction)
            {
                X = xPosition;
                Y = yPosition;
                Letters = new char[length];
                Direction = direction;
            }
        }
    }
}