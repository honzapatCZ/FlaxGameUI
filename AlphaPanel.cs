using System;
using System.Collections.Generic;
using FlaxEngine;

namespace FlaxEngine.GUI
{
    public class AlphaPanel : ContainerControl
    {
        public float Alpha;
        public bool IgnoreStack;

        public override void Draw()
        {
            if (IgnoreStack)
            {
                Render2d.PushTint(Color);
            }
            else
            {
                Render2d.PushTint(Render2d.PeekTint() * Color);
            }
            base.Draw();
            Render2D.PopTint();
        }
    }
}
