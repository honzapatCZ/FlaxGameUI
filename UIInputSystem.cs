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

        public void NavigateTo(ISelectable newSelectable)
        {
            currentlySelected?.OnDeSelect();
            currentlySelected = newSelectable;
            currentlySelected.OnSelect();
        }


        public void Navigate(NavDir navDir)
        {
            if (currentlySelected == null)
                return;
            if (!currentlySelected.OnNavigate(navDir, this))
            {
                Debug.LogWarning("We should auto navigate from " + currentlySelected + " in direction of " + navDir);
            }
            
        }
        public void Submit()
        {
            currentlySelected?.OnSubmit();
        }
    }
}
