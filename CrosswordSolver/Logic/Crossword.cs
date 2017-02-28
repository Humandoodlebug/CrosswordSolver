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
            Words = new List<Word>();
            for (var i = 0; i < Height; i++)
                for (var j = 0; j < Width; j++)
                {
                    if (dataGrid[i, j] == '_') continue;
                    if ((j + 1 < dataGrid.GetLength(1) && dataGrid[i, j + 1] == ' ') &&
                        (j == 0 || dataGrid[i, j - 1] == '_'))
                    {
                        var l = 1;
                        while (j + l < dataGrid.GetLength(1) && dataGrid[i, j + l] == ' ') l++;
                        Words.Add(new Word(i, j, l, Word.Orientation.Across));
                    }
                    if ((i + 1 < dataGrid.GetLength(0) && dataGrid[i + 1, j] == ' ') &&
                        (i == 0 || dataGrid[i - 1, j] == '_'))
                    {
                        var l = 1;
                        while (i + l < dataGrid.GetLength(0) && dataGrid[i + l, j] == ' ') l++;
                        Words.Add(new Word(i, j, l, Word.Orientation.Down));
                    }
                }
        }

        public char[,] CrosswordData
        {
            get
            {
                var array = new char[Height, Width];
                for (var i = 0; i < Height; i++)
                    for (var j = 0; j < Width; j++)
                        array[i, j] = '_';
                foreach (var word in Words)
                    if (word.Direction == Word.Orientation.Down)
                        for (var i = 0; i < word.Length; i++)
                            array[word.Y + i, word.X] = word.Letters[i];
                    else
                        for (var i = 0; i < word.Length; i++)
                            array[word.Y, word.X + i] = word.Letters[i];
                return array;
            }
        }

        public List<Word> Words { get; }
        public int Width { get; }
        public int Height { get; }

        public void Update(char c, int row, int column)
        {
            foreach (var word in Words)
            {
                if (word.Direction == Word.Orientation.Down && column == word.X && row >= word.Y &&
                    row < word.Y + word.Length)
                {
                    for (var i = 0; i < word.Length; i++)
                        if (word.Y + i == row)
                        {
                            word.Letters[i] = c;
                            break;
                        }
                }
                else if (word.Direction == Word.Orientation.Across && row == word.Y && column >= word.X &&
                         column < word.X + word.Length)
                {
                    for (var i = 0; i < word.Length; i++)
                        if (word.X + i == column)
                        {
                            word.Letters[i] = c;
                            break;
                        }
                }
            }
        }

        public void Save(Stream stream)
        {
            using (stream)
            {
                var binForm = new BinaryFormatter();
                binForm.Serialize(stream, this);
            }
        }

        public static Crossword Load(Stream stream)
        {
            using (stream)
            {
                var binForm = new BinaryFormatter();
                return (Crossword) binForm.Deserialize(stream);
            }
        }

        [Serializable]
        public struct Word
        {
            public enum Orientation
            {
                Down,
                Across
            }

            public readonly int X, Y;
            public char[] Letters;
            public int Length => Letters.Length;
            public Orientation Direction;

            public Word(int yPosition, int xPosition, int length, Orientation direction)
            {
                X = xPosition;
                Y = yPosition;
                Letters = new char[length];
                for (var i = 0; i < Letters.Length; i++)
                    Letters[i] = ' ';
                Direction = direction;
            }
        }
    }
}