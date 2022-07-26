using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxGameUI
{
    public class GameButton : Selectable
    {
        public enum ButtonVerke
        {
            BrushSwap,
            ColorTint,
        }
        [EditorDisplay("Game Button"), ExpandGroups]
        public ButtonVerke buttonType;
        bool isColorTint { get => buttonType == ButtonVerke.ColorTint; }
        bool isBrushSwap { get => buttonType == ButtonVerke.BrushSwap; }
        
        [VisibleIf("isColorTint"), EditorDisplay("Game Button"), ExpandGroups]
        public Color NormalColor;
        [VisibleIf("isColorTint"), EditorDisplay("Game Button"), ExpandGroups]
        public Color HoverColor;
        [VisibleIf("isColorTint"), EditorDisplay("Game Button"), ExpandGroups]
        public Color PressedColor;
        [VisibleIf("isColorTint"), EditorDisplay("Game Button"), ExpandGroups]
        public Color DisabledColor;

        [VisibleIf("isBrushSwap"), EditorDisplay("Game Button"), ExpandGroups]
        public IBrush NormalBrush;
        [VisibleIf("isBrushSwap"), EditorDisplay("Game Button"), ExpandGroups]
        public IBrush HoverBrush;
        [VisibleIf("isBrushSwap"), EditorDisplay("Game Button"), ExpandGroups]
        public IBrush PressedBrush;
        [VisibleIf("isBrushSwap"), EditorDisplay("Game Button"), ExpandGroups]
        public IBrush DisabledBrush;

        [Serialize]
        UIControl tragetControl;
        Image targetControlAsImage => tragetControl?.Control as Image;
        Label targetControlAsLabel => tragetControl?.Control as Label;
        Panel targetControlAsPanel => tragetControl?.Control as Panel;

        [NoSerialize, EditorDisplay("Game Button"), ExpandGroups]
        public UIControl TargetControl 
        {
            get => tragetControl;
            set { 
                if (value != null && !(value.Control is Image) && !(value.Control is Label) && !(value.Control is Panel))
                {
                    Debug.LogError("Thats not an Image nor Panel nor Label"); 
                    return; 
                }
                tragetControl = value; 
            }
        }

        [EditorDisplay("Game Button"), ExpandGroups]
        public GameEvent OnClick = new GameEvent();

        public new bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                Recalculate();
            }
        }

        public void Recalculate()
        {
            if (TargetControl != null)
            {
                if (isColorTint)
                {
                    SetColors(!Enabled ? DisabledColor : (IsMouseOver || selected ? HoverColor : NormalColor));
                }
                if (isBrushSwap && targetControlAsImage != null)
                    targetControlAsImage.Brush = !Enabled ? DisabledBrush : (IsMouseOver || selected ? HoverBrush : NormalBrush);
            }
        }

        public void SetColors(Color color)
        {
            if (targetControlAsLabel != null)
            {
                targetControlAsLabel.TextColor = color;
                targetControlAsLabel.TextColorHighlighted = color;
            }
            if (targetControlAsImage != null)
            {
                targetControlAsImage.Color = color;
                targetControlAsImage.MouseOverColor = color;
            }
            if (targetControlAsPanel != null)
            {
                targetControlAsPanel.BackgroundColor = color;
            }
        }

        bool selected = false;

        public override void OnSelect()
        {
            base.OnSelect();
            selected = true;
            Recalculate();
        }
        public override void OnDeSelect()
        {
            base.OnDeSelect();
            selected = false;
            Recalculate();
        }
        /// <inheritdoc/>
        public override void OnSubmit()
        {
            base.OnSubmit();

            OnClick?.Invoke();

            if (TargetControl != null)
            {
                if (isColorTint)
                {
                    SetColors(PressedColor);
                }                    
                if (isBrushSwap)
                    targetControlAsImage.Brush = PressedBrush;
            }
        }   

        /// <inheritdoc />
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();
            Recalculate();
        }
        public override void OnMouseEnter(Float2 location)
        {
            base.OnMouseEnter(location);
            Recalculate();
        }

        /// <inheritdoc />
        public override bool OnMouseDown(Float2 location, MouseButton button)
        {
            
            if (button == MouseButton.Left)
            {
                Debug.Log("Click");
                UIInputSystem.GetInputSystemForRctrl(Root)?.NavigateTo(this);
                OnSubmit();
                return true;
            }

            return base.OnMouseDown(location, button);
        }
    }
}
