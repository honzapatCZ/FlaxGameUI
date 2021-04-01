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

        [NoSerialize, EditorDisplay("Game Button"), ExpandGroups]
        public UIControl TargetControl 
        {
            get => tragetControl;
            set { 
                if (value != null && !(value.Control is Image) && !(value.Control is Label)) {
                    Debug.LogError("Thats not an image"); 
                    return; 
                }
                tragetControl = value; 
            }
        }

        [EditorDisplay("Game Button"), ExpandGroups]
        public GameEvent OnClick;

        public new bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                if (TargetControl != null)
                {
                    if (isColorTint)
                    {
                        if (TargetControl.Control is Label)
                        {
                            targetControlAsLabel.TextColor = !value ? DisabledColor : NormalColor;
                            targetControlAsLabel.TextColorHighlighted = !value ? DisabledColor : NormalColor;
                        }
                        if (targetControlAsImage != null)
                        {
                            targetControlAsImage.Color = !value ? DisabledColor : NormalColor;
                            targetControlAsImage.MouseOverColor = !value ? DisabledColor : NormalColor;
                        }
                    }
                    if (isBrushSwap)
                        targetControlAsImage.Brush = !value ? DisabledBrush : NormalBrush;
                }
            }
        }

        public override void OnSelect()
        {
            base.OnSelect();

            if(TargetControl != null)
            {
                if (isColorTint)
                {
                    if (targetControlAsLabel != null)
                    {
                        targetControlAsLabel.TextColor = HoverColor;
                        targetControlAsLabel.TextColorHighlighted = HoverColor;
                    }                        
                    if (targetControlAsImage != null)
                    {
                        targetControlAsImage.Color = HoverColor;
                        targetControlAsImage.MouseOverColor = HoverColor;
                    }
                }                    
                if (isBrushSwap)
                    targetControlAsImage.Brush = HoverBrush;
            }
        }
        public override void OnDeSelect()
        {
            base.OnDeSelect();

            if (TargetControl != null)
            {
                if (isColorTint)
                {
                    if (targetControlAsLabel != null)
                    {
                        targetControlAsLabel.TextColor = NormalColor;
                        targetControlAsLabel.TextColorHighlighted = NormalColor;
                    }
                    if (targetControlAsImage != null)
                    {
                        targetControlAsImage.Color = NormalColor;
                        targetControlAsImage.MouseOverColor = NormalColor;
                    }
                }                    
                if (isBrushSwap)
                    targetControlAsImage.Brush = NormalBrush;
            }
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
                    if(targetControlAsLabel != null)
                    {
                        targetControlAsLabel.TextColor = PressedColor;
                        targetControlAsLabel.TextColorHighlighted = PressedColor;
                    }
                    if (targetControlAsImage != null)
                    {
                        targetControlAsImage.Color = PressedColor;
                        targetControlAsImage.MouseOverColor = PressedColor;
                    }
                }                    
                if (isBrushSwap)
                    targetControlAsImage.Brush = PressedBrush;
            }
        }   

        /// <inheritdoc />
        public override void OnMouseLeave()
        {
            UIInputSystem.GetInputSystemForRctrl(Root).NavigateTo(null);

            base.OnMouseLeave();
        }
        public override void OnMouseEnter(Vector2 location)
        {
            UIInputSystem.GetInputSystemForRctrl(Root).NavigateTo(this);

            base.OnMouseEnter(location);
        }
        /// <inheritdoc />
        public override bool OnMouseDown(Vector2 location, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                UIInputSystem.GetInputSystemForRctrl(Root).NavigateTo(this);
                OnSubmit();
                return true;
            }

            return base.OnMouseDown(location, button);
        }
    }
}
