﻿using System.Collections.Generic;
using FlaxEngine;
using FlaxEditor;
using FlaxEditor.CustomEditors.Elements;
using FlaxEditor.CustomEditors;
using FlaxEditor.Scripting;
using FlaxEditor.CustomEditors.Editors;
using FlaxEngine.GUI;
using System;
using System.Linq;
using FlaxEditor.GUI;
using FlaxEditor.GUI.ContextMenu;
using System.Reflection;
using FlaxEditor.GUI.Input;
using System.Collections;
using FlaxEditor.CustomEditors.GUI;

namespace FlaxGameUI
{
    [CustomEditor(typeof(GameEventBase.SerializedMethod)), DefaultEditor]
    public class SerializedEventValueEditor : CustomEditor
    {
        SpaceElement firstRow;
        SpaceElement secondRow;

        FlaxObjectRefPickerControl actorPicker;
        Button methodNameBtn;

        public override void Initialize(LayoutElementsContainer layout)
        {
            layout.Space(2);
            firstRow = layout.Space(20);

            actorPicker = new FlaxObjectRefPickerControl();
            actorPicker.Parent = firstRow.ContainerControl;
            actorPicker.AnchorPreset = AnchorPresets.VerticalStretchLeft;
            actorPicker.Offsets = new Margin(1, 0, 1, 1);
            actorPicker.Width = 98;
            actorPicker.Type = TypeUtils.GetType("FlaxEngine.Actor");
            actorPicker.ValueChanged += () => {
                GameEventBase.SerializedMethod editMeth = (GameEventBase.SerializedMethod)Values[0];
                if(actorPicker.Value != editMeth.GetActor())
                {
                    GameEventBase.SerializedMethod newMeth = new GameEventBase.SerializedMethod();
                    Debug.Log(editMeth.Actor);
                    newMeth.Actor = (Actor)actorPicker.Value;
                    Debug.Log(newMeth.Actor);
                    SetValue(newMeth);
                }
            };

            //_element.CustomControl.ValueChanged += () => SetValue(_element.CustomControl.Value);

            methodNameBtn = new Button();
            methodNameBtn.Parent = firstRow.ContainerControl;
            methodNameBtn.AnchorPreset = AnchorPresets.StretchAll;
            methodNameBtn.Offsets = new Margin(101, 1, 1, 1);
            methodNameBtn.Clicked += ()=>ShowPopupMenu();

            secondRow = layout.Space(20);
        }

        GameEventBase.SerializedMethod oldPopUpMenuMethod;
        ContextMenu popupContextMenu;
        public void ShowPopupMenu()
        {
            GameEventBase.SerializedMethod serMeth = (GameEventBase.SerializedMethod)Values[0];

            Actor act = serMeth.GetActor();
            Debug.Log(act);
            actorPicker.Value = act;

            if (popupContextMenu == null || oldPopUpMenuMethod.GetActor() != act)
            {
                popupContextMenu = new ContextMenu();

                if (act != null)
                {
                    ContextMenuChildMenu actorCtx = popupContextMenu.AddChildMenu("Actor");
                    CreateContextMenuForType(actorCtx.ContextMenu, act.GetType(), (info) => {
                        GameEventBase.SerializedMethod editMeth = (GameEventBase.SerializedMethod)Values[0];
                        editMeth.script = null;
                        editMeth.type = GameEventBase.SerializedMethod.SerializedActorType.Actor;
                        editMeth.ApplySerInfo(info);
                        SetValue(editMeth);
                    });

                    foreach (Script scr in act.Scripts)
                    {
                        ContextMenuChildMenu scrCtx = popupContextMenu.AddChildMenu(scr.GetType().Name);
                        CreateContextMenuForType(scrCtx.ContextMenu, scr.GetType(), (info) => {
                            GameEventBase.SerializedMethod editMeth = (GameEventBase.SerializedMethod)Values[0];
                            editMeth.script = scr;
                            editMeth.type = GameEventBase.SerializedMethod.SerializedActorType.Script;
                            editMeth.ApplySerInfo(info);
                            SetValue(editMeth);
                        });
                    }
                }
                else
                {
                    popupContextMenu.AddButton("(Empty)");
                }
            }            
            Debug.Log("Pred show " + methodNameBtn);
            popupContextMenu.Show(methodNameBtn, methodNameBtn.PointFromScreen(Input.MouseScreenPosition));
            Debug.Log("po show");
            oldPopUpMenuMethod = serMeth;
        }
        

        public void CreateContextMenuForType(ContextMenu ctx,Type type, Action<GameEventBase.SerializedMethod.SerInfos> cmcb)
        {
            List<MethodInfo> minfos = type.GetMethods().Where((m)=> !(m.IsSpecialName || m.GetParameters().Count() > 1)).ToList();
            Debug.Log("Vytvářím metody");
            foreach (MethodInfo minfo in minfos)
            {
                List<ParameterInfo> pinfo = minfo.GetParameters().ToList();
                pinfo = pinfo.Count > 0 ? pinfo : null;

                string typeName = pinfo?.ElementAt(0)?.ParameterType?.Name;
                ctx.AddButton(minfo.Name + "(" + typeName + ")", (ctxbtn) => {
                    GameEventBase.SerializedMethod.SerInfos info = new GameEventBase.SerializedMethod.SerInfos();
                    info.minfo = minfo;
                    cmcb(info);
                });
            }
            
            List<PropertyInfo> pinfos = type.GetProperties().Where(prop=>prop.CanWrite && prop.GetSetMethod() != null).ToList();
            if(pinfos.Count > 0 && minfos.Count > 0)
                ctx.AddSeparator();
            Debug.Log("Vytvářím property");
            foreach (PropertyInfo pinfo in pinfos)
            {
                ctx.AddButton(pinfo.PropertyType.Name + " " + pinfo.Name, (ctxbtn) => {
                    GameEventBase.SerializedMethod.SerInfos info = new GameEventBase.SerializedMethod.SerInfos();
                    info.pinfo = pinfo;
                    cmcb(info);
                });
            }

            List<FieldInfo> finfos = type.GetFields().ToList();
            if(finfos.Count > 0 && (pinfos.Count > 0 || minfos.Count > 0))
                ctx.AddSeparator();
            Debug.Log("Vytvářím fieldy");
            foreach (FieldInfo finfo in finfos)
            {
                ctx.AddButton(finfo.FieldType.Name + " " + finfo.Name, (ctxbtn) => {
                    GameEventBase.SerializedMethod.SerInfos info = new GameEventBase.SerializedMethod.SerInfos();
                    info.finfo = finfo;
                    cmcb(info);
                });
            }
        }

        GameEventBase.SerializedMethod oldSerMethod;
        public override void Refresh()
        {
            base.Refresh();

            GameEventBase.SerializedMethod serMeth = (GameEventBase.SerializedMethod)Values[0];
            //Debug.Log("Zjištujem jestli se " + oldSerMethod.GetActor() + " rovná "+ serMeth.GetActor() + " což nám vyšlo " + oldSerMethod.Equals(serMeth));
            if (serMeth.Equals(oldSerMethod))
                return;

            Actor act = serMeth.GetActor();
            Debug.Log("Refreshujeme s "+act);
            actorPicker.Value = act;

            methodNameBtn.Text = serMeth.GetDisplayValueName();

            GameEventBase.SerializedMethod.SerInfos infos = serMeth.GetSerInfos();
            Type tip = infos.GetParOrValueType();
            if(serMeth.IsDynamic != oldSerMethod.IsDynamic || tip != oldSerMethod.GetSerInfos().GetParOrValueType())
            {
                VariableControl?.Dispose();
                dynamicCHeck?.Dispose();

                Debug.Log(serMeth.GetDisplayValueName() + " has allowedDynamicType: " + serMeth.allowedDynamicType);
                Debug.Log("Náš tip je však " + tip);

                if (tip != null && tip == serMeth.allowedDynamicType)
                {
                    dynamicCHeck = new CheckBox(0, 0);
                    dynamicCHeck.Checked = (bool)serMeth.IsDynamic;
                    dynamicCHeck.StateChanged += (cbox) =>
                    {
                        GameEventBase.SerializedMethod editMeth = (GameEventBase.SerializedMethod)Values[0];
                        if (editMeth.IsDynamic != dynamicCHeck.Checked)
                        {
                            editMeth.IsDynamic = dynamicCHeck.Checked;
                            SetValue(editMeth);
                        }
                    };

                    dynamicCHeck.Parent = secondRow.ContainerControl;
                    dynamicCHeck.AnchorPreset = AnchorPresets.VerticalStretchLeft;
                    dynamicCHeck.Offsets = new Margin(1, 0, 1, 1);
                    dynamicCHeck.Width = 98;
                }

                Debug.Log(tip);
                if (tip != null && !serMeth.IsDynamic)
                {
                    if (tip == typeof(bool))
                    {
                        Debug.Log("Je to bool");
                        CheckBox varCon = new CheckBox(0, 0);
                        varCon.Checked = (bool)serMeth.SavedValue;
                        varCon.StateChanged += (cbox) =>
                        {
                            GameEventBase.SerializedMethod editMeth = (GameEventBase.SerializedMethod)Values[0];
                            if ((bool)editMeth.SavedValue != varCon.Checked)
                            {
                                editMeth.SavedValue = varCon.Checked;
                                SetValue(editMeth);
                            }
                        };
                        VariableControl = varCon;
                    }
                    else if (tip == typeof(string))
                    {
                        Debug.Log("Je to string");
                        TextBox varCon = new TextBox(false, 0, 0);
                        varCon.Text = (string)serMeth.SavedValue;
                        varCon.TextChanged += () =>
                        {
                            GameEventBase.SerializedMethod editMeth = (GameEventBase.SerializedMethod)Values[0];
                            if ((string)editMeth.SavedValue != varCon.Text)
                            {
                                editMeth.SavedValue = varCon.Text;
                                SetValue(editMeth);
                            }
                        };
                        VariableControl = varCon;
                    }
                    else if (tip == typeof(float))
                    {
                        Debug.Log("Je to float");
                        FloatValueBox varCon = new FloatValueBox((float)serMeth.SavedValue);
                        varCon.TextChanged += () =>
                        {
                            GameEventBase.SerializedMethod editMeth = (GameEventBase.SerializedMethod)Values[0];
                            if ((string)editMeth.SavedValue != varCon.Text)
                            {
                                editMeth.SavedValue = varCon.Text;
                                SetValue(editMeth);
                            }
                        };
                        VariableControl = varCon;
                    }
                    else if (tip == typeof(int))
                    {
                        Debug.Log("Je to int");
                        int integer = serMeth.SavedValue.GetType() == typeof(long) ? (int)(long)serMeth.SavedValue : (int)serMeth.SavedValue;
                        IntValueBox varCon = new IntValueBox(integer);
                        varCon.ValueChanged += () =>
                        {
                            GameEventBase.SerializedMethod editMeth = (GameEventBase.SerializedMethod)Values[0];
                            if ((int)editMeth.SavedValue != varCon.Value)
                            {
                                editMeth.SavedValue = varCon.Value;
                                SetValue(editMeth);
                            }
                        };
                        VariableControl = varCon;
                    }
                    else if (tip.IsSubclassOf(typeof(FlaxEngine.Object)))
                    {
                        Debug.Log("Je to FlaxObjekt");
                        FlaxObjectRefPickerControl varCon = new FlaxObjectRefPickerControl();
                        varCon.Type = new ScriptType(tip);
                        varCon.Value = (FlaxEngine.Object)serMeth.SavedValue;
                        varCon.ValueChanged += () =>
                        {
                            GameEventBase.SerializedMethod editMeth = (GameEventBase.SerializedMethod)Values[0];
                            if ((FlaxEngine.Object)editMeth.SavedValue != varCon.Value)
                            {
                                editMeth.SavedValue = varCon.Value;
                                SetValue(editMeth);
                            }
                        };
                        VariableControl = varCon;
                    }
                    else
                    {
                        Debug.Log("Nemám nejmenší tušení co to je");
                        Label varCon = new Label();
                        varCon.Text = "Unkown type " + tip.Name + " to display";
                        VariableControl = varCon;
                    }

                    VariableControl.Parent = secondRow.ContainerControl;
                    VariableControl.AnchorPreset = AnchorPresets.StretchAll;
                    VariableControl.Offsets = new Margin(101, 1, 1, 1);
                }
            }




            Debug.Log("Jsme na konci");
            if (serMeth.Equals((GameEventBase.SerializedMethod)Values[0]))
            {
                Debug.Log("něco se aktualizovalo, ukládáme");
                SetValue(serMeth);
            }

            oldSerMethod = serMeth;
        }
        public Control VariableControl;
        public CheckBox dynamicCHeck;
    }

    [CustomEditor(typeof(SerMethList<>)), DefaultEditor]
    public class SerMethListEditor : CustomEditor
    {
        public override DisplayStyle Style => DisplayStyle.InlineIntoParent;

        private int _elementsCount;

        public Type DynamicType
        {
            get
            {
                return Values.Type.GetGenericArguments()[0];
            }
        }

        public override void Initialize(LayoutElementsContainer layout)
        {
            var size = Count;
            if(DynamicType != typeof(GameEvent.GameEventVoid))
                layout.Label("(" + Values.Type.GetGenericArguments()[0].Name + ")");

            // Elements
            _elementsCount = size;
                        
            for (int i = 0; i < size; i++)
            {
                if (DynamicType != typeof(GameEvent.GameEventVoid))
                {
                    List<GameEventBase.SerializedMethod> mthods = (List<GameEventBase.SerializedMethod>)Values[0];
                    GameEventBase.SerializedMethod method = mthods[i];
                    method.allowedDynamicType = DynamicType;
                    mthods[i] = method;
                    Values[0] = mthods;
                }
                layout.Object(new ListValueContainer(ElementType, i, Values));
            }

            var area = layout.Space(20);
            var addButton = new Button(area.ContainerControl.Width - (16 + 16 + 2 + 2), 2, 16, 16)
            {
                Text = "+",
                TooltipText = "Add new item",
                AnchorPreset = AnchorPresets.TopRight,
                Parent = area.ContainerControl
            };
            addButton.Clicked += () =>
            {
                if (IsSetBlocked)
                    return;

                Resize(Count + 1);
            };
            var removeButton = new Button(addButton.Right + 2, addButton.Y, 16, 16)
            {
                Text = "-",
                TooltipText = "Remove last item",
                AnchorPreset = AnchorPresets.TopRight,
                Parent = area.ContainerControl,
                Enabled = size > 0
            };
            removeButton.Clicked += () =>
            {
                if (IsSetBlocked)
                    return;

                Resize(Count - 1);
            };
        }
        public int Count => (Values[0] as IList)?.Count ?? 0;

        protected void Resize(int newSize)
        {
            var list = Values[0] as IList;
            var oldSize = list?.Count ?? 0;

            if (oldSize != newSize)
            {
                // Allocate new list
                var listType = Values.Type;
                var newValues = (IList)listType.CreateInstance();

                var sharedCount = Mathf.Min(oldSize, newSize);
                if (list != null && sharedCount > 0)
                {
                    // Copy old values
                    for (int i = 0; i < sharedCount; i++)
                    {
                        newValues.Add(list[i]);
                    }

                    // Fill new entries with the last value
                    for (int i = oldSize; i < newSize; i++)
                    {
                        newValues.Add(list[oldSize - 1]);
                    }
                }
                else if (newSize > 0)
                {
                    // Fill new entries
                    var defaultValue = TypeUtils.GetDefaultValue(ElementType);

                    for (int i = oldSize; i < newSize; i++)
                    {
                        newValues.Add(defaultValue);
                    }
                }

                SetValue(newValues);
            }
        }
        public ScriptType ElementType
        {
            get
            {
                return new ScriptType(typeof(GameEventBase.SerializedMethod));
            }
        }

        /// <summary>
        /// Moves the specified item at the given index and swaps it with the other item. It supports undo.
        /// </summary>
        /// <param name="srcIndex">Index of the source item.</param>
        /// <param name="dstIndex">Index of the destination item to swap with.</param>
        private void Move(int srcIndex, int dstIndex)
        {
            if (IsSetBlocked)
                return;

            var cloned = CloneValues();

            var tmp = cloned[dstIndex];
            cloned[dstIndex] = cloned[srcIndex];
            cloned[srcIndex] = tmp;

            SetValue(cloned);
        }

        /// <summary>
        /// Removes the item at the specified index. It supports undo.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        private void Remove(int index)
        {
            if (IsSetBlocked)
                return;

            var newValues = Allocate(Count - 1);
            var oldValues = (IList)Values[0];

            for (int i = 0; i < index; i++)
            {
                newValues[i] = oldValues[i];
            }

            for (int i = index; i < newValues.Count; i++)
            {
                newValues[i] = oldValues[i + 1];
            }

            SetValue(newValues);
        }
        protected IList Allocate(int size)
        {
            var listType = Values.Type;
            var list = (IList)listType.CreateInstance();
            var defaultValue = TypeUtils.GetDefaultValue(ElementType);
            for (int i = 0; i < size; i++)
            {
                list.Add(defaultValue);
            }
            return list;
        }
        protected IList CloneValues()
        {
            var list = Values[0] as IList;
            if (list == null)
                return null;

            var size = list.Count;
            var listType = Values.Type;
            var cloned = (IList)listType.CreateInstance();

            for (int i = 0; i < size; i++)
            {
                cloned.Add(list[i]);
            }

            return cloned;
        }
        public override void Refresh()
        {
            base.Refresh();

            // No support for different collections for now
            if (HasDifferentValues || HasDifferentTypes)
                return;

            // Check if collection has been resized (by UI or from external source)
            if (Count != _elementsCount)
            {
                RebuildLayout();
            }
        }
    }
}
