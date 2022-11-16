using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace programTA
{
    class ManipulasiArray
    {
        #region method
        public double[][] TransposeRowsAndColumns(double[][] arr)
        {
            int rowCount = arr.Length;
            int columnCount = arr[0].Length;
            double[][] transposed = new double[columnCount][];
            if (rowCount == columnCount)
            {
                transposed = (double[][])arr.Clone();
                for (int i = 1; i < rowCount; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        double temp = transposed[i][j];
                        transposed[i][j] = transposed[j][i];
                        transposed[j][i] = temp;
                    }
                }
            }
            else
            {
                for (int column = 0; column < columnCount; column++)
                {
                    transposed[column] = new double[rowCount];
                    for (int row = 0; row < rowCount; row++)
                    {
                        transposed[column][row] = arr[row][column];
                    }
                }
            }
            return transposed;
        } // transpose menggunakan jagged array

        public double[,] TransposeRowsAndColumn(double[,] arr)
        {
            int rowCount = arr.GetLength(0);
            int columnCount = arr.GetLength(1);
            double[,] transposed = new double[columnCount, rowCount];
            if (rowCount == columnCount)
            {
                transposed = (double[,])arr.Clone();
                for (int i = 1; i < rowCount; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        double temp = transposed[i, j];
                        transposed[i, j] = transposed[j, i];
                        transposed[j, i] = temp;
                    }
                }
            }
            else
            {
                for (int column = 0; column < columnCount; column++)
                {
                    for (int row = 0; row < rowCount; row++)
                    {
                        transposed[column, row] = arr[row, column];
                    }
                }
            }
            return transposed;
        } // transpose menggunakan array 2d

        public double[] Conv1dArray(double[,] gambarTake)
        {
            List<double> temp1d = new List<double>();
            foreach (int i in gambarTake)
            {
                temp1d.Add(i);
            }
            return temp1d.ToArray();
        } //ubah array 2d ke 1d array

        public double[,] Make2DArray(double[] input, int height, int width)
        {
            double[,] output = new double[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    output[i, j] = input[i * width + j];
                }
            }
            return output;
        } // membuat array 2d dari 1d array

        public double[,] ConvJaggto2D(double[][] source)
        {
            return new[] { new double[source.Length, source[0].Length] }
                .Select(_ => new { x = _, y = source.Select((a, ia) => a.Select((b, ib) => _[ia, ib] = b).Count()).Count() })
                .Select(_ => _.x)
                .First();
        } //konversi array jagged ke array 2d

        public double[][] ToJaggedArray(double[,] twoDimensionalArray)
        {
            int rowsFirstIndex = twoDimensionalArray.GetLowerBound(0);
            int rowsLastIndex = twoDimensionalArray.GetUpperBound(0);
            int numberOfRows = rowsLastIndex - rowsFirstIndex + 1;

            int columnsFirstIndex = twoDimensionalArray.GetLowerBound(1);
            int columnsLastIndex = twoDimensionalArray.GetUpperBound(1);
            int numberOfColumns = columnsLastIndex - columnsFirstIndex + 1;

            double[][] jaggedArray = new double[numberOfRows][];
            for (int i = 0; i < numberOfRows; i++)
            {
                jaggedArray[i] = new double[numberOfColumns];

                for (int j = 0; j < numberOfColumns; j++)
                {
                    jaggedArray[i][j] = twoDimensionalArray[i + rowsFirstIndex, j + columnsFirstIndex];
                }
            }
            return jaggedArray;
        } // rubah matirks ke jagged array

        public double[] JaggedTo1DArray(double[][] gambarTake)
        {
            double[] temp1d = new double[gambarTake.Length * gambarTake[0].Length];
            for (int i = 0; i < gambarTake.Length; i++)
            {
                if (i == 0)
                {
                    gambarTake[i].CopyTo(temp1d, 0);
                }
                else
                {
                    gambarTake[i].CopyTo(temp1d, gambarTake[i].Length * i);
                }
            }
            return temp1d;
        } //ubah array jagged ke 1d array

        public Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            Bitmap destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        } // melakukan resize gambar
        #endregion
    }
}
