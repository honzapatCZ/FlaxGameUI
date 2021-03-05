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
            ColorTint
        }
        [EditorDisplay("Game Button"), ExpandGroups]
        public ButtonVerke buttonType;
        bool isColorTint { get => buttonType == ButtonVerke.ColorTint; }
        bool isBrushSwap { get => buttonType == ButtonVerke.BrushSwap; }
        
        [VisibleIf("isColorTint")]
        [EditorDisplay("Game Button"), ExpandGroups]
        public Color NormalColor;
        [VisibleIf("isColorTint")]
        [EditorDisplay("Game Button"), ExpandGroups]
        public Color HoverColor;
        [VisibleIf("isColorTint")]
        [EditorDisplay("Game Button"), ExpandGroups]
        public Color PressedColor;

        [VisibleIf("isBrushSwap")]
        [EditorDisplay("Game Button"), ExpandGroups]
        public IBrush NormalBrush;
        [VisibleIf("isBrushSwap")]
        [EditorDisplay("Game Button"), ExpandGroups]
        public IBrush HoverBrush;
        [VisibleIf("isBrushSwap")]
        [EditorDisplay("Game Button"), ExpandGroups]
        public IBrush PressedBrush;

        [Serialize]
        UIControl targetImage;
        Image targetImageControl => targetImage?.Control as Image;
        [EditorDisplay("Game Button"), ExpandGroups]
        [NoSerialize]
        public UIControl TargetImage 
        {
            get => targetImage;
            set { if (value != null && !(value.Control is Image)) { Debug.LogError("Thats not an image"); return; } targetImage = value; }
        }

        [EditorDisplay("Game Button"), ExpandGroups]
        public GameEvent<int> OnClick;

        public override void OnSelect()
        {
            base.OnSelect();

            if(targetImageControl != null)
            {
                if (isColorTint)
                    targetImageControl.Color = HoverColor;
                if (isBrushSwap)
                    targetImageControl.Brush = HoverBrush;
            }
        }
        public override void OnDeSelect()
        {
            base.OnDeSelect();

            if (targetImageControl != null)
            {
                if (targetImageControl != null && isColorTint)
                    targetImageControl.Color = NormalColor;
                if (targetImageControl != null && isBrushSwap)
                    targetImageControl.Brush = NormalBrush;
            }
        }
        int numberofClick = 0;
        public override void OnSubmit()
        {
            base.OnSubmit();
            numberofClick++;
            OnClick.Invoke(numberofClick);

            if (targetImageControl != null)
            {
                if (isColorTint)
                    targetImageControl.Color = PressedColor;
                if (isBrushSwap)
                    targetImageControl.Brush = PressedBrush;
            }
        }   

        /// <inheritdoc />
        public override void OnMouseLeave()
        {
            OnDeSelect();

            base.OnMouseLeave();
        }
        public override void OnMouseEnter(Vector2 location)
        {
            OnSelect();

            base.OnMouseEnter(location);
        }
        /// <inheritdoc />
        public override bool OnMouseDown(Vector2 location, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                OnSubmit();
                return true;
            }

            return base.OnMouseDown(location, button);
        }
    }
}
