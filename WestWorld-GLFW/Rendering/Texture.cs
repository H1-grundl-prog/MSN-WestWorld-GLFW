using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static OpenGLTutorial.OpenGL.GL;

namespace OpenGLTutorial.Rendering
{
    public class Texture
    {
        private int m_Width;
        private int m_Height;

        public uint TextureID { get; set; }

        public Texture(string filepath)
        {
            // Loading imageData from a image
            IntPtr imageData = LoadImage(filepath, out m_Width, out m_Height);

            // Generate texture id
            TextureID = glGenTexture();
            glBindTexture(GL_TEXTURE_2D, TextureID);

            // Setting texture parameters
            // Ref: https://www.khronos.org/registry/OpenGL-Refpages/es2.0/xhtml/glTexParameter.xml
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST); // GL_NEAREST

            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);

            // Sending data to the GPU
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA8, m_Width, m_Height, 0, GL_RGBA, GL_UNSIGNED_BYTE, imageData);
            //glGenerateMipmap(GL_TEXTURE_2D);
        }

        public void Bind(int slot = 0)
        {
            // Binding the texture, to a specific texture slot
            glActiveTexture(GL_TEXTURE0 + slot);
            glBindTexture(GL_TEXTURE_2D, TextureID);
        }

        private static IntPtr LoadImage(string filepath, out int width, out int height, bool flipV = true)
        {
            Image<Rgba32> image = Image.Load<Rgba32>(filepath);

            if (flipV)
            {
                image.Mutate(x => x.Flip(FlipMode.Vertical));
            }
            List<byte> pixels = new List<byte>(image.Width * image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                var row = image.GetPixelRowSpan(y);
                for (int x = 0; x < image.Width; x++)
                {
                    pixels.Add(row[x].R);
                    pixels.Add(row[x].G);
                    pixels.Add(row[x].B);
                    pixels.Add(row[x].A);
                }
            }
            width = image.Width;
            height = image.Height;
            image.Dispose();

            GCHandle handle = GCHandle.Alloc(pixels.ToArray(), GCHandleType.Pinned);
            return handle.AddrOfPinnedObject(); ;
        }
    }
}
