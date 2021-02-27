using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxGameUI
{
    public class GameButton : Selectable
    {
        public Image targetImage;
        public Action Clicked;

        public override void OnSelect()
        {
            base.OnSelect();
        }
        public override void OnDeSelect()
        {
            base.OnDeSelect();
        }
        public override void OnSubmit()
        {
            base.OnSubmit();
        }
    }
}
