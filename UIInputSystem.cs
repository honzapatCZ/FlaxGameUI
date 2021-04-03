using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.GUI;
using System.Linq;

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

        static Dictionary<RootControl, List<ISelectable>> selectablesByRoot = new Dictionary<RootControl, List<ISelectable>>();
        static Dictionary<RootControl, UIInputSystem> inputSystemsByRoot = new Dictionary<RootControl, UIInputSystem>();

        static UIInputSystem instance;

        public static UIInputSystem GetInputSystemForRctrl(RootControl rctrl)
        {
            if (inputSystemsByRoot.ContainsKey(rctrl))
                return inputSystemsByRoot[rctrl];
            else
                return instance;
        }

        public static List<ISelectable> GetSelListForRctrl(RootControl rctrl)
        {
            if (!selectablesByRoot.ContainsKey(rctrl))
            {
                selectablesByRoot.Add(rctrl, new List<ISelectable>());
            }
            return selectablesByRoot[rctrl];
        }
        public static void AddSelectable(ISelectable sel)
        {
            GetSelListForRctrl(sel.GetRootControl()).Add(sel);
        }
        public static void RemoveSelectable(ISelectable selec)
        {
            foreach (List<ISelectable> sels in selectablesByRoot.Values)
            {
                if(sels.Contains(selec))
                    sels.Remove(selec);
            }
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
                ISelectable newSel = AutoNaviagate(currentlySelected, navDir);
                Debug.LogWarning("We tried to auto navigate from " + currentlySelected + " in direction of " + navDir + " and found " + newSel);
                if(newSel != null)
                {
                    NavigateTo(newSel);
                }
            }            
        }

        public Vector2 GetUIVectorDirFromNavDir(NavDir navDir)
        {
            switch (navDir)
            {
                case NavDir.Down:
                    return new Vector2(0, 1);//Swapped dirs because UI is upside down
                case NavDir.Up:
                    return new Vector2(0, -1);
                case NavDir.Right:
                    return new Vector2(1, 0);
                case NavDir.Left:
                    return new Vector2(-1, 0);
                default:
                    return Vector2.Zero;
            }
        }

        public ISelectable AutoNaviagate(ISelectable from, NavDir navDir)
        {
            Vector2 directionOfInput = GetUIVectorDirFromNavDir(navDir);

            float closetsDistance = float.PositiveInfinity;
            ISelectable closestSelectable = null;
            foreach(ISelectable potentialSelectable in GetSelListForRctrl(from.GetRootControl()))
            {
                if (potentialSelectable != this && potentialSelectable.EvaluateInAutoNav())
                {
                    Vector2 directionOfDifferences = (potentialSelectable.GetPosition() - from.GetPosition());
                    directionOfDifferences.Normalize();
                    float likelinessOfDirectionCohersion = Vector2.Dot(directionOfInput, directionOfDifferences);
                    if (likelinessOfDirectionCohersion > 0f)
                    {
                        float distance = Vector2.Distance(potentialSelectable.GetPosition(), from.GetPosition());
                        if (distance < closetsDistance)
                        {
                            closetsDistance = distance;
                            closestSelectable = potentialSelectable;
                        }
                    }
                }
            }
            return closestSelectable;
        }

        public void Submit()
        {
            currentlySelected?.OnSubmit();
        }
    }
}
