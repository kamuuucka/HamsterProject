using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CutoutMaskUI : Image
{
    public override Material materialForRendering
    {
        get
        {
            // get a copy of the base material or you going to fuck up the whole project
            Material material = new Material(base.materialForRendering);
            material.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            return material;
        }
    }
}
