﻿using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxGameUI
{
    public class Selectable : ContainerControl, ISelectable
    {
        [Flags]
        public enum PossibleDirs
        {
            Left = 1,
            Right = 2,
            Up = 4,
            Down = 8
        }
        [Serialize]
        UIControl onNavigateUp;
        [Serialize]
        UIControl onNavigateDown;
        [Serialize]
        UIControl onNavigateLeft;
        [Serialize]
        UIControl onNavigateRight;

        public bool upAllowed => (GetAllowedDirs() & PossibleDirs.Up) != 0;
        public bool rightAllowed => (GetAllowedDirs() & PossibleDirs.Right) != 0;
        public bool leftAllowed => (GetAllowedDirs() & PossibleDirs.Left) != 0;
        public bool downAllowed => (GetAllowedDirs() & PossibleDirs.Down) != 0;
        [VisibleIf("upAllowed")]
        [EditorDisplay("OnNavigate"), ExpandGroups]
        public UIControl OnNavigateUp{
            get => onNavigateUp;
            set{if (value != null && !(value.Control is ISelectable)) {Debug.LogError("That UIControl does not implement ISelectable");return;}onNavigateUp = value;}
        }
        [VisibleIf("rightAllowed")]
        [EditorDisplay("OnNavigate"), ExpandGroups]
        public UIControl OnNavigateDown
        {
            get => onNavigateDown;
            set { if (value != null && !(value.Control is ISelectable)) { Debug.LogError("That UIControl does not implement ISelectable"); return; } onNavigateDown = value; }
        }
        [VisibleIf("leftAllowed")]
        [EditorDisplay("OnNavigate"), ExpandGroups]
        public UIControl OnNavigateLeft
        {
            get => onNavigateLeft;
            set { if (value != null && !(value.Control is ISelectable)) { Debug.LogError("That UIControl does not implement ISelectable"); return; } onNavigateLeft = value; }
        }
        [VisibleIf("downAllowed")]
        [EditorDisplay("OnNavigate"), ExpandGroups]
        public UIControl OnNavigateRight
        {
            get => onNavigateRight;
            set { if (value != null && !(value.Control is ISelectable)) { Debug.LogError("That UIControl does not implement ISelectable"); return; } onNavigateRight = value; }
        }

        public virtual PossibleDirs GetAllowedDirs()
        {
            return PossibleDirs.Down | PossibleDirs.Left | PossibleDirs.Right | PossibleDirs.Up;
        }

        public virtual void OnSelect() { }
        public virtual void OnDeSelect(){}

        public virtual bool OnNavigate(NavDir navDir, UIInputSystem system)
        {
            switch (navDir)
            {
                case NavDir.Up:
                    {
                        if (upAllowed && OnNavigateUp)
                        {
                            system.NavigateTo(onNavigateUp.Control as ISelectable);
                            return true;
                        }
                        return false;
                    }
                case NavDir.Down:
                    {
                        if (downAllowed && OnNavigateDown)
                        {
                            system.NavigateTo(OnNavigateDown.Control as ISelectable);
                            return true;
                        }
                        return false;
                    }
                case NavDir.Left:
                    {
                        if (leftAllowed && OnNavigateLeft)
                        {
                            system.NavigateTo(OnNavigateLeft.Control as ISelectable);
                            return true;
                        }
                        return false;
                    }
                case NavDir.Right:
                    {
                        if (rightAllowed && OnNavigateRight)
                        {
                            system.NavigateTo(OnNavigateRight.Control as ISelectable);
                            return true;
                        }
                        return false;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        public virtual void OnSubmit(){}
    }
}