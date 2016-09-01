﻿using CSharpGL;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TracyEnergy.Simba.Data.Keywords;

namespace GridViewer
{
    public partial class FormMain : Form
    {
        private SimulationInputData LoadEclInputData(String fileName)
        {
            KeywordSchema schema = KeywordSchemaExtension.RestoreSchemaFromEmbededResource();
            SimulationInputData inputData = new SimulationInputData(schema);
            inputData.ThrowError = true;
            inputData.LoadFromFile(fileName);
            return inputData;
        }

        private BoundingBoxRenderer GetBoundingBoxRenderer(params SceneObject[] objects)
        {
            var rectangles = new List<IBoundingBox>();
            foreach (var item in objects)
            {
                rectangles.AddRange(GetAllRectangle3Ds(item));
            }
            return GetBoundingBoxRenderer(rectangles.ToArray());
        }

        private IEnumerable<IBoundingBox> GetAllRectangle3Ds(SceneObject obj)
        {
            var item = obj.Renderer as IBoundingBox;
            if (item != null) { yield return item; }

            foreach (var child in obj.Children)
            {
                foreach (var renderer in GetAllRectangle3Ds(child))
                {
                    yield return renderer;
                }
            }
        }

        private BoundingBoxRenderer GetBoundingBoxRenderer(params IBoundingBox[] rectangles)
        {
            IBoundingBox rect;
            if (rectangles.Length > 0)
            {
                rect = rectangles[0];
                for (int i = 1; i < rectangles.Length; i++)
                {
                    rect = rect.Union(rectangles[i]);
                }
            }
            else
            {
                rect = new BoundingBox();
            }

            vec3 lengths = rect.MaxPosition - rect.MinPosition;
            vec3 originalWorldPosition = rect.MaxPosition / 2 + rect.MinPosition / 2;
            BoundingBoxRenderer boxRenderer = BoundingBoxRenderer.Create(lengths, originalWorldPosition);
            boxRenderer.SwitchList.Add(new LineWidthSwitch(1));

            return boxRenderer;
        }
    }
}