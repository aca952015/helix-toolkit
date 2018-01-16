﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using System;
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D11;
    using Utilities;
    /// <summary>
    /// Point Geometry Buffer Model. Use for point rendering
    /// </summary>
    /// <typeparam name="VertexStruct"></typeparam>
    public class PointGeometryBufferModel<VertexStruct> : GeometryBufferModel where VertexStruct : struct
    {
        public delegate VertexStruct[] BuildVertexArrayHandler(PointGeometry3D geometry);
        /// <summary>
        /// Create VertexStruct[] from geometry position, colors etc.
        /// </summary>
        public BuildVertexArrayHandler OnBuildVertexArray;

        public PointGeometryBufferModel(int structSize) : base(PrimitiveTopology.PointList,
            new ImmutableBufferProxy(structSize, BindFlags.VertexBuffer), null)
        {
        }

        protected override void OnCreateVertexBuffer(DeviceContext context, IElementsBufferProxy buffer, Geometry3D geometry)
        {
            // -- set geometry if given
            if (geometry != null && geometry.Positions != null && OnBuildVertexArray != null)
            {
                // --- get geometry
                var mesh = geometry as PointGeometry3D;
                var data = OnBuildVertexArray(mesh);
                buffer.UploadDataToBuffer(context, data, geometry.Positions.Count);
            }
            else
            {
                buffer.DisposeAndClear();
            }
        }

        protected override void OnCreateIndexBuffer(DeviceContext context, IElementsBufferProxy buffer, Geometry3D geometry)
        {
            
        }
    }
}
