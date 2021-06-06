using GLFW;
using OpenGLTutorial.Rendering.Display;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGLTutorial.GameLoop
{
    abstract class Game
    {
        protected int InitialWindowWidth { get; set; }
        protected int InitialWindowHeight { get; set; }
        protected string IntitalWindowTitle { get; set; }

        public Game(int initialWindowWidth, int initialWindowHeight, string intitalWindowTitle)
        {
            InitialWindowWidth = initialWindowWidth;
            InitialWindowHeight = initialWindowHeight;
            IntitalWindowTitle = intitalWindowTitle;
        }

        public void Run()
        {
            Initialize();

            DisplayManager.CreateWindow(InitialWindowWidth, InitialWindowHeight, IntitalWindowTitle);

            LoadContent();

            while(!Glfw.WindowShouldClose(DisplayManager.Window))
            {

                GameTime.DeltaTime = (float)Glfw.Time - GameTime.TotalElapsedSeconds;
                GameTime.TotalElapsedSeconds = (float)Glfw.Time;
                
                Update();

                Glfw.PollEvents();

                Render();
            }

            DisplayManager.CloseWindow();
        }

        protected abstract void Initialize();
        protected abstract void LoadContent();

        protected abstract void Update();
        protected abstract void Render();
    }
}
