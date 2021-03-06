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
            Render2D.PeekTint(out Color oldColor);
            if (IgnoreStack)
            {
                Color newColor = new Color(oldColor.R, oldColor.G, oldColor.B, Alpha);
                Render2D.PushTint(ref newColor, false);
            }
            else
            {
                Color newColor = new Color(oldColor.R, oldColor.G, oldColor.B, oldColor.A * Alpha);
                Render2D.PushTint(ref newColor, false);
            }
            base.Draw();
            
            Render2D.PopTint();
        }
    }
}
