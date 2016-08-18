using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Logic
{
    [Serializable]
    public class Crossword
    {
        public char[,] Grid { get; }
        public List<Word> Words { get; }

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