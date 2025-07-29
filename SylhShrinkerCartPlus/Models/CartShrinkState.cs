using UnityEngine;

namespace SylhShrinkerCartPlus.Models
{
    public class CartShrinkState
    {
        public HashSet<PhysGrabObject> ObjectsToShrink = new();
        public Dictionary<PhysGrabObject, Vector3> OriginalScales = new();
    }
}

