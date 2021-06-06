using GLFW;
using OpenGLTutorial.GameLoop;
using OpenGLTutorial.Rendering;
using OpenGLTutorial.Rendering.Cameras;
using OpenGLTutorial.Rendering.Display;
using OpenGLTutorial.Rendering.Shaders;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using static OpenGLTutorial.OpenGL.GL;


namespace OpenGLTutorial
{
    class TestGame : Game
    {
        uint backgroundVAO;
        uint cowboyVAO1;
        uint cowboyVAO2;
        uint backgroundVbo;
        uint cowboyVbo1;
        uint cowboyVbo2;

        Shader shader;
        OrthographicCamera camera;
        Camera2D camera2D;
        Texture backgroundTexture;
        Texture cowboyTexture1;
        Texture cowboyTexture2;

        Matrix4x4 cowboy1State;
        Matrix4x4 cowboy2State;

        public TestGame(int initialWindowWidth, int initialWindowHeight, string intitalWindowTitle) : base(initialWindowWidth, initialWindowHeight, intitalWindowTitle)
        {
            //float aspectRatio = (float)initialWindowWidth / (float)initialWindowHeight;
            //camera = new OrthographicCamera(-aspectRatio, aspectRatio, -1.0f, 1.0f);
            //camera = new OrthographicCamera(0, 1200, 600, 0); // (0, 0) is top-left, (width, height) is bottom-right
        }

        protected override void Initialize()
        {

        }

        protected unsafe override void LoadContent()
        {
            string vertexShader = @"
                                        #version 330 core
                                        layout (location = 0) in vec2 aPosition;
                                        layout (location = 1) in vec3 aColor;
                                        layout (location = 2) in vec2 aTexCoord;

                                        uniform mat4 uProjViewMatrix;
                                        uniform mat4 uTransform;

                                        out vec2 vTexCoord;

                                        void main() {
                                            vTexCoord = aTexCoord;
                                            gl_Position = uTransform * uProjViewMatrix * vec4(aPosition.xy, 0.0, 1.0);
                                        }";

        string fragmentShader = @"
                                        #version 330 core
                                        layout (location = 0) out vec4 FragColor;

                                        uniform sampler2D uTexture;

                                        in vec2 vTexCoord;

                                        void main() {
                                            //FragColor = vec4(vTexCoord, 0.0, 1.0);
                                            //FragColor = vec4(1.0, 0.0, 0.0, 1.0);
                                            FragColor = texture(uTexture, vTexCoord);
                                            if(FragColor.a < 0.1)
                                            {
                                              discard;
                                            }
                                        }";

            // Loading texture
            backgroundTexture = new Texture("../../../Assets/desert.png");
            cowboyTexture1 = new Texture("../../../Assets/cowboy.png");
            cowboyTexture2 = new Texture("../../../Assets/cowboy.png");

            shader = new Shader(vertexShader, fragmentShader);
            shader.Load();

            // Setting the uTexture to use texture slot 0 (GL_TEXTURE0)
            shader.Use();
            shader.SetInt("uTexture", 0);

            // Background
            {
                backgroundVAO = glGenVertexArray();
                glBindVertexArray(backgroundVAO);

                /*
                float[] backgroundVertices = // x, y, r, g, b, tx, ty
                {
                    -2.0f,  1.0f, 1f, 0f, 0f, 0.0f, 1.0f, // top left
                     2.0f,  1.0f, 0f, 1f, 0f, 1.0f, 1.0f,// top right
                    -2.0f, -1.0f, 0f, 0f, 1f, 0.0f, 0.0f, // bottom left

                     2.0f,  1.0f, 0f, 1f, 0f, 1.0f, 1.0f,// top right
                     2.0f, -1.0f, 0f, 1f, 1f, 1.0f, 0.0f, // bottom right
                    -2.0f, -1.0f, 0f, 0f, 1f, 0.0f, 0.0f, // bottom left
                };
                */

                float[] backgroundVertices = // x, y, r, g, b, tx, ty
{
                        0f,   0f, 1f, 0f, 0f, 0.0f, 1.0f, // top left
                     1200f,   0f, 0f, 1f, 0f, 1.0f, 1.0f, // top right
                        0f, 600f, 0f, 0f, 1f, 0.0f, 0.0f, // bottom left

                     1200f,   0f, 0f, 1f, 0f, 1.0f, 1.0f, // top right
                     1200f, 600f, 0f, 1f, 1f, 1.0f, 0.0f, // bottom right
                        0f, 600f, 0f, 0f, 1f, 0.0f, 0.0f, // bottom left
                };

                backgroundVbo = glGenBuffer();
                glBindBuffer(GL_ARRAY_BUFFER, backgroundVbo);
                fixed (float* v = &backgroundVertices[0])
                {
                    glBufferData(GL_ARRAY_BUFFER, sizeof(float) * backgroundVertices.Length, v, GL_STATIC_DRAW);
                }

                glVertexAttribPointer(0, 2, GL_FLOAT, false, 7 * sizeof(float), (void*)0);
                glEnableVertexAttribArray(0);

                glVertexAttribPointer(1, 3, GL_FLOAT, false, 7 * sizeof(float), (void*)(2 * sizeof(float)));
                glEnableVertexAttribArray(1);

                glVertexAttribPointer(2, 2, GL_FLOAT, false, 7 * sizeof(float), (void*)(5 * sizeof(float)));
                glEnableVertexAttribArray(2);
            }

            // Cowboy
            {
                /*
                float[] cowboyVertices = // x, y, r, g, b, tx, ty
                {
                    -0.3f,  0.5f, 1f, 0f, 0f, 0.0f, 1.0f, // top left
                     0.3f,  0.5f, 0f, 1f, 0f, 1.0f, 1.0f,// top right
                    -0.3f, -0.5f, 0f, 0f, 1f, 0.0f, 0.0f, // bottom left

                     0.3f,  0.5f, 0f, 1f, 0f, 1.0f, 1.0f,// top right
                     0.3f, -0.5f, 0f, 1f, 1f, 1.0f, 0.0f, // bottom right
                    -0.3f, -0.5f, 0f, 0f, 1f, 0.0f, 0.0f, // bottom left
                };
                */

                float[] cowboyVertices = // x, y, r, g, b, tx, ty
{
                       0f,   0f, 1f, 0f, 0f, 0.0f, 1.0f, // top left
                     200f,   0f, 0f, 1f, 0f, 1.0f, 1.0f, // top right
                       0f, 300f, 0f, 0f, 1f, 0.0f, 0.0f, // bottom left

                     200f,   0f, 0f, 1f, 0f, 1.0f, 1.0f, // top right
                     200f, 300f, 0f, 1f, 1f, 1.0f, 0.0f, // bottom right
                       0f, 300f, 0f, 0f, 1f, 0.0f, 0.0f, // bottom left
                };

                //
                cowboyVAO1 = glGenVertexArray();
                glBindVertexArray(cowboyVAO1);

                cowboyVbo1 = glGenBuffer();
                glBindBuffer(GL_ARRAY_BUFFER, cowboyVbo1);

                fixed (float* v = &cowboyVertices[0])
                {
                    glBufferData(GL_ARRAY_BUFFER, sizeof(float) * cowboyVertices.Length, v, GL_STATIC_DRAW);
                }

                glVertexAttribPointer(0, 2, GL_FLOAT, false, 7 * sizeof(float), (void*)0);
                glEnableVertexAttribArray(0);

                glVertexAttribPointer(1, 3, GL_FLOAT, false, 7 * sizeof(float), (void*)(2 * sizeof(float)));
                glEnableVertexAttribArray(1);

                glVertexAttribPointer(2, 2, GL_FLOAT, false, 7 * sizeof(float), (void*)(5 * sizeof(float)));
                glEnableVertexAttribArray(2);

                //
                cowboyVAO2 = glGenVertexArray();
                glBindVertexArray(cowboyVAO2);

                cowboyVbo2 = glGenBuffer();
                glBindBuffer(GL_ARRAY_BUFFER, cowboyVbo2);

                fixed (float* v = &cowboyVertices[0])
                {
                    glBufferData(GL_ARRAY_BUFFER, sizeof(float) * cowboyVertices.Length, v, GL_STATIC_DRAW);
                }

                glVertexAttribPointer(0, 2, GL_FLOAT, false, 7 * sizeof(float), (void*)0);
                glEnableVertexAttribArray(0);

                glVertexAttribPointer(1, 3, GL_FLOAT, false, 7 * sizeof(float), (void*)(2 * sizeof(float)));
                glEnableVertexAttribArray(1);

                glVertexAttribPointer(2, 2, GL_FLOAT, false, 7 * sizeof(float), (void*)(5 * sizeof(float)));
                glEnableVertexAttribArray(2);

                camera = new OrthographicCamera(0, DisplayManager.WindowSize.X, DisplayManager.WindowSize.Y, 0); // (0, 0) is top-left, (width, height) is bottom-right

                //
                cowboy1State = Matrix4x4.Identity;
                //Vector3 position = new Vector3(-0.5f, 0.0f, 0.0f);
                Vector3 position = new Vector3(0.1f, -0.9f, 0.0f);
                cowboy1State *= Matrix4x4.CreateTranslation(position);
                Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);
                cowboy1State *= Matrix4x4.CreateScale(scale);
                float zRotation = 0.0f; // 45.0f * (3.14f / 180.0f);
                cowboy1State *= Matrix4x4.CreateRotationZ(zRotation);

                //
                cowboy2State = Matrix4x4.Identity;
                //position = new Vector3(0.5f, 0.0f, 0.0f);
                position = new Vector3(1.5f, -0.9f, 0.0f);
                cowboy2State *= Matrix4x4.CreateTranslation(position);
                scale = new Vector3(1.0f, 1.0f, 1.0f);
                cowboy2State *= Matrix4x4.CreateScale(scale);
                zRotation = 0.0f; // 45.0f * (3.14f / 180.0f);
                cowboy2State *= Matrix4x4.CreateRotationZ(zRotation);

                //camera2D = new Camera2D(DisplayManager.WindowSize / 2.0f, 1.0f);
            }
        }

        protected override void Update()
        {
        }

        float i = -0.5f;

        protected override void Render()
        {
            i += 0.0001f;

            /*
            cowboy1State = Matrix4x4.Identity;
            Vector3 position = new Vector3( i, -0.45f, 0.0f);
            cowboy1State *= Matrix4x4.CreateTranslation(position);
            Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);
            cowboy1State *= Matrix4x4.CreateScale(scale);
            float zRotation = 0.0f; // 45.0f * (3.14f / 180.0f);
            cowboy1State *= Matrix4x4.CreateRotationZ(zRotation);

            cowboy2State = Matrix4x4.Identity;
            position = new Vector3(0.5f, -0.45f, 0.0f);
            cowboy2State *= Matrix4x4.CreateTranslation(position);
            scale = new Vector3(1.0f, 1.0f, 1.0f);
            cowboy2State *= Matrix4x4.CreateScale(scale);
            zRotation = 0.0f; // 45.0f * (3.14f / 180.0f);
            cowboy2State *= Matrix4x4.CreateRotationZ(zRotation);
            */

            glClearColor(0.2f, 0.2f, 0.2f, 0.2f);
            glClear(GL_COLOR_BUFFER_BIT);

            shader.Use();
            // Sending ProjViewMatrix to the shader (each frame)
            shader.SetMat4("uProjViewMatrix", camera.GetProjViewMatrix());
            glBindVertexArray(backgroundVAO);
            shader.SetMat4("uTransform", Matrix4x4.Identity);
            // Binding texture
            backgroundTexture.Bind();
            glDrawArrays(GL_TRIANGLES, 0, 6);

            shader.SetMat4("uTransform", cowboy1State);
            glBindVertexArray(cowboyVAO1);
            cowboyTexture1.Bind();
            glDrawArrays(GL_TRIANGLES, 0, 6);

            shader.SetMat4("uTransform", cowboy2State);
            glBindVertexArray(cowboyVAO2);
            cowboyTexture2.Bind();
            glDrawArrays(GL_TRIANGLES, 0, 6);

            glBindVertexArray(0);

            Glfw.SwapBuffers(DisplayManager.Window);
        }
    }
}
