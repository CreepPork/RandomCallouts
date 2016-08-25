﻿using System;
using Rage;

namespace RandomCallouts.Extensions
{
    public static class Vector3Extension
    {
        public static Vector3 ExtensionAround(this Vector3 start, float radius)
        {
            // Random direction.
            Vector3 direction = Vector3Extension.ExtensionRandomXY();
            Vector3 around = start + (direction * radius);
            return around;
        }

        public static float ExtensionDistanceTo(this Vector3 start, Vector3 end)
        {
            return (end - start).Length();
        }

        public static Vector3 ExtensionRandomXY()
        {
            Random random = new Random(Environment.TickCount);

            Vector3 vector3 = new Vector3();
            vector3.X = (float)(random.NextDouble() - 0.5);
            vector3.Y = (float)(random.NextDouble() - 0.5);
            vector3.Z = 0.0f;
            vector3.Normalize();
            return vector3;
        }
    }
}