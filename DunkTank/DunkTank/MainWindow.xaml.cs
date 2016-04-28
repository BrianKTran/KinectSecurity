using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using Microsoft.Speech;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace DunkTank
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Class Level Variables
        KinectAudioSource _kinectSource;
        KinectSensor _kinectSensor;
        SpeechRecognitionEngine _speechEngine;
        Stream _stream;
        
        static WaveGestureRight _gestureRight = new WaveGestureRight();
        static WaveGestureLeft _gestureLeft = new WaveGestureLeft();

        ////kinect  camera sensor
        KinectSensor _sensor;
        WriteableBitmap colorBitmap;
        //for head tracking skeletal
        //Skeleton[] skeletonData;


        public MainWindow()
        {
            InitializeComponent();
        }



        private void clearAll()
        {
            brianImg.Visibility = Visibility.Hidden;
            jimmyImg.Visibility = Visibility.Hidden;
            elaineImg.Visibility = Visibility.Hidden;
            ballLeft.Visibility = Visibility.Hidden;
            ballRight.Visibility = Visibility.Hidden;
            binilaImg.Visibility = Visibility.Hidden;
            binilaImgSplash.Visibility = Visibility.Hidden;
            jimmyImgSplash.Visibility = Visibility.Hidden;
            brianImgSplash.Visibility = Visibility.Hidden;
            elaineImgSplash.Visibility = Visibility.Hidden;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                //Could be more than one just set to the first
                _sensor = KinectSensor.KinectSensors[0];

                //Check the State of the sensor
                if (_sensor.Status == KinectStatus.Connected)
                {
                    colorBitmap = new WriteableBitmap
                    (_sensor.ColorStream.FrameWidth,
                    _sensor.ColorStream.FrameHeight,
                    96.0, 96.0, PixelFormats.Bgr32, null);
                    //Enable the features
                    _sensor.ColorStream.Enable();
                    _sensor.DepthStream.Enable();
                    _sensor.SkeletonStream.Enable();
                    _sensor.AllFramesReady += _sensor_AllFramesReady; //Double Tab
                    // Start the sensor!
                    try
                    {
                        _sensor.Start();
                    }
                    catch (IOException)
                    {
                        _sensor = null;
                    }
                }
            }
            clearAll();

            _kinectSensor = KinectSensor.KinectSensors[0];
            _kinectSensor.SkeletonStream.Enable();
            _kinectSensor.SkeletonFrameReady += Sensor_SkeletonFrameReady;
            _gestureRight.GestureRecognizedRight += Gesture_GestureRecognizedRight;
            _gestureLeft.GestureRecognizedLeft += Gesture_GestureRecognizedLeft;
            _kinectSensor.Start();

            var recInstalled = SpeechRecognitionEngine.InstalledRecognizers();
            RecognizerInfo rec = (RecognizerInfo)recInstalled[0];
            _speechEngine = new SpeechRecognitionEngine(rec.Id);

            var choices = new Choices();
            choices.Add("brian");
            choices.Add("jimmy");
            choices.Add("elaine");
            choices.Add("binila");
            choices.Add("clear");
            //choices.Add("MyPictures");
            //choices.Add("paint");


            var gb = new GrammarBuilder { Culture = rec.Culture };
            gb.Append(choices);
            var g = new Grammar(gb);

            _speechEngine.LoadGrammar(g);
            //recognized a word or words that may be a component of multiple complete phrases in a grammar.
            _speechEngine.SpeechHypothesized += new EventHandler<SpeechHypothesizedEventArgs>(SpeechEngineSpeechHypothesized);
            //receives input that matches any of its loaded and enabled Grammar objects.
            _speechEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(_speechEngineSpeechRecognized);

            //C# threads are MTA by default and calling RecognizeAsync in the same thread will cause an COM exception.
            var t = new Thread(StartAudioStream);
            t.Start();
        }

                void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            //using - Automatically dispose of the open when complete
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                {
                    return;
                }
                byte[] pixels = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixels);

                int stride = colorFrame.Width * 4;
                image1.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);

                colorBitmap.WritePixels(
                new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                pixels, this.colorBitmap.PixelWidth * sizeof(int), 0);
            }

            //throw new NotImplementedException();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (null == _sensor)
            {
                return;
            }
            //create a png bitmap encoder which knows how to save a .png file
            BitmapEncoder encoder = new PngBitmapEncoder();

            //create frame from the writeable bitmap and add to encoder
            encoder.Frames.Add(BitmapFrame.Create(colorBitmap));

            string time = System.DateTime.Now.ToString("hh'-'mm'-'ss", System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat);
            string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string path = System.IO.Path.Combine(myPhotos, "KinectSnapshot-" + time + ".png");

            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    encoder.Save(fs);
                }
            }
            catch (IndexOutOfRangeException)
            {
                //Handle Exception
            }

        }

        

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (txtTilt.Text != string.Empty)
            {
                _sensor.ElevationAngle = Convert.ToInt32(txtTilt.Text);
            }
        }
        void StartAudioStream()
        {
            _kinectSource = _kinectSensor.AudioSource;
            //Important to turn this off for speech recognition
            _kinectSource.AutomaticGainControlEnabled = false;
            _kinectSource.EchoCancellationMode = EchoCancellationMode.None;
            _stream = _kinectSource.Start();

            _speechEngine.SetInputToAudioStream(_stream,
                            new SpeechAudioFormatInfo(
                                EncodingFormat.Pcm, 16000, 16, 1,
                                32000, 2, null));

            _speechEngine.RecognizeAsync(RecognizeMode.Multiple);
        }
        

        void _speechEngineSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {


            if (e.Result.Text == "brian")
            {
                brianImg.Visibility = Visibility.Visible;
            }
            else if (e.Result.Text == "jimmy")
            {
                jimmyImg.Visibility = Visibility.Visible;
                if (null == _sensor)
                {
                    return;
                }

            }
            else if (e.Result.Text == "elaine")
            {
                elaineImg.Visibility = Visibility.Visible;
            }
            else if (e.Result.Text == "binila")
            {
                elaineImg.Visibility = Visibility.Visible;
            }

            //if (e.Result.Text == "MyPictures")
            //{
            //    var proc = new ProcessStartInfo();
            //    proc.FileName = "MyPictures.exe";
            //    proc.UseShellExecute = true;
            //    Process.Start(proc);
            //}

            else if (e.Result.Text == "paint")
            {
                var proc = new ProcessStartInfo();
                proc.FileName = "paint.exe";
                proc.UseShellExecute = true;
                Process.Start(proc);
            }
        }

        void SpeechEngineSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            if (e.Result.Text == "brian")
            {
                brianImg.Visibility = Visibility.Visible;
            }
            else if (e.Result.Text == "jimmy")
            {
                jimmyImg.Visibility = Visibility.Visible;
            }
            else if (e.Result.Text == "elaine")
            {
                elaineImg.Visibility = Visibility.Visible;
            }
            else if (e.Result.Text == "binila")
            {
                binilaImg.Visibility = Visibility.Visible;
            }
            else if (e.Result.Text == "clear")
            {
                clearAll();
            }
        }

        void Sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (var frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    Skeleton[] skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                    if (skeletons.Length > 0)
                    {
                        var user = skeletons.Where(
                                   u => u.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();

                        if (user != null)
                        {
                            JointCollection jointCollection = user.Joints;
                            _gestureRight.Update(user);
                            _gestureLeft.Update(user);

                            ////snapshot
                            //BitmapEncoder encoder = new PngBitmapEncoder();

                            ////create frame from the writeable bitmap and add to encoder
                            //encoder.Frames.Add(BitmapFrame.Create(colorBitmap));

                            //string time = System.DateTime.Now.ToString("hh'-'mm'-'ss", System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat);
                            //string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                            //string path = System.IO.Path.Combine(myPhotos, "KinectSnapshot-" + time + ".png");

                            //try
                            //{
                            //    using (FileStream fs = new FileStream(path, FileMode.Create))
                            //    {
                            //        encoder.Save(fs);
                            //    }
                            //}
                            //catch (IndexOutOfRangeException)
                            //{
                            //    //Handle Exception
                            //}//end of snapshot


                        }
                    }
                }
            }
        }



        //Closing Sensor_SkeletonFrameReady method

        void Gesture_GestureRecognizedRight(object sender, EventArgs e)
        {
            //splash();
            ballRight.Visibility = Visibility.Visible;
        }

        void Gesture_GestureRecognizedLeft(object sender, EventArgs e)
        {
            //splash();
            ballLeft.Visibility = Visibility.Visible;
        }

        //private void splash()
        //{
        //    if (brianImg.Visibility == Visibility.Visible)
        //        brianImgSplash.Visibility = Visibility.Visible;
        //    else if (jimmyImg.Visibility == Visibility.Visible)
        //        jimmyImgSplash.Visibility = Visibility.Visible;
        //    else if (elaineImg.Visibility == Visibility.Visible)
        //        elaineImgSplash.Visibility = Visibility.Visible;
        //    else if (binilaImg.Visibility == Visibility.Visible)
        //       binilaImgSplash.Visibility = Visibility.Visible;
        //}
    }
}
