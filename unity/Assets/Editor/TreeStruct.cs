//======================================= 2019, Stéphane Malta e Sousa, sitn-vr =======================================
//
// This structure helps importing JSON treeinstances. JSON from Unity is not able to import root arrays
//
//=====================================================================================================================

namespace SITN
{
    [System.Serializable]
    public struct TreeStruct
    {
        [System.Serializable]
        public struct SITNTree
        {
            public float[] coordinates;
            public int prototypeIndex;
            public float heightScale;
            public float widthScale;
            public float rotation;
        }

        public SITNTree[] trees;
    }
}
