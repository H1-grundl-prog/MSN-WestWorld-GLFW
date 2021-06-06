using System;
using System.Numerics;

namespace OpenGLTutorial.Rendering
{
    public class OrthographicCamera
    {
        public Matrix4x4 ProjectionMatrix { get; }
        public Matrix4x4 ViewMatrix { get; }

        public OrthographicCamera(float left, float right, float bottom, float top)
        {
            ProjectionMatrix = Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, -1.0f, 1.0f);
            ViewMatrix = Matrix4x4.Identity;
        }

        public Matrix4x4 GetProjViewMatrix()
        {
            return ViewMatrix * ProjectionMatrix;
        }
    }
}
