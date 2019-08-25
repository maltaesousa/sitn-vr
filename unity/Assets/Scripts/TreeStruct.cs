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
