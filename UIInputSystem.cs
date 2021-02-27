using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxGameUI
{
    public enum NavDir
    {
        Left, Right, Up, Down
    }
    public class UIInputSystem : Script
    {
        private ISelectable currentlySelected;

        private UIControl defaultSelectedControl;
        public UIControl DefaultSelectedControl
        {
            get
            {
                return defaultSelectedControl;
            }
            set
            {
                if(!(value.Control is ISelectable))
                {
                    Debug.LogError("Default Selected does not implement ISelectable");
                    return;
                }
                defaultSelectedControl = value;
            }
        }

        public void ChangeCurrentlySelcted(ISelectable newSelectable)
        {
            currentlySelected.OnDeSelect();
            currentlySelected = newSelectable;
            currentlySelected.OnSelect();
        }


        public void Navigate(NavDir navDir)
        {
            currentlySelected.OnNavigate(navDir);
        }
        public void Submit()
        {
            currentlySelected.OnSubmit();
        }
    }
}
