using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.MARS.Data
{
    [MovedFrom("Unity.MARS")]
    public enum MRFaceExpression
    {
        LeftEyeClose,
        RightEyeClose,
        LeftEyebrowRaise,
        RightEyebrowRaise,
        BothEyebrowsRaise,
        LeftLipCornerRaise,
        RightLipCornerRaise,
        Smile,
        MouthOpen
    }

    [MovedFrom("Unity.MARS")]
    public class MRFaceExpressionComparer : IEqualityComparer<MRFaceExpression>
    {
        public bool Equals(MRFaceExpression x, MRFaceExpression y)
        {
            return x == y;
        }

        public int GetHashCode(MRFaceExpression obj)
        {
            return (int)obj;
        }
    }
}
