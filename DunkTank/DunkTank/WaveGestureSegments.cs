using Microsoft.Kinect;

namespace DunkTank
{
    public interface IGestureSegment
    {
        GesturePartResult Update(Skeleton skeleton);
    }

    public class WaveSegment1 : IGestureSegment // right
    {
        public GesturePartResult Update(Skeleton skeleton)
        {
            // Hand above elbow
            if (skeleton.Joints[JointType.HandRight].Position.Y >
                skeleton.Joints[JointType.ElbowRight].Position.Y)
            {
                // Hand right of elbow
                if (skeleton.Joints[JointType.HandRight].Position.X >
                    skeleton.Joints[JointType.ElbowRight].Position.X)
                {
                    return GesturePartResult.Succeeded;
                }
            }

            // Hand dropped
            return GesturePartResult.Failed;
        }
    }
     
    public class WaveSegment2 : IGestureSegment  // right
    {
        public GesturePartResult Update(Skeleton skeleton)
        {
            // Hand above elbow
            if (skeleton.Joints[JointType.HandRight].Position.Y >
                skeleton.Joints[JointType.ElbowRight].Position.Y)
            {
                // Hand left of elbow
                if (skeleton.Joints[JointType.HandRight].Position.X <
                    skeleton.Joints[JointType.ElbowRight].Position.X)
                {
                    return GesturePartResult.Succeeded;
                }
            }

            // Hand dropped
            return GesturePartResult.Failed;
        }
    }


   


    public class WaveSegment3 : IGestureSegment  // left 
    {
        public GesturePartResult Update(Skeleton skeleton)
        {
            // Hand above elbow
            if (skeleton.Joints[JointType.HandLeft].Position.Y >
                skeleton.Joints[JointType.ElbowLeft].Position.Y)
            {
                // Hand right of elbow
                if (skeleton.Joints[JointType.HandLeft].Position.X >
                    skeleton.Joints[JointType.ElbowLeft].Position.X)
                {
                    return GesturePartResult.Succeeded;
                }
            }

            // Hand dropped
            return GesturePartResult.Failed;
        }
    }

            public class WaveSegment4 : IGestureSegment //left 
            {
                public GesturePartResult Update(Skeleton skeleton)
                {
                    // Hand above elbow
                    if (skeleton.Joints[JointType.HandLeft].Position.Y >
                        skeleton.Joints[JointType.ElbowLeft].Position.Y)
                    {
                        // Hand left of elbow
                        if (skeleton.Joints[JointType.HandLeft].Position.X <
                            skeleton.Joints[JointType.ElbowLeft].Position.X)
                        {
                            return GesturePartResult.Succeeded;
                        }
                    }

                    // Hand dropped
                    return GesturePartResult.Failed;
                }
            }


    public class WaveSegment5 : IGestureSegment //right 
    {
        public GesturePartResult Update(Skeleton skeleton)
        {
            // Hand right of elbow
            if (skeleton.Joints[JointType.HandRight].Position.X >
                skeleton.Joints[JointType.ElbowRight].Position.X)
            {
                // Hand above elbow
                if (skeleton.Joints[JointType.HandRight].Position.Y >
                    skeleton.Joints[JointType.ElbowRight].Position.Y)
                {
                    return GesturePartResult.Succeeded;
                }
            }

            // Hand dropped
            return GesturePartResult.Failed;
        }
    }

    public class WaveSegment6 : IGestureSegment // right 
    {
        public GesturePartResult Update(Skeleton skeleton)
        {// Hand left of elbow
            if (skeleton.Joints[JointType.HandRight].Position.X <
                skeleton.Joints[JointType.ElbowRight].Position.X)

            {
                // Hand above elbow
                if (skeleton.Joints[JointType.HandRight].Position.Y >
                    skeleton.Joints[JointType.ElbowRight].Position.Y)
                {
                    return GesturePartResult.Succeeded;
                }
            }

            // Hand dropped
            return GesturePartResult.Failed;
        }
    }

    public class WaveSegment7 : IGestureSegment //left 
    {
        public GesturePartResult Update(Skeleton skeleton)
        {
            // Hand above elbow
            if (skeleton.Joints[JointType.HandLeft].Position.Y >
                skeleton.Joints[JointType.ElbowLeft].Position.Y)
            
{
                // Hand right of elbow
                if (skeleton.Joints[JointType.HandLeft].Position.X >
                    skeleton.Joints[JointType.ElbowLeft].Position.X)
                {
                    return GesturePartResult.Succeeded;
                }
            }

            // Hand dropped
            return GesturePartResult.Failed;
        }
    }

    public class WaveSegment8 : IGestureSegment //left
    {
        public GesturePartResult Update(Skeleton skeleton)
        {// Hand left of elbow
                if (skeleton.Joints[JointType.HandLeft].Position.X <
                    skeleton.Joints[JointType.ElbowLeft].Position.X)
            
            {
                // Hand above elbow
            if (skeleton.Joints[JointType.HandLeft].Position.Y >
                skeleton.Joints[JointType.ElbowLeft].Position.Y)
                
                {
                    return GesturePartResult.Succeeded;
                }
            }

            // Hand dropped
            return GesturePartResult.Failed;
        }
    }

}