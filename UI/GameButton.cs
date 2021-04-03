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
                if (TargetControl != null)
                {
                    if (isColorTint)
                    {
                        if (targetControlAsLabel != null)
                        {
                            targetControlAsLabel.TextColor = !value ? DisabledColor : NormalColor;
                            targetControlAsLabel.TextColorHighlighted = !value ? DisabledColor : NormalColor;
                        }
                        if (targetControlAsImage != null)
                        {
                            targetControlAsImage.Color = !value ? DisabledColor : NormalColor;
                            targetControlAsImage.MouseOverColor = !value ? DisabledColor : NormalColor;
                        }
                        if (targetControlAsPanel != null)
                        {
                            targetControlAsPanel.BackgroundColor = !value ? DisabledColor : NormalColor;
                        }
                    }
                    if (isBrushSwap)
                        targetControlAsImage.Brush = !value ? DisabledBrush : NormalBrush;
                }
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
            Highlighted();
        }
        public void Highlighted()
        {
            if (TargetControl != null)
            {
                if (isColorTint)
                {
                    SetColors(HoverColor);
                }
                if (isBrushSwap)
                    targetControlAsImage.Brush = HoverBrush;
            }
        }
        public override void OnDeSelect()
        {
            base.OnDeSelect();
            selected = false;
            UnHighlighted();
        }
        public void UnHighlighted()
        {
            if (selected)
                return;
            if (TargetControl != null)
            {
                if (isColorTint)
                {
                    SetColors(NormalColor);
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
                    SetColors(PressedColor);
                }                    
                if (isBrushSwap)
                    targetControlAsImage.Brush = PressedBrush;
            }
        }   

        /// <inheritdoc />
        public override void OnMouseLeave()
        {
            UnHighlighted();

            base.OnMouseLeave();
        }
        public override void OnMouseEnter(Vector2 location)
        {
            Highlighted();

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
