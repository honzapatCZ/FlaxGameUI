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
        ISelectable currentlySelected;

        [Serialize]
        UIControl defaultSelectedControl;
        [NoSerialize]
        public UIControl DefaultSelectedControl
        {
            get => defaultSelectedControl;
            set
            {
                if(value != null && !(value.Control is ISelectable))
                {
                    Debug.LogError("Default Selected does not implement ISelectable");
                    return;
                }
                defaultSelectedControl = value;
            }
        }
        public override void OnStart()
        {
            base.OnStart();
            NavigateTo(DefaultSelectedControl.Control as ISelectable);
        }

        public void NavigateTo(ISelectable newSelectable)
        {
            currentlySelected?.OnDeSelect();
            currentlySelected = newSelectable;
            currentlySelected.OnSelect();
        }

        public void Navigate(NavDir navDir)
        {
            Debug.Log("Navigating in " + navDir);
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
