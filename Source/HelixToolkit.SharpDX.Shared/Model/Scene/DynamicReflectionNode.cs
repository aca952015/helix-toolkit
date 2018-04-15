﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    using Render;

    public class DynamicReflectionNode : GroupNode, IDynamicReflector
    {
        public Vector3 Center
        {
            set
            {
                (RenderCore as IDynamicReflector).Center = value;
            }
            get
            {
                return (RenderCore as IDynamicReflector).Center;
            }
        }

        public DynamicReflectionNode()
        {
            this.OnAddChildNode += DynamicReflectionNode_OnAddChildNode;
            this.OnRemoveChildNode += DynamicReflectionNode_OnRemoveChildNode;
            this.OnClear += DynamicReflectionNode_OnClear;
        }

        private void DynamicReflectionNode_OnClear(object sender, OnChildNodeChangedArgs e)
        {
            (RenderCore as DynamicCubeMapCore).IgnoredGuid.Clear();
        }

        private void DynamicReflectionNode_OnRemoveChildNode(object sender, OnChildNodeChangedArgs e)
        {
            (RenderCore as DynamicCubeMapCore).IgnoredGuid.Remove(e.Node.RenderCore.GUID);
            if (e.Node is IDynamicReflectable dyn)
            {
                dyn.DynamicReflector = null;
            }
        }

        private void DynamicReflectionNode_OnAddChildNode(object sender, OnChildNodeChangedArgs e)
        {
            (RenderCore as DynamicCubeMapCore).IgnoredGuid.Add(e.Node.RenderCore.GUID);
            if(e.Node is IDynamicReflectable dyn)
            {
                dyn.DynamicReflector = this;
            }
        }

        protected override RenderCore OnCreateRenderCore()
        {
            return new DynamicCubeMapCore();
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                RenderCore.Attach(this.EffectTechnique);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void UpdateNotRender(IRenderContext context)
        {
            base.UpdateNotRender(context);
            if(Octree != null)
            {
                Center = Octree.Bound.Center();
            }
            else
            {
                BoundingBox box = new BoundingBox();
                int i = 0;
                for(; i < Items.Count; ++i)
                {
                    if(Items[i] is IDynamicReflectable)
                    {
                        box = Items[i].BoundsWithTransform;
                        break;
                    }
                }
                for (; i < Items.Count; ++i)
                {
                    if (Items[i] is IDynamicReflectable)
                    {
                        box = BoundingBox.Merge(box, Items[i].BoundsWithTransform);
                    }
                }
                Center = box.Center();
            }
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.Skybox];
        }

        protected override bool CanHitTest(IRenderContext context)
        {
            return false;
        }
        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }

        /// <summary>
        /// Binds the cube map.
        /// </summary>
        /// <param name="deviceContext">The device context.</param>
        public void BindCubeMap(DeviceContextProxy deviceContext)
        {
            (RenderCore as IDynamicReflector).BindCubeMap(deviceContext);
        }
        /// <summary>
        /// Uns the bind cube map.
        /// </summary>
        /// <param name="deviceContext">The device context.</param>
        public void UnBindCubeMap(DeviceContextProxy deviceContext)
        {
            (RenderCore as IDynamicReflector).UnBindCubeMap(deviceContext);
        }
    }
}
