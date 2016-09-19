using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SC.CrosswordSolver.Logic
{
    [Serializable]
    public class Crossword
    {
        public Crossword(int height, int width, List<Word> words)
        {
            Width = width;
            Height = height;
            Words = words;
        }

        public Crossword(char[,] dataGrid)
        {
            Height = dataGrid.GetLength(0);
            Width = dataGrid.GetLength(1);
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    if (dataGrid[i, j] == '*')
                    {
                        if (dataGrid[i, j + 1] == ' ')
                        {
                            var l = 1;
                            while (dataGrid[i, j + l + 1] == ' ') l++;
                            Words.Add(new Word(i,j,l,Word.Orientation.Across));
                        }
                        if (dataGrid[i+1,j] == ' ')
                        {
                            var l = 1;
                            while (dataGrid[i+l+1,j] == ' ') l++;
                            Words.Add(new Word(i, j, l, Word.Orientation.Down));
                        }
                    }
                }
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

            public Word(int yPosition, int xPosition, int length, Orientation direction)
            {
                X = xPosition;
                Y = yPosition;
                Letters = new char[length];
                Direction = direction;
            }
        }

        public void Save(string path)
        {
            using (var stream = File.OpenWrite(path))
            {
                var binForm = new BinaryFormatter();
                binForm.Serialize(stream, this);
            }
        }

        public static Crossword Load(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var binForm = new BinaryFormatter();
                return (Crossword) binForm.Deserialize(stream);
            }
        }
    }
}