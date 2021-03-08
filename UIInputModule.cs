using System;
using System.Collections.Generic;
using FlaxEngine;

namespace FlaxGameUI
{
    public class UIInputModule : Script
    {
        public float MoveRepeatDelay = 0.5f;
        public float MoveRepeatRate = 0.2f;

        [Serialize]
        protected UIInputSystem cachedInputSystem;
        public UIInputSystem InputSystem
        {
            get
            {
                if(cachedInputSystem == null)
                {
                    cachedInputSystem = Actor.GetScript<UIInputSystem>();
                }
                return cachedInputSystem;
            }
        }
    }
}
