﻿// Copyright (c) Microsoft. All rights reserved. 
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System.Drawing.Graphics
{
    public static class ImageExtensions
    {
        
        //Resizing
        public static Image Resize(this Image sourceImage, int width, int height)
        {
            if (width > 0 && height > 0)
            {
                Image destinationImage = Image.Create(width, height);
                DLLImports.gdImageCopyResized(destinationImage.gdImageStructPtr, sourceImage.gdImageStructPtr, 0, 0, 0, 0,
                    destinationImage.WidthInPixels, destinationImage.HeightInPixels, sourceImage.WidthInPixels, sourceImage.HeightInPixels);
                return destinationImage;
            }
            else
            {
                throw new InvalidOperationException("Parameters for resizing an image must be positive integers.");
            }
        }

        //Transparency
        public static void SetTransparency(this Image image, double percentOpacity)
        {
            if(percentOpacity > 100 || percentOpacity < 0)
            {
                throw new InvalidOperationException(" Percent Transparency must be a value between 0 - 100.");
            }

            double alphaAdjustment = (100.0 - percentOpacity) / 100.0;
            for(int y = 0; y < image.HeightInPixels; y++)
            {
                for(int x = 0; x < image.WidthInPixels; x++)
                {
                    //get the current color of the pixel
                    int currentColor = DLLImports.gdImageGetPixel(image.gdImageStructPtr, x, y);
                    //mask to just get the alpha value (7 bits)
                    double currentAlpha = (currentColor >> 24) & 0xff;


                    if (x == 10 && y == 10)
                    {
                        System.Console.WriteLine(currentColor);
                        System.Console.WriteLine(currentAlpha);
                    }
                    ////System.Console.WriteLine(currentAlpha);
                    ////if the current alpha is transparent
                    ////dont bother/ skip over
                    //if (currentAlpha == 127)
                    //    continue;
                    ////calculate the new alpha value given the adjustment
                    currentAlpha += (127 - currentAlpha) * alphaAdjustment;
                    ////if it is somehow transparent now
                    ////dont bother setting pixel/skip over
                    //if (currentAlpha >= 127)
                    //    continue;

                    //make a new color with the new alpha to set the pixel
                    currentColor = (currentColor & 0x00ffffff | ((int)currentAlpha << 24));

                    if (x == 10 && y == 10)
                    {
                        System.Console.WriteLine(currentColor);
                        System.Console.WriteLine(currentAlpha);
                    }
                    //System.Console.WriteLine(currentColor);
                    DLLImports.gdImageSetPixel(image.gdImageStructPtr, x, y, currentColor);

                    if (x == 10 && y == 10)
                    {
                        System.Console.WriteLine(currentColor);
                        System.Console.WriteLine(currentAlpha);
                    }

                }
            }

        }


        //Stamping an Image onto another
        public static void Draw(this Image image, Image imageToDraw, int xOffset, int yOffset)
        {
            //These lines should be added to make it work
            DLLImports.gdImageAlphaBlending(image.gdImageStructPtr, 1);
            DLLImports.gdImageAlphaBlending(imageToDraw.gdImageStructPtr, 1);

            //loop through the source image
            for (int y = 0; y < imageToDraw.HeightInPixels; y++)
            {
                for(int x = 0; x < imageToDraw.WidthInPixels; x++)
                {
                    int color = DLLImports.gdImageGetPixel(imageToDraw.gdImageStructPtr, x, y);

                    int alpha = (color >> 24) & 0xff;
                    if (alpha == 127)
                    {
                        continue;
                    }

                    DLLImports.gdImageSetPixel(image.gdImageStructPtr, x + xOffset, y + yOffset, color);

                }
            }
        }

    }
}