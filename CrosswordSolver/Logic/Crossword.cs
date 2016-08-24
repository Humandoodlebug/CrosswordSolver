using System;
using System.Collections.Generic;

namespace SC.CrosswordSolver.Logic
{
    [Serializable]
    public class Crossword
    {
        public Crossword(int width, int height, List<Word> words)
        {
            Width = width;
            Height = height;
            Words = words;
        }

        public char[,] CrosswordData
        {
            get
            {
                var array = new char[Height, Width];
                for (int i = 0; i < Height; i++)
                    for (int j = 0; j < Width; j++)
                    {
                        array[i, j] = ' ';
                    }
                foreach (var word in Words)
                {
                    if (word.Direction == Word.Orientation.Down)
                        for (int i = 0; i < word.Length; i++)
                        {
                            array[word.Y + i, word.X] = word.Letters[i];
                        }
                    else for (int i = 0; i < word.Length; i++)
                        {
                            array[word.Y, word.X + i] = word.Letters[i];
                        }
                }
                return array;
            }
        }

        public List<Word> Words { get; }
        public int Width { get; }
        public int Height { get; }

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

        public void Save(string path)
        {
            throw new NotImplementedException();
        }

        public static Crossword Load(string path)
        {
            throw new NotImplementedException();
        }
    }
}