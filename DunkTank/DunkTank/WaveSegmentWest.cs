﻿using Microsoft.Kinect;
using System;

namespace DunkTank
    {
        public class WaveGestureWest //left
        {
            //simple gestures that last for approximately a second, a window size of 30 or 50 will work
            readonly int WINDOW_SIZE = 50; //Number of frames for that gesture to last

            IGestureSegment[] _segments;

            int _currentSegment = 0;
            //number of frames we ask for data is called window size
            int _frameCount = 0;

            public event EventHandler GestureRecognizedLeft;

            public WaveGestureWest()
            {
            WaveSegment7 waveSegment7 = new WaveSegment7(); //Hanlde Hand right of the elbow
            WaveSegment8 waveSegment8 = new WaveSegment8(); //Handle Hand left of the elbow

           




                //Gesture parts and specify their order in the the _segments array - waving
                _segments = new IGestureSegment[]
                {
                waveSegment7,
                waveSegment8



                };
            }

            public void Update(Skeleton skeleton)
            {
                //check every segment 
                GesturePartResult result = _segments[_currentSegment].Update(skeleton);

                //check every segment for success 
                if (result == GesturePartResult.Succeeded)
                {
                    if (_currentSegment + 1 < _segments.Length)
                    {
                        _currentSegment++;  //Go to the next gesture part to check
                        _frameCount = 0;
                    }
                    else
                    {
                        if (GestureRecognizedLeft != null)
                        {
                            GestureRecognizedLeft(this, new EventArgs());  //Gesture found
                            Reset(); //Start the gesture over
                        }
                    }
                }
                else if (result == GesturePartResult.Failed || _frameCount == WINDOW_SIZE)
                {
                    Reset();  //Start the gesture over
                }
                else
                {
                    _frameCount++;  //Count the frames
                }
            }

            public void Reset()
            {
                _currentSegment = 0;
                _frameCount = 0;
            }
        }
    }

