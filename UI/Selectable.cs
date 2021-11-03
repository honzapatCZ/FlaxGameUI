using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxGameUI
{
    public class Selectable : ContainerControl, ISelectable
    {
        bool inited = false;
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            if (!inited)
            {
                inited = true;
                //Debug.Log("Adding " + this + " for " + Root + " btw my parent is " + Parent);
                UIInputSystem.AddSelectable( this);
            }
        }
        public override void OnDestroy()
        {
            UIInputSystem.RemoveSelectable(this);
            base.OnDestroy();
        }

        [Flags]
        public enum PossibleDirs
        {
            Left = 1,
            Right = 2,
            Up = 4,
            Down = 8
        }

        public bool upAllowed => (GetAllowedDirs() & PossibleDirs.Up) != 0;
        public bool rightAllowed => (GetAllowedDirs() & PossibleDirs.Right) != 0;
        public bool leftAllowed => (GetAllowedDirs() & PossibleDirs.Left) != 0;
        public bool downAllowed => (GetAllowedDirs() & PossibleDirs.Down) != 0;

        [Serialize]
        protected UIControl onNavigateUp;
        [VisibleIf("upAllowed")]
        [NoSerialize, EditorDisplay("OnNavigate"), ExpandGroups]
        public UIControl OnNavigateUp{
            get => onNavigateUp;
            set{
                if (value != null && !(value.Control is ISelectable)) {
                    Debug.LogError("That UIControl does not implement ISelectable");
                    return;
                }
                onNavigateUp = value;
            }
        }

        [Serialize]
        protected UIControl onNavigateDown;
        [VisibleIf("downAllowed")]
        [NoSerialize, EditorDisplay("OnNavigate"), ExpandGroups]
        public UIControl OnNavigateDown
        {
            get => onNavigateDown;
            set {
                if (value != null && !(value.Control is ISelectable)) { 
                    Debug.LogError("That UIControl does not implement ISelectable"); 
                    return;
                }
                onNavigateDown = value;
            }
        }

        [Serialize]
        protected UIControl onNavigateLeft;
        [VisibleIf("leftAllowed")]
        [NoSerialize, EditorDisplay("OnNavigate"), ExpandGroups]
        public UIControl OnNavigateLeft
        {
            get => onNavigateLeft;
            set {
                if (value != null && !(value.Control is ISelectable)) {
                    Debug.LogError("That UIControl does not implement ISelectable"); 
                    return; 
                } 
                onNavigateLeft = value;
            }
        }

        [Serialize]
        protected UIControl onNavigateRight;
        [VisibleIf("rightAllowed")]
        [NoSerialize, EditorDisplay("OnNavigate"), ExpandGroups]
        public UIControl OnNavigateRight
        {
            get => onNavigateRight;
            set { 
                if (value != null && !(value.Control is ISelectable)) {
                    Debug.LogError("That UIControl does not implement ISelectable"); 
                    return; 
                }
                onNavigateRight = value;
            }
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

        public bool EvaluateInAutoNav()
        {
            return EnabledInHierarchy && VisibleInHierarchy && Root != null && Parent != null;
        }

        public RootControl GetRootControl()
        {
            return Root;
        }

        public Rectangle GetRectangle()
        {
            return new Rectangle(ScreenPos, Size);
        }
    }
}
