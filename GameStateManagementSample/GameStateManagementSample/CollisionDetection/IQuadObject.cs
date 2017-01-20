using System;
using System.Windows;
using Microsoft.Xna.Framework;

namespace GameStateManagement.CollisionDetection
{
    public interface IQuadObject
    {
        Rectangle Bounds { get; }
        event EventHandler BoundsChanged;
    }
}