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

        [NoSerialize]
        public static Dictionary<RootControl, List<ISelectable>> selectablesByRoot = new Dictionary<RootControl, List<ISelectable>>();
        [NoSerialize]
        public static Dictionary<RootControl, UIInputSystem> inputSystemsByRoot = new Dictionary<RootControl, UIInputSystem>();

        public static UIInputSystem instance;

        public static UIInputSystem GetInputSystemForRctrl(RootControl rctrl)
        {
            if (inputSystemsByRoot.ContainsKey(rctrl))
                return inputSystemsByRoot[rctrl];
            else
                return instance;
        }

        public override void OnAwake()
        {
            base.OnAwake();

            if (instance == null)
                instance = this;

            RootControl rctrl = (Actor as UICanvas).GUI;
            if (rctrl != null)
                inputSystemsByRoot.Add(rctrl, this);
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            if (instance == this)
                instance = null;
        }

        public override void OnStart()
        {
            base.OnStart();
            if(DefaultSelectedControl != null)
                NavigateTo(DefaultSelectedControl.Control as ISelectable);
        }

        public void NavigateTo(ISelectable newSelectable)
        {
            currentlySelected?.OnDeSelect();
            currentlySelected = newSelectable;
            currentlySelected?.OnSelect();
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
