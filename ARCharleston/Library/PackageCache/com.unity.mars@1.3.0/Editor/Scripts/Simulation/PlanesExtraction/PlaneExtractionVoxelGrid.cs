using System.Collections.Generic;
using Unity.MARS.Data.Synthetic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.MARS.Data.Synthetic
{
    /// <summary>
    /// A grid of voxels that is used to find planes of a specific orientation all at once.
    /// Planes can be found in each individual layer of the grid based on the density of points in each voxel.
    /// </summary>
    [MovedFrom("Unity.MARS")]
    public class PlaneExtractionVoxelGrid : PlaneVoxelGrid<LayerPlaneData>
    {
        protected override bool CheckEmptyAreaForInitialPlanes
        {
            get { return true; }
        }

        /// <inheritdoc/>
        public PlaneExtractionVoxelGrid(
            VoxelGridOrientation orientation, Bounds containedBounds, float voxelSize, VoxelPlaneFindingParams planeFindingParams)
            : base(orientation, containedBounds, voxelSize, planeFindingParams) { }

        public void ExtractPlanes(List<ExtractedPlaneData> planes)
        {
            FindPlanes();

            for (var i = 0; i < m_Layers; i++)
            {
                var layerPlanes = m_PlanesPerLayer[i];
                var layerOrigin = GetLayerOriginPose(i);
                foreach (var plane in layerPlanes)
                {
                    var extractedPlane = new ExtractedPlaneData { vertices = new List<Vector3>() };
                    if (TryGetPlaneData(plane, layerOrigin, extractedPlane.vertices,
                        out extractedPlane.pose, out extractedPlane.center, out extractedPlane.extents))
                    {
                        planes.Add(extractedPlane);
                    }
                }
            }
        }

        protected override LayerPlaneData CreateLayerPlane(PlaneVoxel startingVoxel) { return LayerPlaneData.GetOrCreate(startingVoxel); }
    }
}
