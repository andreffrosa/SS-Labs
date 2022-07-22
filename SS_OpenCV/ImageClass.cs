using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;

namespace SS_OpenCV
{
    class ImageClass
    {

        /*/// <summary>
        /// Image Negative using EmguCV library
        /// Slower method
        /// </summary>
        /// <param name="img">Image</param>
        public static void Negative(Image<Bgr, byte> img)
        {
            int x, y;

            Bgr aux;
            for (y = 0; y < img.Height; y++)
            {
                for (x = 0; x < img.Width; x++)
                {
                    // acesso directo : mais lento 
                    aux = img[y, x];
                    img[y, x] = new Bgr(255 - aux.Blue, 255 - aux.Green, 255 - aux.Red);
                }
            }
        }*/

        public static void Negative(Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        dataPtr[0] = (byte)(255 - dataPtr[0]);
                        dataPtr[1] = (byte)(255 - dataPtr[1]);
                        dataPtr[2] = (byte)(255 - dataPtr[2]);

                        dataPtr += nChan;
                    }
                    dataPtr += padding;
                }
            }
        }

        /// <summary>
        /// Convert to gray
        /// Direct access to memory - faster method
        /// </summary>
        /// <param name="img">image</param>
        public static void ConvertToGray(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red, gray;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrive 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            // convert to gray
                            gray = (byte)Math.Round(((int)blue + green + red) / 3.0);

                            // store in the image
                            dataPtr[0] = gray;
                            dataPtr[1] = gray;
                            dataPtr[2] = gray;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }


        // point*contrast + bright e verificar overflow
        public static void BrightContrast(Image<Bgr, byte> img, int bright, double contrast) {
            unsafe {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                int value;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        value = (int) Math.Round(dataPtr[0] * contrast + bright);
                        dataPtr[0] = (byte)( value > 255 ? 255 : value < 0 ? 0 : value);

                        value = (int)Math.Round(dataPtr[1] * contrast + bright);
                        dataPtr[1] = (byte)(value > 255 ? 255 : value < 0 ? 0 : value);

                        value = (int)Math.Round(dataPtr[2] * contrast + bright);
                        dataPtr[2] = (byte)(value > 255 ? 255 : value < 0 ? 0 : value);

                        dataPtr += nChan;
                    }

                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }
            }
        }

        /// <summary>
        /// Convert to gray
        /// Direct access to memory - faster method
        /// </summary>
        /// <param name="img">image</param>
        public static void RedChannel(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red, gray;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrive 3 colour components
                            //blue = dataPtr[0];
                            //green = dataPtr[1];
                            red = dataPtr[2];

                            // convert to gray
                            //gray = (byte)Math.Round(((int)blue + green + red) / 3.0);

                            // store in the image
                            dataPtr[0] = red;
                            dataPtr[1] = red;
                            dataPtr[2] = red;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        public static void Translation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int dx, int dy)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte* dataPtr2 = (byte*)img.MIplImage.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        // Coordinates of the current point in the original image
                        int x_original = (x - dx < width && x - dx > 0) ? x - dx : -1;
                        int y_original = (y - dy < height && y - dy > 0) ? y - dy : -1;

                        // If the current point is outside of the image in the orignal
                        if (x_original < 0 || y_original < 0)
                        {
                            dataPtr2[0] = 0;
                            dataPtr2[1] = 0;
                            dataPtr2[2] = 0;
                        }
                        else
                        {
                            // Endereçamento Absoluto
                            byte* point = dataPtr + y_original * m.widthStep + x_original * nChan;
                            dataPtr2[0] = point[0];
                            dataPtr2[1] = point[1];
                            dataPtr2[2] = point[2];
                        }

                        //
                        dataPtr2 += nChan;
                    }

                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr2 += padding;
                }

            }
        }

        public static void Rotation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right
                MIplImage m = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte* dataPtr2 = (byte*)img.MIplImage.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        // Endereçamento Absoluto: Dataptr = dataptr_i + y*widthstep + x*nChan
                        int x_original = (int)Math.Round((x - (width / 2.0)) * Math.Cos(angle) - ((height / 2.0) - y) * Math.Sin(angle) + (width / 2.0));
                        int y_original = (int)Math.Round((height / 2.0) - (x - (width / 2.0)) * Math.Sin(angle) - ((height / 2.0) - y) * Math.Cos(angle));

                        if (x_original < 0 || x_original >= width || y_original < 0 || y_original >= height)
                        {
                            dataPtr2[0] = 0;
                            dataPtr2[1] = 0;
                            dataPtr2[2] = 0;
                        }
                        else
                        {
                            // Endereçamento Absoluto
                            byte* point = dataPtr + y_original * m.widthStep + x_original * nChan;
                            dataPtr2[0] = point[0];
                            dataPtr2[1] = point[1];
                            dataPtr2[2] = point[2];
                        }

                        //
                        dataPtr2 += nChan;
                    }

                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr2 += padding;
                }
            }
        }

        public static void Scale(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor)
        {
            unsafe
            {
                MIplImage m = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the original image
                byte* dataPtr2 = (byte*)img.MIplImage.imageData.ToPointer(); // Pointer to the destination image

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        int x_original = (int)Math.Round(x / scaleFactor);
                        int y_original = (int)Math.Round(y / scaleFactor);

                        if (x_original < 0 || x_original >= width || y_original < 0 || y_original >= height)
                        {
                            dataPtr2[0] = 0;
                            dataPtr2[1] = 0;
                            dataPtr2[2] = 0;
                        }
                        else
                        {
                            byte* point = dataPtr + x_original * nChan + y_original * m.widthStep;

                            dataPtr2[0] = point[0];
                            dataPtr2[1] = point[1];
                            dataPtr2[2] = point[2];
                        }

                        dataPtr2 += nChan;
                    }
                    dataPtr2 += padding;
                }
            }
        }

        public static void Scale_point_xy(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor, int centerX, int centerY)
        {
            unsafe
            {
                MIplImage m = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the original image
                byte* dataPtr2 = (byte*)img.MIplImage.imageData.ToPointer(); // Pointer to the destination image

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        // Endereço na imagem original
                        //int x_original = (int)Math.Round(((x + scaleFactor*centerX - (width / 2.0)) / scaleFactor));
                        //int y_original = (int)Math.Round(((y + scaleFactor*centerY - (height / 2.0)) / scaleFactor));
                        int x_original = (int)Math.Round((x - (width / 2)) / scaleFactor + centerX);
                        int y_original = (int)Math.Round((y - (height / 2)) / scaleFactor + centerY);

                        if (x_original < 0 || x_original >= width || y_original < 0 || y_original >= height)
                        {
                            dataPtr2[0] = 0;
                            dataPtr2[1] = 0;
                            dataPtr2[2] = 0;
                        }
                        else
                        {
                            byte* point = dataPtr + x_original * nChan + y_original * m.widthStep;
                            dataPtr2[0] = point[0];
                            dataPtr2[1] = point[1];
                            dataPtr2[2] = point[2];
                        }

                        dataPtr2 += nChan;
                    }
                    dataPtr2 += padding;
                }
            }
        }

        public static void Mean(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the original image
                byte* dataPtr2 = (byte*)img.MIplImage.imageData.ToPointer(); // Pointer to the destination image

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                // Core
                dataPtr += nChan + m.widthStep;
                dataPtr2 += nChan + m.widthStep;

                for (y = 1; y < height - 1; y++)
                {
                    for (x = 1; x < width - 1; x++)
                    {
                        dataPtr2[0] = (byte)(Math.Round(((dataPtr - nChan - m.widthStep)[0] + (dataPtr - m.widthStep)[0] + (dataPtr + nChan - m.widthStep)[0]
                                + (dataPtr - nChan)[0] + dataPtr[0] + (dataPtr + nChan)[0]
                                + (dataPtr - nChan + m.widthStep)[0] + (dataPtr + m.widthStep)[0] + (dataPtr + nChan + m.widthStep)[0]) / 9.0));
                        dataPtr2[1] = (byte)(Math.Round(((dataPtr - nChan - m.widthStep)[1] + (dataPtr - m.widthStep)[1] + (dataPtr + nChan - m.widthStep)[1]
                                + (dataPtr - nChan)[1] + dataPtr[1] + (dataPtr + nChan)[1]
                                + (dataPtr - nChan + m.widthStep)[1] + (dataPtr + m.widthStep)[1] + (dataPtr + nChan + m.widthStep)[1]) / 9.0));
                        dataPtr2[2] = (byte)(Math.Round(((dataPtr - nChan - m.widthStep)[2] + (dataPtr - m.widthStep)[2] + (dataPtr + nChan - m.widthStep)[2]
                                + (dataPtr - nChan)[2] + dataPtr[2] + (dataPtr + nChan)[2]
                                + (dataPtr - nChan + m.widthStep)[2] + (dataPtr + m.widthStep)[2] + (dataPtr + nChan + m.widthStep)[2]) / 9.0));

                        dataPtr += nChan;
                        dataPtr2 += nChan;
                    }
                    dataPtr += padding + 2 * nChan;
                    dataPtr2 += padding + 2 * nChan;
                }

                // (0,0)
                dataPtr = (byte*)m.imageData.ToPointer();
                dataPtr2 = (byte*)img.MIplImage.imageData.ToPointer();

                dataPtr2[0] = (byte)(Math.Round(((dataPtr[0] << 2) + ((dataPtr + nChan)[0] << 1)
                             + ((dataPtr + m.widthStep)[0] << 1) + (dataPtr + nChan + m.widthStep)[0]) / 9.0));
                dataPtr2[1] = (byte)(Math.Round(((dataPtr[1] << 2) + ((dataPtr + nChan)[1] << 1)
                             + ((dataPtr + m.widthStep)[1] << 1) + (dataPtr + nChan + m.widthStep)[1]) / 9.0));
                dataPtr2[2] = (byte)(Math.Round(((dataPtr[2] << 2) + ((dataPtr + nChan)[2] << 1)
                             + ((dataPtr + m.widthStep)[2] << 1) + (dataPtr + nChan + m.widthStep)[2]) / 9.0));

                dataPtr += nChan;
                dataPtr2 += nChan;

                // Up line
                for (x = 1; x < width - 1; x++)
                {
                    dataPtr2[0] = (byte)(Math.Round((((dataPtr - nChan)[0] << 1) + (dataPtr[0] << 1) + ((dataPtr + nChan)[0] << 1)
                             + (dataPtr - nChan + m.widthStep)[0] + (dataPtr + m.widthStep)[0] + (dataPtr + nChan + m.widthStep)[0]) / 9.0));
                    dataPtr2[1] = (byte)(Math.Round((((dataPtr - nChan)[1] << 1) + (dataPtr[1] << 1) + ((dataPtr + nChan)[1] << 1)
                             + (dataPtr - nChan + m.widthStep)[1] + (dataPtr + m.widthStep)[1] + (dataPtr + nChan + m.widthStep)[1]) / 9.0));
                    dataPtr2[2] = (byte)(Math.Round((((dataPtr - nChan)[2] << 1) + (dataPtr[2] << 1) + ((dataPtr + nChan)[2] << 1)
                             + (dataPtr - nChan + m.widthStep)[2] + (dataPtr + m.widthStep)[2] + (dataPtr + nChan + m.widthStep)[2]) / 9.0));

                    dataPtr += nChan;
                    dataPtr2 += nChan;
                }

                // (width-1,0)
                dataPtr2[0] = (byte)(Math.Round((((dataPtr - nChan)[0] << 1) + (dataPtr[0] << 2) + ((dataPtr + m.widthStep)[0] << 1) + (dataPtr + m.widthStep - nChan)[0]) / 9.0));
                dataPtr2[1] = (byte)(Math.Round((((dataPtr - nChan)[1] << 1) + (dataPtr[1] << 2) + ((dataPtr + m.widthStep)[1] << 1) + (dataPtr + m.widthStep - nChan)[1]) / 9.0));
                dataPtr2[2] = (byte)(Math.Round((((dataPtr - nChan)[2] << 1) + (dataPtr[2] << 2) + ((dataPtr + m.widthStep)[2] << 1) + (dataPtr + m.widthStep - nChan)[2]) / 9.0));

                dataPtr += nChan + padding;
                dataPtr2 += nChan + padding;

                // Left Column
                for (y = 1; y < height - 1; y++)
                {
                    dataPtr2[0] = (byte)(Math.Round((((dataPtr - m.widthStep)[0] << 1) + (dataPtr + nChan - m.widthStep)[0]
                                      + (dataPtr[0] << 1) + (dataPtr + nChan)[0]
                                      + ((dataPtr + m.widthStep)[0] << 1) + (dataPtr + m.widthStep + nChan)[0]) / 9.0));
                    dataPtr2[1] = (byte)(Math.Round((((dataPtr - m.widthStep)[1] << 1) + (dataPtr + nChan - m.widthStep)[1]
                                      + (dataPtr[1] << 1) + (dataPtr + nChan)[1]
                                      + ((dataPtr + m.widthStep)[1] << 1) + (dataPtr + m.widthStep + nChan)[1]) / 9.0));
                    dataPtr2[2] = (byte)(Math.Round((((dataPtr - m.widthStep)[2] << 1) + (dataPtr + nChan - m.widthStep)[2]
                                      + (dataPtr[2] << 1) + (dataPtr + nChan)[2]
                                      + ((dataPtr + m.widthStep)[2] << 1) + (dataPtr + m.widthStep + nChan)[2]) / 9.0));

                    dataPtr += m.widthStep;
                    dataPtr2 += m.widthStep;
                }

                // (0,height-1)
                dataPtr2[0] = (byte)(Math.Round((((dataPtr - m.widthStep)[0] << 1) + (dataPtr + nChan - m.widthStep)[0]
                                          + (dataPtr[0] << 2) + ((dataPtr + nChan)[0] << 1)) / 9.0));
                dataPtr2[1] = (byte)(Math.Round((((dataPtr - m.widthStep)[1] << 1) + (dataPtr + nChan - m.widthStep)[1]
                                          + (dataPtr[1] << 2) + ((dataPtr + nChan)[1] << 1)) / 9.0));
                dataPtr2[2] = (byte)(Math.Round((((dataPtr - m.widthStep)[2] << 1) + (dataPtr + nChan - m.widthStep)[2]
                                          + (dataPtr[2] << 2) + ((dataPtr + nChan)[2] << 1)) / 9.0));

                dataPtr += nChan;
                dataPtr2 += nChan;

                // Bottom line
                for (x = 1; x < width - 1; x++)
                {
                    dataPtr2[0] = (byte)(Math.Round(((dataPtr - nChan - m.widthStep)[0] + (dataPtr - m.widthStep)[0] + (dataPtr + nChan - m.widthStep)[0]
                                        + ((dataPtr - nChan)[0] << 1) + (dataPtr[0] << 1) + ((dataPtr + nChan)[0] << 1)) / 9.0));
                    dataPtr2[1] = (byte)(Math.Round(((dataPtr - nChan - m.widthStep)[1] + (dataPtr - m.widthStep)[1] + (dataPtr + nChan - m.widthStep)[1]
                                        + ((dataPtr - nChan)[1] << 1) + (dataPtr[1] << 1) + ((dataPtr + nChan)[1] << 1)) / 9.0));
                    dataPtr2[2] = (byte)(Math.Round(((dataPtr - nChan - m.widthStep)[2] + (dataPtr - m.widthStep)[2] + (dataPtr + nChan - m.widthStep)[2]
                                        + ((dataPtr - nChan)[2] << 1) + (dataPtr[2] << 1) + ((dataPtr + nChan)[2] << 1)) / 9.0));

                    dataPtr += nChan;
                    dataPtr2 += nChan;
                }

                // (width-1,heigh-1)
                dataPtr2[0] = (byte)(Math.Round((((dataPtr - nChan)[0] << 1) + (dataPtr[0] << 2) + ((dataPtr - m.widthStep)[0] << 1) + (dataPtr - m.widthStep - nChan)[0]) / 9.0));
                dataPtr2[1] = (byte)(Math.Round((((dataPtr - nChan)[1] << 1) + (dataPtr[1] << 2) + ((dataPtr - m.widthStep)[1] << 1) + (dataPtr - m.widthStep - nChan)[1]) / 9.0));
                dataPtr2[2] = (byte)(Math.Round((((dataPtr - nChan)[2] << 1) + (dataPtr[2] << 2) + ((dataPtr - m.widthStep)[2] << 1) + (dataPtr - m.widthStep - nChan)[2]) / 9.0));

                dataPtr -= m.widthStep;
                dataPtr2 -= m.widthStep;

                // Right Column
                for (y = height - 2; y > 0; y--)
                {
                    dataPtr2[0] = (byte)(Math.Round(((dataPtr - m.widthStep - nChan)[0] + ((dataPtr - m.widthStep)[0] << 1)
                                          + (dataPtr - nChan)[0] + (dataPtr[0] << 1)
                                          + (dataPtr - nChan + m.widthStep)[0] + ((dataPtr + m.widthStep)[0] << 1)) / 9.0));

                    dataPtr2[1] = (byte)(Math.Round(((dataPtr - m.widthStep - nChan)[1] + ((dataPtr - m.widthStep)[1] << 1)
                                          + (dataPtr - nChan)[1] + (dataPtr[1] << 1)
                                          + (dataPtr - nChan + m.widthStep)[1] + ((dataPtr + m.widthStep)[1] << 1)) / 9.0));

                    dataPtr2[2] = (byte)(Math.Round(((dataPtr - m.widthStep - nChan)[2] + ((dataPtr - m.widthStep)[2] << 1)
                                          + (dataPtr - nChan)[2] + (dataPtr[2] << 1)
                                          + (dataPtr - nChan + m.widthStep)[2] + ((dataPtr + m.widthStep)[2] << 1)) / 9.0));

                    dataPtr -= m.widthStep;
                    dataPtr2 -= m.widthStep;
                }

            }
        }

        public static void NonUniform(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float[,] matrix, float matrixWeight)
        {

            unsafe
            {
                MIplImage m = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the original image
                byte* dataPtr2 = (byte*)img.MIplImage.imageData.ToPointer(); // Pointer to the destination image

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                int avg;

                // Process the center
                dataPtr += nChan + m.widthStep;
                dataPtr2 += nChan + m.widthStep;

                for (y = 1; y < height - 1; y++)
                {
                    for (x = 1; x < width - 1; x++)
                    {
                        avg = (int)Math.Round((
                            (dataPtr - nChan - m.widthStep)[0] * matrix[0, 0] +
                            (dataPtr - m.widthStep)[0] * matrix[0, 1] +
                            (dataPtr + nChan - m.widthStep)[0] * matrix[0, 2] +
                            (dataPtr - nChan)[0] * matrix[1, 0] +
                            dataPtr[0] * matrix[1, 1] +
                            (dataPtr + nChan)[0] * matrix[1, 2] +
                            (dataPtr - nChan + m.widthStep)[0] * matrix[2, 0] +
                            (dataPtr + m.widthStep)[0] * matrix[2, 1] +
                            (dataPtr + nChan + m.widthStep)[0] * matrix[2, 2]
                            ) / matrixWeight);
                        dataPtr2[0] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                        avg = (int)Math.Round((
                            (dataPtr - nChan - m.widthStep)[1] * matrix[0, 0] +
                            (dataPtr - m.widthStep)[1] * matrix[0, 1] +
                            (dataPtr + nChan - m.widthStep)[1] * matrix[0, 2] +
                            (dataPtr - nChan)[1] * matrix[1, 0] +
                            dataPtr[1] * matrix[1, 1] +
                            (dataPtr + nChan)[1] * matrix[1, 2] +
                            (dataPtr - nChan + m.widthStep)[1] * matrix[2, 0] +
                            (dataPtr + m.widthStep)[1] * matrix[2, 1] +
                            (dataPtr + nChan + m.widthStep)[1] * matrix[2, 2]
                            ) / matrixWeight);
                        dataPtr2[1] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                        avg = (int)Math.Round((
                            (dataPtr - nChan - m.widthStep)[2] * matrix[0, 0] +
                            (dataPtr - m.widthStep)[2] * matrix[0, 1] +
                            (dataPtr + nChan - m.widthStep)[2] * matrix[0, 2] +
                            (dataPtr - nChan)[2] * matrix[1, 0] +
                            dataPtr[2] * matrix[1, 1] +
                            (dataPtr + nChan)[2] * matrix[1, 2] +
                            (dataPtr - nChan + m.widthStep)[2] * matrix[2, 0] +
                            (dataPtr + m.widthStep)[2] * matrix[2, 1] +
                            (dataPtr + nChan + m.widthStep)[2] * matrix[2, 2]
                            ) / matrixWeight);
                        dataPtr2[2] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                        dataPtr += nChan;
                        dataPtr2 += nChan;
                    }
                    dataPtr += padding + 2 * nChan;
                    dataPtr2 += padding + 2 * nChan;
                }

                // (0,0)
                dataPtr = (byte*)m.imageData.ToPointer();
                dataPtr2 = (byte*)img.MIplImage.imageData.ToPointer();

                avg = (int)(Math.Round((dataPtr[0] * (matrix[0, 0] + matrix[0, 1] + matrix[1, 0] + matrix[1, 1])
                                     + (dataPtr + nChan)[0] * (matrix[0, 2] + matrix[1, 2])
                                     + (dataPtr + m.widthStep)[0] * (matrix[2, 0] + matrix[2, 1])
                                     + (dataPtr + nChan + m.widthStep)[0] * matrix[2, 2]) / matrixWeight));
                dataPtr2[0] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                avg = (int)(Math.Round((dataPtr[1] * (matrix[0, 0] + matrix[0, 1] + matrix[1, 0] + matrix[1, 1])
                                     + (dataPtr + nChan)[1] * (matrix[0, 2] + matrix[1, 2])
                                     + (dataPtr + m.widthStep)[1] * (matrix[2, 0] + matrix[2, 1])
                                     + (dataPtr + nChan + m.widthStep)[1] * matrix[2, 2]) / matrixWeight));
                dataPtr2[1] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                avg = (int)(Math.Round((dataPtr[2] * (matrix[0, 0] + matrix[0, 1] + matrix[1, 0] + matrix[1, 1])
                                     + (dataPtr + nChan)[2] * (matrix[0, 2] + matrix[1, 2])
                                     + (dataPtr + m.widthStep)[2] * (matrix[2, 0] + matrix[2, 1])
                                     + (dataPtr + nChan + m.widthStep)[2] * matrix[2, 2]) / matrixWeight));
                dataPtr2[2] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                dataPtr += nChan;
                dataPtr2 += nChan;

                // Up line
                for (x = 1; x < width - 1; x++)
                {
                    avg = (int)(Math.Round(((dataPtr - nChan)[0] * (matrix[0, 0] + matrix[1, 0])
                                           + dataPtr[0] * (matrix[0, 1] + matrix[1, 1])
                                           + (dataPtr + nChan)[0] * (matrix[0, 2] + matrix[1, 2])
                                           + (dataPtr - nChan + m.widthStep)[0] * matrix[2, 0]
                                           + (dataPtr + m.widthStep)[0] * matrix[2, 1]
                                           + (dataPtr + nChan + m.widthStep)[0] * matrix[2, 2]) / matrixWeight));
                    dataPtr2[0] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    avg = (int)(Math.Round(((dataPtr - nChan)[1] * (matrix[0, 0] + matrix[1, 0])
                                           + dataPtr[1] * (matrix[0, 1] + matrix[1, 1])
                                           + (dataPtr + nChan)[1] * (matrix[0, 2] + matrix[1, 2])
                                           + (dataPtr - nChan + m.widthStep)[1] * matrix[2, 0]
                                           + (dataPtr + m.widthStep)[1] * matrix[2, 1]
                                           + (dataPtr + nChan + m.widthStep)[1] * matrix[2, 2]) / matrixWeight));
                    dataPtr2[1] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    avg = (int)(Math.Round(((dataPtr - nChan)[2] * (matrix[0, 0] + matrix[1, 0])
                                           + dataPtr[2] * (matrix[0, 1] + matrix[1, 1])
                                           + (dataPtr + nChan)[2] * (matrix[0, 2] + matrix[1, 2])
                                           + (dataPtr - nChan + m.widthStep)[2] * matrix[2, 0]
                                           + (dataPtr + m.widthStep)[2] * matrix[2, 1]
                                           + (dataPtr + nChan + m.widthStep)[2] * matrix[2, 2]) / matrixWeight));
                    dataPtr2[2] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    dataPtr += nChan;
                    dataPtr2 += nChan;
                }

                // (width-1,0)
                avg = (int)(Math.Round(((dataPtr - nChan)[0] * (matrix[0, 0] + matrix[1, 0])
                                      + dataPtr[0] * (matrix[0, 1] + matrix[0, 2] + matrix[1, 1] + matrix[1, 2])
                                      + (dataPtr + m.widthStep)[0] * (matrix[2, 1] + matrix[2, 2])
                                      + (dataPtr + m.widthStep - nChan)[0] * matrix[2, 0]) / matrixWeight));
                dataPtr2[0] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                avg = (int)(Math.Round(((dataPtr - nChan)[1] * (matrix[0, 0] + matrix[1, 0])
                                      + dataPtr[1] * (matrix[0, 1] + matrix[0, 2] + matrix[1, 1] + matrix[1, 2])
                                      + (dataPtr + m.widthStep)[1] * (matrix[2, 1] + matrix[2, 2])
                                      + (dataPtr + m.widthStep - nChan)[1] * matrix[2, 0]) / matrixWeight));
                dataPtr2[1] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                avg = (int)(Math.Round(((dataPtr - nChan)[2] * (matrix[0, 0] + matrix[1, 0])
                                      + dataPtr[2] * (matrix[0, 1] + matrix[0, 2] + matrix[1, 1] + matrix[1, 2])
                                      + (dataPtr + m.widthStep)[2] * (matrix[2, 1] + matrix[2, 2])
                                      + (dataPtr + m.widthStep - nChan)[2] * matrix[2, 0]) / matrixWeight));
                dataPtr2[2] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                dataPtr += nChan + padding;
                dataPtr2 += nChan + padding;

                // Left Column
                for (y = 1; y < height - 1; y++)
                {
                    avg = (int)(Math.Round(((dataPtr - m.widthStep)[0] * (matrix[0, 0] + matrix[0, 1])
                                          + (dataPtr + nChan - m.widthStep)[0] * matrix[0, 2]
                                          + dataPtr[0] * (matrix[1, 0] + matrix[1, 1])
                                          + (dataPtr + nChan)[0] * matrix[1, 2]
                                          + (dataPtr + m.widthStep)[0] * (matrix[2, 0] + matrix[2, 1])
                                          + (dataPtr + m.widthStep + nChan)[0] * matrix[2, 2]) / matrixWeight));
                    dataPtr2[0] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    avg = (int)(Math.Round(((dataPtr - m.widthStep)[1] * (matrix[0, 0] + matrix[0, 1])
                                          + (dataPtr + nChan - m.widthStep)[1] * matrix[0, 2]
                                          + dataPtr[1] * (matrix[1, 0] + matrix[1, 1])
                                          + (dataPtr + nChan)[1] * matrix[1, 2]
                                          + (dataPtr + m.widthStep)[1] * (matrix[2, 0] + matrix[2, 1])
                                          + (dataPtr + m.widthStep + nChan)[1] * matrix[2, 2]) / matrixWeight));
                    dataPtr2[1] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    avg = (int)(Math.Round(((dataPtr - m.widthStep)[2] * (matrix[0, 0] + matrix[0, 1])
                                          + (dataPtr + nChan - m.widthStep)[2] * matrix[0, 2]
                                          + dataPtr[2] * (matrix[1, 0] + matrix[1, 1])
                                          + (dataPtr + nChan)[2] * matrix[1, 2]
                                          + (dataPtr + m.widthStep)[2] * (matrix[2, 0] + matrix[2, 1])
                                          + (dataPtr + m.widthStep + nChan)[2] * matrix[2, 2]) / matrixWeight));
                    dataPtr2[2] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    dataPtr += m.widthStep;
                    dataPtr2 += m.widthStep;
                }

                // (0,height-1)
                avg = (int)(Math.Round(((dataPtr - m.widthStep)[0] * (matrix[0, 0] + matrix[0, 1])
                                      + (dataPtr + nChan - m.widthStep)[0] * matrix[0, 2]
                                      + dataPtr[0] * (matrix[1, 0] + matrix[1, 1] + matrix[2, 0] + matrix[2, 1])
                                      + (dataPtr + nChan)[0] * (matrix[1, 2] + matrix[2, 2])) / matrixWeight));
                dataPtr2[0] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                avg = (int)(Math.Round(((dataPtr - m.widthStep)[1] * (matrix[0, 0] + matrix[0, 1])
                                      + (dataPtr + nChan - m.widthStep)[1] * matrix[0, 2]
                                      + dataPtr[1] * (matrix[1, 0] + matrix[1, 1] + matrix[2, 0] + matrix[2, 1])
                                      + (dataPtr + nChan)[1] * (matrix[1, 2] + matrix[2, 2])) / matrixWeight));
                dataPtr2[1] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                avg = (int)(Math.Round(((dataPtr - m.widthStep)[2] * (matrix[0, 0] + matrix[0, 1])
                                      + (dataPtr + nChan - m.widthStep)[2] * matrix[0, 2]
                                      + dataPtr[2] * (matrix[1, 0] + matrix[1, 1] + matrix[2, 0] + matrix[2, 1])
                                      + (dataPtr + nChan)[2] * (matrix[1, 2] + matrix[2, 2])) / matrixWeight));
                dataPtr2[2] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                dataPtr += nChan;
                dataPtr2 += nChan;

                // Bottom line
                for (x = 1; x < width - 1; x++)
                {
                    avg = (int)(Math.Round(((dataPtr - nChan - m.widthStep)[0] * matrix[0, 0]
                                          + (dataPtr - m.widthStep)[0] * matrix[0, 1]
                                          + (dataPtr + nChan - m.widthStep)[0] * matrix[0, 2]
                                          + (dataPtr - nChan)[0] * (matrix[1, 0] + matrix[2, 0])
                                          + dataPtr[0] * (matrix[1, 1] + matrix[2, 1])
                                          + (dataPtr + nChan)[0] * (matrix[1, 2] + matrix[2, 2])) / matrixWeight));
                    dataPtr2[0] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    avg = (int)(Math.Round(((dataPtr - nChan - m.widthStep)[1] * matrix[0, 0]
                                          + (dataPtr - m.widthStep)[1] * matrix[0, 1]
                                          + (dataPtr + nChan - m.widthStep)[1] * matrix[0, 2]
                                          + (dataPtr - nChan)[1] * (matrix[1, 0] + matrix[2, 0])
                                          + dataPtr[1] * (matrix[1, 1] + matrix[2, 1])
                                          + (dataPtr + nChan)[1] * (matrix[1, 2] + matrix[2, 2])) / matrixWeight));
                    dataPtr2[1] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    avg = (int)(Math.Round(((dataPtr - nChan - m.widthStep)[2] * matrix[0, 0]
                                          + (dataPtr - m.widthStep)[2] * matrix[0, 1]
                                          + (dataPtr + nChan - m.widthStep)[2] * matrix[0, 2]
                                          + (dataPtr - nChan)[2] * (matrix[1, 0] + matrix[2, 0])
                                          + dataPtr[2] * (matrix[1, 1] + matrix[2, 1])
                                          + (dataPtr + nChan)[2] * (matrix[1, 2] + matrix[2, 2])) / matrixWeight));
                    dataPtr2[2] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    dataPtr += nChan;
                    dataPtr2 += nChan;
                }

                // (width-1,heigh-1)
                avg = (int)(Math.Round(((dataPtr - nChan)[0] * (matrix[1, 0] + matrix[2, 0])
                                      + dataPtr[0] * (matrix[1, 1] + matrix[1, 2] + matrix[2, 1] + matrix[2, 2])
                                      + (dataPtr - m.widthStep)[0] * (matrix[0, 1] + matrix[0, 2])
                                      + (dataPtr - m.widthStep - nChan)[0] * matrix[0, 0]) / matrixWeight));
                dataPtr2[0] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                avg = (int)(Math.Round(((dataPtr - nChan)[1] * (matrix[1, 0] + matrix[2, 0])
                                      + dataPtr[1] * (matrix[1, 1] + matrix[1, 2] + matrix[2, 1] + matrix[2, 2])
                                      + (dataPtr - m.widthStep)[1] * (matrix[0, 1] + matrix[0, 2])
                                      + (dataPtr - m.widthStep - nChan)[1] * matrix[0, 0]) / matrixWeight));
                dataPtr2[1] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                avg = (int)(Math.Round(((dataPtr - nChan)[2] * (matrix[1, 0] + matrix[2, 0])
                                      + dataPtr[2] * (matrix[1, 1] + matrix[1, 2] + matrix[2, 1] + matrix[2, 2])
                                      + (dataPtr - m.widthStep)[2] * (matrix[0, 1] + matrix[0, 2])
                                      + (dataPtr - m.widthStep - nChan)[2] * matrix[0, 0]) / matrixWeight));
                dataPtr2[2] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                dataPtr -= m.widthStep;
                dataPtr2 -= m.widthStep;

                // Right Column
                for (y = height - 2; y > 0; y--)
                {
                    avg = (int)(Math.Round(((dataPtr - m.widthStep - nChan)[0] * matrix[0, 0]
                                         + (dataPtr - m.widthStep)[0] * (matrix[0, 1] + matrix[0, 2])
                                         + (dataPtr - nChan)[0] * matrix[1, 0]
                                         + dataPtr[0] * (matrix[1, 1] + matrix[1, 2])
                                         + (dataPtr - nChan + m.widthStep)[0] * matrix[2, 0]
                                         + (dataPtr + m.widthStep)[0] * (matrix[2, 1] + matrix[2, 2])) / matrixWeight));
                    dataPtr2[0] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    avg = (int)(Math.Round(((dataPtr - m.widthStep - nChan)[1] * matrix[0, 0]
                                         + (dataPtr - m.widthStep)[1] * (matrix[0, 1] + matrix[0, 2])
                                         + (dataPtr - nChan)[1] * matrix[1, 0]
                                         + dataPtr[1] * (matrix[1, 1] + matrix[1, 2])
                                         + (dataPtr - nChan + m.widthStep)[1] * matrix[2, 0]
                                         + (dataPtr + m.widthStep)[1] * (matrix[2, 1] + matrix[2, 2])) / matrixWeight));
                    dataPtr2[1] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    avg = (int)(Math.Round(((dataPtr - m.widthStep - nChan)[2] * matrix[0, 0]
                                         + (dataPtr - m.widthStep)[2] * (matrix[0, 1] + matrix[0, 2])
                                         + (dataPtr - nChan)[2] * matrix[1, 0]
                                         + dataPtr[2] * (matrix[1, 1] + matrix[1, 2])
                                         + (dataPtr - nChan + m.widthStep)[2] * matrix[2, 0]
                                         + (dataPtr + m.widthStep)[2] * (matrix[2, 1] + matrix[2, 2])) / matrixWeight));
                    dataPtr2[2] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    dataPtr -= m.widthStep;
                    dataPtr2 -= m.widthStep;
                }
            }
        }

        public static void Sobel(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the original image
                byte* dataPtr2 = (byte*)img.MIplImage.imageData.ToPointer(); // Pointer to the destination image

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                // Core
                dataPtr += nChan + m.widthStep;
                dataPtr2 += nChan + m.widthStep;

                int sum; // verificar Overflow sempre que há contas de menos

                for (y = 1; y < height - 1; y++)
                {
                    for (x = 1; x < width - 1; x++)
                    {
                        sum = (Math.Abs(((dataPtr - nChan - m.widthStep)[0] + ((dataPtr - nChan)[0] << 1) + (dataPtr - nChan + m.widthStep)[0])
                                    - ((dataPtr + nChan - m.widthStep)[0] + ((dataPtr + nChan)[0] << 1) + (dataPtr + nChan + m.widthStep)[0]))
                                    +
                                    Math.Abs(((dataPtr + m.widthStep - nChan)[0] + ((dataPtr + m.widthStep)[0] << 1) + (dataPtr + nChan + m.widthStep)[0])
                                    - ((dataPtr - nChan - m.widthStep)[0] + ((dataPtr - m.widthStep)[0] << 1) + (dataPtr + nChan - m.widthStep)[0])));
                        dataPtr2[0] = (byte)(sum > 255 ? 255 : sum);

                        sum = (Math.Abs(((dataPtr - nChan - m.widthStep)[1] + ((dataPtr - nChan)[1] << 1) + (dataPtr - nChan + m.widthStep)[1])
                                   - ((dataPtr + nChan - m.widthStep)[1] + ((dataPtr + nChan)[1] << 1) + (dataPtr + nChan + m.widthStep)[1]))
                                   +
                                   Math.Abs(((dataPtr + m.widthStep - nChan)[1] + ((dataPtr + m.widthStep)[1] << 1) + (dataPtr + nChan + m.widthStep)[1])
                                   - ((dataPtr - nChan - m.widthStep)[1] + ((dataPtr - m.widthStep)[1] << 1) + (dataPtr + nChan - m.widthStep)[1])));
                        dataPtr2[1] = (byte)(sum > 255 ? 255 : sum);

                        sum = (Math.Abs(((dataPtr - nChan - m.widthStep)[2] + ((dataPtr - nChan)[2] << 1) + (dataPtr - nChan + m.widthStep)[2])
                                    - ((dataPtr + nChan - m.widthStep)[2] + ((dataPtr + nChan)[2] << 1) + (dataPtr + nChan + m.widthStep)[2]))
                                    +
                                    Math.Abs(((dataPtr + m.widthStep - nChan)[2] + ((dataPtr + m.widthStep)[2] << 1) + (dataPtr + nChan + m.widthStep)[2])
                                    - ((dataPtr - nChan - m.widthStep)[2] + ((dataPtr - m.widthStep)[2] << 1) + (dataPtr + nChan - m.widthStep)[2])));
                        dataPtr2[2] = (byte)(sum > 255 ? 255 : sum);

                        dataPtr += nChan;
                        dataPtr2 += nChan;
                    }
                    dataPtr += padding + 2 * nChan;
                    dataPtr2 += padding + 2 * nChan;
                }

                // (0,0)
                dataPtr = (byte*)m.imageData.ToPointer();
                dataPtr2 = (byte*)img.MIplImage.imageData.ToPointer();

                sum = (Math.Abs(((dataPtr)[0] * 3 + (dataPtr + m.widthStep)[0])
                                - ((dataPtr + nChan)[0] * 3 + (dataPtr + nChan + m.widthStep)[0]))
                                +
                                Math.Abs(((dataPtr + m.widthStep)[0] * 3 + (dataPtr + m.widthStep + nChan)[0])
                                - (dataPtr[0] * 3 + ((dataPtr - m.widthStep)[2] << 1) + (dataPtr + nChan)[0])));
                dataPtr2[0] = (byte)(sum > 255 ? 255 : sum);

                sum = (Math.Abs(((dataPtr)[1] * 3 + (dataPtr + m.widthStep)[1])
                                - ((dataPtr + nChan)[1] * 3 + (dataPtr + nChan + m.widthStep)[1]))
                                +
                                Math.Abs(((dataPtr + m.widthStep)[1] * 3 + (dataPtr + m.widthStep + nChan)[1])
                                - (dataPtr[1] * 3 + ((dataPtr - m.widthStep)[2] << 1) + (dataPtr + nChan)[1])));
                dataPtr2[1] = (byte)(sum > 255 ? 255 : sum);

                sum = (Math.Abs(((dataPtr)[2] * 3 + (dataPtr + m.widthStep)[2])
                                                    - ((dataPtr + nChan)[2] * 3 + (dataPtr + nChan + m.widthStep)[2]))
                                                    +
                                                    Math.Abs(((dataPtr + m.widthStep)[2] * 3 + (dataPtr + m.widthStep + nChan)[2])
                                                    - (dataPtr[2] * 3 + ((dataPtr - m.widthStep)[2] << 1) + (dataPtr + nChan)[2])));
                dataPtr2[2] = (byte)(sum > 255 ? 255 : sum);

                dataPtr += nChan;
                dataPtr2 += nChan;

                // Up line
                for (x = 1; x < width - 1; x++)
                {
                    // Sx = (3d+g)-(3f+i);
                    // Sy = (g + 2h + i)-(d + 2e+f);
                    sum = (Math.Abs(((dataPtr - nChan)[0] * 3 + (dataPtr + m.widthStep - nChan)[0])
                        - ((dataPtr + nChan)[0] * 3 + (dataPtr + nChan + m.widthStep)[0]))
                        +
                        Math.Abs(((dataPtr + m.widthStep - nChan)[0] + ((dataPtr + m.widthStep)[0]) << 1) + (dataPtr + nChan + m.widthStep)[0]
                        - ((dataPtr - nChan)[0] + (dataPtr[0] << 1) + (dataPtr + nChan)[0])));
                    dataPtr2[0] = (byte)(sum > 255 ? 255 : sum);

                    sum = (Math.Abs(((dataPtr - nChan)[1] * 3 + (dataPtr + m.widthStep - nChan)[1])
                        - ((dataPtr + nChan)[1] * 3 + (dataPtr + nChan + m.widthStep)[1]))
                        +
                        Math.Abs(((dataPtr + m.widthStep - nChan)[1] + ((dataPtr + m.widthStep)[1]) << 1) + (dataPtr + nChan + m.widthStep)[1]
                        - ((dataPtr - nChan)[1] + (dataPtr[1] << 1) + (dataPtr + nChan)[1])));
                    dataPtr2[1] = (byte)(sum > 255 ? 255 : sum);

                    sum = (Math.Abs(((dataPtr - nChan)[2] * 3 + (dataPtr + m.widthStep - nChan)[2])
                                            - ((dataPtr + nChan)[2] * 3 + (dataPtr + nChan + m.widthStep)[2]))
                                            +
                                            Math.Abs(((dataPtr + m.widthStep - nChan)[2] + ((dataPtr + m.widthStep)[2]) << 1) + (dataPtr + nChan + m.widthStep)[2]
                                            - ((dataPtr - nChan)[2] + (dataPtr[2] << 1) + (dataPtr + nChan)[2])));
                    dataPtr2[2] = (byte)(sum > 255 ? 255 : sum);

                    dataPtr += nChan;
                    dataPtr2 += nChan;
                }

                /*// (width-1,0)
                avg = (int)(Math.Round(((dataPtr - nChan)[0] * (matrix[0, 0] + matrix[1, 0])
                                      + dataPtr[0] * (matrix[0, 1] + matrix[0, 2] + matrix[1, 1] + matrix[1, 2])
                                      + (dataPtr + m.widthStep)[0] * (matrix[2, 1] + matrix[2, 2])
                                      + (dataPtr + m.widthStep - nChan)[0] * matrix[2, 0]) / matrixWeight));
                dataPtr2[0] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                avg = (int)(Math.Round(((dataPtr - nChan)[1] * (matrix[0, 0] + matrix[1, 0])
                                      + dataPtr[1] * (matrix[0, 1] + matrix[0, 2] + matrix[1, 1] + matrix[1, 2])
                                      + (dataPtr + m.widthStep)[1] * (matrix[2, 1] + matrix[2, 2])
                                      + (dataPtr + m.widthStep - nChan)[1] * matrix[2, 0]) / matrixWeight));
                dataPtr2[1] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                avg = (int)(Math.Round(((dataPtr - nChan)[2] * (matrix[0, 0] + matrix[1, 0])
                                      + dataPtr[2] * (matrix[0, 1] + matrix[0, 2] + matrix[1, 1] + matrix[1, 2])
                                      + (dataPtr + m.widthStep)[2] * (matrix[2, 1] + matrix[2, 2])
                                      + (dataPtr + m.widthStep - nChan)[2] * matrix[2, 0]) / matrixWeight));
                dataPtr2[2] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                dataPtr += nChan + padding;
                dataPtr2 += nChan + padding;

                // Left Column
                for (y = 1; y < height - 1; y++)
                {
                    avg = (int)(Math.Round(((dataPtr - m.widthStep)[0] * (matrix[0, 0] + matrix[0, 1])
                                          + (dataPtr + nChan - m.widthStep)[0] * matrix[0, 2]
                                          + dataPtr[0] * (matrix[1, 0] + matrix[1, 1])
                                          + (dataPtr + nChan)[0] * matrix[1, 2]
                                          + (dataPtr + m.widthStep)[0] * (matrix[2, 0] + matrix[2, 1])
                                          + (dataPtr + m.widthStep + nChan)[0] * matrix[2, 2]) / matrixWeight));
                    dataPtr2[0] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    avg = (int)(Math.Round(((dataPtr - m.widthStep)[1] * (matrix[0, 0] + matrix[0, 1])
                                          + (dataPtr + nChan - m.widthStep)[1] * matrix[0, 2]
                                          + dataPtr[1] * (matrix[1, 0] + matrix[1, 1])
                                          + (dataPtr + nChan)[1] * matrix[1, 2]
                                          + (dataPtr + m.widthStep)[1] * (matrix[2, 0] + matrix[2, 1])
                                          + (dataPtr + m.widthStep + nChan)[1] * matrix[2, 2]) / matrixWeight));
                    dataPtr2[1] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    avg = (int)(Math.Round(((dataPtr - m.widthStep)[2] * (matrix[0, 0] + matrix[0, 1])
                                          + (dataPtr + nChan - m.widthStep)[2] * matrix[0, 2]
                                          + dataPtr[2] * (matrix[1, 0] + matrix[1, 1])
                                          + (dataPtr + nChan)[2] * matrix[1, 2]
                                          + (dataPtr + m.widthStep)[2] * (matrix[2, 0] + matrix[2, 1])
                                          + (dataPtr + m.widthStep + nChan)[2] * matrix[2, 2]) / matrixWeight));
                    dataPtr2[2] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    dataPtr += m.widthStep;
                    dataPtr2 += m.widthStep;
                }

                // (0,height-1)
                avg = (int)(Math.Round(((dataPtr - m.widthStep)[0] * (matrix[0, 0] + matrix[0, 1])
                                      + (dataPtr + nChan - m.widthStep)[0] * matrix[0, 2]
                                      + dataPtr[0] * (matrix[1, 0] + matrix[1, 1] + matrix[2, 0] + matrix[2, 1])
                                      + (dataPtr + nChan)[0] * (matrix[1, 2] + matrix[2, 2])) / matrixWeight));
                dataPtr2[0] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                avg = (int)(Math.Round(((dataPtr - m.widthStep)[1] * (matrix[0, 0] + matrix[0, 1])
                                      + (dataPtr + nChan - m.widthStep)[1] * matrix[0, 2]
                                      + dataPtr[1] * (matrix[1, 0] + matrix[1, 1] + matrix[2, 0] + matrix[2, 1])
                                      + (dataPtr + nChan)[1] * (matrix[1, 2] + matrix[2, 2])) / matrixWeight));
                dataPtr2[1] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                avg = (int)(Math.Round(((dataPtr - m.widthStep)[2] * (matrix[0, 0] + matrix[0, 1])
                                      + (dataPtr + nChan - m.widthStep)[2] * matrix[0, 2]
                                      + dataPtr[2] * (matrix[1, 0] + matrix[1, 1] + matrix[2, 0] + matrix[2, 1])
                                      + (dataPtr + nChan)[2] * (matrix[1, 2] + matrix[2, 2])) / matrixWeight));
                dataPtr2[2] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                dataPtr += nChan;
                dataPtr2 += nChan;

                // Bottom line
                for (x = 1; x < width - 1; x++)
                {
                    avg = (int)(Math.Round(((dataPtr - nChan - m.widthStep)[0] * matrix[0, 0]
                                          + (dataPtr - m.widthStep)[0] * matrix[0, 1]
                                          + (dataPtr + nChan - m.widthStep)[0] * matrix[0, 2]
                                          + (dataPtr - nChan)[0] * (matrix[1, 0] + matrix[2, 0])
                                          + dataPtr[0] * (matrix[1, 1] + matrix[2, 1])
                                          + (dataPtr + nChan)[0] * (matrix[1, 2] + matrix[2, 2])) / matrixWeight));
                    dataPtr2[0] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    avg = (int)(Math.Round(((dataPtr - nChan - m.widthStep)[1] * matrix[0, 0]
                                          + (dataPtr - m.widthStep)[1] * matrix[0, 1]
                                          + (dataPtr + nChan - m.widthStep)[1] * matrix[0, 2]
                                          + (dataPtr - nChan)[1] * (matrix[1, 0] + matrix[2, 0])
                                          + dataPtr[1] * (matrix[1, 1] + matrix[2, 1])
                                          + (dataPtr + nChan)[1] * (matrix[1, 2] + matrix[2, 2])) / matrixWeight));
                    dataPtr2[1] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    avg = (int)(Math.Round(((dataPtr - nChan - m.widthStep)[2] * matrix[0, 0]
                                          + (dataPtr - m.widthStep)[2] * matrix[0, 1]
                                          + (dataPtr + nChan - m.widthStep)[2] * matrix[0, 2]
                                          + (dataPtr - nChan)[2] * (matrix[1, 0] + matrix[2, 0])
                                          + dataPtr[2] * (matrix[1, 1] + matrix[2, 1])
                                          + (dataPtr + nChan)[2] * (matrix[1, 2] + matrix[2, 2])) / matrixWeight));
                    dataPtr2[2] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    dataPtr += nChan;
                    dataPtr2 += nChan;
                }

                // (width-1,heigh-1)
                avg = (int)(Math.Round(((dataPtr - nChan)[0] * (matrix[1, 0] + matrix[2, 0])
                                      + dataPtr[0] * (matrix[1, 1] + matrix[1, 2] + matrix[2, 1] + matrix[2, 2])
                                      + (dataPtr - m.widthStep)[0] * (matrix[0, 1] + matrix[0, 2])
                                      + (dataPtr - m.widthStep - nChan)[0] * matrix[0, 0]) / matrixWeight));
                dataPtr2[0] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                avg = (int)(Math.Round(((dataPtr - nChan)[1] * (matrix[1, 0] + matrix[2, 0])
                                      + dataPtr[1] * (matrix[1, 1] + matrix[1, 2] + matrix[2, 1] + matrix[2, 2])
                                      + (dataPtr - m.widthStep)[1] * (matrix[0, 1] + matrix[0, 2])
                                      + (dataPtr - m.widthStep - nChan)[1] * matrix[0, 0]) / matrixWeight));
                dataPtr2[1] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                avg = (int)(Math.Round(((dataPtr - nChan)[2] * (matrix[1, 0] + matrix[2, 0])
                                      + dataPtr[2] * (matrix[1, 1] + matrix[1, 2] + matrix[2, 1] + matrix[2, 2])
                                      + (dataPtr - m.widthStep)[2] * (matrix[0, 1] + matrix[0, 2])
                                      + (dataPtr - m.widthStep - nChan)[2] * matrix[0, 0]) / matrixWeight));
                dataPtr2[2] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                dataPtr -= m.widthStep;
                dataPtr2 -= m.widthStep;

                // Right Column
                for (y = height - 2; y > 0; y--)
                {
                    avg = (int)(Math.Round(((dataPtr - m.widthStep - nChan)[0] * matrix[0, 0]
                                         + (dataPtr - m.widthStep)[0] * (matrix[0, 1] + matrix[0, 2])
                                         + (dataPtr - nChan)[0] * matrix[1, 0]
                                         + dataPtr[0] * (matrix[1, 1] + matrix[1, 2])
                                         + (dataPtr - nChan + m.widthStep)[0] * matrix[2, 0]
                                         + (dataPtr + m.widthStep)[0] * (matrix[2, 1] + matrix[2, 2])) / matrixWeight));
                    dataPtr2[0] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    avg = (int)(Math.Round(((dataPtr - m.widthStep - nChan)[1] * matrix[0, 0]
                                         + (dataPtr - m.widthStep)[1] * (matrix[0, 1] + matrix[0, 2])
                                         + (dataPtr - nChan)[1] * matrix[1, 0]
                                         + dataPtr[1] * (matrix[1, 1] + matrix[1, 2])
                                         + (dataPtr - nChan + m.widthStep)[1] * matrix[2, 0]
                                         + (dataPtr + m.widthStep)[1] * (matrix[2, 1] + matrix[2, 2])) / matrixWeight));
                    dataPtr2[1] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    avg = (int)(Math.Round(((dataPtr - m.widthStep - nChan)[2] * matrix[0, 0]
                                         + (dataPtr - m.widthStep)[2] * (matrix[0, 1] + matrix[0, 2])
                                         + (dataPtr - nChan)[2] * matrix[1, 0]
                                         + dataPtr[2] * (matrix[1, 1] + matrix[1, 2])
                                         + (dataPtr - nChan + m.widthStep)[2] * matrix[2, 0]
                                         + (dataPtr + m.widthStep)[2] * (matrix[2, 1] + matrix[2, 2])) / matrixWeight));
                    dataPtr2[2] = (byte)(avg > 255 ? 255 : avg < 0 ? 0 : avg);

                    dataPtr -= m.widthStep;
                    dataPtr2 -= m.widthStep;
                }*/
            }
        }

    }
}

