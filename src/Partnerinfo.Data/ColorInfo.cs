// Copyright (c) János Janka. All rights reserved.

using System;
using Newtonsoft.Json;

namespace Partnerinfo
{
    [JsonConverter(typeof(ColorInfoConverter))]
    public sealed class ColorInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorInfo" /> class.
        /// </summary>
        public ColorInfo() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorInfo" /> class.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public ColorInfo(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorInfo" /> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public ColorInfo(int color)
        {
            RGB = color;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorInfo" /> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public ColorInfo(string color)
        {
            if (color == null || color.Length == 0)
            {
                RGB = 16777215;
            }
            else
            {
                if ((color[0] == '#') && ((color.Length == 7) || (color.Length == 4)))
                {
                    if (color.Length == 7)
                    {
                        R = Convert.ToByte(color.Substring(1, 2), 16);
                        G = Convert.ToByte(color.Substring(3, 2), 16);
                        B = Convert.ToByte(color.Substring(5, 2), 16);
                    }
                    else
                    {
                        string sr = char.ToString(color[1]);
                        string sg = char.ToString(color[2]);
                        string sb = char.ToString(color[3]);

                        R = Convert.ToByte(sr + sr, 16);
                        G = Convert.ToByte(sg + sg, 16);
                        B = Convert.ToByte(sb + sb, 16);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the red component of this <seealso cref="ColorInfo" />.
        /// </summary>
        /// <value>
        /// The red component.
        /// </value>
        public byte R { get; set; }

        /// <summary>
        /// Gets or sets the green component of this <see cref="ColorInfo" />.
        /// </summary>
        /// <value>
        /// The green component.
        /// </value>
        public byte G { get; set; }

        /// <summary>
        /// Gets or sets the blue component of this <see cref="ColorInfo" />.
        /// </summary>
        /// <value>
        /// The blue component.
        /// </value>
        public byte B { get; set; }

        /// <summary>
        /// Gets or sets the color of this <see cref="ColorInfo" />.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public int RGB
        {
            get
            {
                return (R << 16) | (G << 8) | B;
            }
            set
            {
                R = (byte)(value >> 16);
                G = (byte)(value >> 8);
                B = (byte)(value);
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "#" + R.ToString("x2", null) + G.ToString("x2", null) + B.ToString("x2", null);
        }
    }
}
