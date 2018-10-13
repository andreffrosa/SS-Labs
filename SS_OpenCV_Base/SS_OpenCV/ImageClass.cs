using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;

namespace SS_OpenCV
{
    class ImageClass
    {

        /// <summary>
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

        internal static void Translation(Image<Bgr, byte> origin, Image<Bgr, byte> dest, int dx, int dy)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = dest.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte* dataPtr2 = (byte*)origin.MIplImage.imageData.ToPointer(); // Pointer to the image

                int width = dest.Width;
                int height = dest.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        // Endereçamento Absoluto: Dataptr = dataptr_i + y*widthstep + x*nChan
                        int x_original = (x - dx < width && x - dx > 0) ? x - dx : -1;
                        int y_original = (y - dy < height && y - dy > 0) ? y - dy : -1;

                        if ( x_original < 0 || y_original < 0)
                        {
                            dataPtr[0] = 0;
                            dataPtr[1] = 0;
                            dataPtr[2] = 0;
                        } else {

                            int index = y_original * m.widthStep + x_original * nChan;
                            dataPtr[0] = dataPtr2[0 + index];
                            dataPtr[1] = dataPtr2[1 + index];
                            dataPtr[2] = dataPtr2[2 + index];
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }

                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }

            }
        }

        internal static void Rotation(Image<Bgr, byte> origin, Image<Bgr, byte> dest, double theta)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right
                MIplImage m = dest.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte* dataPtr2 = (byte*)origin.MIplImage.imageData.ToPointer(); // Pointer to the image

                int width = dest.Width;
                int height = dest.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        // Endereçamento Absoluto: Dataptr = dataptr_i + y*widthstep + x*nChan
                        int x_original = (int)Math.Round( (x-(width/2.0) ) * Math.Cos(theta) - ((height / 2.0) - y) * Math.Sin(theta) + (width / 2.0) );
                        int y_original = (int)Math.Round( (height / 2.0) - (x - (width / 2.0)) * Math.Sin(theta) - ((height / 2.0) - y) * Math.Cos(theta));

                        if (x_original < 0 || x_original >= width || y_original < 0 || y_original >= height)
                        {
                            dataPtr[0] = 0;
                            dataPtr[1] = 0;
                            dataPtr[2] = 0;
                        }
                        else
                        {

                            int index = y_original * m.widthStep + x_original * nChan;
                            dataPtr[0] = dataPtr2[0 + index];
                            dataPtr[1] = dataPtr2[1 + index];
                            dataPtr[2] = dataPtr2[2 + index];
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }

                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }

            }
        }

        internal static void Scale(Image<Bgr, byte> origin, Image<Bgr, byte> dest, double s, int p_x, int p_y)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right
                MIplImage m = dest.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte* dataPtr2 = (byte*)origin.MIplImage.imageData.ToPointer(); // Pointer to the image

                int width = dest.Width;
                int height = dest.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        // Endereçamento Absoluto: Dataptr = dataptr_i + y*widthstep + x*nChan
                        int x_original = (int)Math.Round( (x - (width / 2.0))/s - p_x );
                        int y_original = (int)Math.Round( (y - (width/2.0))/ s - p_y );
                        
                        // Puxar a img para o centro e só depois mover a img para o ponto escolhido
                    
                        if (x_original < 0 || x_original >= width || y_original < 0 || y_original >= height)
                        {
                            dataPtr[0] = 0;
                            dataPtr[1] = 0;
                            dataPtr[2] = 0;
                        }
                        else
                        {

                            int index = y_original * m.widthStep + x_original * nChan;
                            dataPtr[0] = dataPtr2[0 + index];
                            dataPtr[1] = dataPtr2[1 + index];
                            dataPtr[2] = dataPtr2[2 + index];
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }

                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }

            }
        }

        public static void Mean(Image<Bgr, byte> origin, Image<Bgr, byte> dest)
        {
            unsafe
            {
                MIplImage m = dest.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the original image
                byte* dataPtr2 = (byte*)origin.MIplImage.imageData.ToPointer(); // Pointer to the destination image

                int width = dest.Width;
                int height = dest.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                dataPtr += nChan + m.widthStep;
                dataPtr2 += nChan + m.widthStep;

                // Core
                for (y = 1; y < height - 1; y++) {
                    for (x = 1; x < width - 1; x++) {
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
                dataPtr2 = (byte*)origin.MIplImage.imageData.ToPointer();

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
                dataPtr2[0] = 0;/*(byte)(Math.Round((2 * (dataPtr - nChan)[0] + 4 * dataPtr[0] + 2 * (dataPtr + m.widthStep)[0] + (dataPtr + m.widthStep - nChan)[0]) / 9.0));*/
                dataPtr2[1] = 255;/*(byte)(Math.Round((2 * (dataPtr - nChan)[1] + 4 * dataPtr[1] + 2 * (dataPtr + m.widthStep)[1] + (dataPtr + m.widthStep - nChan)[1]) / 9.0));*/
                dataPtr2[2] = 0;/*(byte)(Math.Round((2 * (dataPtr - nChan)[2] + 4 * dataPtr[2] + 2 * (dataPtr + m.widthStep)[2] + (dataPtr + m.widthStep - nChan)[2]) / 9.0));*/
                
                dataPtr += nChan + padding;
                dataPtr2 += nChan + padding;
                
            //    // Left Column
            //    for (y = 1; y < height - 1; y++)
            //    {

            //        dataPtr2[0] = 0;/*(byte)(Math.Round((2 * (dataPtr - m.widthStep)[0] + (dataPtr + nChan - m.widthStep)[0]
            //                          + 2 * dataPtr[0] + (dataPtr + nChan)[0]
            //                          + 2 * (dataPtr + m.widthStep)[0] + (dataPtr + m.widthStep + nChan)[0]) / 9.0));*/
            //        dataPtr2[1] = 0;/* (byte)(Math.Round((2 * (dataPtr - m.widthStep)[1] + (dataPtr + nChan - m.widthStep)[1]
            //                          + 2 * dataPtr[1] + (dataPtr + nChan)[1]
            //                          + 2 * (dataPtr + m.widthStep)[1] + (dataPtr + m.widthStep + nChan)[1]) / 9.0));*/
            //        dataPtr2[2] = 255;/* (byte)(Math.Round((2 * (dataPtr - m.widthStep)[2] + (dataPtr + nChan - m.widthStep)[2]
            //                          + 2 * dataPtr[2] + (dataPtr + nChan)[2]
            //                          + 2 * (dataPtr + m.widthStep)[2] + (dataPtr + m.widthStep + nChan)[2]) / 9.0));*/

            //        dataPtr += m.widthStep;
            //        dataPtr2 += m.widthStep;
            //    }

            //    // (height-1,0)
            //    dataPtr2[0] = 255;/*(byte)(Math.Round((2 * (dataPtr - m.widthStep)[0] + (dataPtr + nChan - m.widthStep)[0]
            //                          + 4 * dataPtr[0] + 2 * (dataPtr + nChan)[0]) / 9.0));*/
            //    dataPtr2[1] = 0;/* (byte)(Math.Round((2 * (dataPtr - m.widthStep)[1] + (dataPtr + nChan - m.widthStep)[1]
            //                          + 4 * dataPtr[1] + 2 * (dataPtr + nChan)[1]) / 9.0));*/
            //    dataPtr2[2] = 0;/* (byte)(Math.Round((2 * (dataPtr - m.widthStep)[2] + (dataPtr + nChan - m.widthStep)[2]
            //                          + 4 * dataPtr[2] + 2 * (dataPtr + nChan)[2]) / 9.0));*/

            //    dataPtr += nChan;
            //    dataPtr2 += nChan;
                
            //    // Bottom line
            //    for (x = 1; x < width - 1; x++)
            //    {
            //        dataPtr2[0] = 0;/* (byte)(Math.Round(((dataPtr - nChan - m.widthStep)[0] + (dataPtr - m.widthStep)[0] + (dataPtr + nChan - m.widthStep)[0]
            //                        + 2 * (dataPtr - nChan)[0] + 2 * dataPtr[0] + 2 * (dataPtr + nChan)[0]) / 9.0));*/
            //        dataPtr2[1] = 0;/* (byte)(Math.Round(((dataPtr - nChan - m.widthStep)[1] + (dataPtr - m.widthStep)[1] + (dataPtr + nChan - m.widthStep)[1]
            //                        + 2 * (dataPtr - nChan)[1] + 2 * dataPtr[0] + 2 * (dataPtr + nChan)[1]) / 9.0));*/
            //        dataPtr2[2] = 0;/* (byte)(Math.Round(((dataPtr - nChan - m.widthStep)[2] + (dataPtr - m.widthStep)[2] + (dataPtr + nChan - m.widthStep)[2]
            //                        + 2 * (dataPtr - nChan)[2] + 2 * dataPtr[2] + 2 * (dataPtr + nChan)[2]) / 9.0));*/

            //        dataPtr += nChan;
            //        dataPtr2 += nChan;
            //    }

            //    // (width-1,heigh-1)
            //    dataPtr2[0] = 255;/*(byte)(Math.Round((2 * (dataPtr - nChan)[0] + 4 * dataPtr[0] + 2 * (dataPtr + m.widthStep)[0] + (dataPtr + m.widthStep - nChan)[0]) / 9.0));*/
            //    dataPtr2[1] = 0;/*(byte)(Math.Round((2 * (dataPtr - nChan)[1] + 4 * dataPtr[1] + 2 * (dataPtr + m.widthStep)[1] + (dataPtr + m.widthStep - nChan)[1]) / 9.0));*/
            //    dataPtr2[2] = 0;/*(byte)(Math.Round((2 * (dataPtr - nChan)[2] + 4 * dataPtr[2] + 2 * (dataPtr + m.widthStep)[2] + (dataPtr + m.widthStep - nChan)[2]) / 9.0));*/

            //    dataPtr -= m.widthStep;
            //    dataPtr2 -= m.widthStep;
                
            //    // Right Column
            //    for (y = height - 1; y > 0 ; y--) {

            //        dataPtr2[0] = 0; /*(byte)(Math.Round(( (dataPtr - m.widthStep - nChan)[0] + 2 *(dataPtr - m.widthStep)[0] +
            //                          + (dataPtr - nChan)[0]  + 2 * dataPtr[0] 
            //                          + (dataPtr - nChan + m.widthStep)[0] + 2 * (dataPtr + m.widthStep)[0] ) / 9.0));*/

            //        dataPtr2[1] = 0;/*(byte)(Math.Round(((dataPtr - m.widthStep - nChan)[1] + 2 * (dataPtr - m.widthStep)[1] +
            //                          + (dataPtr - nChan)[1] + 2 * dataPtr[1]
            //                          + (dataPtr - nChan + m.widthStep)[1] + 2 * (dataPtr + m.widthStep)[1]) / 9.0));*/

            //        dataPtr2[2] = 255; /* (byte)(Math.Round(((dataPtr - m.widthStep - nChan)[2] + 2 * (dataPtr - m.widthStep)[2] +
            //                          + (dataPtr - nChan)[2] + 2 * dataPtr[2]
            //                          + (dataPtr - nChan + m.widthStep)[2] + 2 * (dataPtr + m.widthStep)[2]) / 9.0));*/

            //        dataPtr -= m.widthStep;
            //        dataPtr2 -= m.widthStep;
            //    }
              
            }
        }

        public static void NonUniform(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float[,] matrix, float matrixWeight) {

            unsafe {
                MIplImage m = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the original image
                byte* dataPtr2 = (byte*)img.MIplImage.imageData.ToPointer(); // Pointer to the destination image

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                // Process the center
                dataPtr += nChan + m.widthStep;
                dataPtr2 += nChan + m.widthStep;

                int avg;

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
                        dataPtr2[0] = (byte)( avg > 255 ? 255 : avg < 0 ? 0 : avg);

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
            }
        }
    }
}

