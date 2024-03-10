using OpenGlTIPE.Rendering.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OpenGlTIPE.Rendering.Camera
{
    class Camera2d
    {
        Vector2 focusPosition;
        public float zoom;

        public Camera2d(Vector2 focusPosition, float zoom)
        {
            this.focusPosition = focusPosition;
            this.zoom = zoom;
        }

        public Matrix4x4 GetProjectionMatrix()
        {
            float left = focusPosition.X  - DisplayManager.WindowSize.X / 2;
            float right = focusPosition.X + DisplayManager.WindowSize.X / 2;
            float bottom = focusPosition.Y- DisplayManager.WindowSize.Y / 2;
            float top = focusPosition.Y   + DisplayManager.WindowSize.Y / 2;
            Matrix4x4 orthoMatrix = Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, 0.1f, 100f);
            Matrix4x4 zoomMatrix = Matrix4x4.CreateScale(zoom);

            return orthoMatrix * zoomMatrix;
        }
    }
}
