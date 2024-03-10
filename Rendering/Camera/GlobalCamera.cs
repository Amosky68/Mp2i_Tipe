using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OpenGlTIPE.Rendering.Camera
{
    class GlobalCamera
    {
        private float _x;
        private float _y;
        private float _z;

        private float _pitch;
        private float _yaw;
        private float _roll;

        public GlobalCamera(float x, float y, float z, float pitch, float yaw, float roll)
        {
            _x = x;
            _y = y;
            _z = z;
            _pitch = pitch;
            _yaw = yaw;
            _roll = roll;
        }

        public Matrix4x4 GetMatrix()
        {
            return Matrix4x4.CreateTranslation(_x, _y, _z) *
                Matrix4x4.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(_yaw, _pitch, _roll));
        }
    }
}
